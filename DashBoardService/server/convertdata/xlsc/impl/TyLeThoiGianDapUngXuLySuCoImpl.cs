using ClassModel.connnection.oracle;
using ClassModel.convertdata.xlsc;
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

namespace DashBoardService.server.convertdata.xlsc.impl
{
    public class TyLeThoiGianDapUngXuLySuCoImpl: ITyLeThoiGianDapUngXuLySuCo
    {
        private IConfiguration m_configuration;
        public TyLeThoiGianDapUngXuLySuCoImpl(IConfiguration _configuration)
        {
            m_configuration = _configuration;
        }

        public void insertCCDV(List<TyLeThoiGianDapUngXuLySuCo> listData)
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
                        var query = "INSERT INTO TyLeThoiGianDapUngXuLySuCo(donvi_id,dv_cha_id,ten_dv,ten_trungtam,ngay_ht,nhomlc_id,st_tong,bh_tong,st_quagio,tyle_chuagiamtru,timeinsert)" +
                                  "VALUES(" + item.donvi_id + "," + item.dv_cha_id + ",N'" + item.ten_dv + "',N'" + item.ten_trungtam + "','" + item.ngay_ht.Month + "-" + item.ngay_ht.Day + "-" + item.ngay_ht.Year + "'," + item.nhomlc_id + ","
                                  + item.st_tong + "," + item.bh_tong + "," + item.st_quagio + "," + item.tyle_chuagiamtru
                                  + "," + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString()
                                  + ")";
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
                return checkconvertXlscToDb(startime, endtime);
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public void convertXlscToDb(string startime, string endtime)//string startime, string endtime
        {
            try
            {

                List<TyLeThoiGianDapUngXuLySuCo> result = new List<TyLeThoiGianDapUngXuLySuCo>();
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
                    var query = "dashboard.Ty_Le_Dap_Ung_Thoi_Gian_Xu_Ly_Su_Co_KTR_TSL";
                    result = SqlMapper.Query<TyLeThoiGianDapUngXuLySuCo>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<TyLeThoiGianDapUngXuLySuCo>();
                    conn.Close();
                }
                insertCCDV(result);
            }
            catch (Exception e)
            {
            }
        }
        private dynamic checkconvertXlscToDb(string startime, string endtime)
        {
            string connStr = m_configuration.GetConnectionString("DefaultConnection1");
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"select endtimeupdate,endtime from TimeConvert where timeid = 3", conn))
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
                        convertXlscToDb(startime, endtime);
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
                        using (var cmd1 = new SqlCommand(@"update TimeConvert set endtime = (convert(datetime,'" + time2.AddDays(1).ToString("dd/MM/yyyy") + "', 103)),timeinsert = " + timeinsert + " where timeid = 3", conn))
                        {
                            cmd1.ExecuteNonQuery();
                        }
                        convertXlscToDb(etime.ToString("dd/MM/yyyy"), endtime); //convert tu ngay cuoi den ngay lay du lieu
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
