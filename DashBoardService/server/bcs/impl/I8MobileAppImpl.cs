using ClassModel.connnection.oracle;
using ClassModel.connnection.reponsitory.impl;
using ClassModel.connnection.sql;
using ClassModel.model.bsc;
using ClassModel.model.RqGrafana;
using Dapper;
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
        public I8MobileAppImpl(IConfiguration configuration , DataContext context) : base(context)
        {
            m_configuration = configuration;
        }

        public IDbConnection GetConnection()
        {
            var connectionString = m_configuration.GetSection("connectionstrings").GetSection("defaultconnection2").Value;
            var conn = new OracleConnection(connectionString);
            return conn;
        }

        public dynamic usageStatistic(RqGrafana rq)
        {
            DateTime tngay = Convert.ToDateTime(rq.range.from);
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            var vtungay = (tngay.Day < 10 ? ("0" + tngay.Day.ToString()) : tngay.Day.ToString()) +
                "/" + (tngay.Month < 10 ? "0" + tngay.Month.ToString() : tngay.Month.ToString()) + "/" + tngay.Year.ToString();
            var vdenngay = (dngay.Day < 10 ? ("0" + dngay.Day.ToString()) : dngay.Day.ToString()) +
                "/" + (dngay.Month < 10 ? "0" + dngay.Month.ToString() : dngay.Month.ToString()) + "/" + dngay.Year.ToString();

            List<UsageResponse> result = new List<UsageResponse>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, vtungay);
            dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, vdenngay);
            dyParam.Add("vtrangthai", OracleDbType.Varchar2, ParameterDirection.Input, "all");

            dyParam.Add("returnds", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.i8_tk_tyle_nvvt_sudung_mobile_app";
                result = SqlMapper.Query<UsageResponse>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<UsageResponse>();
            }
            return result;
        }
    }
}
