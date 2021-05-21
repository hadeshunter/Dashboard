using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.convertdata.xlsc;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.common;
using DashBoardService.server.convertdata.xlsc;
using Microsoft.Extensions.Configuration;

namespace DashBoardService.server.pktReport.detail.impl
{
    public class XLSCImpl : IXLSC
    {
        private IConfiguration m_configuration;
        private ICommon m_common;
        private ITyLeThoiGianDapUngXuLySuCo m_xlsc;
        public XLSCImpl(IConfiguration configuration, ICommon common, ITyLeThoiGianDapUngXuLySuCo xlsc)
        {
            m_configuration = configuration;
            m_common = common;
            m_xlsc = xlsc;
        }

        private dynamic getXLSC_oracle(int cable, string startime, string endtime)
        {
            var checkDate = m_xlsc.toDataConvert(startime, endtime);
            List<TyLeThoiGianDapUngXuLySuCo> data = new List<TyLeThoiGianDapUngXuLySuCo>();
            if (checkDate == true)
            {
                string connStr = m_configuration.GetConnectionString("DefaultConnection");
                var dt = new DataTable();
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(@"select * from [dbo].TyLeThoiGianDapUngXuLySuCo where ngay_ht between CONVERT(DATETIME,'" + startime + "',103) and CONVERT(DATETIME,'" + endtime + "',103)" + (cable == -1 ? "" : "and nhomlc_id = " + cable), conn))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            TyLeThoiGianDapUngXuLySuCo row = new TyLeThoiGianDapUngXuLySuCo();
                            row.donvi_id = Convert.ToInt32(rdr["donvi_id"]);
                            row.dv_cha_id = Convert.ToInt32(rdr["dv_cha_id"]);
                            row.nhomlc_id = Convert.ToInt32(rdr["nhomlc_id"]);
                            row.ten_dv = rdr["ten_dv"].ToString();
                            row.ten_trungtam = rdr["ten_trungtam"].ToString();
                            row.ngay_ht = (DateTime)rdr["ngay_ht"];
                            row.st_tong = Convert.ToInt32(rdr["st_tong"]);
                            row.bh_tong = Convert.ToInt32(rdr["bh_tong"]);
                            row.st_quagio = Convert.ToInt32(rdr["st_quagio"]);
                            row.tyle_chuagiamtru = Convert.ToDouble(rdr["tyle_chuagiamtru"]);
                            data.Add(row);
                        }
                    }
                    conn.Close();
                }
                return data;
            } else
            {
                return data;
            }
        }

        private dynamic getXLSC_SLBH_TTVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Day, l.ngay_ht.Month, l.ngay_ht.Year, l.dv_cha_id, l.ten_trungtam })
                .Select(lg =>
                    new
                    {
                        lg.Key.dv_cha_id,
                        lg.Key.ten_trungtam,
                        bh_tong = lg.Sum(l => l.bh_tong),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.dv_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.bh_tong, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_SLBH_TTVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listTTVT = m_common.getListTTVT();
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Month, l.ngay_ht.Year, l.dv_cha_id, l.ten_trungtam })
                .Select(lg =>
                    new
                    {
                        lg.Key.dv_cha_id,
                        lg.Key.ten_trungtam,
                        bh_tong = lg.Sum(l => l.bh_tong),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.dv_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.bh_tong, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_SLBH_DoiVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list.FindAll(item => item.dv_cha_id == unit)
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Day, l.ngay_ht.Month, l.ngay_ht.Year, l.donvi_id, l.ten_dv })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dv,
                        bh_tong = lg.Sum(l => l.bh_tong),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.bh_tong, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_SLBH_DoiVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list.FindAll(item => item.dv_cha_id == unit)
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Month, l.ngay_ht.Year, l.donvi_id, l.ten_dv })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dv,
                        bh_tong = lg.Sum(l => l.bh_tong),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.bh_tong, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_SLST_TTVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Day, l.ngay_ht.Month, l.ngay_ht.Year, l.dv_cha_id, l.ten_trungtam })
                .Select(lg =>
                    new
                    {
                        lg.Key.dv_cha_id,
                        lg.Key.ten_trungtam,
                        st_tong = lg.Sum(l => l.st_tong),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.dv_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.st_tong, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_SLST_TTVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listTTVT = m_common.getListTTVT();
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Month, l.ngay_ht.Year, l.dv_cha_id, l.ten_trungtam })
                .Select(lg =>
                    new
                    {
                        lg.Key.dv_cha_id,
                        lg.Key.ten_trungtam,
                        st_tong = lg.Sum(l => l.st_tong),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.dv_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.st_tong, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_SLST_DoiVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list.FindAll(item => item.dv_cha_id == unit)
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Day, l.ngay_ht.Month, l.ngay_ht.Year, l.donvi_id, l.ten_dv })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dv,
                        st_tong = lg.Sum(l => l.st_tong),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.st_tong, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_SLST_DoiVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list.FindAll(item => item.dv_cha_id == unit)
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Month, l.ngay_ht.Year, l.donvi_id, l.ten_dv })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dv,
                        st_tong = lg.Sum(l => l.st_tong),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.st_tong, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_SLQG_TTVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Day, l.ngay_ht.Month, l.ngay_ht.Year, l.dv_cha_id, l.ten_trungtam })
                .Select(lg =>
                    new
                    {
                        lg.Key.dv_cha_id,
                        lg.Key.ten_trungtam,
                        st_quagio = lg.Sum(l => l.st_quagio),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.dv_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.st_quagio, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_SLQG_TTVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listTTVT = m_common.getListTTVT();
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Month, l.ngay_ht.Year, l.dv_cha_id, l.ten_trungtam })
                .Select(lg =>
                    new
                    {
                        lg.Key.dv_cha_id,
                        lg.Key.ten_trungtam,
                        st_quagio = lg.Sum(l => l.st_quagio),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.dv_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.st_quagio, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_SLQG_DoiVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list.FindAll(item => item.dv_cha_id == unit)
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Day, l.ngay_ht.Month, l.ngay_ht.Year, l.donvi_id, l.ten_dv })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dv,
                        st_quagio = lg.Sum(l => l.st_quagio),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.st_quagio, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_SLQG_DoiVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list.FindAll(item => item.dv_cha_id == unit)
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Month, l.ngay_ht.Year, l.donvi_id, l.ten_dv })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dv,
                        st_quagio = lg.Sum(l => l.st_quagio),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.st_quagio, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_TL_TTVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Day, l.ngay_ht.Month, l.ngay_ht.Year, l.dv_cha_id, l.ten_trungtam })
                .Select(lg =>
                    new
                    {
                        lg.Key.dv_cha_id,
                        lg.Key.ten_trungtam,
                        tyle_chuagiamtru = Math.Round((double)lg.Sum(l => l.st_tong) * 100 / lg.Sum(l => l.bh_tong), 4),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.dv_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_chuagiamtru, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_TL_TTVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listTTVT = m_common.getListTTVT();
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Month, l.ngay_ht.Year, l.dv_cha_id, l.ten_trungtam })
                .Select(lg =>
                    new
                    {
                        lg.Key.dv_cha_id,
                        lg.Key.ten_trungtam,
                        tyle_chuagiamtru = Math.Round((double)lg.Sum(l => l.st_tong) * 100 / lg.Sum(l => l.bh_tong), 4),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.dv_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_chuagiamtru, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_TL_DoiVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list.FindAll(item => item.dv_cha_id == unit)
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Day, l.ngay_ht.Month, l.ngay_ht.Year, l.donvi_id, l.ten_dv })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dv,
                        tyle_chuagiamtru = Math.Round((double)lg.Sum(l => l.st_tong) * 100 / lg.Sum(l => l.bh_tong), 4),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_chuagiamtru, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getXLSC_TL_DoiVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<TyLeThoiGianDapUngXuLySuCo> list = getXLSC_oracle(cable, startime, endtime);
            var list_xlsc = list.FindAll(item => item.dv_cha_id == unit)
                .OrderBy(ele => (ele.dv_cha_id, ele.ngay_ht))
                .GroupBy(l => new { l.ngay_ht.Month, l.ngay_ht.Year, l.donvi_id, l.ten_dv })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dv,
                        tyle_chuagiamtru = Math.Round((double)lg.Sum(l => l.st_tong) * 100 / lg.Sum(l => l.bh_tong), 4),
                        unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_xlsc.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_chuagiamtru, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        public dynamic getXLSC_SLBH_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getXLSC_SLBH_TTVT_date(cable, unit, startime, endtime);
            }
            else
            {
                data = getXLSC_SLBH_DoiVT_date(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getXLSC_SLBH(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getXLSC_SLBH_TTVT(cable, unit, startime, endtime);
            }
            else
            {
                data = getXLSC_SLBH_DoiVT(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getXLSC_SLST_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getXLSC_SLST_TTVT_date(cable, unit, startime, endtime);
            }
            else
            {
                data = getXLSC_SLST_DoiVT_date(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getXLSC_SLST(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getXLSC_SLST_TTVT(cable, unit, startime, endtime);
            }
            else
            {
                data = getXLSC_SLST_DoiVT(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getXLSC_SLQG_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getXLSC_SLQG_TTVT_date(cable, unit, startime, endtime);
            }
            else
            {
                data = getXLSC_SLQG_DoiVT_date(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getXLSC_SLQG(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getXLSC_SLQG_TTVT(cable, unit, startime, endtime);
            }
            else
            {
                data = getXLSC_SLQG_DoiVT(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getXLSC_TL_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getXLSC_TL_TTVT_date(cable, unit, startime, endtime);
            }
            else
            {
                data = getXLSC_TL_DoiVT_date(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getXLSC_TL(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getXLSC_TL_TTVT(cable, unit, startime, endtime);
            }
            else
            {
                data = getXLSC_TL_DoiVT(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getXLSC(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var date = m_common.convertToString(rq);

            if ((int)rq.scopedVars.type.value == 1011) //Bao hong
            {
                if (rq.targets[0].data.graph == "line")
                {
                    data = getXLSC_SLBH_date((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                }
                else
                {
                    data = getXLSC_SLBH((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                }                
            }
            else if ((int)rq.scopedVars.type.value == 1012) //Sua dung gio
            {
                if (rq.targets[0].data.graph == "line")
                {
                    data = getXLSC_SLST_date((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                }
                else
                {
                    data = getXLSC_SLST((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                }                
            }
            else if ((int)rq.scopedVars.type.value == 1013) //Sua qua gio
            {
                if (rq.targets[0].data.graph == "line")
                {
                    data = getXLSC_SLQG_date((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                }
                else
                {
                    data = getXLSC_SLQG((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                }                
            }
            else if ((int)rq.scopedVars.type.value == 6) //Ty le
            {
                if (rq.targets[0].data.graph == "line")
                {
                    data = getXLSC_TL_date((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                }
                else if(rq.targets[0].data.graph == "pie")
                {
                    data = getXLSC_SLST((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                }
                else
                {
                    data = getXLSC_TL((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                }                
            }
            
            return data;
        }
    }
}
