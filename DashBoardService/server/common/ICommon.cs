using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using System;
using System.Collections.Generic;

namespace DashBoardService.server.common
{
    public interface ICommon
    {
        dynamic getFullList(List<dynamic> list);
        dynamic getMaxPoints(List<dynamic> list);
        dynamic getMinPoints(List<dynamic> list);
        dynamic getAveragePoints(List<dynamic> list);  
        dynamic getListCenter(RqGrafana rq);
        dynamic getListTTVT();
        List<Unit> getListDoiVT();
        dynamic getTargetList(dynamic data);
        (string, string) convertToString(RqGrafana rq);
        (string, string) convertThisYearToString(RqGrafana rq);
        dynamic convertToUnix(DateTime time);
        dynamic convertMonthToUnix(long month, long year);
        dynamic convertDayToUnix(long day, long month, long year);
        dynamic GetIso8601WeekOfYear(DateTime time);
        dynamic GetMonthsBetween(DateTime from, DateTime to);
        IEnumerable<DateTime> EachDay(string fromDate, string toDate);
        IEnumerable<long> EachUnixDay(string fromDate, string toDate);
    }

    public static partial class DateTimeExtensions
    {
        public static DateTime FirstDayOfWeek(this DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-diff).Date;
        }

        public static DateTime LastDayOfWeek(this DateTime dt) =>
            dt.FirstDayOfWeek().AddDays(6);

        public static DateTime FirstDayOfMonth(this DateTime dt) =>
            new DateTime(dt.Year, dt.Month, 1);

        public static DateTime LastDayOfMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);

        public static DateTime FirstDayOfNextMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1);
    }
}
