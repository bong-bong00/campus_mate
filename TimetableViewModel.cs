using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using CampusSignal.Infrastructure;
using CampusSignal.Models;
using CampusSignal.Services;

namespace CampusSignal.ViewModels;

/// <summary>
/// 프로그램 충돌 판정의 입력값을 넣는 화면.
/// 수강신청 기능이 아니라는 점을 화면에서 명시한다.
/// </summary>
public sealed class TimetableViewModel : ScreenViewModel
{
    public const int FirstHour = 9;
    public const int LastHour = 19;

    private readonly List<ClassBlock> _classes;
    private string _newName = "";
    private string _newDay = "";
    private string _newPeriod = "";
    private string _importStatus = "";

    public TimetableViewModel(AppServices services, IAppNavigator nav) : base(services, nav)
    {
        _classes = [.. Store.Classes];
        AddClassCommand = new RelayCommand(AddClass, CanAddClass);
        Rebuild();
    }

    public override ScreenKind Kind => ScreenKind.Timetable;
    public override string Title => "시간표";
    public override string Subtitle => "프로그램 충돌 판정과 마감 일정 계산에만 사용합니다";

    public IReadOnlyList<string> Weekdays { get; } = ["월", "화", "수", "목", "금"];

    public IReadOnlyList<string> Hours { get; } =
        [.. Enumerable.Range(FirstHour, LastHour - FirstHour).Select(h => $"{h:00}:00")];

    public ObservableCollection<ClassBlockViewModel> Blocks { get; } = [];

    /// <summary>격자 배경용 빈 칸 (5일 × 10시간).</summary>
    public IReadOnlyList<int> Cells { get; } = [.. Enumerable.Range(0, 5 * (LastHour - FirstHour))];

    public ICommand AddClassCommand { get; }

    public string DropZoneTitle => "시간표 파일 업로드";
    public string DropZoneBody => "포털에서 내려받은 xlsx · 캡처 이미지\n끌어다 놓으세요";

    public string Disclaimer =>
        "시간표는 프로그램 충돌 판정과 마감 일정 계산에만 사용됩니다. 수강신청 기능은 제공하지 않습니다.";

    public string NewName { get => _newName; set => Set(ref _newName, value); }
    public string NewDay { get => _newDay; set => Set(ref _newDay, value); }
    public string NewPeriod { get => _newPeriod; set => Set(ref _newPeriod, value); }

    public string ImportStatus { get => _importStatus; set => Set(ref _importStatus, value); }

    /// <summary>드롭존이 파일을 받았을 때. 실제 파서(xlsx · OCR)는 후속 작업이다.</summary>
    public void ImportFiles(IEnumerable<string> paths)
    {
        var names = paths.Select(Path.GetFileName).Where(n => n is not null).ToList();
        ImportStatus = names.Count == 0
            ? ""
            : $"{names[0]} 외 {Math.Max(0, names.Count - 1)}건을 읽는 중… (파서 연동 예정)";
    }

    private bool CanAddClass() =>
        !string.IsNullOrWhiteSpace(NewName) &&
        Weekdays.Contains(NewDay.Trim()) &&
        TryParsePeriod(NewPeriod, out _, out _);

    private void AddClass()
    {
        if (!TryParsePeriod(NewPeriod, out var start, out var end)) return;

        _classes.Add(new ClassBlock
        {
            Name = NewName.Trim(),
            Room = "미지정",
            DayIndex = Weekdays.ToList().IndexOf(NewDay.Trim()),
            StartHour = start,
            EndHour = end,
            ToneIndex = _classes.Count,
        });

        NewName = "";
        NewDay = "";
        NewPeriod = "";
        Rebuild();
    }

    /// <summary>"10-12" · "10~12" · "10" (1시간) 형태를 받는다.</summary>
    private static bool TryParsePeriod(string text, out int start, out int end)
    {
        start = end = 0;
        if (string.IsNullOrWhiteSpace(text)) return false;

        var parts = text.Split(['-', '~', '–'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (!int.TryParse(parts[0], out start)) return false;

        end = parts.Length > 1 && int.TryParse(parts[1], out var e) ? e : start + 1;

        return start >= FirstHour && end <= LastHour && end > start;
    }

    private void Rebuild()
    {
        Blocks.Clear();
        foreach (var c in _classes)
        {
            Blocks.Add(new ClassBlockViewModel
            {
                Name = c.Name,
                Room = c.Room,
                Row = c.StartHour - FirstHour,
                RowSpan = c.EndHour - c.StartHour,
                Column = c.DayIndex,
                Fill = Palette.ClassFill(c.ToneIndex),
                Bar = Palette.ClassBar(c.ToneIndex),
            });
        }
    }
}
