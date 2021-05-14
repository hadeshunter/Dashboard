using ClassModel.connnection.oracle;
using ClassModel.convertdata.ccdv;
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

namespace DashBoardService.server.convertdata.scdv.impl
{
    public class Sua_Chua_DV_Dung_TG_Quy_Dinh_NewImpl : ISua_Chua_DV_Dung_TG_Quy_Dinh_New
    {

        private IConfiguration m_configuration;
        public Sua_Chua_DV_Dung_TG_Quy_Dinh_NewImpl(IConfiguration _configuration)
        {
            m_configuration = _configuration;
        }

        public void insertCCDV(List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> listData)
        {
            var conn = m_configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(conn))
            {
                con.Open();
                try
                {
                    for (var i = 0; i < listData.Count; i++)
                    {
                        var item = listData[i];
                        var query = "INSERT INTO Sua_Chua_DV_Dung_TG_Quy_Dinh_New(donvi_id,donvi_cha_id,ten_donvi,bh_dtcd,bh_mega,bh_fiber,bh_mytv,bh_khac,bh_tong," +
                            "st_dtcd,st_mega,st_fiber,st_mytv,st_khac,st_tong,st_quagio,tyle_chuagiamtru,timeinsert,nhomlc_id,ten_dvql,thoigian_suatot)" +
                                  "VALUES(" + item.donvi_id + "," + item.donvi_cha_id + ",N'" + item.ten_donvi + "'," + item.bh_dtcd + "," + item.bh_mega + "," + item.bh_fiber + "," +
                                  + item.bh_mytv + "," + item.bh_khac + "," + item.bh_tong + "," + item.st_dtcd + "," + item.st_mega + "," + item.st_fiber + ","
                                  + item.st_mytv + "," + item.st_khac + "," + item.st_tong
                                  + "," + item.st_quagio + "," + item.tyle_chuagiamtru
                                  + "," + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() 
                                  + "," + item.nhomlc_id + ",N'" + item.ten_dvql + "','" + item.thoigian_suatot.Month + "-" + item.thoigian_suatot.Day + "-" + item.thoigian_suatot.Year + "')";
                        using (SqlCommand command = new SqlCommand(query, con))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                    con.Close();
                }
                catch (Exception e)
                {
                }
            }
        }

        public Boolean toDataConvert(string startime, string endtime)
        {
            try
            {
                return checkconvertCcdvToDb(startime, endtime);
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public void convertCcdvToDb(string startime, string endtime)//string startime, string endtime
        {
            try
            {

                List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> result = new List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New>();
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, startime);
                dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, endtime);
                //dyParam.Add("vdonvi_id", OracleDbType.Int32, ParameterDirection.Input, donvi_id);
                dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
                var conn = GetConnection();
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var query = "dashboard.Sua_Chua_DV_Dung_TG_Quy_Dinh_new";
                    result = SqlMapper.Query<Sua_Chua_DV_Dung_TG_Quy_Dinh_New>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<Sua_Chua_DV_Dung_TG_Quy_Dinh_New>();
                }
                insertCCDV(result);
            }
            catch (Exception e)
            {
            }
        }
        private dynamic checkconvertCcdvToDb(string startime, string endtime)
        {
            string connStr = m_configuration.GetConnectionString("DefaultConnection1");
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"select endtimeupdate,endtime from TimeConvert where timeid = 2", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (var rdr = cmd.ExecuteReader())
                    {
                        dt.Load(rdr);
                    }
                    if (dt.Rows.Count == 0)
                    {
                        return 0;
                    }
                    var row = dt.Rows[0];
                    DateTime etime = (DateTime)row["endtime"];
                    var stime = DateTime.ParseExact(endtime.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString().Substring(0, 10);
                    if (etime == null)//chuaw convert lan nao
                    {
                        convertCcdvToDb(startime, endtime);
                    }

                    var time1 = etime;// DateTime.ParseExact(etime.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var time2 = DateTime.ParseExact(endtime, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    if (time2 >= DateTime.Now)
                    {
                        time2 = DateTime.Now.AddDays(-1);
                    }
                    if (time2 > time1)
                    {
                        var timeinsert = DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();
                        using (var cmd1 = new SqlCommand(@"update TimeConvert set endtime = (convert(datetime,'" + time2.AddDays(1).ToString("dd/MM/yyyy") + "', 103)),timeinsert = " + timeinsert + " where timeid = 2", conn))
                        {
                            cmd1.ExecuteNonQuery();
                        }
                        convertCcdvToDb(etime.ToString("dd/MM/yyyy"), endtime); //convert tu ngay cuoi den ngay lay du lieu
                        return true;
                    }
                    conn.Close();

                }
            }
            return true;
        }

        public IDbConnection GetConnection()
        {
            var connectionString = m_configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection2").Value;
            var conn = new OracleConnection(connectionString);
            return conn;
        }
    }
}
