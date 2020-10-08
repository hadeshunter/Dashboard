using ClassModel.connnection.oracle;
using ClassModel.connnection.reponsitory.impl;
using ClassModel.connnection.sql;
using ClassModel.model.bsc;
using ClassModel.model.RqGrafana;
using Dapper;
using DashBoardService.server.bcs;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;

namespace DashBoardServicve.server.bcs.impl
{
    public class Detail_goImpl : Reponsitory<Detail_go>, IDetail_go
    {
        private IConfiguration m_configuration;
        public Detail_goImpl(DataContext context, IConfiguration configuration) : base(context)
        {
            m_configuration = configuration;
        }

        public dynamic execureDetailgo(RqGrafana rq)
        {
            List<Detail_go> result = new List<Detail_go>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, "");
            dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, "");
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "kiemsoat.bc_dashboard.detail_go";
                result = SqlMapper.Query<Detail_go>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<Detail_go>();
                //insertDetal_go(result);
            }
            return result;
        }

        private void insertDetal_go(List<Detail_go> listdetals)
        {
            foreach (var i in listdetals)
            {
                insert(i);
            }
        }

        public IDbConnection GetConnection()
        {
            var connectionString = m_configuration.GetSection("connectionstrings").GetSection("defaultconnection2").Value;
            var conn = new OracleConnection(connectionString);
            return conn;
        }
    }
}
