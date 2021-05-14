using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.connnection.oracle;
using ClassModel.model.customerSatisfaction;
using ClassModel.model.khonghailong;
using ClassModel.model.RqGrafana;
using Dapper;
using DashBoardService.server.common;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace DashBoardService.server.customerSatisfaction.ServeQuality
{
    public class ServeQualityImpl : IServeQuality
    {

        private ICommon m_common;
        private IConfiguration m_configuration;
        public ServeQualityImpl(ICommon m_common, IConfiguration m_configuration)
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

        public dynamic GetData_Dissatisfied_ServeQuality(RqGrafana rq)
        {
            var date = m_common.convertToString(rq);
            //string startdate = "01/" + date.Item1.Substring(3, 2) + "/" + date.Item1.Substring(6, 4);
            //string enddate = DateTime.DaysInMonth(Int32.Parse(date.Item1.Substring(6, 4)), Int32.Parse(date.Item1.Substring(3, 2))).ToString()+"/"+ date.Item1.Substring(3, 2) + "/" + date.Item1.Substring(6, 4);

            List<Dissatisfied> result = new List<Dissatisfied>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("vtungay", OracleDbType.Varchar2, ParameterDirection.Input, date.Item1);
            dyParam.Add("vdenngay", OracleDbType.Varchar2, ParameterDirection.Input, date.Item2);

            dyParam.Add("ref_cur", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.TK_KhongHaiLong_CLPV";
                result = SqlMapper.Query<Dissatisfied>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<Dissatisfied>();
            }
            return result;
        }

        public dynamic GetData_Dissatisfied_ServeQuality_Grafana(RqGrafana rq)
        {
            List<Dissatisfied> result = GetData_Dissatisfied_ServeQuality(rq);
            List<dynamic> response = new List<dynamic>();
            if (rq.targets[0].type == "table")
            {
                List<dynamic> col = new List<dynamic>();
                List<dynamic> row = new List<dynamic>();

                col = new List<dynamic>
                {
                    new { text = "Tuần", type = "string"},
                    new { text = "TTVT Tân Bình", type = "number"},
                    new { text = "TTVT Chợ Lớn", type = "number"},
                    new { text = "TTVT Bình Chánh", type = "number" },
                    new { text = "TTVT Củ Chi", type = "number"},
                    new { text = "TTVT Hóc Môn", type = "number" },
                    new { text = "TTVT Sài Gòn", type = "number" },
                    new { text = "TTVT Gia Định", type = "number" },
                    new { text = "TTVT Nam Sài Gòn", type = "number" },
                    new { text = "TTVT Thủ Đức", type = "number" }

                };
                foreach (Dissatisfied item in result)
                {
                    row.Add(new List<dynamic> { item.tuan, item.tan_binh, item.cho_lon, item.binh_chanh, item.cu_chi, item.hoc_mon, item.sai_gon, item.gia_dinh, item.nam_sg, item.thu_duc });
                }
                response = new List<dynamic>
                {
                    new
                    {
                        columns = col,
                        rows = row,
                        type = "table"
                    }
                };
            }
            else
            {
                DateTime dngay = Convert.ToDateTime(rq.range.to);
                long unix_time = m_common.convertDayToUnix(01, dngay.Month, dngay.Year);
                foreach (Dissatisfied item in result)
                {
                    List<dynamic> points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.tan_binh, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "Tân Bình " + item.tuan, datapoints = points });
                    points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.gia_dinh, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "Gia Định " + item.tuan, datapoints = points });
                    points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.hoc_mon, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "Hóc Môn " + item.tuan, datapoints = points });
                    points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.nam_sg, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "Nam Sài Gòn " + item.tuan, datapoints = points });
                    points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.sai_gon, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "Sài Gòn " + item.tuan, datapoints = points });
                    points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.thu_duc, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "Thủ Đức " + item.tuan, datapoints = points });
                    points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.binh_chanh, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "Bình Chánh " + item.tuan, datapoints = points });
                    points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.cu_chi, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "Củ Chi " + item.tuan, datapoints = points });
                    points = new List<dynamic>();
                    points.Add(new List<dynamic> { item.cho_lon, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                    response.Add(new { target = "Chợ Lớn " + item.tuan, datapoints = points });

                }
            }
            return response;
        }

        
    }
}
