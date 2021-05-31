using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.connnection.oracle;
using ClassModel.model.ThoaitraNLML;
using ClassModel.model.RqGrafana;
using Dapper;
using DashBoardService.server.common;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace DashBoardService.server.ThoaitraNLML
{
    public class ThoaitraNLMLImpl : IThoaitraNLML
    {
        private ICommon m_common;
        private IConfiguration m_configuration;
        public ThoaitraNLMLImpl(ICommon m_common, IConfiguration m_configuration)
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
        public dynamic GetThoaitraNLMLDate(RqGrafana rq)
        {
            List<ThoaitraNLMLModel_date> result = new List<ThoaitraNLMLModel_date>();
            var date = m_common.convertToString(rq);
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
                var query = "dashboard.tk_thoaitra_nlml_date";
                result = SqlMapper.Query<ThoaitraNLMLModel_date>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<ThoaitraNLMLModel_date>();
            }
            return result;
        }
        public dynamic GetThoaitraNLML(string month)
        {
            List<ThoaitraNLMLModel> result = new List<ThoaitraNLMLModel>();
            if (Int32.Parse(month) < Int32.Parse(DateTime.Now.ToString("yyyyMM")))
            {
                //var date = m_common.convertToString(rq);
                //string month = date.Item1.Substring(6, 4) + date.Item1.Substring(3, 2);
                
                var dyParam = new OracleDynamicParameters();
                dyParam.Add("v_thang", OracleDbType.Varchar2, ParameterDirection.Input, month);
                dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
                var conn = GetConnection();
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                if (conn.State == ConnectionState.Open)
                {
                    var query = "dashboard.tk_thoaitra_nlml";
                    try
                    {
                        result = SqlMapper.Query<ThoaitraNLMLModel>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<ThoaitraNLMLModel>();
                    }
                    catch (Exception)
                    {
                        CreateTable(month);
                        InsertDataToTable(month);
                        result = SqlMapper.Query<ThoaitraNLMLModel>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<ThoaitraNLMLModel>();

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
            List<ThoaitraNLMLModel> result = new List<ThoaitraNLMLModel>();
            foreach (dynamic month in listmonth)
            {
                result.AddRange(GetThoaitraNLML(month.ToString("yyyyMM")));
            }

            if (rq.targets[0].type == "table")
            {
                List<dynamic> lcolumns = new List<dynamic>();
                lcolumns.Add(new { text = "Đơn vị", type = "string" });
                foreach (var month in listmonth)
                {
                    lcolumns.Add(new { text = "Thoái trả độc quyền Tháng " + month.ToString("MM/yyyy"), type = "string" });
                }
                foreach (var month in listmonth)
                {
                    lcolumns.Add(new { text = "Thoái trả NLML Tháng " + month.ToString("MM/yyyy"), type = "string" });
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
         
                    foreach (var item in group)
                    {
                        items.Add(item.thoaitra_docquyen);
                        items.Add(item.thoaitra_nlml);
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
            
                foreach (ThoaitraNLMLModel item in result)
                {
                    List<dynamic> points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.thoaitra_docquyen, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "Thoái trả ĐQ "+item.donvi +" "+ item.thang, datapoints = points });
                    points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.thoaitra_nlml, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "Thoái trả NLML " + item.donvi + " " + item.thang, datapoints = points });

                }
            }
            return response;
        }
        public dynamic CreateTable(string month)
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
                    var query = "create table CCDV_VTTH_" + month + " ("
                                + "HDTB_ID NUMBER(10,0),"
                                + "MA_GD VARCHAR2(16 BYTE),"
                                + "HDKH_ID NUMBER(12,0),"
                                + "MA_TB VARCHAR2(30 BYTE),"
                                + "TEN_TB VARCHAR2(500 BYTE),"
                                + "DIACHI_TB VARCHAR2(4000 BYTE),"
                                + "TEN_DT VARCHAR2(100 BYTE),"
                                + "TEN_KH VARCHAR2(500 BYTE),"
                                + "MA_PLKH VARCHAR2(10 BYTE),"
                                + "LOAIHD_ID NUMBER(3,0),"
                                + "TTHD_ID NUMBER(5,0),"
                                + "TRANGTHAI_HD	VARCHAR2(50 BYTE),"
                                + "LOAITB_ID NUMBER(5,0),"
                                + "LOAIHINH_TB VARCHAR2(50 BYTE),"
                                + "TEN_KIEULD VARCHAR2(100 BYTE),"
                                + "NGAY_YC DATE,"
                                + "NGAYCN_BBBG DATE,"
                                + "NGAY_BBBG DATE,"
                                + "NGAYGIAO_DHTT   DATE,"
                                + "NGAYGIAO_DHTT2 DATE,"
                                + "NGAYGIAO_TTVT   DATE,"
                                + "NGAYGIAO_TTVT2 DATE,"
                                + "NGAY_HEN    DATE,"
                                + "NGAY_TT DATE,"
                                + "LYDO_THOAITRA VARCHAR2(341 BYTE),"
                                + "NHOM_THOAITRA VARCHAR2(100 BYTE),"
                                + "NGAYTRA_TTDH	DATE,"
                                + "LYDOTRA_ID_TTDH	NUMBER(5,0),"
                                + "TEN_LYDO_TTDH	VARCHAR2(341 BYTE),"
                                + "ND_TRA_TTDH	VARCHAR2(600 BYTE),"
                                + "ND_TRA_TTVT	VARCHAR2(600 BYTE),"
                                + "NGAYTRA_TTVT	DATE,"
                                + "LYDOTRA_ID_TTVT	NUMBER(5,0),"
                                + "TEN_LYDO_TTVT	VARCHAR2(341 BYTE),"
                                + "NHANVIEN_HOANTAT	VARCHAR2(432 BYTE),"
                                + "DONVI_ID	NUMBER(5,0),"
                                + "DONVI VARCHAR2(100 BYTE),"
                                + "DONVI_CHA	VARCHAR2(150 BYTE),"
                                + "DONVI_CHA_ID	NUMBER(5,0),"
                                + "DONVI_ID_HOSO	NUMBER(5,0),"
                                + "DONVI_NHAN_HOSO	VARCHAR2(203 BYTE),"
                                + "NHANVIEN_ID_HOSO	NUMBER(5,0),"
                                + "NHANVIEN_NHAN_HOSO	VARCHAR2(436 BYTE),"
                                + "NHANVIEN_TIEPTHI	VARCHAR2(436 BYTE),"
                                + "DONVI_TIEPTHI	VARCHAR2(203 BYTE),"
                                + "IP_CN VARCHAR2(30 BYTE))";
                    SqlMapper.Query(conn, query, param: null, commandType: CommandType.Text);
                }
                return 1;
            }
            catch (Exception)
            {

                return 0;
            }
        }
        public dynamic InsertDataToTable(string month)
        {
            try
            {
                    var dyParam = new OracleDynamicParameters();
                    dyParam.Add("v_namthang", OracleDbType.Varchar2, ParameterDirection.Input, month);
                    dyParam.Add("Return_Value", OracleDbType.Int16, ParameterDirection.ReturnValue);

                    var conn = GetConnection();
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    if (conn.State == ConnectionState.Open)
                    {
                        var query = "dashboard.them_dulieu_vao_table_ccdv_vtth";
                        SqlMapper.Query(conn, query, param: dyParam, commandType: CommandType.StoredProcedure);
                        conn.Close();
                    }
                    return 1;
            }
            catch (Exception)
            {

                return 0;
            }
        }
        

    }
}
