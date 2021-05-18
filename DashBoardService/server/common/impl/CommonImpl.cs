using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.origanization;

namespace DashBoardService.server.common.impl
{
    public class CommonImpl : ICommon
    {
        private IOrganization m_organization;
        public CommonImpl(IOrganization organization)
        {
            m_organization = organization;
        }

        public List<dynamic> Sort(List<dynamic> input, string property)
        {
            return input.OrderBy(p => p.GetType()
                                       .GetProperty(property)
                                       .GetValue(p, null)).ToList();
        }

        public dynamic getTargetList(dynamic data)
        {
            var donvi_cha_id = data.donvi_cha_id.ToString();

            return donvi_cha_id;
        }

        public (string, string) convertThisYearToString(RqGrafana rq)
        {
            var date = Convert.ToDateTime(rq.range.to);
            DateTime tngay = Convert.ToDateTime(date.Year.ToString() + "-01-01");
            DateTime dngay = Convert.ToDateTime(date.Year.ToString() + "-12-31");
            var vtungay = (tngay.Day < 10 ? ("0" + tngay.Day.ToString()) : tngay.Day.ToString()) +
                    "/" + (tngay.Month < 10 ? "0" + tngay.Month.ToString() : tngay.Month.ToString()) + "/" + tngay.Year.ToString();
            var vdenngay = (dngay.Day < 10 ? ("0" + dngay.Day.ToString()) : dngay.Day.ToString()) +
                "/" + (dngay.Month < 10 ? "0" + dngay.Month.ToString() : dngay.Month.ToString()) + "/" + dngay.Year.ToString();
            return (vtungay, vdenngay);
        }

        public (string, string) convertToString(RqGrafana rq)
        {
            DateTime tngay = Convert.ToDateTime(rq.range.from);
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            var vtungay = (tngay.Day < 10 ? ("0" + tngay.Day.ToString()) : tngay.Day.ToString()) +
                    "/" + (tngay.Month < 10 ? "0" + tngay.Month.ToString() : tngay.Month.ToString()) + "/" + tngay.Year.ToString();
            var vdenngay = (dngay.Day < 10 ? ("0" + dngay.Day.ToString()) : dngay.Day.ToString()) +
                "/" + (dngay.Month < 10 ? "0" + dngay.Month.ToString() : dngay.Month.ToString()) + "/" + dngay.Year.ToString();
            return (vtungay, vdenngay);
        }

        public dynamic convertToUnix(DateTime time)
        {
            DateTime utc = time.ToUniversalTime();
            long unix = ((DateTimeOffset)utc).ToUnixTimeMilliseconds();
            return unix;
        }

        public dynamic convertMonthToUnix(long month, long year)
        {
            var date = year.ToString() + "-" + (month < 10 ? "0" + month.ToString() : month.ToString()) + "-" + "01";
            DateTime utc = DateTime.Parse(date).ToUniversalTime();
            long unix = ((DateTimeOffset)utc).ToUnixTimeMilliseconds();
            return unix;
        }

        public dynamic convertDayToUnix(long day, long month, long year)
        {
            var date = year.ToString() + "-" + (month < 10 ? "0" + month.ToString() : month.ToString()) + "-" + (day < 10 ? "0" + day.ToString() : day.ToString());
            DateTime utc = DateTime.Parse(date).ToUniversalTime();
            long unix = ((DateTimeOffset)utc).ToUnixTimeMilliseconds();
            return unix;
        }

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public dynamic GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public dynamic getListCenter(RqGrafana rq)
        {
            UnitRequest type_center = new UnitRequest();
            type_center.target = (string)rq.targets[0].data.center;
            List<Unit> listUnit = m_organization.getAllCenter(type_center);
            return listUnit;
        }

        public dynamic getListTTVT()
        {
            List<Unit> listUnit = m_organization.getAllTTVT();
            return listUnit;
        }

        public List<Unit> getListDoiVT()
        {
            List<Unit> listUnit = m_organization.getAllDoiVT();
            return listUnit;
        }

        public dynamic getMaxPoints(List<dynamic> list)
        {
            List<dynamic> max_points = new List<dynamic>();
            for (var i = 0; i < list[0].datapoints.Count; i++)
            {
                var max = list[0].datapoints[i][0];
                var date = list[0].datapoints[i][1];
                for (var j = 0; j < list.Count; j++)
                {
                    if (max < list[j].datapoints[i][0])
                    {
                        max = list[j].datapoints[i][0];
                    }
                }
                max_points.Add(new List<dynamic> { max, date });
            }
            return max_points;
        }

        public dynamic getMinPoints(List<dynamic> list)
        {
            List<dynamic> min_points = new List<dynamic>();
            for (var i = 0; i < list[0].datapoints.Count; i++)
            {
                var min = list[0].datapoints[i][0];
                var date = list[0].datapoints[i][1];
                for (var j = 0; j < list.Count; j++)
                {
                    if (min > list[j].datapoints[i][0])
                    {
                        min = list[j].datapoints[i][0];
                    }
                }
                min_points.Add(new List<dynamic> { min, date });
            }
            return min_points;
        }

        public dynamic getAveragePoints(List<dynamic> list)
        {
            //------ Average -------
            List<dynamic> avr_points = new List<dynamic>();
            for (var i = 0; i < list[0].datapoints.Count; i++)
            {
                var sum = 0;
                var date = list[0].datapoints[i][1];
                for (var j = 0; j < list.Count; j++)
                {
                    sum += list[j].datapoints[i][0];
                }
                var avr = sum / list.Count;
                avr_points.Add(new List<dynamic> { avr, date });
            }
            return avr_points;
        }

        public dynamic getFullList(List<dynamic> list)
        {
            List<dynamic> fullList = new List<dynamic>();
            foreach (var arr in list)
            {
                List<dynamic> dates = arr;
                var combine = fullList.Union(dates).ToList();
                fullList = combine;
            }
            return fullList;
        }

        public dynamic GetMonthsBetween(DateTime from, DateTime to)
        {
            if (from > to) return GetMonthsBetween(to, from);

            var monthDiff = Math.Abs((to.Year * 12 + (to.Month - 1)) - (from.Year * 12 + (from.Month - 1)));

            if (from.AddMonths(monthDiff) > to || to.Day < from.Day)
            {
                monthDiff -= 1;
            }

            List<dynamic> results = new List<dynamic>();
            for (int i = monthDiff; i >= 0; i--)
            {
                results.Add(to.AddMonths(-i));
            }

            return results;
        }

        public IEnumerable<DateTime> EachDay(string fromDate, string toDate)
        {
            DateTime from = Convert.ToDateTime(fromDate);
            DateTime to = Convert.ToDateTime(toDate);
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return day;
        }

        public IEnumerable<long> EachUnixDay(string fromDate, string toDate)
        {
            DateTime from = Convert.ToDateTime(fromDate);
            DateTime to = Convert.ToDateTime(toDate);
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return convertDayToUnix(day.Day, day.Month, day.Year);
        }
    }
}
