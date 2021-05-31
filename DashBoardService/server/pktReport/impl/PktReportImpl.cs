using ClassModel.connnection.oracle;
using ClassModel.connnection.reponsitory.impl;
using ClassModel.connnection.sql;
using ClassModel.convertdata.ccdv;
using ClassModel.model.BTS;
using ClassModel.model.cable;
using ClassModel.model.pktReport;
using ClassModel.model.reflection;
using ClassModel.model.RqGrafana;
using Dapper;
using DashBoardService.server.common;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.pktReport.impl
{
    public class PktReportImpl : Reponsitory<dynamic>, IPktReport
    {
        private ICommon m_common;
        private IConfiguration m_configuration;
        public PktReportImpl(ICommon common, IConfiguration configuration, DataContext context) : base(context)
        {
            m_configuration = configuration;
            m_common = common;
        }

        public IDbConnection GetConnection()
        {
            var connectionString = m_configuration.GetSection("connectionstrings").GetSection("defaultconnection2").Value;
            var conn = new OracleConnection(connectionString);
            return conn;
        }

        public dynamic GetMonthsBetween(DateTime from, DateTime to)
        {
            if (from > to) return GetMonthsBetween(to, from);

            var monthDiff = Math.Abs((to.Year * 12 + (to.Month - 1)) - (from.Year * 12 + (from.Month - 1)));

            if (from.AddMonths(monthDiff) > to || to.Day < from.Day)
            {
                monthDiff -= 1;
            }

            List<dynamic> results = new List<dynamic>();
            for (int i = monthDiff; i >= 0; i--)
            {
                results.Add(to.AddMonths(-i));
            }

            return results;
        }

        public dynamic executeCCDVDTG(RqGrafana rq)
        {
            var date = m_common.convertToString(rq);
            List<Ccdv_Dung_Tg> result = new List<Ccdv_Dung_Tg>();
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
                var query = "dashboard.Cung_Cap_DV_Dung_TG_Quy_Dinh";
                result = SqlMapper.Query<Ccdv_Dung_Tg>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<Ccdv_Dung_Tg>();
                conn.Close();
            }
            return result;
        }
        public dynamic getCCDVDTG(RqGrafana rq)
        {
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            List<dynamic> response = new List<dynamic>();
            List<Ccdv_Dung_Tg> list = executeCCDVDTG(rq);
            var list_CCDVDTG = list
                .GroupBy(l => new { l.donvi_cha_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        ten_dv = lg.Key.ten_dvql,
                        tong_pct = lg.Sum(l => l.tong_pct),
                        soluong_khonghen_ccdv = lg.Sum(l => l.soluong_khonghen_ccdv),
                        ok_khonghen_ccdv = lg.Sum(l => l.ok_khonghen_ccdv),
                        tregio_khonghen_ccdv = lg.Sum(l => l.tregio_khonghen_ccdv),
                        soluong_cohen_ccdv = lg.Sum(l => l.soluong_cohen_ccdv),
                        ok_cohen_ccdv = lg.Sum(l => l.ok_cohen_ccdv),
                        tregio_cohen_ccdv = lg.Sum(l => l.tregio_cohen_ccdv),
                        tyle_ccdv = Math.Round((double)(lg.Sum(l => l.ok_khonghen_ccdv) + lg.Sum(l => l.ok_cohen_ccdv)) * 100 / lg.Sum(l => l.tong_pct), 4)
                    });
            if (rq.targets[0].type == "table")
            {
                
                List<dynamic> col = new List<dynamic>
                    {
                        new { text = "Tên đơn vị", type = "string"}, //String thì ko hiện lên chart
                        new { text = "Tổng PCT", type = "number"}, //Number hiện sô liệu lên chart
                        new { text = "SL không hẹn", type = "number"}, //Number hiện sô liệu lên chart
                        new { text = "Đúng giờ không hẹn", type = "number"},
                        new { text = "Trễ giờ không hẹn", type = "number"},
                        new { text = "SL có hẹn", type = "number"}, //Number hiện sô liệu lên chart
                        new { text = "Đúng giờ có hẹn", type = "number"},
                        new { text = "Trễ giờ có hẹn", type = "number"},
                        new { text = "Tỷ lệ CCDV", type = "number"}
                    };
                List<dynamic> row = new List<dynamic>();
                foreach (var item in list_CCDVDTG)
                {
                    row.Add(new List<dynamic> {
                        item.ten_dv,
                        item.tong_pct,
                        item.soluong_khonghen_ccdv,
                        item.ok_khonghen_ccdv,
                        item.tregio_khonghen_ccdv,
                        item.soluong_cohen_ccdv,
                        item.ok_cohen_ccdv,
                        item.tregio_cohen_ccdv,
                        item.tyle_ccdv
                    });
                }
                response = new List<dynamic> {
                        new {
                                columns = col,
                                rows = row,
                                type = "table"
                            }
                    };
            }
            else
            {
                foreach (var item in list_CCDVDTG)
                {
                    List<dynamic> points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.tyle_ccdv, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = item.ten_dv, datapoints = points });
    
                }
            }
            return response;
        }

        public dynamic executeSCDVDTG(RqGrafana rq)
        {

            var date = m_common.convertToString(rq);
            List<SCDVDTG> result = new List<SCDVDTG>();
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
                var query = "dashboard.Sua_Chua_DV_Dung_TG_Quy_Dinh";
                result = SqlMapper.Query<SCDVDTG>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<SCDVDTG>();
                conn.Close();
            }
            return result;
        }
        public dynamic getSCDVDTG(RqGrafana rq)
        {
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            List<dynamic> response = new List<dynamic>();
            List<SCDVDTG> list_SCDVDTG = executeSCDVDTG(rq);
            if (rq.targets[0].type == "table")
            {
                
                List<dynamic> col = new List<dynamic>
                    {
                        new { text = "Tên đơn vị", type = "string"}, //String thì ko hiện lên chart
                        new { text = "BH DTCD", type = "number"}, //Number hiện sô liệu lên chart
                        new { text = "BH Mega", type = "number"}, //Number hiện sô liệu lên chart
                        new { text = "BH Fiber", type = "number"},
                        new { text = "BH MyTV", type = "number"},
                        new { text = "BH Khác", type = "number"}, //Number hiện sô liệu lên chart
                        new { text = "BH Tổng", type = "number"},
                        new { text = "ST DTCD", type = "number"},
                        new { text = "ST Mega", type = "number"}, //Number hiện sô liệu lên chart
                        new { text = "ST Fiber", type = "number"},
                        new { text = "ST MyTV", type = "number"},
                        new { text = "ST Khác", type = "number"}, //Number hiện sô liệu lên chart
                        new { text = "ST Tổng", type = "number"},
                        new { text = "Tỷ lệ chưa giảm trừ", type = "number"},
                    };
                List<dynamic> row = new List<dynamic>();
                foreach (SCDVDTG item in list_SCDVDTG)
                {
                    row.Add(new List<dynamic> {
                        item.ten_donvi,
                        item.bh_dtcd,
                        item.bh_mega,
                        item.bh_fiber,
                        item.bh_mytv,
                        item.bh_khac,
                        item.bh_tong,
                        item.st_dtcd,
                        item.st_mega,
                        item.st_fiber,
                        item.st_mytv,
                        item.st_khac,
                        item.st_tong,
                        item.tyle_chuagiamtru
                    });
                }
                response = new List<dynamic> {
                        new {
                                columns = col,
                                rows = row,
                                type = "table"
                            }
                    };
            }
            else
            { 
                foreach (SCDVDTG item in list_SCDVDTG)
                {
                    List<dynamic> points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.tyle_chuagiamtru, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = item.ten_donvi, datapoints = points });

                }
            }
            return response;
        }

        public dynamic executeTLDUTGXLSC(RqGrafana rq)
        {
            var date = m_common.convertToString(rq);
            List<TLDUTGXLSC> result = new List<TLDUTGXLSC>();
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
                var query = "dashboard.Ty_Le_Dap_Ung_Thoi_Gian_Xu_Ly_Su_Co_KTR_TSL";
                result = SqlMapper.Query<TLDUTGXLSC>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<TLDUTGXLSC>();
                conn.Close();
            }
            return result;
        }
        public dynamic getTLDUTGXLSC(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            List<TLDUTGXLSC> list = executeTLDUTGXLSC(rq);
            var list_TLDUTGXLSC = list
                .GroupBy(l => new { l.dv_cha_id, l.ten_trungtam })
                .Select(lg =>
                    new
                    {
                        lg.Key.ten_trungtam,
                        st_tong = lg.Sum(l => l.st_tong),
                        bh_tong = lg.Sum(l => l.bh_tong),
                        st_quagio = lg.Sum(l => l.st_quagio),
                        tyle_chuagiamtru = Math.Round((double)(lg.Sum(l => l.st_tong)) * 100 / lg.Sum(l => l.bh_tong), 4)
                    });
            if (rq.targets[0].type == "table")
            {
                List<dynamic> col = new List<dynamic>
                    {
                        new { text = "Tên đơn vị", type = "string"},
                        new { text = "ST Tổng", type = "number"},
                        new { text = "BH Tổng", type = "number"},
                        new { text = "ST Quá giờ", type = "number"},
                        new { text = "Tỷ lệ chưa giảm trừ", type = "number"},
                    };
                List<dynamic> row = new List<dynamic>();
                foreach (var item in list_TLDUTGXLSC)
                {
                    row.Add(new List<dynamic> {
                        item.ten_trungtam,
                        item.st_tong,
                        item.bh_tong,
                        item.st_quagio,
                        item.tyle_chuagiamtru
                    });
                }
                response = new List<dynamic> {
                        new {
                                columns = col,
                                rows = row,
                                type = "table"
                            }
                    };
            }
            else
            {
                DateTime dngay = Convert.ToDateTime(rq.range.to);
                long unix_time = m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year);
                if (rq.targets[0].target == "1") //ST Tổng
                {
                    List<TLDUTGXLSC> result = new List<TLDUTGXLSC>();
                    foreach (var item in list_TLDUTGXLSC)
                    {
                        TLDUTGXLSC item2 = new TLDUTGXLSC();
                        item2.ten_trungtam = item.ten_trungtam.Replace("Trung Tâm Viễn Thông ", "TTVT ") + " - ST Tổng";
                        item2.st_tong = item.st_tong;
                        result.Add(item2);
                    }
                    int i = 1;

                    foreach (var unit in result)
                    {

                        List<dynamic> points = new List<dynamic>();
                        points.Add(new List<dynamic> { unit.st_tong
                            , m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });

                        response.Add(new { target = unit.ten_trungtam, datapoints = points });
                        i++;
                    }

                }

                if (rq.targets[0].target == "2") //BH Tổng
                {
                    List<TLDUTGXLSC> result = new List<TLDUTGXLSC>();
                    foreach (var item in list_TLDUTGXLSC)
                    {
                        TLDUTGXLSC item2 = new TLDUTGXLSC();
                        item2.ten_trungtam = item.ten_trungtam.Replace("Trung Tâm Viễn Thông ", "TTVT ") + " - BH Tổng";
                        item2.bh_tong = item.bh_tong;
                        result.Add(item2);
                    }
                    int i = 1;

                    foreach (var unit in result)
                    {

                        List<dynamic> points = new List<dynamic>();
                        points.Add(new List<dynamic> { unit.bh_tong
                            , m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });

                        response.Add(new { target = unit.ten_trungtam, datapoints = points });
                        i++;
                    }

                }
                if (rq.targets[0].target == "3") //ST quá giờ
                {
                    List<TLDUTGXLSC> result = new List<TLDUTGXLSC>();
                    foreach (var item in list_TLDUTGXLSC)
                    {
                        TLDUTGXLSC item2 = new TLDUTGXLSC();
                        item2.ten_trungtam = item.ten_trungtam.Replace("Trung Tâm Viễn Thông ", "TTVT ") + " - ST Quá giờ";
                        item2.st_quagio = item.st_quagio;
                        result.Add(item2);
                    }
                    int i = 1;

                    foreach (var unit in result)
                    {

                        List<dynamic> points = new List<dynamic>();
                        points.Add(new List<dynamic> { unit.st_quagio
                            , m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });

                        response.Add(new { target = unit.ten_trungtam, datapoints = points });
                        i++;
                    }

                }
                if (rq.targets[0].target == "4") //Biểu đồ cột
                {
                    List<TLDUTGXLSC> result2 = new List<TLDUTGXLSC>();
                    foreach (var item in list_TLDUTGXLSC)
                    {
                        if (item.st_tong >= 0)
                        {
                            TLDUTGXLSC item2 = new TLDUTGXLSC();
                            item2.solieu = item.st_tong;
                            item2.ten_trungtam = item.ten_trungtam.Replace("Trung Tâm Viễn Thông ", "TTVT ") + " - ST Tổng";
                            result2.Add(item2);

                        }
                        if (item.bh_tong >= 0)
                        {
                            TLDUTGXLSC item2 = new TLDUTGXLSC();
                            item2.solieu = item.bh_tong;
                            item2.ten_trungtam = item.ten_trungtam.Replace("Trung Tâm Viễn Thông ", "TTVT ") + " - BH Tổng";
                            result2.Add(item2);

                        }
                        if (item.st_quagio >= 0)
                        {
                            TLDUTGXLSC item2 = new TLDUTGXLSC();
                            item2.solieu = item.st_quagio;
                            item2.ten_trungtam = item.ten_trungtam.Replace("Trung Tâm Viễn Thông ", "TTVT ") + " - ST Quá giờ";
                            result2.Add(item2);
                        }
                    }
                    int i = 1;

                    foreach (var unit in result2)
                    {

                        List<dynamic> points = new List<dynamic>();
                        points.Add(new List<dynamic> { unit.solieu
                            , m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });

                        response.Add(new { target = unit.ten_trungtam, datapoints = points });
                        i++;
                    }
                }
            }
            return response;
        }

        public dynamic executeHSSDCD(RqGrafana rq)
        {
            List<UsingPerformance> result = new List<UsingPerformance>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.hssd_capdong";
                result = SqlMapper.Query<UsingPerformance>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<UsingPerformance>();
                conn.Close();
            }
            return result;
        }
        public dynamic getHSSDCD(RqGrafana rq)
        {
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            var date = m_common.convertToString(rq);
            List<dynamic> response = new List<dynamic>();
            List<UsingPerformance> list_HSSDCD = executeHSSDCD(rq);
            if (rq.targets[0].type == "table")
            {
    
                List<dynamic> col = new List<dynamic>
                    {
                        new { text = "Tên đơn vị", type = "string"},
                        new { text = "DL cáp gốc", type = "number"},
                        new { text = "DL cáp gốc sử dụng", type = "number"},
                        new { text = "DL cáp gốc trống", type = "number"},
                        new { text = "HSSD cáp gốc", type = "number"},
                        new { text = "DL cáp phối", type = "number"},
                        new { text = "DL cáp phối sử dụng", type = "number"},
                        new { text = "DL cáp phối trống", type = "number"},
                        new { text = "HSSD cáp phối", type = "number"},
                    };
                List<dynamic> row = new List<dynamic>();
                foreach (UsingPerformance item in list_HSSDCD)
                {
                    row.Add(new List<dynamic> {
                        item.ttvt,
                        item.dungluong_capgoc,
                        item.dungluong_capgoc_sudung,
                        item.dungluong_capgoc_trong,
                        item.hssd_cap_goc,
                        item.dungluong_capphoi,
                        item.dungluong_capphoi_sudung,
                        item.dungluong_capphoi_trong,
                        item.hssd_cap_phoi
                    });
                }
                response = new List<dynamic> {
                        new {
                                columns = col,
                                rows = row,
                                type = "table"
                            }
                    };
            }
            else
            {
                foreach (UsingPerformance item in list_HSSDCD)
                {
                    List<dynamic> points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.hssd_cap_phoi, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "HSSD Cáp phối " + item.ttvt, datapoints = points });
                    points = new List<dynamic>(); 
                    points.Add(new List<dynamic> { item.hssd_cap_goc, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "HSSD Cáp gốc " + item.ttvt, datapoints = points });
                }
            }
            
            return response;
        }

        public dynamic executeHSSDCQ(RqGrafana rq)
        {
            List<UsingPerformance> result = new List<UsingPerformance>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.hssd_capquang";
                result = SqlMapper.Query<UsingPerformance>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<UsingPerformance>();
                conn.Close();
            }
            return result;
        }
        public dynamic getHSSDCQ(RqGrafana rq)
        {
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            var date = m_common.convertToString(rq);
            List<dynamic> response = new List<dynamic>();
            List<UsingPerformance> list_HSSDCQ = executeHSSDCQ(rq);
            if (rq.targets[0].type == "table")
            {
                
                List<dynamic> col = new List<dynamic>
                    {
                        new { text = "Tên đơn vị", type = "string"},
                        new { text = "DL cáp gốc", type = "number"},
                        new { text = "DL cáp gốc sử dụng", type = "number"},
                        new { text = "DL cáp gốc trống", type = "number"},
                        new { text = "HSSD cáp gốc", type = "number"},
                        new { text = "DL cáp phối", type = "number"},
                        new { text = "DL cáp phối sử dụng", type = "number"},
                        new { text = "DL cáp phối trống", type = "number"},
                        new { text = "HSSD cáp phối", type = "number"},
                        new { text = "DL PON", type = "number"},
                        new { text = "DL PON sử dụng", type = "number"},
                        new { text = "HSSD PON", type = "number"},
                    };
                List<dynamic> row = new List<dynamic>();
                foreach (UsingPerformance item in list_HSSDCQ)
                {
                    row.Add(new List<dynamic> {
                        item.ttvt,
                        item.dungluong_capgoc,
                        item.dungluong_capgoc_sudung,
                        item.dungluong_capgoc_trong,
                        item.hssd_cap_goc,
                        item.dungluong_capphoi,
                        item.dungluong_capphoi_sudung,
                        item.dungluong_capphoi_trong,
                        item.hssd_cap_phoi,
                        item.dungluong_pon,
                        item.dungluong_pon_sudung,
                        item.hssd_pon
                    });
                }
                response = new List<dynamic> {
                        new {
                                columns = col,
                                rows = row,
                                type = "table"
                            }
                    };
            }
            else
            {
                foreach (UsingPerformance item in list_HSSDCQ)
                {
                    List<dynamic> points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.hssd_cap_phoi, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "HSSD Cáp phối "+item.ttvt, datapoints = points });
                    points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.hssd_cap_goc, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "HSSD Cáp gốc "+item.ttvt, datapoints = points });
                    points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.hssd_pon, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "HSSD PON " + item.ttvt, datapoints = points });
                }
            }
            return response;
        }

        public dynamic executeMegaVNN(string month)
        {
            List<MegaVNNModel> result = new List<MegaVNNModel>();
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
                var query = "dashboard.solieu_megavnn_1thang";
                result = SqlMapper.Query<MegaVNNModel>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).ToList();
                conn.Close();
            }
            return result;
        }
        public dynamic getMegaVNN(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            var date = m_common.convertToString(rq);
            var startdate = new DateTime(Int32.Parse(date.Item1.Substring(6, 4)), Int32.Parse(date.Item1.Substring(3, 2)), Int32.Parse(date.Item1.Substring(0, 2)));
            var enddate = new DateTime(Int32.Parse(date.Item2.Substring(6, 4)), Int32.Parse(date.Item2.Substring(3, 2)), Int32.Parse(date.Item2.Substring(0, 2)));
            var listmonth = GetMonthsBetween(startdate, enddate);
            List<MegaVNNModel> list_MegaVNN = new List<MegaVNNModel>();
            foreach (dynamic month in listmonth)
            {
                list_MegaVNN.AddRange(executeMegaVNN(month.ToString("yyyyMM")));
            }

            if (rq.targets[0].type == "table")
            {
                if (rq.targets[0].target == "1")
                //{
                //    List<dynamic> col = new List<dynamic>
                //    {
                //        new { text = "Tên đơn vị", type = "string"},
                //        new { text = "Tháng " + dngay.Month, type = "number"}
                //    };
                //    List<dynamic> row = new List<dynamic>();
                //    foreach (MegaVNNModel item in list_MegaVNN)
                //    {
                //        row.Add(new List<dynamic> {
                //        item.ten_dv.Replace("Trung Tâm Viễn Thông", "TTVT"),
                //        item.tong_thuebao
                //    });
                //    }
                //    response = new List<dynamic> {
                //        new {
                //                columns = col,
                //                rows = row,
                //                type = "table"
                //            }
                //    };
                //}
                {
                    List<dynamic> lcolumns = new List<dynamic>();
                    lcolumns.Add(new { text = "Tên TTVT", type = "string" });
                    foreach (var month in listmonth)
                    {
                        lcolumns.Add(new { text = "Tháng " + month.ToString("MM/yyyy"), type = "string" });
                    }
                    List<dynamic> lrows = new List<dynamic>();
                    var filter = from item in list_MegaVNN
                                group item by item.ten_dv;


                    foreach (var group in filter)
                    {
                        List<dynamic> items = new List<dynamic>();
                        items.Add(group.Key);
                        foreach (var item in group)
                        {
                            items.Add(item.tong_thuebao);
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

            }
            else
            {
                    long unix_time = m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year);
                    int i = 1;
                foreach (MegaVNNModel item in list_MegaVNN)
                {
                    List<dynamic> points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.tong_thuebao, unix_time });
                    response.Add(new { target = item.ten_dv.Replace("Trung Tâm Viễn Thông ", "") + " - Tháng " + item.thang, datapoints = points });
                }

            }
            return response;
        }

        public dynamic executeLuykeLapgoFiber(RqGrafana rq)
        {
            var date = m_common.convertToString(rq);
            List<LuykeLapgoFiber> result = new List<LuykeLapgoFiber>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("i_denngay", OracleDbType.Varchar2, ParameterDirection.Input, date.Item1);
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.BC_3_8_LUYKE_LAPGO_FIBERVNN";
                result = SqlMapper.Query<LuykeLapgoFiber>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).ToList();
                conn.Close();
            }
            return result;
        }
        public dynamic executeLuykeLapgoFiberDate(RqGrafana rq)
        {
            var date = m_common.convertToString(rq);
            List<LuykeLapgoFiber_date> result = new List<LuykeLapgoFiber_date>();
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
                var query = "dashboard.tk_luyke_lapgo_fibervnn_date";
                result = SqlMapper.Query<LuykeLapgoFiber_date>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<LuykeLapgoFiber_date>();
                conn.Close();
            }
            return result;
        }
        public dynamic getLuykeLapgoFiber(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            List<LuykeLapgoFiber> list_LuykeLapgoFiber = executeLuykeLapgoFiber(rq);
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            var date = m_common.convertToString(rq);
            if (rq.targets[0].type == "table")
            {
                
                List<dynamic> col = new List<dynamic>
                    {
                        new { text = "Tên đơn vị", type = "string"},
                        new { text = "Thực tăng năm", type = "number"},
                        new { text = "Lắp mới lũy kế lắp đặt", type = "number"},
                        new { text = "Phục hồi lũy kế lắp đặt", type = "number"},
                        new { text = "Dịch chuyển lũy kế lắp đặt", type = "number"},
                        new { text = "Chuyển đổi lũy kế lắp đặt", type = "number"},
                        new { text = "Gỡ - Yêu cầu lũy kế hủy bỏ", type = "number"},
                        new { text = "Gỡ - Làm sạch lũy kế hủy bỏ", type = "number"},
                        new { text = "Dịch chuyển lũy kế hủy bỏ", type = "number"},
                        new { text = "Chuyển đổi lũy kế hủy bỏ", type = "number"},
                        new { text = "Tổng thuê bao", type = "number"},
                        new { text = "Tỉ lệ", type = "number"}
                    };
                List<dynamic> row = new List<dynamic>();
                foreach (LuykeLapgoFiber item in list_LuykeLapgoFiber)
                {
                    row.Add(new List<dynamic> {
                        item.donvi, item.luyke_nam, item.lapmoi_lk, item.phuchoi_lk, item.dd_lm_lk, item.chuyendoi_lk,
                        item.go_yeucau_lk, item.go_lamsach_lk, item.dd_go_lk, item.cd_giam_lk, item.tong_thuebao, item.tile
                    });
                }
                response = new List<dynamic> {
                        new {
                                columns = col,
                                rows = row,
                                type = "table"
                            }
                    };
            }
            else
            {
                foreach (LuykeLapgoFiber item in list_LuykeLapgoFiber)
                {
                    List<dynamic> points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.tile, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = item.donvi , datapoints = points });
                }
            }
            return response;
        }

        public dynamic executeMLLBTS(RqGrafana rq)
        {
            var date = m_common.convertToString(rq);
            List<MLLBTS> result = new List<MLLBTS>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("loai_loi", OracleDbType.Varchar2, ParameterDirection.Input, (string)rq.scopedVars.loailoi.value);
            dyParam.Add("i_ttvt", OracleDbType.Varchar2, ParameterDirection.Input, (string)rq.scopedVars.ttvt.value);
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
                var query = "dashboard.mll_bts";
                result = SqlMapper.Query<MLLBTS>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<MLLBTS>();
                conn.Close();
            }
            return result;
        }
        public dynamic getMLLBTS(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            List<MLLBTS> list_MLLBTS = executeMLLBTS(rq);
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            var date = m_common.convertToString(rq);
            if (rq.targets[0].type == "table")
            {

                List<dynamic> col = new List<dynamic>
                    {
                        new { text = "Đơn vị", type = "string"},
                        new { text = "Số lượng trạm MLL", type = "number"},
                        new { text = "TG MLL Tổng", type = "number"},
                        new { text = "TG MLL TB", type = "double"},
                        new { text = "Tháng", type = "string"}
                    };
                List<dynamic> row = new List<dynamic>();
                foreach (MLLBTS item in list_MLLBTS)
                {
                    row.Add(new List<dynamic> {
                        item.ttvt, item.sl_mll, item.tg_mll_tong, item.tg_mll_tb, item.thangketthuc
                    });
                }
                response = new List<dynamic> {
                        new {
                                columns = col,
                                rows = row,
                                type = "table"
                            }
                    };
            }
            else
            {
                var newList = list_MLLBTS.OrderBy(x => x.ttvt).ThenBy(x => x.thangketthuc).ToList();
                foreach (MLLBTS item in newList)
                {
                    List<dynamic> points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.tg_mll_tb, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = item.ttvt +" \n"+ item.thangketthuc, datapoints = points });
                }
            }
            return response;
        }
        
        public dynamic executeMLLNN(RqGrafana rq)
        {
            var date = m_common.convertToString(rq);
            List<MLLNN> result = new List<MLLNN>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("loai_loi", OracleDbType.Varchar2, ParameterDirection.Input, (string)rq.scopedVars.loailoi.value);
            dyParam.Add("i_ttvt", OracleDbType.Varchar2, ParameterDirection.Input, (string)rq.scopedVars.ttvt.value);
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
                var query = "dashboard.mll_nguyennhan";
                result = SqlMapper.Query<MLLNN>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<MLLNN>();
                conn.Close();
            }
            return result;
        }
        public dynamic getMLLNN(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            List<MLLNN> list_MLLNN = executeMLLNN(rq);
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            var date = m_common.convertToString(rq);
            if (rq.targets[0].type == "table")
            {

                List<dynamic> col = new List<dynamic>
                    {
                        new { text = "Tên lỗi", type = "string"},
                        new { text = "Số lượng trạm MLL", type = "number"},
                        new { text = "Tổng số", type = "number"},
                        new { text = "Tỷ lệ", type = "double"},
                        new { text = "Tháng", type = "string"}
                    };
                List<dynamic> row = new List<dynamic>();
                foreach (MLLNN item in list_MLLNN)
                {
                    row.Add(new List<dynamic> {
                        item.ten_loi, item.sl_mll, item.tongcong, item.ty_le, item.thangketthuc
                    });
                }
                response = new List<dynamic> {
                        new {
                                columns = col,
                                rows = row,
                                type = "table"
                            }
                    };
            }
            else
            {
                dynamic newList = list_MLLNN.GroupBy(g => new
                {
                    g.ten_loi
                }).Select(lg => new
                {
                    lg.Key.ten_loi,
                    sl_mll = lg.Where(o => o.ten_loi == lg.Key.ten_loi).Sum(o => o.sl_mll)
                }).OrderByDescending(o => o.sl_mll);
                foreach (var item in newList)
                {
                    List<dynamic> points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.sl_mll, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = item.ten_loi, datapoints = points });
                }
            }
            return response;
        }

        public dynamic executeTKMLLBTS(RqGrafana rq, string loai_mang)
        {
            var date = m_common.convertToString(rq);
            List<tk_mll_bts> result = new List<tk_mll_bts>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("loai_loi", OracleDbType.Varchar2, ParameterDirection.Input, loai_mang);
            dyParam.Add("i_ttvt", OracleDbType.Int16, ParameterDirection.Input, (int)rq.scopedVars.unit.value);
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
                var query = "dashboard.tk_mll_bts";
                result = SqlMapper.Query<tk_mll_bts>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<tk_mll_bts>();
                conn.Close();
            }
            return result;
        }

        public dynamic executeTKMLLBTS_NN(RqGrafana rq, string loai_mang)
        {
            var date = m_common.convertToString(rq);
            List<tk_mll_bts_nn> result = new List<tk_mll_bts_nn>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("loai_loi", OracleDbType.Varchar2, ParameterDirection.Input, loai_mang);
            dyParam.Add("i_ttvt", OracleDbType.Int16, ParameterDirection.Input, (int)rq.scopedVars.unit.value);
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
                var query = "dashboard.tk_mll_bts_nn";
                result = SqlMapper.Query<tk_mll_bts_nn>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<tk_mll_bts_nn>();
                conn.Close();
            }
            return result;
        }

        public dynamic executePAKHDD(RqGrafana rq)
        {
            var date = m_common.convertToString(rq);
            List<ReflectionMobileQuality> result = new List<ReflectionMobileQuality>();
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
                var query = "dashboard.pakh_dd";
                result = SqlMapper.Query<ReflectionMobileQuality>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<ReflectionMobileQuality>();
                conn.Close();
            }
            return result;
        }
        public dynamic getPAKHDD(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            List<ReflectionMobileQuality> list_PAKHDD = executePAKHDD(rq);
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            var date = m_common.convertToString(rq);
            if (rq.targets[0].type == "table")
            {

                List<dynamic> col = new List<dynamic>
                    {
                        new { text = "Tháng", type = "string" },
                        new { text = "Đơn vị", type = "string" },
                        new { text = "Số lượng xử lý", type = "number" },
                        new { text = "Tổng số", type = "number" },
                        new { text = "Tỷ lệ", type = "double" },
                    };
                List<dynamic> row = new List<dynamic>();
                foreach (ReflectionMobileQuality item in list_PAKHDD)
                {
                    row.Add(new List<dynamic> {
                        item.ngay_tn, item.dv_xl, item.sl_xl, item.tongcong, item.ty_le
                    });
                }
                response = new List<dynamic> {
                        new {
                                columns = col,
                                rows = row,
                                type = "table"
                            }
                    };
            }
            else
            {
                foreach (ReflectionMobileQuality item in list_PAKHDD)
                {
                    List<dynamic> points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.sl_xl, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = item.dv_xl, datapoints = points });
                }
            }
            return response;
        }
    }
}
    
