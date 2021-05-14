using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.RqGrafana;
using ClassModel.model.ThoaitraPCT;
using ClassModel.model.unit;
using DashBoardService.server.common;
using DashBoardService.server.ThoaitraPCT;
using Microsoft.Extensions.Configuration;

namespace DashBoardService.server.pktReport.detail.impl
{
    public class ThoaiTraImpl : IThoaiTra
    {
        private ICommon m_common;
        private IConfiguration m_configuration;
        private IThoaitraPCT m_thoaiTraPTC;
        public ThoaiTraImpl(ICommon m_common, IConfiguration m_configuration, IThoaitraPCT thoaiTraPTC)
        {
            this.m_common = m_common;
            this.m_configuration = m_configuration;
            m_thoaiTraPTC = thoaiTraPTC;
        }

        public dynamic getThoaiTraDate(RqGrafana rq)
        {
            List<ThoaitraPTC> data = new List<ThoaitraPTC>();
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            var date = m_common.convertToString(rq);
            var startdate = new DateTime(Int32.Parse(date.Item1.Substring(6, 4)), Int32.Parse(date.Item1.Substring(3, 2)), Int32.Parse(date.Item1.Substring(0, 2)));
            var enddate = new DateTime(Int32.Parse(date.Item2.Substring(6, 4)), Int32.Parse(date.Item2.Substring(3, 2)), Int32.Parse(date.Item2.Substring(0, 2)));
            var listmonth = m_common.GetMonthsBetween(startdate, enddate);            
            foreach (var month in listmonth)
            {
                m_thoaiTraPTC.GetThoaitraPCT(month.ToString("yyyyMM"));
            }
            data = m_thoaiTraPTC.GetThoaitraPCTDate(rq);
            return data;
        }

        private dynamic getThoaiTraTTVT_date(RqGrafana rq, List<ThoaitraPTC> list)
        {
            List<dynamic> data = new List<dynamic>();
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_thoaitra = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.donvi })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.donvi,
                                pct_thoaitra_ttvt = lg.Sum(l => l.pct_thoaitra_ttvt),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_thoaitra)
                    {
                        points.Add(new List<dynamic> { item.pct_thoaitra_ttvt, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                var list_thoaitra = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.donvi })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.donvi,
                                pct_thoaitra_ttvt = lg.Sum(l => l.pct_thoaitra_ttvt),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == donvi_id);
                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_thoaitra)
                {
                    points.Add(new List<dynamic> { item.pct_thoaitra_ttvt, item.unix_date });
                }

                data.Add(new { target = list.FirstOrDefault(item => item.donvi_id == donvi_id).donvi, datapoints = points });
            }
            return data;
        }

        private dynamic getThoaiTraTTVT(RqGrafana rq, List<ThoaitraPTC> list)
        {
            List<dynamic> data = new List<dynamic>();
            if ((int)rq.scopedVars.unit.value == 0) {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_thoaitra = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.donvi })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.donvi,
                                pct_thoaitra_ttvt = lg.Sum(l => l.pct_thoaitra_ttvt),
                                unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_thoaitra)
                    {
                        points.Add(new List<dynamic> { item.pct_thoaitra_ttvt, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                var list_thoaitra = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.donvi })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.donvi,
                                pct_thoaitra_ttvt = lg.Sum(l => l.pct_thoaitra_ttvt),
                                unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == donvi_id);
                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_thoaitra)
                {
                    points.Add(new List<dynamic> { item.pct_thoaitra_ttvt, item.unix_date });
                }

                data.Add(new { target = list.FirstOrDefault(item => item.donvi_id == donvi_id).donvi, datapoints = points });
            }
            return data;
        }

        private dynamic getThoaiTraTTKD_date(RqGrafana rq, List<ThoaitraPTC> list)
        {
            List<dynamic> data = new List<dynamic>();
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_thoaitra = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.donvi })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.donvi,
                                pct_thoaitra_ttkd = lg.Sum(l => l.pct_thoaitra_ttkd),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_thoaitra)
                    {
                        points.Add(new List<dynamic> { item.pct_thoaitra_ttkd, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                var list_thoaitra = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.donvi })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.donvi,
                                pct_thoaitra_ttkd = lg.Sum(l => l.pct_thoaitra_ttkd),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == donvi_id);
                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_thoaitra)
                {
                    points.Add(new List<dynamic> { item.pct_thoaitra_ttkd, item.unix_date });
                }

                data.Add(new { target = list.FirstOrDefault(item => item.donvi_id == donvi_id).donvi, datapoints = points });
            }
            return data;
        }

        private dynamic getThoaiTraTTKD(RqGrafana rq, List<ThoaitraPTC> list)
        {
            List<dynamic> data = new List<dynamic>();
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_thoaitra = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.donvi })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.donvi,
                                pct_thoaitra_ttkd = lg.Sum(l => l.pct_thoaitra_ttkd),
                                unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_thoaitra)
                    {
                        points.Add(new List<dynamic> { item.pct_thoaitra_ttkd, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                var list_thoaitra = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.donvi })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.donvi,
                                pct_thoaitra_ttkd = lg.Sum(l => l.pct_thoaitra_ttkd),
                                unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == donvi_id);
                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_thoaitra)
                {
                    points.Add(new List<dynamic> { item.pct_thoaitra_ttkd, item.unix_date });
                }

                data.Add(new { target = list.FirstOrDefault(item => item.donvi_id == donvi_id).donvi, datapoints = points });
            }
            return data;
        }

        public dynamic getThoaiTraPTC(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<ThoaitraPTC> list = getThoaiTraDate(rq);
            if ((int)rq.scopedVars.type.value == 1006)
            {
                if (rq.targets[0].data.graph == "line")
                {
                    data = getThoaiTraTTVT_date(rq, list);
                }
                else
                {
                    data = getThoaiTraTTVT(rq, list);
                }
            } else if ((int)rq.scopedVars.type.value == 1007)
            {
                if (rq.targets[0].data.graph == "line")
                {
                    data = getThoaiTraTTKD_date(rq, list);
                }
                else
                {
                    data = getThoaiTraTTKD(rq, list);
                }                
            }
            return data;
        }
    }
}
