using CampusSignal.Infrastructure;
using CampusSignal.Models;
using CampusSignal.Services;

namespace CampusSignal.ViewModels;

public abstract class ScreenViewModel(AppServices services, IAppNavigator nav) : ObservableObject
{
    protected AppServices Services { get; } = services;
    protected IAppNavigator Nav { get; } = nav;
    protected SampleDataStore Store => Services.Store;
    protected NoticeCatalog Catalog => Services.Catalog;

    public abstract ScreenKind Kind { get; }
    public abstract string Title { get; }
    public abstract string Subtitle { get; }

    /// <summary>화면으로 들어올 때마다 호출된다. 파생 값을 다시 만드는 자리.</summary>
    public virtual void Activate() { }
}
