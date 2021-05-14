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

namespace DashBoardService.server.convertdata.ccdv.impl
{
    public class CcdvDungThoiGianImpl: ICcdvDungThoiGian
    {

        private IConfiguration m_configuration;
        public CcdvDungThoiGianImpl(IConfiguration _configuration)
        {
            m_configuration = _configuration;
        }

        public void insertCCDV(List<Ccdv_Dung_Tg> listData)
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
                        var query = "INSERT INTO Ccdv_Dung_Tg(donvi_id,nhomlc_id,donvi_cha_id,ten_dvql,ten_dv,tong_pct,ngaycn_bbbg,soluong_khonghen_ccdv,ok_khonghen_ccdv,tregio_khonghen_ccdv,soluong_cohen_ccdv" +
                            ",ok_cohen_ccdv,tregio_cohen_ccdv,tyle_ccdv,timeinsert" +
                                  ")" +
                                  "VALUES(" + item.donvi_id +"," + item.nhomlc_id + "," + item.donvi_cha_id + ",N'" + item.ten_dvql + "',N'" + item.ten_dv + "'," + item.tong_pct + ",'" + item.ngaycn_bbbg.Month +"-"+ item.ngaycn_bbbg.Day +"-"+ item.ngaycn_bbbg.Year+ "'," + item.soluong_khonghen_ccdv +
                                  "," + item.ok_khonghen_ccdv +
                                  "," + item.tregio_khonghen_ccdv + "," + item.soluong_cohen_ccdv + "," + item.ok_cohen_ccdv + "," + item.tregio_cohen_ccdv + "," + item.tyle_ccdv +","+DateTime.Now.Year.ToString()+DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString()+ ")";
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

        public dynamic toDataConvert(string startime, string endtime)
        {
            try
            {
                return checkconvertCcdvToDb(startime, endtime);
            }
            catch(Exception e)
            {
                return e;
            }
        }
        public void convertCcdvToDb(string startime, string endtime)//string startime, string endtime
        {
            try
            {
               
                List<Ccdv_Dung_Tg> result = new List<Ccdv_Dung_Tg>();
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, startime);
                dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, endtime);
                dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
                var conn = GetConnection();
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var query = "dashboard.Cung_Cap_DV_Dung_TG_Quy_Dinh";
                    result = SqlMapper.Query<Ccdv_Dung_Tg>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<Ccdv_Dung_Tg>();
                }
                insertCCDV(result);
            }
            catch(Exception e)
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
                using (var cmd = new SqlCommand(@"select endtimeupdate,endtime from TimeConvert where timeid = 1", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    using (var rdr = cmd.ExecuteReader())
                    {
                        dt.Load(rdr);
                    }
                    if(dt.Rows.Count == 0)
                    {
                        return 0;
                    }
                    var row = dt.Rows[0];
                    DateTime etime =  (DateTime)row["endtime"] ;
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
                        using (var cmd1 = new SqlCommand(@"update TimeConvert set endtime = (convert(datetime,'" + time2.AddDays(1).ToString("dd/MM/yyyy") + "', 103)),timeinsert = " + timeinsert + " where timeid = 1", conn))
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
