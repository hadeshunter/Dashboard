using ClassModel.model.pktReport;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.pktReport.detail.impl
{
    public class LuykeLapgoFiberImpl: ILuykeLapgoFiber
    {
        private ICommon m_common;
        private IConfiguration m_configuration;
        private IPktReport m_pktReport;
        public LuykeLapgoFiberImpl(ICommon m_common, IConfiguration m_configuration, IPktReport pktReport)
        {
            this.m_common = m_common;
            this.m_configuration = m_configuration;
            m_pktReport = pktReport;
        }
        
        private dynamic getLuykeLapgoFiber_oracle(RqGrafana rq)
        {
            var data = m_pktReport.executeLuykeLapgoFiberDate(rq);
            return data;
        }

        private dynamic getLuykeLapMoi_TTVT(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listTTVT = m_common.getListTTVT();
            List<LuykeLapgoFiber_date> list = getLuykeLapgoFiber_oracle(rq);
            var list_lapmoi = list
                .OrderBy(ele => (ele.donvi_id, ele.ngay))
                .GroupBy(l => new { l.ngay.Month, l.ngay.Year, l.donvi_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        sl_lapmoi = lg.Sum(l => l.lapmoi_lk) + lg.Sum(l => l.phuchoi_lk) + lg.Sum(l => l.dd_lm_lk) + lg.Sum(l => l.chuyendoi_lk),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_lapmoi.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.sl_lapmoi, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getLuykeLapMoi_TTVT_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<LuykeLapgoFiber_date> list = getLuykeLapgoFiber_oracle(rq);
            var list_lapmoi = list
                .OrderBy(ele => (ele.donvi_id, ele.ngay))
                .GroupBy(l => new { l.ngay.Day, l.ngay.Month, l.ngay.Year, l.donvi_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        sl_lapmoi = lg.Sum(l => l.lapmoi_lk) + lg.Sum(l => l.phuchoi_lk) + lg.Sum(l => l.dd_lm_lk) + lg.Sum(l => l.chuyendoi_lk),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_lapmoi.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.sl_lapmoi, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getLuykeGo_TTVT(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listTTVT = m_common.getListTTVT();
            List<LuykeLapgoFiber_date> list = getLuykeLapgoFiber_oracle(rq);
            var list_go = list
                .OrderBy(ele => (ele.donvi_id, ele.ngay))
                .GroupBy(l => new { l.ngay.Month, l.ngay.Year, l.donvi_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        sl_go = lg.Sum(l => l.go_yeucau_lk) + lg.Sum(l => l.go_lamsach_lk) + lg.Sum(l => l.dd_go_lk) + lg.Sum(l => l.cd_giam_lk),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_go.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.sl_go, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getLuykeGo_TTVT_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<LuykeLapgoFiber_date> list = getLuykeLapgoFiber_oracle(rq);
            var list_go = list
                .OrderBy(ele => (ele.donvi_id, ele.ngay))
                .GroupBy(l => new { l.ngay.Day, l.ngay.Month, l.ngay.Year, l.donvi_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        sl_go = lg.Sum(l => l.go_yeucau_lk) + lg.Sum(l => l.go_lamsach_lk) + lg.Sum(l => l.dd_go_lk) + lg.Sum(l => l.cd_giam_lk),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_go.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.sl_go, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getLuykeThucTang_TTVT(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listTTVT = m_common.getListTTVT();
            List<LuykeLapgoFiber_date> list = getLuykeLapgoFiber_oracle(rq);
            var list_tt = list
                .OrderBy(ele => (ele.donvi_id, ele.ngay))
                .GroupBy(l => new { l.ngay.Month, l.ngay.Year, l.donvi_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        tyle_golm = Math.Round((double)(lg.Sum(l => l.go_yeucau_lk) + lg.Sum(l => l.go_lamsach_lk) + lg.Sum(l => l.dd_go_lk) + lg.Sum(l => l.cd_giam_lk))*100/(lg.Sum(l => l.lapmoi_lk) + lg.Sum(l => l.phuchoi_lk) + lg.Sum(l => l.dd_lm_lk) + lg.Sum(l => l.chuyendoi_lk)),2),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_tt.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_golm, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getLuykeThucTang_TTVT_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<LuykeLapgoFiber_date> list = getLuykeLapgoFiber_oracle(rq);
            var list_tt = list
                .OrderBy(ele => (ele.donvi_id, ele.ngay))
                .GroupBy(l => new { l.ngay.Day, l.ngay.Month, l.ngay.Year, l.donvi_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        tyle_golm = Math.Round((double)(lg.Sum(l => l.go_yeucau_lk) + lg.Sum(l => l.go_lamsach_lk) + lg.Sum(l => l.dd_go_lk) + lg.Sum(l => l.cd_giam_lk)) * 100 / (lg.Sum(l => l.lapmoi_lk) + lg.Sum(l => l.phuchoi_lk) + lg.Sum(l => l.dd_lm_lk) + lg.Sum(l => l.chuyendoi_lk)), 2),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_tt.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_golm, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        public dynamic getLuykeLapgoFiberVNN(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            switch ((int)rq.scopedVars.type.value)
            {
                case 2011:  //TL go/lap moi                        
                    if (rq.targets[0].data.graph == "line")
                    {
                        data = getLuykeThucTang_TTVT_date(rq);
                    }
                    else
                    {
                        data = getLuykeThucTang_TTVT(rq);
                    }
                    break;
                case 2012:  //SL go   
                    if (rq.targets[0].data.graph == "line")
                    {
                        data = getLuykeGo_TTVT_date(rq);
                    }
                    else
                    {
                        data = getLuykeGo_TTVT(rq);
                    }
                    break;
                case 2013:  //SL lap moi   
                    if (rq.targets[0].data.graph == "line")
                    {
                        data = getLuykeLapMoi_TTVT_date(rq);
                    }
                    else
                    {
                        data = getLuykeLapMoi_TTVT(rq);
                    }
                    break;
            }
            return data;
        }
    }
}
