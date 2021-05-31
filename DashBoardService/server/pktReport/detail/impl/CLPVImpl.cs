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
    public class CLPVImpl : ICLPV
    {
        private IConfiguration m_configuration;
        private ICommon m_common;
        private ITK_KhongHaiLong_CLPV m_clpv_khl;
        public CLPVImpl(IConfiguration configuration, ICommon common, ITK_KhongHaiLong_CLPV clpv_khl)
        {
            m_configuration = configuration;
            m_common = common;
            m_clpv_khl = clpv_khl;
        }

        private dynamic getCLPV_khl_oracle(string startime, string endtime)
        {
            var checkDate = m_clpv_khl.toDataConvert(startime, endtime);
            List<TK_KhongHaiLong_CLPV> data = new List<TK_KhongHaiLong_CLPV>();
            if (checkDate == true)
            {
                string connStr = m_configuration.GetConnectionString("DefaultConnection");
                var dt = new DataTable();
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(@"select tuan, ngay, donvi_cha_id, sl from [dbo].TK_KhongHaiLong_CLPV where ngay between CONVERT(DATETIME,'" + startime + "',103) and CONVERT(DATETIME,'" + endtime + "',103)", conn))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            TK_KhongHaiLong_CLPV row = new TK_KhongHaiLong_CLPV();
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

        private dynamic getCLPV_KHL_date(RqGrafana rq, List<TK_KhongHaiLong_CLPV> list)
        {
            List<dynamic> data = new List<dynamic>();

            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_clpv_khl = list
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
                    foreach (var item in list_clpv_khl)
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
                var list_clpv_khl = list
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
                foreach (var item in list_clpv_khl)
                {
                    points.Add(new List<dynamic> { item.sl, item.unix_date });
                }

                data.Add(new { target = ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getCLPV_KHL(RqGrafana rq, List<TK_KhongHaiLong_CLPV> list)
        {
            List<dynamic> data = new List<dynamic>();
            var date = m_common.convertToString(rq);
            DateTime time = DateTime.ParseExact(date.Item1, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_clpv_khl = list
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
                    foreach (var item in list_clpv_khl)
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
                var list_clpv_khl = list
                        .OrderBy(ele => (ele.donvi_cha_id, ele.ngay))
                        .GroupBy(l => new { l.ngay.Month, l.ngay.Year, l.donvi_cha_id })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_cha_id,
                                sl = lg.Sum(l => l.sl),
                                unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_cha_id == donvi_id);
                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_clpv_khl)
                {
                    points.Add(new List<dynamic> { item.sl, item.unix_date });
                }

                data.Add(new { target = ten_dv, datapoints = points });
            }
            return data;
        }

        public dynamic getCLPV(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var date = m_common.convertToString(rq);
            List<TK_KhongHaiLong_CLPV> list = getCLPV_khl_oracle(date.Item1, date.Item2);
            if (rq.targets[0].data.graph == "line")
            {
                data = getCLPV_KHL_date(rq, list);
            }
            else
            {
                data = getCLPV_KHL(rq, list);
            }
            return data;
        }
    }
}
