using ClassModel.connnection.oracle;
using ClassModel.convertdata.ccdv;
using ClassModel.model.testing;
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
using TestingListening.service.test;

namespace TestingListening.service.ccdv.impl
{
    public class Ccdv_Dung_TgImpl: ICcdv_Dung_Tg
    {

        private IConfiguration m_configuration;
        private ITest m_test;
        public Ccdv_Dung_TgImpl( IConfiguration _configuration, ITest _test)
        {
            m_configuration = _configuration;
            m_test = _test;
        }

        public void onChange()
        {
            string connStr = m_configuration.GetConnectionString("DefaultConnection1");
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"select endtimeupdate from [dbo].TimeChange where timeid = 1", conn))
                {
                    SqlDependency dependency = new SqlDependency(cmd);
                    dependency.OnChange += new System.Data.SqlClient.OnChangeEventHandler(onChange1);
                    SqlDependency.Start(connStr);
                    cmd.ExecuteReader();
                }
                conn.Dispose();
                conn.Close();
            }
        }
        public void onChange1(object sender, SqlNotificationEventArgs e)
        {

            //string connStr = m_configuration.GetConnectionString("DefaultConnection1");
            //var dt = new DataTable();
            //using (var conn = new SqlConnection(connStr))
            //{
            //    conn.Open();
            //    using (var cmd = new SqlCommand(@"select endtimeupdate,endtime from [dbo].TimeChange where timeid = 1", conn))
            //    {
            //        using (var rdr = cmd.ExecuteReader())
            //        {
            //            dt.Load(rdr);
            //        }
            //        var row = dt.Rows[0];
            //        var endtime = row["endtimeupdate"].ToString().Substring(0, 10);
            //        var starttime = row["endtime"].ToString().Substring(0, 10);
            //        var time1 = DateTime.ParseExact(starttime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //        var time2 = DateTime.ParseExact(endtime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //        if (time2 >= DateTime.Now)
            //        {
            //            time2 = DateTime.Now.AddDays(-1);
            //        }
            //        if (time2 > time1)
            //        {

            //            using (var cmd1 = new SqlCommand(@"update TimeChange set endtime = (convert(datetime,'" + time2.AddDays(1).ToString().Substring(0, 10) + "', 103)) where timeid = 1", conn))
            //            {
            //                cmd1.ExecuteNonQuery();
            //            }
            //            getCCDV(starttime, time2.ToString().Substring(0, 10));
            //        }
            //        else
            //        {
            //            onChange();
            //        }
            //        conn.Close();
            //    }
            //    conn.Close();
            //}
            var test = new TestData();
            test.name = DateTime.Now.ToString();
            m_test.insert(test);
            onChange();

        }
        //public void insertCCDV(List<Ccdv_Dung_Tg> listData)
        //{
        //    var conn = m_configuration.GetConnectionString("DefaultConnection");
        //    using (SqlConnection con = new SqlConnection(conn))
        //    {
        //        con.Open();
        //        try
        //        {
        //            for (var i = 0; i < listData.Count; i++)
        //            {
        //                var item = listData[i];
        //                var query = "INSERT INTO Ccdv_Dung_Tg(donvi_id,ten_dv,tong_pct,ngaycn_bbbg,soluong_khonghen_ccdv,ok_khonghen_ccdv,tregio_khonghen_ccdv,soluong_cohen_ccdv" +
        //                    ",ok_cohen_ccdv,tregio_cohen_ccdv,tyle_ccdv" +
        //                          ")" +
        //                          "VALUES(" + item.donvi_id + ",N'" + item.ten_dv + "'," + item.tong_pct + ",'" + item.ngaycn_bbbg.ToString("MM-dd-yyyy") + "'," + item.soluong_khonghen_ccdv +
        //                          "," + item.ok_khonghen_ccdv +
        //                          "," + item.tregio_khonghen_ccdv + "," + item.soluong_cohen_ccdv + "," +item.ok_cohen_ccdv + "," + item.tregio_cohen_ccdv + "," + item.tyle_ccdv+ ")";
        //                using (SqlCommand command = new SqlCommand(query, con))
        //                {
        //                    command.ExecuteNonQuery();
        //                }
        //            }
        //            con.Close();
        //        }
        //        catch (Exception e)
        //        {
        //        }
        //    }
        //    onChange();
        //}

        //public void getCCDV(string startime, string endtime)//string startime, string endtime
        //{

        //    List<Ccdv_Dung_Tg> result = new List<Ccdv_Dung_Tg>();
        //    var dyParam = new OracleDynamicParameters();
        //    dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, startime);
        //    dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, endtime);
        //    dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
        //    var conn = GetConnection();
        //    if (conn.State == ConnectionState.Closed)
        //    {
        //        conn.Open();
        //    }
        //    if (conn.State == ConnectionState.Open)
        //    {
        //        var query = "dashboard.Cung_Cap_DV_Dung_TG_Quy_Dinh";
        //        result = SqlMapper.Query<Ccdv_Dung_Tg>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<Ccdv_Dung_Tg>();
        //    }
        //    insertCCDV(result);
        //    //return result;
        //}

        //public void onChange2()
        //{
        //    string connStr = m_configuration.GetConnectionString("DefaultConnection");
        //    var dt = new DataTable();
        //    using (var conn = new SqlConnection(connStr))
        //    {
        //        conn.Open();
        //        using (var cmd = new SqlCommand(@"insert into Test(name) values ('Testing +" + DateTime.Now.ToString() + "')", conn))
        //        {
        //            dt.Load(cmd.ExecuteReader());
        //        }
        //    }
        //    onChange();
        //}
        //connect oracle
        public IDbConnection GetConnection()
        {
            var connectionString = m_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection2").Value;
            var conn = new OracleConnection(connectionString);
            return conn;
        }
    }
}
