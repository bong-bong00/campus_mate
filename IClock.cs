namespace CampusSignal.Services;

public interface IClock
{
    DateTime Today { get; }
}

/// <summary>운영용 시계.</summary>
public sealed class SystemClock : IClock
{
    public DateTime Today => DateTime.Today;
}

/// <summary>
/// 샘플 데이터가 핸드오프 문서와 같은 날짜(2025-10-13)를 기준으로 읽히도록 고정한 시계.
/// 실제 수집기를 붙일 때 App.xaml.cs 에서 <see cref="SystemClock"/> 로 바꾸면 된다.
/// </summary>
public sealed class FixedClock(DateTime today) : IClock
{
    public DateTime Today { get; } = today.Date;
}
