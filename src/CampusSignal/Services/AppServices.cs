namespace CampusSignal.Services;

/// <summary>앱 전체가 공유하는 서비스 묶음. DI 컨테이너를 붙이기 전까지의 조립 지점.</summary>
public sealed class AppServices
{
    public AppServices(IClock clock)
    {
        Clock = clock;
        Store = new SampleDataStore();
        Engine = new JudgmentEngine(clock) { ExamPeriod = Store.ExamPeriod };
        Catalog = new NoticeCatalog(Store, Engine, clock);
    }

    public IClock Clock { get; }
    public SampleDataStore Store { get; }
    public JudgmentEngine Engine { get; }
    public NoticeCatalog Catalog { get; }
}
