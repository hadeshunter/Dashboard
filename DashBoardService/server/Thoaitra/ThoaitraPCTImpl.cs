using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.connnection.oracle;
using ClassModel.model.ThoaitraPCT;
using ClassModel.model.RqGrafana;
using Dapper;
using DashBoardService.server.common;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace DashBoardService.server.ThoaitraPCT
{
    public class ThoaitraPCTImpl : IThoaitraPCT
    {
        private ICommon m_common;
        private IConfiguration m_configuration;
        public ThoaitraPCTImpl(ICommon m_common, IConfiguration m_configuration)
        {
            this.m_common = m_common;
            this.m_configuration = m_configuration;
        }
        public IDbConnection GetConnection()
        {
            var connectionString = m_configuration.GetSection("connectionstrings").GetSection("defaultconnection2").Value;
            var conn = new OracleConnection(connectionString);
            return conn;
        }
        public dynamic GetThoaitraPCT(string month)
        {
            //var date = m_common.convertToString(rq);
            //string month = date.Item1.Substring(6, 4) + date.Item1.Substring(3, 2);

            List<ThoaitraPCTModel> result = new List<ThoaitraPCTModel>();
            if (Int32.Parse(month) < Int32.Parse(DateTime.Now.ToString("yyyyMM")))
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("v_namthang", OracleDbType.Varchar2, ParameterDirection.Input, month);
                dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
                var conn = GetConnection();
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var query = "dashboard.tk_thoaitra_pct";
                    try
                    {
                        result = SqlMapper.Query<ThoaitraPCTModel>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<ThoaitraPCTModel>();
                    }
                    catch (Exception)
                    {
                        CreateTableCCDV_GBDB(month);
                        CreateTableThoaitra(month);
                        InsertDataToTableCCDV_GBDB(month);
                        InsertDataToTableThoaitra(month);
                        result = SqlMapper.Query<ThoaitraPCTModel>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<ThoaitraPCTModel>();
                    }
                    conn.Close();
                }
            }
            return result;
        }

        public dynamic GetDataForGrafana(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            var date = m_common.convertToString(rq);
            var startdate = new DateTime(Int32.Parse(date.Item1.Substring(6, 4)), Int32.Parse(date.Item1.Substring(3, 2)), Int32.Parse(date.Item1.Substring(0, 2)));
            var enddate = new DateTime(Int32.Parse(date.Item2.Substring(6, 4)), Int32.Parse(date.Item2.Substring(3, 2)), Int32.Parse(date.Item2.Substring(0, 2)));
            var listmonth = m_common.GetMonthsBetween(startdate, enddate);
            List<ThoaitraPCTModel> result = new List<ThoaitraPCTModel>();
            foreach (var month in listmonth)
            {
                result.AddRange(GetThoaitraPCT(month.ToString("yyyyMM")));
            }

            if (rq.targets[0].type == "table")
            {
                List<dynamic> lcolumns = new List<dynamic>();
                lcolumns.Add(new { text = "Đơn vị", type = "string" });
                foreach (var month in listmonth)
                {
                    lcolumns.Add(new { text = "PCT Thoái trả đối soát TTKD Tháng " + month.ToString("MM/yyyy"), type = "string" });
                }
                foreach (var month in listmonth)
                {
                    lcolumns.Add(new { text = "PCT Thoái trả Tháng " + month.ToString("MM/yyyy"), type = "string" });
                }
                List<dynamic> lrows = new List<dynamic>();
                var fiter = from item in result
                            group item by item.donvi into gr
                            orderby gr.Key
                            select gr;
                foreach (var group in fiter)
                {
                    List<dynamic> items = new List<dynamic>();
                    items.Add(group.Key);
                    if (rq.targets[0].target == "1")
                        foreach (var item in group)
                        {
                            items.Add(item.pct_thoaitra_ttkd);
                            items.Add(item.pct_thoaitra_ttvt);
                        }
                    else if (rq.targets[0].target == "2")
                        foreach (var item in group)
                        {
                            items.Add(item.pct_thoaitra_ttkd);
    
                        }
                    else
                        foreach (var item in group)
                        {
           
                            items.Add(item.pct_thoaitra_ttvt);
                        }

                    lrows.Add(items);

                }


                response = new List<dynamic>
                {
                    new
                    {
                        columns = lcolumns,
                        rows = lrows,
                        type = "table"
                    }
                };
                
            }
            else
            {
                if (rq.targets[0].target == "1")
                {
                    foreach (ThoaitraPCTModel item in result)
                    {
                        List<dynamic> points = new List<dynamic>();
                        points.Add(new List<dynamic> { item.pct_thoaitra_ttkd, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                        response.Add(new { target = "PCT Thoái trả đối soát TTKD " + item.donvi + " " + item.thang, datapoints = points });
                        points = new List<dynamic>();
                        points.Add(new List<dynamic> { item.pct_thoaitra_ttvt, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                        response.Add(new { target = "PCT Thoái trả " + item.donvi + " " + item.thang, datapoints = points });

                    }
           
                }else if(rq.targets[0].target == "2")
                    foreach (ThoaitraPCTModel item in result)
                    {
                        List<dynamic> points = new List<dynamic>();
                        points.Add(new List<dynamic> { item.pct_thoaitra_ttkd, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                        response.Add(new { target = "PCT Thoái trả đối soát TTKD " + item.donvi +" "+item.thang, datapoints = points });


                    }
                else
                    foreach (ThoaitraPCTModel item in result)
                    {
                        List<dynamic> points = new List<dynamic>();
                        points.Add(new List<dynamic> { item.pct_thoaitra_ttvt, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                        response.Add(new { target = "PCT Thoái trả " + item.donvi + " " + item.thang, datapoints = points });

                    }
            }
            return response;
        }
        public dynamic CreateTableThoaitra(string month)
        {
            try
            {
                var conn = GetConnection();
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var query = "create table THOAITRA_"+ month + "("
                                + " HDKH_ID NUMBER(12,0),"
                                + " MA_GD VARCHAR2(16 BYTE),"
                                + " MA_HD VARCHAR2(30 BYTE),"
                                + " MA_KH VARCHAR2(20 BYTE),"
                                + " KHACHHANG_ID NUMBER(12,0),"
                                + " TEN_KH VARCHAR2(500 BYTE),"
                                + " DIACHI_KH VARCHAR2(500 BYTE),"
                                + " NGAYLAP_HD DATE,"
                                + " GHICHU  VARCHAR2(500 BYTE),"
                                + " DONVI_ID NUMBER(5,0),"
                                + " LOAIGT_ID NUMBER(5,0),"
                                + " NHANVIEN_ID NUMBER(5,0),"
                                + " KHLON_ID NUMBER(2,0),"
                                + " LOAIHD_ID NUMBER(3,0),"
                                + " BOSUNGTB_ID NUMBER(1,0),"
                                + " LOAIKH_ID NUMBER(2,0),"
                                + " NGAY_YC DATE,"
                                + " NGAY_CN DATE,"
                                + " HDKH_CHA_ID NUMBER(12,0),"
                                + " HDTB_ID NUMBER(10,0),"
                                + " THUEBAO_ID NUMBER(12,0),"
                                + " MA_TB VARCHAR2(30 BYTE),"
                                + " TEN_TB VARCHAR2(500 BYTE),"
                                + " TTHD_ID NUMBER(5,0),"
                                + " DONVI_ID_TB NUMBER(5,0),"
                                + " NGAY_HT DATE,"
                                + " NGAY_TT DATE,"
                                + " DIACHI_LD VARCHAR2(500 BYTE) )";

                    SqlMapper.Query(conn, query, param: null, commandType: CommandType.Text);
                }
                return 1;
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public dynamic CreateTableCCDV_GBDB(string month)
        {
            try
            {
                var conn = GetConnection();
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var query = "create table CCDV_GBDB_" + month + "("
                                + " HDTB_ID NUMBER(10,0),"
                                +" MA_GD VARCHAR2(16 BYTE),"
                                +" HDKH_ID NUMBER(12,0),"
                                +" MA_TB VARCHAR2(30 BYTE),"
                                +" TEN_TB VARCHAR2(500 BYTE),"
                                +" DIACHI_TB VARCHAR2(4000 BYTE),"
                                +" TEN_DT VARCHAR2(100 BYTE),"
                                +" TEN_KH VARCHAR2(500 BYTE),"
                                +" MA_PLKH VARCHAR2(10 BYTE),"
                                +" LOAIHD_ID NUMBER(3,0),"
                                +" TTHD_ID NUMBER(5,0),"
                                +" LOAITB_ID NUMBER(5,0),"
                                +" LOAIHINH_TB VARCHAR2(50 BYTE),"
                                +" TEN_KIEULD VARCHAR2(100 BYTE),"
                                +" NGAY_YC DATE,"
                                +" NGAYCN_BBBG DATE,"
                                +" NGAY_BBBG DATE,"
                                +" NGAYGIAO_DHTT   DATE,"
                                +" NGAYGIAO_DHTT2 DATE,"
                                +" NGAYGIAO_TTVT   DATE,"
                                +" NGAYGIAO_TTVT2 DATE,"
                                +" NGAY_HEN    DATE,"
                                +" NGAY_TT DATE,"
                                +" NGAYTRA_TTDH    DATE,"
                                +" LYDOTRA_ID_TTDH NUMBER(5,0),"
                                +" TEN_LYDO_TTDH VARCHAR2(341 BYTE),"
                                +" ND_TRA_TTDH VARCHAR2(600 BYTE),"
                                +" ND_TRA_TTVT VARCHAR2(600 BYTE),"
                                +" NGAYTRA_TTVT DATE,"
                                +" LYDOTRA_ID_TTVT NUMBER(5, 0),"
                                +" TEN_LYDO_TTVT VARCHAR2(341 BYTE),"
                                +" NHANVIEN_HOANTAT VARCHAR2(432 BYTE),"
                                +" DONVI_ID NUMBER(5,0),"
                                +" DONVI VARCHAR2(100 BYTE),"
                                +" DONVI_CHA VARCHAR2(150 BYTE),"
                                +" DONVI_CHA_ID NUMBER(5,0),"
                                +" DONVI_ID_HOSO NUMBER(5,0),"
                                +" DONVI_NHAN_HOSO VARCHAR2(203 BYTE),"
                                +" NHANVIEN_ID_HOSO NUMBER(5,0),"
                                +" NHANVIEN_NHAN_HOSO VARCHAR2(436 BYTE),"
                                +" NHANVIEN_TIEPTHI VARCHAR2(436 BYTE),"
                                +" DONVI_TIEPTHI VARCHAR2(203 BYTE),"
                                +" IP_CN VARCHAR2(30 BYTE) )";

                    SqlMapper.Query(conn, query, param: null, commandType: CommandType.Text);
                }
                return 1;
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public dynamic InsertDataToTableCCDV_GBDB(string month)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("v_namthang", OracleDbType.Varchar2, ParameterDirection.Input, month);
               

                var conn = GetConnection();
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var query = "dashboard.THEM_DULIEU_VAO_TALBE_GBDB_CCDV_THANG";
                    SqlMapper.Query(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    conn.Close();
                }
                return 1;
            }
            catch (Exception ex)
            {

                return 0;
            }
        }
        public dynamic InsertDataToTableThoaitra(string month)
        {
            try
            {
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("v_namthang", OracleDbType.Varchar2, ParameterDirection.Input, month);
               

                var conn = GetConnection();
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var query = "dashboard.THEM_DULIEU_VAO_TALBE_THOAITRA_THANG";
                    SqlMapper.Query(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                    conn.Close();
                }
                return 1;
            }
            catch (Exception ex)
            {

                return 0;
            }
        }

        public dynamic GetThoaitraPCTDate(RqGrafana rq)
        {
            var date = m_common.convertToString(rq);
            List<ThoaitraPTC> result = new List<ThoaitraPTC>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, date.Item1);
            dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, date.Item2);
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.tk_thoaitra_pct_date";
                result = SqlMapper.Query<ThoaitraPTC>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<ThoaitraPTC>();
            }
            return result;
        }
    }
}
