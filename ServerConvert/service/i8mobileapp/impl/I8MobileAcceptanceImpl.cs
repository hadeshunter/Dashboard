using ClassModel.connnection.oracle;
using ClassModel.model.bsc;
using ClassModel.model.request;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ServerConvert.service.i8mobileapp.impl
{
    public class I8MobileAcceptanceImpl: II8MobileAcceptance
    {
        private IConfiguration m_configuration;
        public I8MobileAcceptanceImpl(IConfiguration configuration)
        {
            m_configuration = configuration;
        }

        public IDbConnection GetConnection()
        {
            var connectionString = m_configuration.GetSection("connectionstrings").GetSection("defaultconnection2").Value;
            var conn = new OracleConnection(connectionString);
            return conn;
        }

        public dynamic getI8MobileAcceptance(string starttime, string endtime)
        {
            List<I8MobileAcceptance> result = new List<I8MobileAcceptance>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, starttime);
            dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, endtime);
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
            insertI8mobileAcceptance(result);
            return result;
        }

        private void insertI8mobileAcceptance(List<I8MobileAcceptance> list)
        {
            var conn = m_configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                try
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        var query = "INSERT INTO I8MobileAcceptanceV2(donvi_id,ten_dv,ttvt_id,ttvt,ngay_ht,PCT_CCDV_VA_SCDV_HOAN_TAT,PCT_HOAN_TAT_QUA_MOBILE_APP)" +
                                    "VALUES(" + item.donvi_id + ",N'" + item.doi_vt + "'," + item.donvi_cha_id + ",N'" + item.ttvt + "'," + "convert(datetime,'" + item.ngay_ht + "', 103)" + "," + item.PCT_CCDV_VA_SCDV_HOAN_TAT + "," + item.PCT_HOAN_TAT_QUA_MOBILE_APP + ")";
                        using (SqlCommand command = new SqlCommand(query, con))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    con.Close();
                }
                catch (Exception e)
                {
                    //Console.WriteLine("Something went wrong");
                }
            }
        }

        public void onChangeI8MobileAcceptance()
        {
            string connStr = m_configuration.GetConnectionString("DefaultConnection1");
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"select endtimeupdate,endtime from [dbo].TimeChange where timeid = 3", conn))
                {
                    SqlDependency dependency = new SqlDependency(cmd);
                    dependency.OnChange += new System.Data.SqlClient.OnChangeEventHandler(onChange);
                    SqlDependency.Start(connStr);
                    cmd.ExecuteReader();
                    cmd.Dispose();
                }
                conn.Dispose();
            }
        }

        public void onChange(object sender, SqlNotificationEventArgs e)
        {
            SqlDependency dependency = sender as SqlDependency;
            string connStr = m_configuration.GetConnectionString("DefaultConnection1");
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"select endtimeupdate,endtime from [dbo].TimeChange where timeid = 3", conn))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        dt.Load(rdr);
                    }
                    cmd.Parameters.Clear();
                }
                var row = dt.Rows[0];
                var endtime = row["endtimeupdate"].ToString().Substring(0, 10);
                var starttime = row["endtime"].ToString().Substring(0, 10);

                var time1 = DateTime.ParseExact(starttime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var time2 = DateTime.ParseExact(endtime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (time2 >= DateTime.Now)
                {
                    time2 = DateTime.Now.AddDays(-1);
                }
                if (time2 > time1)
                {
                    var filter = new I8Request();
                    var x = getI8MobileAcceptance(starttime, time2.ToString().Substring(0, 10));
                    using (var cmd1 = new SqlCommand(@"update TimeChange set endtime = (convert(datetime,'" + time2.AddDays(1).ToString().Substring(0, 10) + "', 103)) where timeid = 3", conn))
                    {
                        cmd1.ExecuteNonQuery();
                    }
                }
                conn.Close();
            }
            onChangeI8MobileAcceptance();
        }
    }
}
