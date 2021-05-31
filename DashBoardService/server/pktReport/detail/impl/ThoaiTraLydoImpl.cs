using ClassModel.model.RqGrafana;
using ClassModel.model.ThoaitraNLML;
using ClassModel.model.unit;
using DashBoardService.server.common;
using DashBoardService.server.ThoaitraNLML;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.pktReport.detail.impl
{
    public class ThoaiTraLydoImpl: IThoaiTraLydo
    {
        private ICommon m_common;
        private IConfiguration m_configuration;
        private IThoaitraNLML m_thoaiTraNLML;
        public ThoaiTraLydoImpl(ICommon m_common, IConfiguration m_configuration, IThoaitraNLML thoaiTraNLML)
        {
            this.m_common = m_common;
            this.m_configuration = m_configuration;
            m_thoaiTraNLML = thoaiTraNLML;
        }

        private dynamic getThoaiTraNLMLDate_oracle(RqGrafana rq)
        {
            List<ThoaitraNLMLModel_date> data = new List<ThoaitraNLMLModel_date>();
            var date = m_common.convertToString(rq);
            var startdate = new DateTime(Int32.Parse(date.Item1.Substring(6, 4)), Int32.Parse(date.Item1.Substring(3, 2)), Int32.Parse(date.Item1.Substring(0, 2)));
            var enddate = new DateTime(Int32.Parse(date.Item2.Substring(6, 4)), Int32.Parse(date.Item2.Substring(3, 2)), Int32.Parse(date.Item2.Substring(0, 2)));
            var listmonth = m_common.GetMonthsBetween(startdate, enddate);
            foreach (var month in listmonth)
            {
                m_thoaiTraNLML.GetThoaitraNLML(month.ToString("yyyyMM"));
            }
            data = m_thoaiTraNLML.GetThoaitraNLMLDate(rq);
            return data;
        }

        private dynamic getThoaiTra_DQ_TTVT(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listTTVT = m_common.getListTTVT();
            List<ThoaitraNLMLModel_date> list = getThoaiTraNLMLDate_oracle(rq);
            var list_dq = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_cha_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        thoaitra_docquyen = lg.Sum(l => l.thoaitra_docquyen),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_dq.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.thoaitra_docquyen, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getThoaiTra_DQ_TTVT_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            List<Unit> listTTVT = m_common.getListTTVT();
            List<ThoaitraNLMLModel_date> list = getThoaiTraNLMLDate_oracle(rq);
            var list_dq = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_cha_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        thoaitra_docquyen = lg.Sum(l => l.thoaitra_docquyen),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_dq.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.thoaitra_docquyen, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getThoaiTra_DQ_DoiVT(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == (int)rq.scopedVars.unit.value);
            List<ThoaitraNLMLModel_date> list = getThoaiTraNLMLDate_oracle(rq);
            var list_dq = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        thoaitra_docquyen = lg.Sum(l => l.thoaitra_docquyen),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit doivt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_dq.Where(item => item.donvi_id == doivt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.thoaitra_docquyen, item.unix_date });
                }

                data.Add(new { target = doivt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getThoaiTra_DQ_DoiVT_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == (int)rq.scopedVars.unit.value);
            List<ThoaitraNLMLModel_date> list = getThoaiTraNLMLDate_oracle(rq);
            var list_dq = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        thoaitra_docquyen = lg.Sum(l => l.thoaitra_docquyen),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit doivt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_dq.Where(item => item.donvi_id == doivt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.thoaitra_docquyen, item.unix_date });
                }

                data.Add(new { target = doivt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getThoaiTra_NLML_TTVT(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listTTVT = m_common.getListTTVT();
            List<ThoaitraNLMLModel_date> list = getThoaiTraNLMLDate_oracle(rq);
            var list_dq = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_cha_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        thoaitra_nlml = lg.Sum(l => l.thoaitra_nlml),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_dq.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.thoaitra_nlml, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getThoaiTra_NLML_TTVT_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            List<Unit> listTTVT = m_common.getListTTVT();
            List<ThoaitraNLMLModel_date> list = getThoaiTraNLMLDate_oracle(rq);
            var list_dq = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_cha_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        thoaitra_nlml = lg.Sum(l => l.thoaitra_nlml),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_dq.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.thoaitra_nlml, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getThoaiTra_NLML_DoiVT(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == (int)rq.scopedVars.unit.value);
            List<ThoaitraNLMLModel_date> list = getThoaiTraNLMLDate_oracle(rq);
            var list_dq = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        thoaitra_nlml = lg.Sum(l => l.thoaitra_nlml),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit doivt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_dq.Where(item => item.donvi_id == doivt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.thoaitra_nlml, item.unix_date });
                }

                data.Add(new { target = doivt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getThoaiTra_NLML_DoiVT_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == (int)rq.scopedVars.unit.value);
            List<ThoaitraNLMLModel_date> list = getThoaiTraNLMLDate_oracle(rq);
            var list_dq = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        thoaitra_nlml = lg.Sum(l => l.thoaitra_nlml),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit doivt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_dq.Where(item => item.donvi_id == doivt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.thoaitra_nlml, item.unix_date });
                }

                data.Add(new { target = doivt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getThoaiTra_DQ(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            int unit = (int)rq.scopedVars.unit.value;
            if (unit == 0)
            {
                data = getThoaiTra_DQ_TTVT(rq);
            }
            else
            {
                data = getThoaiTra_DQ_DoiVT(rq);
            }
            return data;
        }

        private dynamic getThoaiTra_DQ_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            int unit = (int)rq.scopedVars.unit.value;
            if (unit == 0)
            {
                data = getThoaiTra_DQ_TTVT_date(rq);
            }
            else
            {
                data = getThoaiTra_DQ_DoiVT_date(rq);
            }
            return data;
        }

        private dynamic getThoaiTra_NLML(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            int unit = (int)rq.scopedVars.unit.value;
            if (unit == 0)
            {
                data = getThoaiTra_NLML_TTVT(rq);
            }
            else
            {
                data = getThoaiTra_NLML_DoiVT(rq);
            }
            return data;
        }

        private dynamic getThoaiTra_NLML_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            int unit = (int)rq.scopedVars.unit.value;
            if (unit == 0)
            {
                data = getThoaiTra_NLML_TTVT_date(rq);
            }
            else
            {
                data = getThoaiTra_NLML_DoiVT_date(rq);
            }
            return data;
        }        

        public dynamic getLydoThoaiTra(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            switch ((int)rq.scopedVars.type.value)
            {
                case 2009:  //NLML                        
                    if (rq.targets[0].data.graph == "line")
                    {
                        data = getThoaiTra_NLML_date(rq);
                    }
                    else
                    {
                        data = getThoaiTra_NLML(rq);
                    }
                    break;
                case 2010:  //Doc quyen   
                    if (rq.targets[0].data.graph == "line")
                    {
                        data = getThoaiTra_DQ_date(rq);
                    }
                    else
                    {
                        data = getThoaiTra_DQ(rq);
                    }
                    break;
            }
            return data;
        }
    }
}
