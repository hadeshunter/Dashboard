using ClassModel.connnection.oracle;
using ClassModel.connnection.reponsitory.impl;
using ClassModel.connnection.sql;
using ClassModel.model.bsc;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using Dapper;
using DashBoardService.server.common;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.bcs.impl
{
    public class Detail_lapmoiImpl : Reponsitory<Detail_lapmoi>, IDetail_lapmoi
    {
        private IConfiguration m_configuration;
        private ICommon m_common;
        public Detail_lapmoiImpl(DataContext context, IConfiguration configuration, ICommon common) : base(context)
        {
            m_configuration = configuration;
            m_common = common;
        }

        public IDbConnection GetConnection()
        {
            var connectionString = m_configuration.GetSection("connectionstrings").GetSection("defaultconnection2").Value;
            var conn = new OracleConnection(connectionString);
            return conn;
        }

        public dynamic executeDetailLapmoi(RqGrafana rq)
        {
            (string, string) date;
            if (rq.targets[0].data.graph == "pie_chart")
            {
                date = m_common.convertThisYearToString(rq);
            }
            else
            {
                date = m_common.convertToString(rq);
            }
            var vloaitb_id = (int)rq.scopedVars.service.value;
            List<Detail_lapmoi> result = new List<Detail_lapmoi>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, date.Item1);
            dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, date.Item2);
            dyParam.Add("vloaitb_id", OracleDbType.Int32, ParameterDirection.Input, vloaitb_id);
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "khanhnv.DASHBOARD.detail_lapmoi";
                result = SqlMapper.Query<Detail_lapmoi>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<Detail_lapmoi>();
            }
            return result;
        }

        public dynamic getNewByDate(RqGrafana rq)
        {
            List<Detail_lapmoi> newList = executeDetailLapmoi(rq);
            dynamic newListByDate;
            switch ((string)rq.targets[0].data.time_scale)
            {
                case "week":
                    List<dynamic> temp = new List<dynamic>();
                    foreach (var element in newList)
                    {
                        int week = m_common.GetIso8601WeekOfYear(element.ngaycn_bbbg);
                        var lastDayWeek = element.ngaycn_bbbg.LastDayOfWeek();
                        temp.Add(new { element.dvicha_id_tt, element.ngaycn_bbbg, week, lastDayWeek });
                    }
                    newListByDate = temp
                        .OrderBy(ele => (ele.dvicha_id_tt, ele.ngaycn_bbbg))
                        .GroupBy(l => new
                        {
                            l.lastDayWeek.Day,
                            l.lastDayWeek.Month,
                            l.lastDayWeek.Year,
                            l.dvicha_id_tt
                        })
                        .Select(lg =>
                            new
                            {
                                lg.Key.Day,
                                lg.Key.Month,
                                lg.Key.Year,
                                donvi_id = lg.Key.dvicha_id_tt,
                                sl_new = lg.Count(),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            });
                    break;
                case "month":
                    newListByDate = newList
                    .OrderBy(ele => (ele.dvicha_id_tt, ele.ngaycn_bbbg))
                    .GroupBy(l => new { l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.dvicha_id_tt })
                    .Select(lg =>
                        new
                        {
                            lg.Key.Month,
                            lg.Key.Year,
                            donvi_id = lg.Key.dvicha_id_tt,
                            sl_new = lg.Count(),
                            unix_date = m_common.convertMonthToUnix(lg.Key.Month, lg.Key.Year)
                        });
                    break;
                case "year":
                    newListByDate = newList
                    .OrderBy(ele => (ele.dvicha_id_tt, ele.ngaycn_bbbg))
                    .GroupBy(l => new { l.ngaycn_bbbg.Year, l.dvicha_id_tt })
                    .Select(lg =>
                        new
                        {
                            lg.Key.Year,
                            donvi_id = lg.Key.dvicha_id_tt,
                            sl_new = lg.Count(),
                            unix_date = m_common.convertDayToUnix(31, 12, lg.Key.Year)
                        });
                    break;
                default:
                    newListByDate = newList
                    .OrderBy(ele => (ele.dvicha_id_tt, ele.ngaycn_bbbg))
                    .GroupBy(l => new { l.ngaycn_bbbg.Day, l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.dvicha_id_tt })
                    .Select(lg =>
                        new
                        {
                            lg.Key.Day,
                            lg.Key.Month,
                            lg.Key.Year,
                            donvi_id = lg.Key.dvicha_id_tt,
                            sl_new = lg.Count(),
                            unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                        });
                    break;
            }
            List<dynamic> list = new List<dynamic>(newListByDate);
            return list;
        }

        public dynamic getPTMByDate(RqGrafana rq)
        {
            List<Detail_lapmoi> result = executeDetailLapmoi(rq);
            var ptmList = result.FindAll(r => r.loai_ptm.ToLower() == "ptm");
            dynamic ptmListByDate;
            switch ((string)rq.targets[0].data.time_scale)
            {
                case "week":
                    List<dynamic> temp = new List<dynamic>();
                    foreach (var element in ptmList)
                    {
                        int week = m_common.GetIso8601WeekOfYear(element.ngaycn_bbbg);
                        var lastDayWeek = element.ngaycn_bbbg.LastDayOfWeek();
                        temp.Add(new { element.dvicha_id_tt, element.ngaycn_bbbg, week, lastDayWeek });
                    }
                    ptmListByDate = temp
                        .OrderBy(ele => (ele.dvicha_id_tt, ele.ngaycn_bbbg))
                        .GroupBy(l => new
                        {
                            l.lastDayWeek.Day,
                            l.lastDayWeek.Month,
                            l.lastDayWeek.Year,
                            l.dvicha_id_tt
                        })
                        .Select(lg =>
                            new
                            {
                                lg.Key.Day,
                                lg.Key.Month,
                                lg.Key.Year,
                                donvi_id = lg.Key.dvicha_id_tt,
                                sl_ptm = lg.Count(),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            });
                    break;
                case "month":
                    ptmListByDate = ptmList
                        .OrderBy(ele => (ele.dvicha_id_tt, ele.ngaycn_bbbg))
                        .GroupBy(l => new { l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.dvicha_id_tt })
                        .Select(lg =>
                            new
                            {
                                lg.Key.Month,
                                lg.Key.Year,
                                donvi_id = lg.Key.dvicha_id_tt,
                                sl_ptm = lg.Count(),
                                unix_date = m_common.convertMonthToUnix(lg.Key.Month, lg.Key.Year)
                            });
                    break;
                case "year":
                    ptmListByDate = ptmList
                        .OrderBy(ele => (ele.dvicha_id_tt, ele.ngaycn_bbbg))
                        .GroupBy(l => new { l.ngaycn_bbbg.Year, l.dvicha_id_tt })
                        .Select(lg =>
                            new
                            {
                                lg.Key.Year,
                                donvi_id = lg.Key.dvicha_id_tt,
                                sl_ptm = lg.Count(),
                                unix_date = m_common.convertDayToUnix(31, 12, lg.Key.Year)
                            });
                    break;
                default:
                    ptmListByDate = ptmList
                        .OrderBy(ele => (ele.dvicha_id_tt, ele.ngaycn_bbbg))
                        .GroupBy(l => new { l.ngaycn_bbbg.Day, l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.dvicha_id_tt })
                        .Select(lg =>
                            new
                            {
                                lg.Key.Day,
                                lg.Key.Month,
                                lg.Key.Year,
                                donvi_id = lg.Key.dvicha_id_tt,
                                sl_ptm = lg.Count(),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            });
                    break;
            }
            List<dynamic> list = new List<dynamic>(ptmListByDate);
            return list;
        }

        public dynamic getComeBackByDate(RqGrafana rq)
        {
            List<Detail_lapmoi> result = executeDetailLapmoi(rq);
            var comebackList = result.FindAll(r => r.loai_ptm.ToLower() == "kp");
            dynamic comebackListByDate;
            switch ((string)rq.targets[0].data.time_scale)
            {
                case "week":
                    List<dynamic> temp = new List<dynamic>();
                    foreach (var element in comebackList)
                    {
                        int week = m_common.GetIso8601WeekOfYear(element.ngaycn_bbbg);
                        var lastDayWeek = element.ngaycn_bbbg.LastDayOfWeek();
                        temp.Add(new { element.dvicha_id_tt, element.ngaycn_bbbg, week, lastDayWeek });
                    }
                    comebackListByDate = temp
                        .OrderBy(ele => (ele.dvicha_id_tt, ele.ngaycn_bbbg))
                        .GroupBy(l => new
                        {
                            l.lastDayWeek.Day,
                            l.lastDayWeek.Month,
                            l.lastDayWeek.Year,
                            l.dvicha_id_tt
                        })
                        .Select(lg =>
                            new
                            {
                                lg.Key.Day,
                                lg.Key.Month,
                                lg.Key.Year,
                                donvi_id = lg.Key.dvicha_id_tt,
                                sl_kp = lg.Count(),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            });
                    break;
                case "month":
                    comebackListByDate = comebackList
                        .OrderBy(ele => (ele.dvicha_id_tt, ele.ngaycn_bbbg))
                        .GroupBy(l => new { l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.dvicha_id_tt })
                        .Select(lg =>
                            new
                            {
                                lg.Key.Month,
                                lg.Key.Year,
                                donvi_id = lg.Key.dvicha_id_tt,
                                sl_kp = lg.Count(),
                                unix_date = m_common.convertMonthToUnix(lg.Key.Month, lg.Key.Year)
                            });
                    break;
                case "year":
                    comebackListByDate = comebackList
                        .OrderBy(ele => (ele.dvicha_id_tt, ele.ngaycn_bbbg))
                        .GroupBy(l => new { l.ngaycn_bbbg.Year, l.dvicha_id_tt })
                        .Select(lg =>
                            new
                            {
                                lg.Key.Year,
                                donvi_id = lg.Key.dvicha_id_tt,
                                sl_kp = lg.Count(),
                                unix_date = m_common.convertDayToUnix(31, 12, lg.Key.Year)
                            });
                    break;
                default:
                    comebackListByDate = comebackList
                        .OrderBy(ele => (ele.dvicha_id_tt, ele.ngaycn_bbbg))
                        .GroupBy(l => new { l.ngaycn_bbbg.Day, l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.dvicha_id_tt })
                        .Select(lg =>
                            new
                            {
                                lg.Key.Day,
                                lg.Key.Month,
                                lg.Key.Year,
                                donvi_id = lg.Key.dvicha_id_tt,
                                sl_kp = lg.Count(),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            });
                    break;
            }
            List<dynamic> list = new List<dynamic>(comebackListByDate);
            return list;
        }

        public dynamic getConfigByDate(RqGrafana rq)
        {
            List<Detail_lapmoi> configList = executeDetailLapmoi(rq);
            dynamic configListByDate;
            switch ((string)rq.targets[0].data.time_scale)
            {
                case "week":
                    List<dynamic> temp_config = new List<dynamic>();
                    foreach (var element in configList)
                    {
                        int week = m_common.GetIso8601WeekOfYear(element.ngaycn_bbbg);
                        var lastDayWeek = element.ngaycn_bbbg.LastDayOfWeek();
                        temp_config.Add(new { element.donvi_id, element.ngaycn_bbbg, week, lastDayWeek });
                    }
                    configListByDate = temp_config
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
                    configListByDate = configList
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
                    configListByDate = configList
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
                    configListByDate = configList
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
            List<dynamic> list = new List<dynamic>(configListByDate);
            return list;
        }

        public dynamic getEqualLengthNew(List<Unit> listUnit, List<dynamic> list, List<dynamic> list_date)
        {
            List<dynamic> new_list = new List<dynamic>();
            foreach (List<dynamic> data in list)
            {
                var unit = listUnit.Where(r => r.donvi_id == data[0].donvi_id).FirstOrDefault();
                List<dynamic> temp = new List<dynamic>();
                foreach (var date in list_date)
                {
                    var filter = data.Where(r => r.unix_date == date).FirstOrDefault();
                    if (filter == null)
                    {
                        temp.Add(new { unit.donvi_id, unit.ten_dv, sl_new = 0, unix_date = date });
                    }
                    else
                    {
                        temp.Add(new { unit.donvi_id, unit.ten_dv, filter.sl_new, unix_date = date });
                    }
                }
                List<dynamic> sorted = new List<dynamic>(temp.OrderBy(r => r.unix_date));
                new_list.Add(sorted);
            }
            return new_list;
        }

        public dynamic getEqualLengthConfig(List<Unit> listUnit, List<dynamic> list, List<dynamic> list_date)
        {
            List<dynamic> config_list = new List<dynamic>();
            foreach (List<dynamic> data in list)
            {
                var unit = listUnit.Where(r => r.donvi_id == data[0].donvi_id).FirstOrDefault();
                List<dynamic> temp = new List<dynamic>();
                foreach (var date in list_date)
                {
                    var filter = data.Where(r => r.unix_date == date).FirstOrDefault();
                    if (filter == null)
                    {
                        temp.Add(new { unit.donvi_id, unit.ten_dv, sl_lapdat = 0, unix_date = date });
                    }
                    else
                    {
                        temp.Add(new { unit.donvi_id, unit.ten_dv, filter.sl_lapdat, unix_date = date });
                    }
                }
                List<dynamic> sorted = new List<dynamic>(temp.OrderBy(r => r.unix_date));
                config_list.Add(sorted);
            }
            return config_list;
        }

        public dynamic getTimeseriesNew(List<dynamic> list)
        {
            List<dynamic> total_timeseries = new List<dynamic>();
            foreach (List<dynamic> data in list)
            {
                List<dynamic> datapoints = new List<dynamic>();
                foreach (var element in data)
                {
                    datapoints.Add(new List<dynamic> { element.sl_new, element.unix_date });
                }
                total_timeseries.Add(new { target = data[0].ten_dv, datapoints });
            }
            return total_timeseries;
        }

        public dynamic getTimeseriesConfig(List<dynamic> list)
        {
            List<dynamic> total_timeseries = new List<dynamic>();
            foreach (List<dynamic> data in list)
            {
                List<dynamic> datapoints = new List<dynamic>();
                foreach (var element in data)
                {
                    datapoints.Add(new List<dynamic> { element.sl_lapdat, element.unix_date });
                }
                total_timeseries.Add(new { target = data[0].ten_dv, datapoints });
            }
            return total_timeseries;
        }

        public dynamic getNew(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            dynamic lst_donviId;
            if ((string)rq.targets[0].data.center == "ttkd")
            {
                lst_donviId = rq.scopedVars.ttkd.value;
            }
            else
            {
                lst_donviId = rq.scopedVars.ttvt.value;
            }

            if (rq.targets[0].type == "timeseries")
            {
                List<dynamic> total = new List<dynamic>();
                List<dynamic> total_date = new List<dynamic>();
                List<dynamic> total_timeseries = new List<dynamic>();
                List<Unit> listUnit = m_common.getListCenter(rq);
                List<dynamic> newListByDate = getNewByDate(rq);
                foreach (var unit in listUnit)
                {
                    List<dynamic> lst_new = new List<dynamic>(newListByDate.FindAll(r => r.donvi_id == unit.donvi_id));
                    List<dynamic> lst_new_date = new List<dynamic>(lst_new.Select(lg => lg.unix_date));
                    total_date.Add(lst_new_date);
                    total.Add(lst_new);
                }
                total_date = m_common.getFullList(total_date);
                total = getEqualLengthNew(listUnit, total, total_date);
                total_timeseries = getTimeseriesNew(total);
                if (rq.targets[0].data.graph == "pie_chart")
                {
                    response = total_timeseries;
                }
                else
                {
                    List<dynamic> max_points = m_common.getMaxPoints(total_timeseries);
                    List<dynamic> min_points = m_common.getMinPoints(total_timeseries);
                    List<dynamic> avr_points = m_common.getAveragePoints(total_timeseries);
                    response.Add(new { target = "GT MAX", datapoints = max_points });
                    response.Add(new { target = "GT trung bình", datapoints = avr_points });
                    response.Add(new { target = "GT MIN", datapoints = min_points });
                    if ((int)lst_donviId != 1)
                    {
                        var unit = listUnit.Where(r => r.donvi_id == (int)lst_donviId).FirstOrDefault();
                        var unit_data = total_timeseries.Where(r => r.target == unit.ten_dv).FirstOrDefault();
                        response.Add(unit_data);
                    };
                }
            }
            else if (rq.targets[0].type == "table")
            {
            }
            return response;
        }
    }
}
