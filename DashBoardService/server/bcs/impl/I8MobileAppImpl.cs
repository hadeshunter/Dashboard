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
    public class I8MobileAppImpl : Reponsitory<I8MobileApp>, II8MobileApp
    {
        private IConfiguration m_configuration;
        private ICommon m_common;
        public I8MobileAppImpl(ICommon common, IConfiguration configuration, DataContext context) : base(context)
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

        public dynamic executeI8MobileApp(RqGrafana rq)
        {
            var date = m_common.convertToString(rq);
            var vdonvi_id = (int)rq.scopedVars.ttvt.value;
            List<UsageResponse> result = new List<UsageResponse>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, date.Item1);
            dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, date.Item2);
            dyParam.Add("vdonvi_id", OracleDbType.Int32, ParameterDirection.Input, vdonvi_id);

            dyParam.Add("returnds", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.i8_sudung_mobile_app_by_khanh";
                result = SqlMapper.Query<UsageResponse>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<UsageResponse>();
            }
            return result;
        }

        public dynamic getI8MobileApp(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            List<UsageResponse> result = executeI8MobileApp(rq);
            if (rq.targets[0].type == "table")
            {
                List<dynamic> col = new List<dynamic>();
                List<dynamic> row = new List<dynamic>();
                if ((int)rq.scopedVars.ttvt.value == 1)
                {
                    col = new List<dynamic>
                    {
                        new { text = "TTVT", type = "string"},
                        new { text = "Login", type = "number"},
                        new { text = "Tổng", type = "number"},
                        new { text = "Tỷ lệ", type = "number"}
                    };
                    List<dynamic> filter = new List<dynamic>(result
                        .GroupBy(g => new { g.ttvt })
                        .Select(lg => new {
                            lg.Key.ttvt,
                            login = lg.Sum(w => w.login),
                            tong = lg.Sum(w => w.tong),
                            ty_le = Math.Round((double)lg.Sum(w => w.login) * 100 / lg.Sum(w => w.tong),4)
                        }));
                    foreach (var element in filter)
                    {
                        row.Add(new List<dynamic> { element.ttvt, element.login, element.tong, element.ty_le });
                    }
                }
                else
                {
                    col = new List<dynamic>
                    {
                        new { text = "Đội VT", type = "string"},
                        new { text = "TTVT", type = "string"},
                        new { text = "Login", type = "number"},
                        new { text = "Tổng", type = "number"},
                        new { text = "Tỷ lệ", type = "number"}
                    };
                    foreach (UsageResponse element in result)
                    {
                        row.Add(new List<dynamic> { element.ten_dv, element.ttvt, element.login, element.tong, element.ty_le * 100 });
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
                DateTime dngay = Convert.ToDateTime(rq.range.to);
                long unix_time = m_common.convertDayToUnix(01, dngay.Month, dngay.Year);
                if (rq.scopedVars.ttvt.value == 1)
                {
                    List<dynamic> filter = new List<dynamic>(result
                        .GroupBy(g => new { g.ttvt })
                        .Select(lg => new {
                            lg.Key.ttvt,
                            login = lg.Sum(w => w.login)
                        }));
                    foreach (var unit in filter)
                    {
                        List<dynamic> points = new List<dynamic>();
                        points.Add(new List<dynamic> { unit.login, unix_time });
                        response.Add(new { target = unit.ttvt, datapoints = points });
                    }
                }
                else
                {
                    foreach (var element in result)
                    {
                        List<dynamic> points = new List<dynamic>();
                        points.Add(new List<dynamic> { element.login, unix_time });
                        response.Add(new { target = element.ten_dv, datapoints = points });
                    }
                }
            }
            return response;
        }
    }
}
