using System.Windows.Input;
using CampusSignal.Infrastructure;
using CampusSignal.Models;
using CampusSignal.Services;

namespace CampusSignal.ViewModels;

/// <summary>놓치면 안 되는 순간(신규·마감·착수·변경·충돌)을 모아 보는 화면.</summary>
public sealed class NotificationsViewModel : ScreenViewModel
{
    public NotificationsViewModel(AppServices services, IAppNavigator nav) : base(services, nav)
    {
        Rows = Store.Alerts.Select(ToRow).ToList();

        Settings = Store.NotificationSettings
            .Select(s => new SettingRowViewModel(s.Label, s.Enabled))
            .ToList();

        foreach (var s in Settings)
            s.Command = new RelayCommand(() => s.IsEnabled = !s.IsEnabled);

        PreviewToastCommand = new RelayCommand(() => Nav.ShowToast(Store.SampleToast));
    }

    public override ScreenKind Kind => ScreenKind.Notifications;
    public override string Title => $"알림 {Store.Alerts.Count}건";
    public override string Subtitle => "신규 공고 · 마감 임박 · 서류 착수 시점";

    public IReadOnlyList<AlertRowViewModel> Rows { get; }
    public IReadOnlyList<SettingRowViewModel> Settings { get; }
    public ICommand PreviewToastCommand { get; }

    public string PreviewButtonText => "Windows 토스트 미리보기";
    public string SettingsTitle => "알림 설정";

    public int UnreadCount => Store.Alerts.Count(a => a.IsUnread);

    private AlertRowViewModel ToRow(AlertItem a) => new()
    {
        Dot = Palette.AlertDot(a.Kind),
        KindText = a.KindText,
        KindBackground = Palette.AlertBg(a.Kind),
        KindForeground = Palette.AlertFg(a.Kind),
        Title = a.Title,
        Body = a.Body,
        TimeText = a.TimeText,
        RowBackground = Palette.Get(a.IsUnread ? "RowUnread" : "Surface"),
        Command = new RelayCommand(() =>
        {
            if (a.NoticeId is { } id) Nav.OpenNotice(id);
            else if (a.TargetScreen is { } screen) Nav.Go(screen);
        }),
    };
}
