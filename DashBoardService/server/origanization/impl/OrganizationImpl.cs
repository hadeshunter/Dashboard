using ClassModel.connnection.oracle;
using ClassModel.connnection.reponsitory.impl;
using ClassModel.connnection.sql;
using ClassModel.model.bsc;
using ClassModel.model.organization;
using ClassModel.model.unit;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.origanization.impl
{
    public class OrganizationImpl : Reponsitory<Organization>, IOrganization
    {
        private IConfiguration m_configuration;
        public OrganizationImpl(DataContext context, IConfiguration configuration) : base(context)
        {
            m_configuration = configuration;
        }

        public dynamic execureOrganization()
        {
            List<Organization> result = new List<Organization>();
            var dyParam = new OracleDynamicParameters();
            //dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, bscRequest.vtungay);
            //dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, bscRequest.vdenngay);

            //dyParam.Add("returnds", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "select dv.* from ADMIN_HCM.donvi dv where dv.donvi_id is not null and dv.donvi_id in(select donvi_id from  ADMIN_HCM.nhanvien_dv nvdv)";
                result = SqlMapper.Query<Organization>(conn, query, param: dyParam, commandType: CommandType.Text).AsList<Organization>();
                insertOrganization(result);
            }
            return result;
        }
        private void insertOrganization(List<Organization> listData)
        {
            foreach (var i in listData)
            {
                insert(i);
            }
        }

        public dynamic getAllDoiVT()
        {
            List<Unit> result = new List<Unit>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.getTTVT_DoiVT";
                result = SqlMapper.Query<Unit>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<Unit>();
            }
            return result;
        }

        public dynamic getAllCenter(UnitRequest rq)
        {
            List<Unit> ttvt = new List<Unit>();
            List<Unit> ttkd = new List<Unit>();
            List<Unit> execute = new List<Unit>();
            List<Unit> result = new List<Unit>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.getTTVT";
                execute = SqlMapper.Query<Unit>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<Unit>();
            }
            foreach (var e in execute)
            {
                if (e.donvi_id > 40 && e.donvi_id < 62)
                {
                    ttvt.Add(e);
                } else if (e.donvi_id < 41 || e.donvi_id > 61) {
                    ttkd.Add(e);
                }
            }
            if (rq.target.ToLower() == "ttvt")
            {
                result = ttvt;
            } else if (rq.target.ToLower() == "ttkd") {
                result = ttkd;
            }
            return result;
        }

        public dynamic getAllTTVT()
        {
            List<Unit> result = new List<Unit>();
            List<Unit> ttvt = new List<Unit>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.getTTVT";
                result = SqlMapper.Query<Unit>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<Unit>();
            }
            foreach (var center in result)
            {
                if (center.donvi_id > 40 && center.donvi_id < 62)
                {
                    ttvt.Add(center);
                }
            }
            return ttvt;
        }

        public dynamic getAllTTKD()
        {
            List<Unit> result = new List<Unit>();
            List<Unit> ttkd = new List<Unit>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.getTTVT";
                result = SqlMapper.Query<Unit>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<Unit>();
            }
            foreach (var center in result)
            {
                if (center.donvi_id < 41 || center.donvi_id > 61)
                {
                    ttkd.Add(center);
                }
            }
            return ttkd;
        }


        public IDbConnection GetConnection()
        {
            var connectionString = m_configuration.GetSection("connectionstrings").GetSection("defaultconnection2").Value;
            var conn = new OracleConnection(connectionString);
            return conn;
        }
    }
}
