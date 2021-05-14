//Push 11/10/2020

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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.bcs.impl
{
    public class I8MobileAcceptanceImpl : Reponsitory<I8MobileAcceptance>, II8MobileAcceptance
    {
        private IConfiguration m_configuration;
        private ICommon m_common;
        public I8MobileAcceptanceImpl(ICommon common, IConfiguration configuration, DataContext context) : base(context)
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

        public dynamic executeI8MobileAcceptance(RqGrafana rq)
        {
            var date = m_common.convertToString(rq);
            List<I8MobileAcceptance> result = new List<I8MobileAcceptance>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, date.Item1);
            dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, date.Item2);
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.i8NghiemThuByKhanh";
                result = SqlMapper.Query<I8MobileAcceptance>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<I8MobileAcceptance>();
            }
            return result;
        }

        public dynamic getI8MobileAcceptance(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            List<I8MobileAcceptance> result = executeI8MobileAcceptance(rq);
            if (rq.targets[0].type == "table")
            {
                List<dynamic> col = new List<dynamic>();
                List<dynamic> row = new List<dynamic>();
                if ((int)rq.scopedVars.ttvt.value == 1)
                {
                    List<dynamic> listByDate = new List<dynamic>(result
                        .OrderBy(ele => (ele.ttvt, ele.ngay_ht))
                        .GroupBy(l => new { l.ngay_ht.Month, l.ngay_ht.Year, l.ttvt })
                        .Select(lg =>
                           new {
                               lg.Key.Month,
                               lg.Key.Year,
                               lg.Key.ttvt,
                               unix_time = m_common.convertMonthToUnix(lg.Key.Month, lg.Key.Year),
                               PCT_HOAN_TAT_QUA_MOBILE_APP = lg.Sum(w => w.PCT_HOAN_TAT_QUA_MOBILE_APP),
                               PCT_CCDV_VA_SCDV_HOAN_TAT = lg.Sum(w => w.PCT_CCDV_VA_SCDV_HOAN_TAT)
                           }));
                    col = new List<dynamic> {
                        new { text = "TTVT", type = "string"},
                        new { text = "Ngày HT", type = "string"},
                        new { text = "Số phiếu HT qua App", type = "number"},
                        new { text = "Số phiếu HT", type = "number"}
                    };
                    foreach (var element in listByDate)
                    {
                        var month = (element.Month < 10 ? "0" + element.Month.ToString() : element.Month.ToString()) + "/" + element.Year.ToString();
                        row.Add(new List<dynamic> { element.ttvt, month, element.PCT_HOAN_TAT_QUA_MOBILE_APP, element.PCT_CCDV_VA_SCDV_HOAN_TAT });
                    }
                }
                else
                {
                    List<I8MobileAcceptance> findTTVT = result.FindAll(r => r.donvi_cha_id == (int)rq.scopedVars.ttvt.value);
                    List<dynamic> listByDate = new List<dynamic>(findTTVT
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_ht))
                        .GroupBy(l => new { l.ngay_ht.Month, l.ngay_ht.Year, l.doi_vt, l.ttvt })
                        .Select(lg =>
                           new {
                               lg.Key.Month,
                               lg.Key.Year,
                               lg.Key.doi_vt,
                               lg.Key.ttvt,
                               unix_time = m_common.convertMonthToUnix(lg.Key.Month, lg.Key.Year),
                               PCT_HOAN_TAT_QUA_MOBILE_APP = lg.Sum(w => w.PCT_HOAN_TAT_QUA_MOBILE_APP),
                               PCT_CCDV_VA_SCDV_HOAN_TAT = lg.Sum(w => w.PCT_CCDV_VA_SCDV_HOAN_TAT)
                           }));
                    col = new List<dynamic> {
                        new { text = "Đội VT", type = "string"},
                        new { text = "TTVT", type = "string"},
                        new { text = "Ngày HT", type = "string"},
                        new { text = "Số phiếu HT qua App", type = "number"},
                        new { text = "Số phiếu HT", type = "number"}
                    };
                    foreach (var element in listByDate)
                    {
                        var month = (element.Month < 10 ? "0" + element.Month.ToString() : element.Month.ToString()) + "/" + element.Year.ToString();
                        row.Add(new List<dynamic> { element.doi_vt, element.ttvt, month, element.PCT_HOAN_TAT_QUA_MOBILE_APP, element.PCT_CCDV_VA_SCDV_HOAN_TAT });
                    }
                }
                response = new List<dynamic> {
                        new {
                                columns = col,
                                rows = row,
                                type = "table"
                            }
                    };
            }
            else if (rq.targets[0].type == "timeseries")
            {
                if ((int)rq.scopedVars.ttvt.value == 1)
                {
                    List<Unit> listUnit = m_common.getListCenter(rq);
                    List<dynamic> listByDate = new List<dynamic>(result
                        .OrderBy(ele => (ele.ttvt, ele.ngay_ht))
                        .GroupBy(l => new { l.ngay_ht.Month, l.ngay_ht.Year, l.donvi_cha_id,l.ttvt })
                        .Select(lg =>
                           new {
                               ttvt_id = lg.Key.donvi_cha_id,
                               lg.Key.ttvt,
                               unix_time = m_common.convertMonthToUnix(lg.Key.Month, lg.Key.Year),
                               PCT_HOAN_TAT_QUA_MOBILE_APP = lg.Sum(w => w.PCT_HOAN_TAT_QUA_MOBILE_APP),
                               PCT_CCDV_VA_SCDV_HOAN_TAT = lg.Sum(w => w.PCT_CCDV_VA_SCDV_HOAN_TAT)
                           }));
                    if (rq.targets[0].data.condition == "usaged")
                    {
                        foreach (Unit unit in listUnit)
                        {
                            List<dynamic> points = new List<dynamic>();
                            List<dynamic> filter = new List<dynamic>(listByDate.FindAll(r => r.ttvt_id == unit.donvi_id));
                            foreach (var element in filter)
                            {
                                points.Add(new List<dynamic> { element.PCT_HOAN_TAT_QUA_MOBILE_APP, element.unix_time });
                            }
                            response.Add(new { target = unit.ten_dv, datapoints = points });
                        }
                    }
                    else
                    {
                        foreach (Unit unit in listUnit)
                        {
                            List<dynamic> points = new List<dynamic>();
                            List<dynamic> filter = new List<dynamic>(listByDate.FindAll(r => r.ttvt_id == unit.donvi_id));
                            foreach (var element in filter)
                            {
                                points.Add(new List<dynamic> { element.PCT_CCDV_VA_SCDV_HOAN_TAT, element.unix_time });
                            }
                            response.Add(new { target = unit.ten_dv, datapoints = points });
                        }
                    }
                }
                else
                {
                    List<I8MobileAcceptance> findTTVT = result.FindAll(r => r.donvi_cha_id == (int)rq.scopedVars.ttvt.value);
                    List<dynamic> listByDate = new List<dynamic>(findTTVT
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_ht))
                        .GroupBy(l => new { l.ngay_ht.Month, l.ngay_ht.Year, l.doi_vt })
                        .Select(lg =>
                           new {
                               lg.Key.doi_vt,
                               unix_time = m_common.convertMonthToUnix(lg.Key.Month, lg.Key.Year),
                               PCT_HOAN_TAT_QUA_MOBILE_APP = lg.Sum(w => w.PCT_HOAN_TAT_QUA_MOBILE_APP),
                               PCT_CCDV_VA_SCDV_HOAN_TAT = lg.Sum(w => w.PCT_CCDV_VA_SCDV_HOAN_TAT)
                           }));
                    List<dynamic> doi_vt = new List<dynamic>(findTTVT
                        .GroupBy(l => new { l.doi_vt })
                        .Select(lg =>
                           new {
                               lg.Key.doi_vt
                           }));
                    if (rq.targets[0].data.condition == "usaged")
                    {
                        foreach (var unit in doi_vt)
                        {
                            List<dynamic> points = new List<dynamic>();
                            List<dynamic> filter = listByDate.FindAll(r => r.doi_vt == unit.doi_vt);
                            foreach (var element in filter)
                            {
                                points.Add(new List<dynamic> { element.PCT_HOAN_TAT_QUA_MOBILE_APP, element.unix_time });
                            }
                            response.Add(new { target = unit.doi_vt, datapoints = points });
                        }
                    }
                    else
                    {
                        foreach (var unit in doi_vt)
                        {
                            List<dynamic> points = new List<dynamic>();
                            List<dynamic> filter = listByDate.FindAll(r => r.doi_vt == unit.doi_vt);
                            foreach (var element in filter)
                            {
                                points.Add(new List<dynamic> { element.PCT_CCDV_VA_SCDV_HOAN_TAT, element.unix_time });
                            }
                            response.Add(new { target = unit.doi_vt, datapoints = points });
                        }
                    }
                }
            }
            return response;
        }
    }
}
