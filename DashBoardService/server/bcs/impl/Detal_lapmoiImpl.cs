using ClassModel.connnection.oracle;
using ClassModel.connnection.reponsitory.impl;
using ClassModel.connnection.sql;
using ClassModel.model.bsc;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardApi.server.bcs.impl
{
    public class Detal_lapmoiImpl : Reponsitory<Detal_lapmoi>, IDetal_lapmoi
    {
        private IConfiguration m_configuration;
        public Detal_lapmoiImpl(DataContext context, IConfiguration configuration) : base(context)
        {
            m_configuration = configuration;
        }

        public dynamic execureDetailLapmoi(BscRequest bscRequest)
        {
            List<Detal_lapmoi> result = new List<Detal_lapmoi>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, bscRequest.vtungay);
            dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, bscRequest.vdenngay);
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "kiemsoat.bc_dashboard.detail_lapmoi";
                result = SqlMapper.Query<Detal_lapmoi>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<Detal_lapmoi>();
                insertDetal_lapmoi(result);
            }
            return result;
        }

        private void insertDetal_lapmoi(List<Detal_lapmoi> listdetals)
        {
            foreach(var i in listdetals)
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
