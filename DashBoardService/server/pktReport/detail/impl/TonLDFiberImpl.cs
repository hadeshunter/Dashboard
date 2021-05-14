using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.installationInventoryFiber;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.common;
using DashBoardService.server.installationInventoryFiber;
using Microsoft.Extensions.Configuration;

namespace DashBoardService.server.pktReport.detail.impl
{
    public class TonLDFiberImpl : ITonLDFiber
    {
        private ICommon m_common;
        private IConfiguration m_configuration;
        private IInstallationInventoryFiber m_tonLDFiber;
        public TonLDFiberImpl(ICommon m_common, IConfiguration m_configuration, IInstallationInventoryFiber tonLDFiber)
        {
            this.m_common = m_common;
            this.m_configuration = m_configuration;
            m_tonLDFiber = tonLDFiber;
        }

        private dynamic getTonLDFiber_Oracle(RqGrafana rq)
        {
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            var date = m_common.convertToString(rq);
            var startdate = new DateTime(Int32.Parse(date.Item1.Substring(6, 4)), Int32.Parse(date.Item1.Substring(3, 2)), Int32.Parse(date.Item1.Substring(0, 2)));
            var enddate = new DateTime(Int32.Parse(date.Item2.Substring(6, 4)), Int32.Parse(date.Item2.Substring(3, 2)), Int32.Parse(date.Item2.Substring(0, 2)));
            var listmonth = m_common.GetMonthsBetween(startdate, enddate);
            List<installationInventoryFiberModel> result = new List<installationInventoryFiberModel>();
            foreach (dynamic month in listmonth)
            {
                result.AddRange(m_tonLDFiber.GetInstallationInventoryFiber(month.ToString("yyyyMM")));
            }
            return result;
        }

        private dynamic getTonLDFiber_SLTon_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<installationInventoryFiberModel> list = getTonLDFiber_Oracle(rq);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_sl_ton = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv, l.tyle_ton })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                ton_fiber = lg.Sum(l => l.ton_fiber),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_sl_ton)
                    {
                        points.Add(new List<dynamic> { item.ton_fiber, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                var list_sl_ton = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv, l.tyle_ton })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                ton_fiber = lg.Sum(l => l.ton_fiber),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == donvi_id);
                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_sl_ton)
                {
                    points.Add(new List<dynamic> { item.ton_fiber, item.unix_date });
                }

                data.Add(new { target = list.FirstOrDefault(item => item.donvi_id == donvi_id).ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getTonLDFiber_SLTon(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<installationInventoryFiberModel> list = getTonLDFiber_Oracle(rq);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_sl_ton = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv, l.tyle_ton })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                ton_fiber = lg.Sum(l => l.ton_fiber),
                                unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_sl_ton)
                    {
                        points.Add(new List<dynamic> { item.ton_fiber, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                var list_sl_ton = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv, l.tyle_ton })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                ton_fiber = lg.Sum(l => l.ton_fiber),
                                unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == donvi_id);
                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_sl_ton)
                {
                    points.Add(new List<dynamic> { item.ton_fiber, item.unix_date });
                }

                data.Add(new { target = list.FirstOrDefault(item => item.donvi_id == donvi_id).ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getTonLDFiber_SLLD_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<installationInventoryFiberModel> list = getTonLDFiber_Oracle(rq);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_slld = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv, l.tyle_ton })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                lapdat_fiber = lg.Sum(l => l.lapdat_fiber),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_slld)
                    {
                        points.Add(new List<dynamic> { item.lapdat_fiber, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                var list_slld = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv, l.tyle_ton })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                lapdat_fiber = lg.Sum(l => l.lapdat_fiber),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == donvi_id);
                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_slld)
                {
                    points.Add(new List<dynamic> { item.lapdat_fiber, item.unix_date });
                }

                data.Add(new { target = list.FirstOrDefault(item => item.donvi_id == donvi_id).ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getTonLDFiber_SLLD(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<installationInventoryFiberModel> list = getTonLDFiber_Oracle(rq);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_slld = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv, l.tyle_ton })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                lapdat_fiber = lg.Sum(l => l.lapdat_fiber),
                                unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_slld)
                    {
                        points.Add(new List<dynamic> { item.lapdat_fiber, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                var list_slld = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv, l.tyle_ton })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                lapdat_fiber = lg.Sum(l => l.lapdat_fiber),
                                unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == donvi_id);
                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_slld)
                {
                    points.Add(new List<dynamic> { item.lapdat_fiber, item.unix_date });
                }

                data.Add(new { target = list.FirstOrDefault(item => item.donvi_id == donvi_id).ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getTonLDFiber_TL_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<installationInventoryFiberModel> list = getTonLDFiber_Oracle(rq);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_tyle_ton = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv, l.tyle_ton })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                tyle_ton = Math.Round((double)lg.Sum(l => l.ton_fiber) * 100 / lg.Sum(l => l.lapdat_fiber), 2),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_tyle_ton)
                    {
                        points.Add(new List<dynamic> { item.tyle_ton, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                var list_tyle_ton = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv, l.tyle_ton })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                tyle_ton = Math.Round((double)lg.Sum(l => l.ton_fiber) * 100 / lg.Sum(l => l.lapdat_fiber), 2),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == donvi_id);
                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_tyle_ton)
                {
                    points.Add(new List<dynamic> { item.tyle_ton, item.unix_date });
                }

                data.Add(new { target = list.FirstOrDefault(item => item.donvi_id == donvi_id).ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getTonLDFiber_TL(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<installationInventoryFiberModel> list = getTonLDFiber_Oracle(rq);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_tyle_ton = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv, l.tyle_ton })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                tyle_ton = Math.Round((double)lg.Sum(l => l.ton_fiber) * 100 / lg.Sum(l => l.lapdat_fiber), 2),
                                unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_tyle_ton)
                    {
                        points.Add(new List<dynamic> { item.tyle_ton, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                var list_tyle_ton = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv, l.tyle_ton })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                tyle_ton = Math.Round((double)lg.Sum(l => l.ton_fiber) * 100 / lg.Sum(l => l.lapdat_fiber), 2),
                                unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == donvi_id);
                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_tyle_ton)
                {
                    points.Add(new List<dynamic> { item.tyle_ton, item.unix_date });
                }

                data.Add(new { target = list.FirstOrDefault(item => item.donvi_id == donvi_id).ten_dv, datapoints = points });
            }
            return data;
        }

        public dynamic getTonLDFiber(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            if ((int)rq.scopedVars.type.value == 1008) //SL ton
            {
                if (rq.targets[0].data.graph == "line")
                {
                    data = getTonLDFiber_SLTon_date(rq);
                }
                else
                {
                    data = getTonLDFiber_SLTon(rq);
                }               
            }
            else if ((int)rq.scopedVars.type.value == 1009) //SL lap dat
            {
                if (rq.targets[0].data.graph == "line")
                {
                    data = getTonLDFiber_SLLD_date(rq);
                }
                else
                {
                    data = getTonLDFiber_SLLD(rq);
                }                
            }
            else if ((int)rq.scopedVars.type.value == 1010) //Ty le ton
            {
                if (rq.targets[0].data.graph == "line")
                {
                    data = getTonLDFiber_TL_date(rq);
                }
                else if(rq.targets[0].data.graph == "pie")
                {
                    data = getTonLDFiber_SLTon(rq);
                }
                {
                    data = getTonLDFiber_TL(rq);
                }                
            }
            return data;
        }
    }
}
