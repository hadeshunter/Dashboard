using ClassModel.connnection.oracle;
using ClassModel.convertdata.tk_khl;
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

namespace DashBoardService.server.convertdata.tk_khl.impl
{
    public class TK_KhongHaiLong_CLPVImpl: ITK_KhongHaiLong_CLPV
    {
        private IConfiguration m_configuration;
        public TK_KhongHaiLong_CLPVImpl(IConfiguration _configuration)
        {
            m_configuration = _configuration;
        }

        public void insertKhongHaiLong_CLPV(List<TK_KhongHaiLong_CLPV> listData)
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
                        var query = "INSERT INTO TK_KhongHaiLong_CLPV(tuan,ngay,donvi_cha_id,sl,timeinsert)" +
                                  "VALUES('" + item.tuan + "','" + (item.ngay.Month < 10 ? "0" + item.ngay.Month.ToString() : item.ngay.Month.ToString()) + "-" + (item.ngay.Day < 10 ? "0" + item.ngay.Day.ToString() : item.ngay.Day.ToString()) + "-" + item.ngay.Year + "'," + item.donvi_cha_id + "," + item.sl + "," + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + ")";
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
                return checkconvertKhongHaiLong_CLPV(startime, endtime);
            }
            catch (Exception e)
            {
                return e;
            }
        }
        public void convertKhongHaiLong_CLPV(string startime, string endtime)//string startime, string endtime
        {
            try
            {

                List<TK_KhongHaiLong_CLPV> result = new List<TK_KhongHaiLong_CLPV>();
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, startime);
                dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, endtime);
                dyParam.Add("ref_cur", OracleDbType.RefCursor, ParameterDirection.Output);
                var conn = GetConnection();
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var query = "dashboard.TK_KhongHaiLong_CLPV_date";
                    result = SqlMapper.Query<TK_KhongHaiLong_CLPV>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<TK_KhongHaiLong_CLPV>();
                }
                insertKhongHaiLong_CLPV(result);
            }
            catch (Exception e)
            {
            }
        }
        private dynamic checkconvertKhongHaiLong_CLPV(string startime, string endtime)
        {
            string connStr = m_configuration.GetConnectionString("DefaultConnection1");
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"select endtimeupdate,endtime from TimeConvert where timeid = 5", conn))
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
                        convertKhongHaiLong_CLPV(startime, endtime);
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
                        using (var cmd1 = new SqlCommand(@"update TimeConvert set endtime = (convert(datetime,'" + time2.AddDays(1).ToString("dd/MM/yyyy") + "', 103)),timeinsert = " + timeinsert + " where timeid = 5", conn))
                        {
                            cmd1.ExecuteNonQuery();
                        }
                        convertKhongHaiLong_CLPV(etime.ToString("dd/MM/yyyy"), endtime); //convert tu ngay cuoi den ngay lay du lieu
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
