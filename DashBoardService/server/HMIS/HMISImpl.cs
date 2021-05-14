using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.connnection.oracle;
using ClassModel.model.HMIS;
using ClassModel.model.RqGrafana;
using Dapper;
using DashBoardService.server.common;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace DashBoardService.server.HMIS
{
    public class HMISImpl : IHMIS
    {
        private ICommon m_common;
        private IConfiguration m_configuration;
        public HMISImpl(ICommon m_common, IConfiguration m_configuration)
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
        public dynamic executeHMIS(RqGrafana rq)
        {
            List<HMISModel> result = new List<HMISModel>();
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.ds_hmis";
                result = SqlMapper.Query<HMISModel>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<HMISModel>();
                conn.Close();
            }
            return result;
        }
        public dynamic execute_chart_HMIS(RqGrafana rq)
        {
            List<HMISModel> result = new List<HMISModel>();
            var date = m_common.convertToString(rq);
            var dyParam = new OracleDynamicParameters();
            dyParam.Add("o_data", OracleDbType.RefCursor, ParameterDirection.Output);
            dyParam.Add("v_denngay", OracleDbType.Varchar2, ParameterDirection.Input, date.Item2);
            var conn = GetConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Open)
            {
                var query = "dashboard.chart_hmis";
                result = SqlMapper.Query<HMISModel>(conn, query, param: dyParam, commandType: CommandType.StoredProcedure).AsList<HMISModel>();
                conn.Close();
            }
            return result;
        }

        public dynamic getHMIS(RqGrafana rq)
        {
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            var date = m_common.convertToString(rq);
            List<dynamic> response = new List<dynamic>();
            List<HMISModel> list_HMIS = executeHMIS(rq);
            List<HMISModel> chart_HMIS = execute_chart_HMIS(rq);
            if (rq.targets[0].type == "table")
            {

                List<dynamic> col = new List<dynamic>
                    {
                        new { text = "ID", type = "number"},
                        new { text = "Đơn vị bán hàng", type = "string"},
                        new { text = "Mã tỉnh", type = "number"},
                        new { text = "CSYT Quận/huyện", type = "string"},
                        new { text = "Mã CSYT (BHYT)", type = "string"},
                        new { text = "Mã CSYT (ngoài BHYT)", type = "string"},
                        new { text = "Tuyến", type = "string"},
                        new { text = "Tên KH", type = "string"},
                        new { text = "Địa chỉ", type = "string"},
                        new { text = "Tổ khai báo hệ thống", type = "string"},
                        new { text = "Ngày khai báo hệ thống", type = "string"},
                        new { text = "Ngày triển khai HIS", type = "string"},
                        new { text = "Ngày triển khai HMIS", type = "string"},
                        new { text = "Biến động", type = "string"},
                        new { text = "Tổ triển khai", type = "string"},
                        new { text = "Ghi chú", type = "string"},
                        new { text = "Ngày CN", type = "string"},
                        new { text = "Người CN", type = "string"},
                    };
                List<dynamic> row = new List<dynamic>();
                foreach (HMISModel item in list_HMIS)
                {
                    row.Add(new List<dynamic> {
                        item.id, item.donvi_banhang, item.matinh, item.csyt_quan_huyen, item.ma_csyt_bhyt, item.ma_csyt_ngoaibhyt, item.tuyen,
                        item.ten_kh, item.diachi, item.to_khaibao_hethong, item.ngay_khaibao_hethong, item.ngay_trienkhai_his, item.ngay_trienkhai_hmis,
                        item.biendong, item.to_trienkhai, item.ghichu, item.ngay_cn, item.user_cn
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
                if (rq.targets[0].target == "2")
                    foreach (HMISModel item in chart_HMIS)
                    {
                        List<dynamic> points = new List<dynamic>();
                        points.Add(new List<dynamic> { item.da_trienkhai_hmis, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                        response.Add(new { target = item.csyt_quan_huyen, datapoints = points });

                    }

                if (rq.targets[0].target == "3")
                    foreach (HMISModel item in chart_HMIS)
                    {
                        List<dynamic> points = new List<dynamic>();
                        points.Add(new List<dynamic> { item.chua_trienkhai_hmis, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                        response.Add(new { target = item.csyt_quan_huyen, datapoints = points });

                    }

                if (rq.targets[0].target == "4")
                    foreach (HMISModel item in chart_HMIS)
                    {
                        List<dynamic> points = new List<dynamic>();
                        points.Add(new List<dynamic> { item.chua_trienkhai_hmis, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                        response.Add(new { target = item.csyt_quan_huyen + " - Chưa triển khai", datapoints = points });
                        points = new List<dynamic>();
                        points.Add(new List<dynamic> { item.da_trienkhai_hmis, m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                        response.Add(new { target = item.csyt_quan_huyen + " - Đã triển khai", datapoints = points });
                        points = new List<dynamic>();
                        points.Add(new List<dynamic> { " ", m_common.convertDayToUnix(dngay.Day, dngay.Month, dngay.Year) });
                        response.Add(new { target = " ", datapoints = points });


                    }
            }

            return response;
        }
    }
}

