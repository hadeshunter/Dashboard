using ClassModel.connnection.oracle;
using ClassModel.connnection.reponsitory.impl;
using ClassModel.connnection.sql;
using ClassModel.model.bsc;
using ClassModel.model.RqGrafana;
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
    public class ComboHomeImpl : Reponsitory<ComboHome>, IComboHome
    {
        private IConfiguration m_configuration;
        private ICommon m_common;
        public ComboHomeImpl(DataContext context, IConfiguration configuration, ICommon common) : base(context)
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

        public dynamic executeComboHome(RqGrafana rq)
        {
            int month = (int)rq.scopedVars.month.value;
            var date = Convert.ToDateTime((string)rq.scopedVars.year.value + "-" + (month < 10 ? "0" + month.ToString() : month.ToString()));
            var v_thang = date.Year.ToString() + (date.Month < 10 ? "0" + date.Month.ToString() : date.Month.ToString());

            List<ComboHome> result = new List<ComboHome>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("v_thang", OracleDbType.Varchar2, ParameterDirection.Input, v_thang);
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "khanhnv.DASHBOARD.combohome";
                result = SqlMapper.Query<ComboHome>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<ComboHome>();
            }
            return result;
        }

        public dynamic getComboHome(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            if (rq.targets[0].type == "table")
            {
                List<ComboHome> result = executeComboHome(rq);
                List<dynamic> col = new List<dynamic>
                    {   //Phải khai báo type = "time" để cột thời gian nhận unixtime mili giây
                        new { text = "Tên đơn vị", type = "string"}, //String thì ko hiện lên chart
                        new { text = "HC Fiber lắp mới", type = "number"}, //Number hiện sô liệu lên chart
                        new { text = "HC MyTV lắp mới", type = "number"}, //Number hiện sô liệu lên chart
                        new { text = "HC Fiber hiện hữu", type = "number"},
                        new { text = "HC MyTV hiện hữu", type = "number"},
                        new { text = "Tổng số PTM HC", type = "number"},
                        new { text = "Hủy khách quan", type = "number"},
                        new { text = "Hủy chủ quan", type = "number"},
                        new { text = "Tổng hủy", type =  "number"},
                        new { text = "Thực tăng", type =  "number"},
                    };
                List<dynamic> row = new List<dynamic>();
                foreach (ComboHome unit in result)
                {
                    row.Add(new List<dynamic> {
                        unit.ten_dv,
                        unit.thuebao_lapmoi_fiber,
                        unit.thuebao_lapmoi_mytv,
                        unit.thuebao_hienhuu_fiber,
                        unit.thuebao_hienhuu_mytv,
                        unit.tongso_ptm_hc,
                        unit.huykhachquan,
                        unit.huychuquan,
                        unit.tonghuy,
                        unit.thuctang
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
