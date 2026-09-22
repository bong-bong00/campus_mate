using System.Windows;
using CampusSignal.Services;
using CampusSignal.ViewModels;

namespace CampusSignal;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 표본 데이터가 핸드오프 문서와 같은 날짜로 읽히도록 시계를 고정해 둔다.
        // 실제 수집기를 붙이면 new SystemClock() 으로 바꾼다.
        var services = new AppServices(new FixedClock(new DateTime(2025, 10, 13)));

        new MainWindow { DataContext = new ShellViewModel(services) }.Show();
    }
}
