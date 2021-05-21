using ClassModel.connnection.oracle;
using ClassModel.model.installationInventoryFiber;
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

namespace DashBoardService.server.convertdata.tonLDFiber.impl
{
    public class TonLapdatFiberImpl : ITonLapdatFiber
    {
        private IConfiguration m_configuration;
        public TonLapdatFiberImpl(IConfiguration _configuration)
        {
            m_configuration = _configuration;
        }

        public void insertTonLDFiber(List<installationInventoryFiberModel> listData)
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
                        var query = "INSERT INTO TonLDFiber(donvi_id,donvi_cha_id,ten_dv,ten_dv_cha,thang,ngay_yc,tong_fiber,ton_fiber,lapdat_fiber,tyle_ton,timeinsert)" +
                                  "VALUES(" + item.donvi_id + "," + item.donvi_cha_id + ",N'" + item.ten_dv + "',N'" + item.ten_dv_cha + "'," + item.thang + 
                                  ",'" + item.ngay_yc.Month + "-" + item.ngay_yc.Day + "-" + item.ngay_yc.Year + "'," + item.tong_fiber + "," + item.ton_fiber + "," + item.lapdat_fiber + "," + item.tyle_ton + "," + 
                                  DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + ")";
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
                return checkconvertTonLDFiberToDb(startime, endtime);
            }
            catch (Exception e)
            {
                return e;
            }
        }
        public void convertTonLDFiberToDb(string startime, string endtime)//string startime, string endtime
        {
            try
            {

                List<installationInventoryFiberModel> result = new List<installationInventoryFiberModel>();
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
                    var query = "dashboard.tk_giamtyle_tonlapdat_fiber_date";
                    result = SqlMapper.Query<installationInventoryFiberModel>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<installationInventoryFiberModel>();
                }
                insertTonLDFiber(result);
            }
            catch (Exception e)
            {
            }
        }
        private dynamic checkconvertTonLDFiberToDb(string startime, string endtime)
        {
            string connStr = m_configuration.GetConnectionString("DefaultConnection1");
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"select endtimeupdate,endtime from TimeConvert where timeid = 6", conn))
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
                        convertTonLDFiberToDb(startime, endtime);
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
                        using (var cmd1 = new SqlCommand(@"update TimeConvert set endtime = (convert(datetime,'" + time2.AddDays(1).ToString("dd/MM/yyyy") + "', 103)),timeinsert = " + timeinsert + " where timeid = 6", conn))
                        {
                            cmd1.ExecuteNonQuery();
                        }
                        convertTonLDFiberToDb(etime.ToString("dd/MM/yyyy"), endtime); //convert tu ngay cuoi den ngay lay du lieu
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
