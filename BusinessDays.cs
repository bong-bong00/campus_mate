namespace CampusSignal.Services;

/// <summary>착수일 역산에 쓰는 영업일 계산. 주말만 제외한다(공휴일 달력은 후속 과제).</summary>
public static class BusinessDays
{
    public static bool IsBusinessDay(DateTime d) =>
        d.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday);

    public static DateTime Subtract(DateTime from, int businessDays)
    {
        var d = from.Date;
        var left = businessDays;
        while (left > 0)
        {
            d = d.AddDays(-1);
            if (IsBusinessDay(d)) left--;
        }
        return d;
    }

    public static DateTime Add(DateTime from, int businessDays)
    {
        var d = from.Date;
        var left = businessDays;
        while (left > 0)
        {
            d = d.AddDays(1);
            if (IsBusinessDay(d)) left--;
        }
        return d;
    }

    public static string KoreanDay(DateTime d) => d.DayOfWeek switch
    {
        DayOfWeek.Monday => "월",
        DayOfWeek.Tuesday => "화",
        DayOfWeek.Wednesday => "수",
        DayOfWeek.Thursday => "목",
        DayOfWeek.Friday => "금",
        DayOfWeek.Saturday => "토",
        _ => "일",
    };
}
