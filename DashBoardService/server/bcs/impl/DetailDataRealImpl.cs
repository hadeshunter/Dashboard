using ClassModel.connnection.reponsitory.impl;
using ClassModel.connnection.sql;
using ClassModel.model.bsc;
using ClassModel.model.realIncrease;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.bcs.impl
{
    public class DetailDataRealImpl : Reponsitory<DetailDataReal>, IDetailDataReal
    {
        private ICommon m_common;
        private IDetail_lapmoi m_detail_lapmoi;
        private IDetail_go m_detail_go;
        public DetailDataRealImpl(ICommon common, IDetail_lapmoi detail_lapmoi, IDetail_go detail_go, DataContext context) : base(context)
        {
            m_common = common;
            m_detail_lapmoi = detail_lapmoi;
            m_detail_go = detail_go;
        }

        public dynamic getConfigByDate(RqGrafana rq)
        {
            List<Detail_lapmoi> newList = m_detail_lapmoi.executeDetailLapmoi(rq);
            List<dynamic> temp_LD = new List<dynamic>();
            dynamic newListByDate;
            switch ((string)rq.targets[0].data.time_scale)
            {
                case "week":
                    foreach (var element in newList)
                    {
                        int week = m_common.GetIso8601WeekOfYear(element.ngaycn_bbbg);
                        var lastDayWeek = element.ngaycn_bbbg.LastDayOfWeek();
                        temp_LD.Add(new { element.donvi_id, element.ngaycn_bbbg, week, lastDayWeek });
                    }
                    newListByDate = temp_LD
                                    .OrderBy(ele => (ele.donvi_id, ele.ngaycn_bbbg))
                                    .GroupBy(l => new
                                    {
                                        l.lastDayWeek.Day,
                                        l.lastDayWeek.Month,
                                        l.lastDayWeek.Year,
                                        l.donvi_id
                                    })
                                    .Select(lg =>
                                       new
                                       {
                                           lg.Key.Day,
                                           lg.Key.Month,
                                           lg.Key.Year,
                                           lg.Key.donvi_id,
                                           sl_lapdat = lg.Count(),
                                           unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                                       });
                    break;
                case "month":
                    newListByDate = newList
                            .OrderBy(ele => (ele.donvi_id, ele.ngaycn_bbbg))
                            .GroupBy(l => new { l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_id })
                            .Select(lg =>
                                new
                                {
                                    lg.Key.Month,
                                    lg.Key.Year,
                                    lg.Key.donvi_id,
                                    sl_lapdat = lg.Count(),
                                    unix_date = m_common.convertMonthToUnix(lg.Key.Month, lg.Key.Year)
                                });
                    break;
                case "year":
                    newListByDate = newList
                            .OrderBy(ele => (ele.donvi_id, ele.ngaycn_bbbg))
                            .GroupBy(l => new { l.ngaycn_bbbg.Year, l.donvi_id })
                            .Select(lg =>
                                new
                                {
                                    lg.Key.Year,
                                    lg.Key.donvi_id,
                                    sl_lapdat = lg.Count(),
                                    unix_date = m_common.convertDayToUnix(31, 12, lg.Key.Year)
                                });
                    break;
                default:
                    newListByDate = newList
                            .OrderBy(ele => (ele.donvi_id, ele.ngaycn_bbbg))
                            .GroupBy(l => new { l.ngaycn_bbbg.Day, l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_id })
                            .Select(lg =>
                                new
                                {
                                    lg.Key.Day,
                                    lg.Key.Month,
                                    lg.Key.Year,
                                    lg.Key.donvi_id,
                                    sl_lapdat = lg.Count(),
                                    unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                                });
                    break;
            }
            List<dynamic> list = new List<dynamic>(newListByDate);
            return list;
        }

        public dynamic getRemoveByDate(RqGrafana rq)
        {
            List<Detail_go> removeList = m_detail_go.execureDetailgo(rq);
            dynamic removeListByDate;
            switch ((string)rq.targets[0].data.time_scale)
            {
                case "week":
                    List<dynamic> temp_RM = new List<dynamic>();
                    foreach (var element in removeList)
                    {
                        int week = m_common.GetIso8601WeekOfYear(element.ngaycn_bbbg);
                        var lastDayWeek = element.ngaycn_bbbg.LastDayOfWeek();
                        temp_RM.Add(new { element.donvi_id, element.ngaycn_bbbg, week, lastDayWeek });
                    }
                    removeListByDate = temp_RM
                        .OrderBy(ele => (ele.donvi_id, ele.ngaycn_bbbg))
                        .GroupBy(l => new
                        {
                            l.lastDayWeek.Day,
                            l.lastDayWeek.Month,
                            l.lastDayWeek.Year,
                            l.donvi_id
                        })
                        .Select(lg =>
                           new
                           {
                               lg.Key.Day,
                               lg.Key.Month,
                               lg.Key.Year,
                               lg.Key.donvi_id,
                               sl_huy = lg.Count(),
                               unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                           });
                    break;
                case "month":
                    removeListByDate = removeList
                    .OrderBy(ele => (ele.donvi_id, ele.ngaycn_bbbg))
                    .GroupBy(l => new { l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_id })
                    .Select(lg =>
                       new
                       {
                           lg.Key.Month,
                           lg.Key.Year,
                           lg.Key.donvi_id,
                           sl_huy = lg.Count(),
                           unix_date = m_common.convertMonthToUnix(lg.Key.Month, lg.Key.Year)
                       });
                    break;
                case "year":
                    removeListByDate = removeList
                            .OrderBy(ele => (ele.donvi_id, ele.ngaycn_bbbg))
                            .GroupBy(l => new { l.ngaycn_bbbg.Year, l.donvi_id })
                            .Select(lg =>
                                new
                                {
                                    lg.Key.Year,
                                    lg.Key.donvi_id,
                                    sl_huy = lg.Count(),
                                    unix_date = m_common.convertDayToUnix(31, 12, lg.Key.Year)
                                });
                    break;
                default:
                    removeListByDate = removeList
                    .OrderBy(ele => (ele.donvi_id, ele.ngaycn_bbbg))
                    .GroupBy(l => new { l.ngaycn_bbbg.Day, l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_id })
                    .Select(lg =>
                       new
                       {
                           lg.Key.Day,
                           lg.Key.Month,
                           lg.Key.Year,
                           lg.Key.donvi_id,
                           sl_huy = lg.Count(),
                           unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                       });
                    break;
            }
            List<dynamic> list = new List<dynamic>(removeListByDate);
            return list;
        }

        public dynamic getEqualLengthActual(List<dynamic> list, List<dynamic> list_date)
        {
            List<dynamic> new_list = new List<dynamic>();
            foreach (List<dynamic> data in list)
            {
                foreach (var date in list_date)
                {
                    var filter = data.Where(r => r.unix_date == date).FirstOrDefault();
                    if (filter == null)
                    {
                        data.Add(new { data[0].donvi_id, data[0].ten_dv, sl_lapdat = 0, sl_huy = 0, unix_date = date });
                    }
                }
                List<dynamic> sorted = new List<dynamic>(data.OrderBy(r => r.unix_date));
                new_list.Add(sorted);
            }
            return new_list;
        }

        public dynamic getAveragePercent(List<dynamic> list)
        {
            //------ Average -------
            List<dynamic> avr_points = new List<dynamic>();
            for (var i = 0; i < list[0].Count; i++)
            {
                double configSum = 0.0000;
                double removeSum = 0.0000;
                var date = list[0][i].unix_date;
                for (var j = 0; j < list.Count; j++)
                {
                    configSum += list[j][i].sl_lapdat;
                    removeSum += list[j][i].sl_huy;
                }
                double avr = Math.Round((configSum - removeSum) * 100 / configSum, 6);
                avr_points.Add(new List<dynamic> { avr, date });
            }
            return avr_points;
        }

        public dynamic getTimeseriesActual(List<dynamic> list)
        {
            List<dynamic> total_timeseries = new List<dynamic>();
            foreach (List<dynamic> data in list)
            {
                List<dynamic> datapoints = new List<dynamic>();
                foreach (var element in data)
                {
                    double percent_actual = Math.Round((double)(element.sl_lapdat - element.sl_huy) * 100 / element.sl_lapdat, 6);
                    datapoints.Add(new List<dynamic> { percent_actual, element.unix_date });
                }
                total_timeseries.Add(new { target = data[0].ten_dv, datapoints });
            }
            return total_timeseries;
        }

        public dynamic getActualPercent(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            if (rq.targets[0].type == "timeseries")
            {
                List<dynamic> total_date = new List<dynamic>();
                List<dynamic> total = new List<dynamic>();
                List<dynamic> total_timeseries = new List<dynamic>();
                List<Unit> listUnit = m_common.getListCenter(rq);
                List<dynamic> configListByDate = getConfigByDate(rq);
                List<dynamic> removeListByDate = getRemoveByDate(rq);
                foreach (var unit in listUnit)
                {
                    List<dynamic> data = new List<dynamic>();
                    List<dynamic> lst_config = new List<dynamic>(configListByDate.FindAll(r => r.donvi_id == unit.donvi_id));
                    List<dynamic> lst_remove = new List<dynamic>(removeListByDate.FindAll(r => r.donvi_id == unit.donvi_id));
                    List<dynamic> lst_config_date = new List<dynamic>(lst_config.Select(lg => lg.unix_date));
                    List<dynamic> lst_remove_date = new List<dynamic>(lst_remove.Select(lg => lg.unix_date));
                    List<dynamic> combine_date = lst_config_date.Union(lst_remove_date).ToList();
                    foreach (var date in combine_date)
                    {
                        var config_point = lst_config.Where(r => r.unix_date == date).FirstOrDefault();
                        var remove_point = lst_remove.Where(r => r.unix_date == date).FirstOrDefault();
                        int sl_lapdat = 0;
                        int sl_huy = 0;
                        if (config_point != null)
                        {
                            sl_lapdat = config_point.sl_lapdat;
                        }
                        if (remove_point != null)
                        {
                            sl_huy = remove_point.sl_huy;
                        }
                        data.Add(new { unit.donvi_id, unit.ten_dv, sl_lapdat, sl_huy, unix_date = date });
                    }
                    total_date.Add(combine_date);
                    total.Add(data);
                }
                total_date = m_common.getFullList(total_date);
                total = getEqualLengthActual(total, total_date);
                total_timeseries = getTimeseriesActual(total);
                if (rq.targets[0].data.graph == "pie_chart")
                {
                    response = total_timeseries;
                }
                else
                {
                    List<dynamic> avr_points = getAveragePercent(total);
                    List<dynamic> max_points = m_common.getMaxPoints(total_timeseries);
                    List<dynamic> min_points = m_common.getMinPoints(total_timeseries);
                    response.Add(new { target = "GT MAX", datapoints = max_points });
                    response.Add(new { target = "GT trung bình", datapoints = avr_points });
                    response.Add(new { target = "GT MIN", datapoints = min_points });
                }
            }
            else if (rq.targets[0].type == "table")
            {
                var time_scale = (string)rq.targets[0].data.time_scale;
                List<dynamic> result = new List<dynamic>();
                List<dynamic> col = new List<dynamic>
                    {
                        new { text = "Tên trung tâm", type = "string"}, //String thì ko hiện lên chart
                        new { text = "Phát triển mới", type = "number"},
                        new { text = "Khôi phục", type = "number"},
                        new { text = "Lắp đặt", type = "number"}, //Number hiện sô liệu lên chart
                        new { text = "Hủy", type = "number"},
                        new { text = "PSC TT/PTM", type = "number"},
                        new { text = "Tỷ lệ Hủy/PTM", type = "number"},
                        new { text = "Thời gian", type =  time_scale == "month" ? "string" : "time"}, //Phải khai báo type = "time" để cột thời gian nhận unixtime mili giây
                    };
                List<dynamic> row = new List<dynamic>();
                List<Unit> listUnit = m_common.getListCenter(rq);
                //PTM
                List<dynamic> ptm = m_detail_lapmoi.getPTMByDate(rq);
                //KP
                List<dynamic> kp = m_detail_lapmoi.getComeBackByDate(rq);
                //LD
                List<dynamic> ld = m_detail_lapmoi.getConfigByDate(rq);
                //Huy
                List<dynamic> remove = getRemoveByDate(rq);
                List<dynamic> data = new List<dynamic>();
                foreach (var unit in listUnit)
                {
                    List<dynamic> lst_ptm = new List<dynamic>(ptm.FindAll(r => r.donvi_id == unit.donvi_id));
                    List<dynamic> lst_comeback = new List<dynamic>(kp.FindAll(r => r.donvi_id == unit.donvi_id));
                    List<dynamic> lst_ld = new List<dynamic>(ld.FindAll(r => r.donvi_id == unit.donvi_id));
                    List<dynamic> lst_remove = new List<dynamic>(remove.FindAll(r => r.donvi_id == unit.donvi_id));
                    List<dynamic> lst_ptm_date = new List<dynamic>(lst_ptm.Select(lg => lg.unix_date));
                    List<dynamic> lst_comeback_date = new List<dynamic>(lst_comeback.Select(lg => lg.unix_date));
                    List<dynamic> lst_ld_date = new List<dynamic>(lst_ld.Select(lg => lg.unix_date));
                    List<dynamic> lst_remove_date = new List<dynamic>(lst_remove.Select(lg => lg.unix_date));
                    List<dynamic> combine_date = lst_ptm_date.Union(lst_comeback_date).Union(lst_ld_date).Union(lst_remove_date).ToList();
                    foreach (var date in combine_date)
                    {
                        var ptm_point = lst_ptm.Where(r => r.unix_date == date).FirstOrDefault();
                        var comeback_point = lst_comeback.Where(r => r.unix_date == date).FirstOrDefault();
                        var ld_point = lst_ld.Where(r => r.unix_date == date).FirstOrDefault();
                        var remove_point = lst_remove.Where(r => r.unix_date == date).FirstOrDefault();
                        int sl_ptm = 0;
                        int sl_kp = 0;
                        int sl_lapdat = 0;
                        int sl_huy = 0;
                        if (ptm_point != null)
                        {
                            sl_ptm = ptm_point.sl_ptm;
                        }
                        if (comeback_point != null)
                        {
                            sl_kp = comeback_point.sl_kp;
                        }
                        if (ld_point != null)
                        {
                            sl_lapdat = ld_point.sl_lapdat;
                        }
                        if (remove_point != null)
                        {
                            sl_huy = remove_point.sl_huy;
                        }
                        data.Add(new
                        {
                            unit.ten_dv,
                            sl_ptm,
                            sl_kp,
                            sl_lapdat,
                            sl_huy,
                            actual_percent = Math.Round((double)(sl_lapdat - sl_huy) / sl_lapdat, 6),
                            remove_ld_percent = Math.Round((double)sl_huy / sl_lapdat, 6),
                            unix_date = date
                        });
                    }
                }

                List<dynamic> sorted = new List<dynamic>(data.OrderBy(r => r.unix_date));

                foreach (var element in sorted)
                {
                    var local_time = DateTimeOffset.FromUnixTimeMilliseconds(element.unix_date).LocalDateTime;
                    string date = local_time.Month.ToString() + "/" + local_time.Year.ToString();
                    row.Add(new List<dynamic> {
                            element.ten_dv,
                            element.sl_ptm,
                            element.sl_kp,
                            element.sl_lapdat,
                            element.sl_huy,
                            element.actual_percent,
                            element.remove_ld_percent,
                            time_scale == "month" ? date : element.unix_date
                        });
                }

                response = new List<dynamic> {
                        new {
                                columns = col,
                                rows = row,
                                type = "table"
                            }
                    };
            }
            return response;
        }
    }
}
