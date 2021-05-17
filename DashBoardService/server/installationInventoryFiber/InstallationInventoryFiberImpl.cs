using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.connnection.oracle;
using ClassModel.model.installationInventoryFiber;
using ClassModel.model.RqGrafana;
using Dapper;
using DashBoardService.server.common;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace DashBoardService.server.installationInventoryFiber
{
    public class InstallationInventoryFiberImpl : IInstallationInventoryFiber
    {
        private ICommon m_common;
        private IConfiguration m_configuration;
        public InstallationInventoryFiberImpl(ICommon m_common, IConfiguration m_configuration)
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

        public dynamic GetInstallationInventoryFiberByDate(string month)
        {
            //var date = m_common.convertToString(rq);
            //string month = date.Item1.Substring(6, 4) + date.Item1.Substring(3, 2);
            List<installationInventoryFiberModel> result = new List<installationInventoryFiberModel>();
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
                var query = "dashboard.tk_giamtyle_tonlapdat_fiber_date";
                try
                {
                    result = SqlMapper.Query<installationInventoryFiberModel>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<installationInventoryFiberModel>();
                }
                catch (Exception)
                {
                    CreateTable(month);
                    InsertDataToTable(month);
                    result = SqlMapper.Query<installationInventoryFiberModel>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<installationInventoryFiberModel>();
                }
                conn.Close();
            }
            return result;
        }

        public dynamic GetInstallationInventoryFiber(string month)
        {
            //var date = m_common.convertToString(rq);
            //string month = date.Item1.Substring(6, 4) + date.Item1.Substring(3, 2);
            List<installationInventoryFiberModel> result = new List<installationInventoryFiberModel>();
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
                var query = "dashboard.tk_giamtyle_tonlapdat_fiber";
                try
                {
                    result = SqlMapper.Query<installationInventoryFiberModel>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<installationInventoryFiberModel>();
                }
                catch (Exception)
                {
                    CreateTable(month);
                    InsertDataToTable(month);
                    result = SqlMapper.Query<installationInventoryFiberModel>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<installationInventoryFiberModel>();
                }
                conn.Close();
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
            List<installationInventoryFiberModel> result = new List<installationInventoryFiberModel>();
            foreach (dynamic month in listmonth)
            {
                result.AddRange(GetInstallationInventoryFiber(month.ToString("yyyyMM")));
            }
             
            if (rq.targets[0].type == "table") {
                List<dynamic> lcolumns = new List<dynamic>();
                lcolumns.Add(new { text = "Đơn vị", type = "string" });
                foreach (var month in listmonth)
                {
                    lcolumns.Add(new { text = "Tháng " + month.ToString("MM/yyyy"), type = "string" });
                }
                
                List<dynamic> lrows = new List<dynamic>();
                var fiter = from item in result
                            group item by item.ten_dv into gr
                            orderby gr.Key
                            select gr;
                foreach (var group in fiter)
                {
                    List<dynamic> items = new List<dynamic>();
                    items.Add(group.Key);
                 
                    foreach (var item in group)
                    {
                        items.Add(item.tyle_ton);

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
            else {
                
                foreach(installationInventoryFiberModel item in result)
                {
                    List<dynamic> points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.tyle_ton, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = item.ten_dv+ " " +item.thang, datapoints = points });
                    
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
                    var query = "create table ton_ccdv_"+month+" ("
                                +"HDTB_ID NUMBER(10,0),"
                                +"MA_GD VARCHAR2(16 BYTE),"
                                +"HDKH_ID NUMBER(12,0),"
                                +"MA_TB VARCHAR2(30 BYTE),"
                                +"TEN_TB VARCHAR2(500 BYTE),"
                                +"DIACHI_LD VARCHAR2(500 BYTE),"
                                +"TEN_DT VARCHAR2(100 BYTE),"
                                +"MA_PLKH VARCHAR2(10 BYTE),"
                                +"TTHD_ID NUMBER(5,0),"
                                +"LOAITB_ID NUMBER(5,0),"
                                +"LOAIHINH_TB VARCHAR2(50 BYTE),"
                                +"DICHVUVT_ID NUMBER(2,0),"
                                +"TEN_DVVT VARCHAR2(50 BYTE),"
                                +"TEN_KIEULD VARCHAR2(100 BYTE),"
                                +"NGAY_YC DATE,"
                                +"NGAYCN_BBBG DATE,"
                                +"NGAY_BBBG DATE,"
                                +"NGAYGIAO_DHTT   DATE,"
                                +"NGAYGIAO_DHTT2 DATE,"
                                +"NGAYGIAO_TTVT   DATE,"
                                +"NGAYGIAO_TTVT2 DATE,"
                                +"NGAY_HEN    DATE,"
                                +"NGAY_TT DATE,"
                                +"ND_TRA_TTDH VARCHAR2(400 BYTE),"
                                +"NGAYTRA_TTDH DATE,"
                                +"LYDOTRA_ID_TTDH NUMBER(5, 0),"
                                +"TEN_LYDO_TTDH VARCHAR2(300 BYTE),"
                                +"NHOM_TON_TTDH VARCHAR2(100 BYTE),"
                                +"ND_TRA_TTVT VARCHAR2(400 BYTE),"
                                +"NGAYTRA_TTVT DATE,"
                                +"LYDOTRA_ID_TTVT NUMBER(5, 0),"
                                +"TEN_LYDO_TTVT VARCHAR2(300 BYTE),"
                                +"DONVI_ID NUMBER,"
                                +"DONVI   VARCHAR2(100 BYTE),"
                                +"DONVI_CHA_ID NUMBER(5,0),"
                                +"DONVI_CHA VARCHAR2(150 BYTE),"
                                + "MA_NV_TTVT VARCHAR2(30 BYTE),"
                                + "TEN_NV_TTVT VARCHAR2(200 BYTE),"
                                + "DONVI_TTKD VARCHAR2(100 BYTE),"
                                + "PHONG_TTKD VARCHAR2(100 BYTE),"
                                + "MA_NV_TTKD VARCHAR2(30 BYTE),"
                                + "TEN_NV_TTKD VARCHAR2(200 BYTE),"
                                + "SDT_NV_TTKD VARCHAR2(200 BYTE),"
                                + "NHANVIEN_TIEPTHI VARCHAR2(436 BYTE),"
                                + "DONVI_TIEPTHI VARCHAR2(203 BYTE) )";
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
                    var query = "dashboard.them_dulieu_vao_table_ton_ccdv";
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
