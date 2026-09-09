using CampusSignal.Models;

namespace CampusSignal.ViewModels;

/// <summary>화면 전환은 셸이 소유한다. 각 화면 VM 은 이 인터페이스만 안다.</summary>
public interface IAppNavigator
{
    void Go(ScreenKind screen);
    void OpenNotice(string noticeId);
    void ShowToast(ToastMessage message);
    void HideToast();
}
