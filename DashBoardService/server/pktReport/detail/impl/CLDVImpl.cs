using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.convertdata.tk_khl;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.common;
using DashBoardService.server.convertdata.tk_khl;
using Microsoft.Extensions.Configuration;

namespace DashBoardService.server.pktReport.detail.impl
{
    public class CLDVImpl : ICLDV
    {
        private IConfiguration m_configuration;
        private ICommon m_common;
        private ITK_KhongHaiLong_CLDV m_cldv_khl;
        public CLDVImpl(IConfiguration configuration, ICommon common, ITK_KhongHaiLong_CLDV cldv_khl)
        {
            m_configuration = configuration;
            m_common = common;
            m_cldv_khl = cldv_khl;
        }

        private dynamic getCLDV_khl_oracle(string startime, string endtime)
        {
            var checkDate = m_cldv_khl.toDataConvert(startime, endtime);
            List<TK_KhongHaiLong_CLDV> data = new List<TK_KhongHaiLong_CLDV>();
            if (checkDate == true)
            {
                string connStr = m_configuration.GetConnectionString("DefaultConnection");
                var dt = new DataTable();
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(@"select tuan, ngay, donvi_cha_id, sl from [dbo].TK_KhongHaiLong_CLDV where ngay between CONVERT(DATETIME,'" + startime + "',103) and CONVERT(DATETIME,'" + endtime + "',103)", conn))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            TK_KhongHaiLong_CLDV row = new TK_KhongHaiLong_CLDV();
                            row.donvi_cha_id = Convert.ToInt32(rdr["donvi_cha_id"]);
                            row.ngay = (DateTime)rdr["ngay"];
                            row.tuan = rdr["tuan"].ToString();                            
                            row.sl = Convert.ToInt32(rdr["sl"]);
                            data.Add(row);
                        }
                    }
                    conn.Close();
                }
                return data;
            }
            else
            {
                return data;
            }
        }

        private dynamic getCLDV_KHL_date(RqGrafana rq, List<TK_KhongHaiLong_CLDV> list)
        {
            List<dynamic> data = new List<dynamic>();

            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_cldv_khl = list
                        .OrderBy(ele => (ele.donvi_cha_id, ele.ngay))
                        .GroupBy(l => new { l.ngay.Day, l.ngay.Month, l.ngay.Year, l.donvi_cha_id })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_cha_id,
                                sl = lg.Sum(l => l.sl),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_cha_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_cldv_khl)
                    {
                        points.Add(new List<dynamic> { item.sl, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                string ten_dv = (string)rq.scopedVars.unit.text;
                var list_cldv_khl = list
                        .OrderBy(ele => (ele.donvi_cha_id, ele.ngay))
                        .GroupBy(l => new { l.ngay.Day, l.ngay.Month, l.ngay.Year, l.donvi_cha_id })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_cha_id,
                                sl = lg.Sum(l => l.sl),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_cha_id == donvi_id);
                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_cldv_khl)
                {
                    points.Add(new List<dynamic> { item.sl, item.unix_date });
                }

                data.Add(new { target = ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getCLDV_KHL(RqGrafana rq, List<TK_KhongHaiLong_CLDV> list)
        {
            List<dynamic> data = new List<dynamic>();
            var date = m_common.convertToString(rq);
            DateTime time = DateTime.ParseExact(date.Item1, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if ((int)rq.scopedVars.unit.value == 0)
            {                
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_cldv_khl = list
                        .OrderBy(ele => (ele.donvi_cha_id, ele.ngay))
                        .GroupBy(l => new { l.ngay.Month, l.ngay.Year, l.donvi_cha_id })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_cha_id,
                                sl = lg.Sum(l => l.sl),
                                unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_cha_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_cldv_khl)
                    {
                        points.Add(new List<dynamic> { item.sl, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                string ten_dv = (string)rq.scopedVars.unit.text;
                var list_cldv_khl = list
                        .OrderBy(ele => (ele.donvi_cha_id, ele.ngay))
                        .GroupBy(l => new { l.ngay.Month, l.ngay.Year, l.donvi_cha_id})
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_cha_id,
                                sl = lg.Sum(l => l.sl),
                                unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_cha_id == donvi_id);
                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_cldv_khl)
                {
                    points.Add(new List<dynamic> { item.sl, item.unix_date });
                }

                data.Add(new { target = ten_dv, datapoints = points });
            }
            return data;
        }


        public dynamic getCLDV(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var date = m_common.convertToString(rq);
            List<TK_KhongHaiLong_CLDV> list = getCLDV_khl_oracle(date.Item1, date.Item2);
            if (rq.targets[0].data.graph == "line")
            {
                data = getCLDV_KHL_date(rq, list);
            }
            else
            {
                data = getCLDV_KHL(rq, list);
            }            
            return data;
        }
    }
}
