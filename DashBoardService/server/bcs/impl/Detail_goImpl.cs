using ClassModel.connnection.oracle;
using ClassModel.connnection.reponsitory.impl;
using ClassModel.connnection.sql;
using ClassModel.model.bsc;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using Dapper;
using DashBoardService.server.bcs;
using DashBoardService.server.common;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace DashBoardServicve.server.bcs.impl
{
    public class Detail_goImpl : Reponsitory<Detail_go>, IDetail_go
    {
        private IConfiguration m_configuration;
        private ICommon m_common;
        public Detail_goImpl(DataContext context, IConfiguration configuration, ICommon common) : base(context)
        {
            m_configuration = configuration;
            m_common = common;
        }

        public dynamic execureDetailgo(RqGrafana rq)
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
            List<Detail_go> result = new List<Detail_go>();
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
                var query = "khanhnv.DASHBOARD.detail_go";
                result = SqlMapper.Query<Detail_go>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<Detail_go>();
                //insertDetail_go(result);


            }
            return result;
        }

        private void insertDetail_go(List<Detail_go> listdetails)
        {
            foreach (var i in listdetails)
            {
                insert(i);
            }
        }

        public dynamic getRemoveByDate(RqGrafana rq)
        {
            List<Detail_go> removeList = execureDetailgo(rq);
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

        public dynamic getEqualLengthRemove(List<Unit> listUnit, List<dynamic> list, List<dynamic> list_date)
        {
            List<dynamic> remove_list = new List<dynamic>();
            foreach (List<dynamic> data in list)
            {
                var unit = listUnit.Where(r => r.donvi_id == data[0].donvi_id).FirstOrDefault();
                List<dynamic> temp = new List<dynamic>();
                foreach (var date in list_date)
                {
                    var filter = data.Where(r => r.unix_date == date).FirstOrDefault();
                    if (filter == null)
                    {
                        temp.Add(new { unit.donvi_id, unit.ten_dv, sl_huy = 0, unix_date = date });
                    }
                    else
                    {
                        temp.Add(new { unit.donvi_id, unit.ten_dv, filter.sl_huy, unix_date = date });
                    }
                }
                List<dynamic> sorted = new List<dynamic>(temp.OrderBy(r => r.unix_date));
                remove_list.Add(sorted);
            }
            return remove_list;
        }

        public dynamic getTimeseriesRemove(List<dynamic> list)
        {
            List<dynamic> total_timeseries = new List<dynamic>();
            foreach (List<dynamic> data in list)
            {
                List<dynamic> datapoints = new List<dynamic>();
                foreach (var element in data)
                {
                    datapoints.Add(new List<dynamic> { element.sl_huy, element.unix_date });
                }
                total_timeseries.Add(new { target = data[0].ten_dv, datapoints });
            }
            return total_timeseries;
        }

        public dynamic getRemove(RqGrafana rq)
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
                List<dynamic> removeListByDate = getRemoveByDate(rq);
                foreach (var unit in listUnit)
                {
                    List<dynamic> lst_remove = new List<dynamic>(removeListByDate.FindAll(r => r.donvi_id == unit.donvi_id));
                    List<dynamic> lst_remove_date = new List<dynamic>(lst_remove.Select(lg => lg.unix_date));
                    total_date.Add(lst_remove_date);
                    total.Add(lst_remove);
                }
                total_date = m_common.getFullList(total_date);
                total = getEqualLengthRemove(listUnit, total, total_date);
                total_timeseries = getTimeseriesRemove(total);
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

        public IDbConnection GetConnection()
        {
            var connectionString = m_configuration.GetSection("connectionstrings").GetSection("defaultconnection2").Value;
            var conn = new OracleConnection(connectionString);
            return conn;
        }
    }
}
