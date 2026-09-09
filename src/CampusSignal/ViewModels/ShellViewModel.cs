using System.Windows.Input;
using CampusSignal.Infrastructure;
using CampusSignal.Models;
using CampusSignal.Services;

namespace CampusSignal.ViewModels;

/// <summary>단일 창 / 뷰 스위칭 구조의 셸. 헤더 · 내비게이션 · 토스트를 소유한다.</summary>
public sealed class ShellViewModel : ObservableObject, IAppNavigator
{
    private readonly AppServices _services;
    private readonly Dictionary<ScreenKind, ScreenViewModel> _cache = [];

    private ScreenViewModel _current = null!;
    private ToastMessage? _toast;

    public ShellViewModel(AppServices services)
    {
        _services = services;

        NavigateCommand = new RelayCommand(p =>
        {
            if (p is ScreenKind s) Go(s);
        });

        HideToastCommand = new RelayCommand(HideToast);
        ToastPrimaryCommand = new RelayCommand(OnToastPrimary);

        NavItems =
        [
            new NavItemViewModel("대시보드", ScreenKind.Dashboard, NavigateCommand),
            new NavItemViewModel("공고", ScreenKind.NoticeList, NavigateCommand),
            new NavItemViewModel("혜택", ScreenKind.Benefits, NavigateCommand),
            new NavItemViewModel("캘린더", ScreenKind.Calendar, NavigateCommand),
            new NavItemViewModel("시간표", ScreenKind.Timetable, NavigateCommand),
            new NavItemViewModel("프로필", ScreenKind.Profile, NavigateCommand),
            new NavItemViewModel("알림", ScreenKind.Notifications, NavigateCommand),
        ];

        _services.Catalog.Changed += RefreshNavBadges;
        RefreshNavBadges();
        Go(ScreenKind.Dashboard);
    }

    public IReadOnlyList<NavItemViewModel> NavItems { get; }
    public ICommand NavigateCommand { get; }
    public ICommand HideToastCommand { get; }
    public ICommand ToastPrimaryCommand { get; }

    public string BrandMark => "CAMPUS SIGNAL";

    public ScreenViewModel Current
    {
        get => _current;
        private set
        {
            var previous = _current;
            if (!Set(ref _current, value)) return;

            // 화면이 스스로 제목·부제를 다시 계산하면 헤더도 따라가야 한다.
            if (previous is not null) previous.PropertyChanged -= OnScreenPropertyChanged;
            _current.PropertyChanged += OnScreenPropertyChanged;

            Raise(nameof(PageTitle));
            Raise(nameof(PageSubtitle));
        }
    }

    private void OnScreenPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ScreenViewModel.Title)) Raise(nameof(PageTitle));
        if (e.PropertyName is nameof(ScreenViewModel.Subtitle)) Raise(nameof(PageSubtitle));
    }

    public string PageTitle => Current.Title;
    public string PageSubtitle => Current.Subtitle;

    // ── 토스트 ────────────────────────────────────────────────────────────
    public ToastMessage? Toast
    {
        get => _toast;
        private set
        {
            if (Set(ref _toast, value)) Raise(nameof(IsToastVisible));
        }
    }

    public bool IsToastVisible => Toast is not null;

    public void ShowToast(ToastMessage message) => Toast = message;

    public void HideToast() => Toast = null;

    private void OnToastPrimary()
    {
        var id = Toast?.NoticeId;
        HideToast();
        if (id is not null) OpenNotice(id);
    }

    // ── 내비게이션 ────────────────────────────────────────────────────────
    public void Go(ScreenKind screen)
    {
        if (!_cache.TryGetValue(screen, out var vm))
        {
            vm = Create(screen);
            _cache[screen] = vm;
        }

        vm.Activate();
        Current = vm;
        UpdateActiveTab(screen);
    }

    public void OpenNotice(string noticeId)
    {
        // 상세는 목록의 하위 뷰다. 공고마다 새로 만들되 활성 탭은 "공고"를 유지한다.
        var vm = new NoticeDetailViewModel(_services, this, noticeId);
        _cache[ScreenKind.NoticeDetail] = vm;
        Current = vm;
        UpdateActiveTab(ScreenKind.NoticeDetail);
    }

    private ScreenViewModel Create(ScreenKind screen) => screen switch
    {
        ScreenKind.Dashboard => new DashboardViewModel(_services, this),
        ScreenKind.NoticeList => new NoticeListViewModel(_services, this),
        ScreenKind.Benefits => new BenefitsViewModel(_services, this),
        ScreenKind.Calendar => new CalendarViewModel(_services, this),
        ScreenKind.Timetable => new TimetableViewModel(_services, this),
        ScreenKind.Profile => new ProfileViewModel(_services, this),
        ScreenKind.Notifications => new NotificationsViewModel(_services, this),
        _ => throw new ArgumentOutOfRangeException(nameof(screen)),
    };

    private void UpdateActiveTab(ScreenKind screen)
    {
        // 공고 상세를 보는 동안에도 "공고" 탭이 활성 상태를 유지한다.
        var active = screen == ScreenKind.NoticeDetail ? ScreenKind.NoticeList : screen;
        foreach (var item in NavItems) item.IsActive = item.Screen == active;
    }

    private void RefreshNavBadges()
    {
        NavItems[1].SetLabel($"공고 {_services.Catalog.All.Count}");
        NavItems[6].SetLabel($"알림 {_services.Store.Alerts.Count(a => a.IsUnread)}");

        Raise(nameof(PageTitle));
        Raise(nameof(PageSubtitle));
    }
}
