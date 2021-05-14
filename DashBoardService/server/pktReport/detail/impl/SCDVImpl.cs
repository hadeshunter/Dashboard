using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.convertdata.ccdv;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.common;
using DashBoardService.server.convertdata.scdv;
using Microsoft.Extensions.Configuration;

namespace DashBoardService.server.pktReport.detail.impl
{
    public class SCDVImpl : ISCDV
    {
        private IConfiguration m_configuration;
        private ICommon m_common;
        private ISua_Chua_DV_Dung_TG_Quy_Dinh_New m_scdv;
        public SCDVImpl(IConfiguration configuration, ICommon common, ISua_Chua_DV_Dung_TG_Quy_Dinh_New scdv)
        {
            m_configuration = configuration;
            m_common = common;
            m_scdv = scdv;
        }

        private dynamic getSCDV_oracle(int cable, string startime, string endtime)
        {
            var checkDate = m_scdv.toDataConvert(startime, endtime);
            List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> data = new List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New>();
            if (checkDate == true)
            {
                string connStr = m_configuration.GetConnectionString("DefaultConnection");
                var dt = new DataTable();
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    //@"select * from [dbo].Ccdv_Dung_Tg where ngaycn_bbbg between CONVERT(date,'" + startime + "',103) and CONVERT(date,'" + endtime + "',103)"
                    using (var cmd = new SqlCommand(@"select * from [dbo].Sua_Chua_DV_Dung_TG_Quy_Dinh_New where thoigian_suatot between CONVERT(date,'" + startime + "',103) and CONVERT(date,'" + endtime + "',103)" + (cable == -1 ? "" : "and nhomlc_id = " + cable), conn))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            Sua_Chua_DV_Dung_TG_Quy_Dinh_New row = new Sua_Chua_DV_Dung_TG_Quy_Dinh_New();
                            row.donvi_id = Convert.ToInt32(rdr["donvi_id"]);
                            row.donvi_cha_id = Convert.ToInt32(rdr["donvi_cha_id"]);
                            row.nhomlc_id = Convert.ToInt32(rdr["nhomlc_id"]);
                            row.ten_donvi = rdr["ten_donvi"].ToString();
                            row.ten_dvql = rdr["ten_dvql"].ToString();
                            row.thoigian_suatot = (DateTime)rdr["thoigian_suatot"];
                            row.bh_dtcd = Convert.ToInt32(rdr["bh_dtcd"]);
                            row.bh_fiber = Convert.ToInt32(rdr["bh_fiber"]);
                            row.bh_mytv = Convert.ToInt32(rdr["bh_mytv"]);
                            row.bh_mega = Convert.ToInt32(rdr["bh_mega"]);
                            row.bh_khac = Convert.ToInt32(rdr["bh_khac"]);
                            row.bh_tong = Convert.ToInt32(rdr["bh_tong"]);
                            row.st_dtcd = Convert.ToInt32(rdr["st_dtcd"]);
                            row.st_fiber = Convert.ToInt32(rdr["st_fiber"]);
                            row.st_mytv = Convert.ToInt32(rdr["st_mytv"]);
                            row.st_mega = Convert.ToInt32(rdr["st_mega"]);
                            row.st_quagio = Convert.ToInt32(rdr["st_quagio"]);
                            row.st_tong = Convert.ToInt32(rdr["st_tong"]);
                            row.tyle_chuagiamtru = Convert.ToDouble(rdr["tyle_chuagiamtru"]);
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

        private dynamic getSCDV_DTG_TTVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> list = getSCDV_oracle(cable, startime, endtime);
            var list_scdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.thoigian_suatot))
                .GroupBy(l => new { l.thoigian_suatot.Day, l.thoigian_suatot.Month, l.thoigian_suatot.Year, l.donvi_cha_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        lg.Key.ten_dvql,
                        ok_scdv = lg.Sum(l => l.st_tong),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_scdv.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.ok_scdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getSCDV_DTG_TTVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> list = getSCDV_oracle(cable, startime, endtime);
            var list_scdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.thoigian_suatot))
                .GroupBy(l => new { l.thoigian_suatot.Month, l.thoigian_suatot.Year, l.donvi_cha_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        lg.Key.ten_dvql,
                        ok_scdv = lg.Sum(l => l.st_tong),
                        unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_scdv.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.ok_scdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getSCDV_DTG_DoiVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> list = getSCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.thoigian_suatot))
                .GroupBy(l => new { l.thoigian_suatot.Day, l.thoigian_suatot.Month, l.thoigian_suatot.Year, l.donvi_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dvql,
                        ok_scdv = lg.Sum(l => l.st_tong),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.ok_scdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getSCDV_DTG_DoiVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> list = getSCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.thoigian_suatot))
                .GroupBy(l => new { l.thoigian_suatot.Month, l.thoigian_suatot.Year, l.donvi_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dvql,
                        ok_scdv = lg.Sum(l => l.st_tong),
                        unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.ok_scdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getSCDV_TTG_TTVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> list = getSCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.thoigian_suatot))
                .GroupBy(l => new { l.thoigian_suatot.Day, l.thoigian_suatot.Month, l.thoigian_suatot.Year, l.donvi_cha_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        lg.Key.ten_dvql,
                        st_quagio = lg.Sum(l => l.st_quagio),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.st_quagio, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getSCDV_TTG_TTVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> list = getSCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.thoigian_suatot))
                .GroupBy(l => new { l.thoigian_suatot.Month, l.thoigian_suatot.Year, l.donvi_cha_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        lg.Key.ten_dvql,
                        st_quagio = lg.Sum(l => l.st_quagio),
                        unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.st_quagio, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getSCDV_TTG_DoiVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> list = getSCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.thoigian_suatot))
                .GroupBy(l => new { l.thoigian_suatot.Day, l.thoigian_suatot.Month, l.thoigian_suatot.Year, l.donvi_id, l.ten_donvi })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_donvi,
                        st_quagio = lg.Sum(l => l.st_quagio),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.st_quagio, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getSCDV_TTG_DoiVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> list = getSCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.thoigian_suatot))
                .GroupBy(l => new { l.thoigian_suatot.Month, l.thoigian_suatot.Year, l.donvi_id, l.ten_donvi })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_donvi,
                        st_quagio = lg.Sum(l => l.st_quagio),
                        unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.st_quagio, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getSCDV_TL_TTVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> list = getSCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.thoigian_suatot))
                .GroupBy(l => new { l.thoigian_suatot.Day, l.thoigian_suatot.Month, l.thoigian_suatot.Year, l.donvi_cha_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        lg.Key.ten_dvql,
                        tyle_scdv = Math.Round((double)(lg.Sum(l => l.st_tong)) * 100 / lg.Sum(l => l.bh_tong), 4),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_scdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getSCDV_TL_TTVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> list = getSCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.thoigian_suatot))
                .GroupBy(l => new { l.thoigian_suatot.Month, l.thoigian_suatot.Year, l.donvi_cha_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        lg.Key.ten_dvql,
                        tyle_scdv = Math.Round((double)(lg.Sum(l => l.st_tong)) * 100 / lg.Sum(l => l.bh_tong), 4),
                        unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_scdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getSCDV_TL_DoiVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> list = getSCDV_oracle(cable, startime, endtime);
            var list_ccdv = list.FindAll(item => item.donvi_cha_id == unit)
                .OrderBy(ele => (ele.donvi_cha_id, ele.thoigian_suatot))
                .GroupBy(l => new { l.thoigian_suatot.Day, l.thoigian_suatot.Month, l.thoigian_suatot.Year, l.donvi_id, l.ten_donvi })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_donvi,
                        tyle_scdv = Math.Round((double)(lg.Sum(l => l.st_tong)) * 100 / lg.Sum(l => l.bh_tong), 4),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_scdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getSCDV_TL_DoiVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<Sua_Chua_DV_Dung_TG_Quy_Dinh_New> list = getSCDV_oracle(cable, startime, endtime);
            var list_ccdv = list.FindAll(item => item.donvi_cha_id == unit)
                .OrderBy(ele => (ele.donvi_cha_id, ele.thoigian_suatot))
                .GroupBy(l => new { l.thoigian_suatot.Month, l.thoigian_suatot.Year, l.donvi_id, l.ten_donvi })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_donvi,
                        tyle_scdv = Math.Round((double)(lg.Sum(l => l.st_tong)) * 100 / lg.Sum(l => l.bh_tong), 4),
                        unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_scdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        public dynamic getSCDV_DTG_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getSCDV_DTG_TTVT_date(cable, unit, startime, endtime);
            }
            else
            {
                data = getSCDV_DTG_DoiVT_date(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getSCDV_DTG(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getSCDV_DTG_TTVT(cable, unit, startime, endtime);
            }
            else
            {
                data = getSCDV_DTG_DoiVT(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getSCDV_TTG_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getSCDV_TTG_TTVT_date(cable, unit, startime, endtime);
            }
            else
            {
                data = getSCDV_TTG_DoiVT_date(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getSCDV_TTG(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getSCDV_TTG_TTVT(cable, unit, startime, endtime);
            }
            else
            {
                data = getSCDV_TTG_DoiVT(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getSCDV_TL_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getSCDV_TL_TTVT_date(cable, unit, startime, endtime);
            }
            else
            {
                data = getSCDV_TL_DoiVT_date(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getSCDV_TL(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getSCDV_TL_TTVT(cable, unit, startime, endtime);
            }
            else
            {
                data = getSCDV_TL_DoiVT(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getSCDV(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var date = m_common.convertToString(rq);
            switch ((int)rq.scopedVars.type.value)
            {
                case 1: //Đúng giờ
                    if (rq.targets[0].data.graph == "line")
                    {
                        data = getSCDV_DTG_date((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    }
                    else
                    {
                        data = getSCDV_DTG((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    }                    
                    break;
                case 2: //Trễ giờ
                    if (rq.targets[0].data.graph == "line")
                    {
                        data = getSCDV_TTG_date((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    }
                    else
                    {
                        data = getSCDV_TTG((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    }                    
                    break;
                case 3: //Tỷ lệ
                    if (rq.targets[0].data.graph == "line")
                    {
                        data = getSCDV_TL_date((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    }
                    else if (rq.targets[0].data.graph == "pie")
                    {
                        data = getSCDV_DTG((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    }
                    else
                    {
                        data = getSCDV_TL((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    }
                    
                    break;
            }
            return data;
        }
    }
}
