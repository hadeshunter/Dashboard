using ClassModel.convertdata.ccdv;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.common;
using DashBoardService.server.convertdata.ccdv;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.pktReport.detail.impl
{
    public class CCDVImpl : ICCDV
    {
        private IConfiguration m_configuration;
        private ICommon m_common;
        private ICcdvDungThoiGian m_ccdvDungThoiGian;
        public CCDVImpl(IConfiguration configuration, ICommon common, ICcdvDungThoiGian ccdvDungThoiGian)
        {
            m_configuration = configuration;
            m_common = common;
            m_ccdvDungThoiGian = ccdvDungThoiGian;
        }

        public dynamic getCCDV_oracle(int cable, string startime, string endtime)
        {
            var checkDate = m_ccdvDungThoiGian.toDataConvert(startime, endtime);
            List<Ccdv_Dung_Tg> data = new List<Ccdv_Dung_Tg>();
            if (checkDate == true)
            {
                string connStr = m_configuration.GetConnectionString("DefaultConnection");
                var dt = new DataTable();
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    //@"select * from [dbo].Ccdv_Dung_Tg where ngaycn_bbbg between CONVERT(date,'" + startime + "',103) and CONVERT(date,'" + endtime + "',103)"
                    using (var cmd = new SqlCommand(@"select * from [dbo].Ccdv_Dung_Tg where ngaycn_bbbg between CONVERT(DATETIME,'" + startime + "',103) and CONVERT(DATETIME,'" + endtime + "',103)" + (cable == -1 ? "" : "and nhomlc_id = " + cable), conn))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            Ccdv_Dung_Tg row = new Ccdv_Dung_Tg();
                            row.donvi_id = Convert.ToInt32(rdr["donvi_id"]);
                            row.donvi_cha_id = Convert.ToInt32(rdr["donvi_cha_id"]);
                            row.nhomlc_id = Convert.ToInt32(rdr["nhomlc_id"]);
                            row.ten_dv = rdr["ten_dv"].ToString();
                            row.ten_dvql = rdr["ten_dvql"].ToString();
                            row.ngaycn_bbbg = (DateTime)rdr["ngaycn_bbbg"];
                            row.tong_pct = Convert.ToInt32(rdr["tong_pct"]);
                            row.soluong_khonghen_ccdv = Convert.ToInt32(rdr["soluong_khonghen_ccdv"]);
                            row.soluong_cohen_ccdv = Convert.ToInt32(rdr["soluong_cohen_ccdv"]);
                            row.tregio_khonghen_ccdv = Convert.ToInt32(rdr["tregio_khonghen_ccdv"]);
                            row.tregio_cohen_ccdv = Convert.ToInt32(rdr["tregio_cohen_ccdv"]);
                            row.ok_khonghen_ccdv = Convert.ToInt32(rdr["ok_khonghen_ccdv"]);
                            row.ok_cohen_ccdv = Convert.ToInt32(rdr["ok_cohen_ccdv"]);
                            row.tyle_ccdv = Convert.ToDouble(rdr["tyle_ccdv"]);
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

        private dynamic getCCDV_DTG_TTVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<Ccdv_Dung_Tg> list = getCCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngaycn_bbbg))
                .GroupBy(l => new { l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_cha_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        lg.Key.ten_dvql,
                        ok_ccdv = lg.Sum(l => l.ok_khonghen_ccdv) + lg.Sum(l => l.ok_cohen_ccdv),
                        unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.ok_ccdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getCCDV_DTG_TTVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<Ccdv_Dung_Tg> list = getCCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngaycn_bbbg))
                .GroupBy(l => new { l.ngaycn_bbbg.Day, l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_cha_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        lg.Key.ten_dvql,
                        ok_ccdv = lg.Sum(l => l.ok_khonghen_ccdv) + lg.Sum(l => l.ok_cohen_ccdv),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.ok_ccdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getCCDV_DTG_DoiVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<Ccdv_Dung_Tg> list = getCCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngaycn_bbbg))
                .GroupBy(l => new { l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dvql,
                        ok_ccdv = lg.Sum(l => l.ok_khonghen_ccdv) + lg.Sum(l => l.ok_cohen_ccdv),
                        unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.ok_ccdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getCCDV_DTG_DoiVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<Ccdv_Dung_Tg> list = getCCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngaycn_bbbg))
                .GroupBy(l => new { l.ngaycn_bbbg.Day, l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dvql,
                        ok_ccdv = lg.Sum(l => l.ok_khonghen_ccdv) + lg.Sum(l => l.ok_cohen_ccdv),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.ok_ccdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getCCDV_TTG_TTVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<Ccdv_Dung_Tg> list = getCCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngaycn_bbbg))
                .GroupBy(l => new { l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_cha_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        lg.Key.ten_dvql,
                        tregio_ccdv = lg.Sum(l => l.tregio_khonghen_ccdv) + lg.Sum(l => l.tregio_cohen_ccdv),
                        unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tregio_ccdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getCCDV_TTG_TTVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<Ccdv_Dung_Tg> list = getCCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngaycn_bbbg))
                .GroupBy(l => new { l.ngaycn_bbbg.Day, l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_cha_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        lg.Key.ten_dvql,
                        tregio_ccdv = lg.Sum(l => l.tregio_khonghen_ccdv) + lg.Sum(l => l.tregio_cohen_ccdv),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tregio_ccdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getCCDV_TTG_DoiVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<Ccdv_Dung_Tg> list = getCCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngaycn_bbbg))
                .GroupBy(l => new { l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dvql,
                        tregio_ccdv = lg.Sum(l => l.tregio_khonghen_ccdv) + lg.Sum(l => l.tregio_cohen_ccdv),
                        unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tregio_ccdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getCCDV_TTG_DoiVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<Ccdv_Dung_Tg> list = getCCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngaycn_bbbg))
                .GroupBy(l => new { l.ngaycn_bbbg.Day, l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dvql,
                        tregio_ccdv = lg.Sum(l => l.tregio_khonghen_ccdv) + lg.Sum(l => l.tregio_cohen_ccdv),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tregio_ccdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getCCDV_TL_TTVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<Ccdv_Dung_Tg> list = getCCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngaycn_bbbg))
                .GroupBy(l => new { l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_cha_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        lg.Key.ten_dvql,
                        tyle_ccdv = Math.Round((double)(lg.Sum(l => l.ok_khonghen_ccdv)+lg.Sum(l=>l.ok_cohen_ccdv))*100/lg.Sum(l => l.tong_pct),2),
                        unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_ccdv, item.unix_date });
                }
                
                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getCCDV_TL_TTVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listTTVT = m_common.getListTTVT();
            List<Ccdv_Dung_Tg> list = getCCDV_oracle(cable, startime, endtime);
            var list_ccdv = list
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngaycn_bbbg))
                .GroupBy(l => new { l.ngaycn_bbbg.Day, l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_cha_id, l.ten_dvql })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_cha_id,
                        lg.Key.ten_dvql,
                        tyle_ccdv = Math.Round((double)(lg.Sum(l => l.ok_khonghen_ccdv) + lg.Sum(l => l.ok_cohen_ccdv)) * 100 / lg.Sum(l => l.tong_pct), 2),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listTTVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_cha_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_ccdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getCCDV_TL_DoiVT(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<Ccdv_Dung_Tg> list = getCCDV_oracle(cable, startime, endtime);
            var list_ccdv = list.FindAll(item => item.donvi_cha_id == unit)
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngaycn_bbbg))
                .GroupBy(l => new { l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_id, l.ten_dv })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dv,
                        tyle_ccdv = Math.Round((double)(lg.Sum(l => l.ok_khonghen_ccdv) + lg.Sum(l => l.ok_cohen_ccdv))*100/lg.Sum(l => l.tong_pct), 2),
                        unix_date = m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_ccdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        private dynamic getCCDV_TL_DoiVT_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == unit);
            List<Ccdv_Dung_Tg> list = getCCDV_oracle(cable, startime, endtime);
            var list_ccdv = list.FindAll(item => item.donvi_cha_id == unit)
                .OrderBy(ele => (ele.donvi_cha_id, ele.ngaycn_bbbg))
                .GroupBy(l => new { l.ngaycn_bbbg.Day, l.ngaycn_bbbg.Month, l.ngaycn_bbbg.Year, l.donvi_id, l.ten_dv })
                .Select(lg =>
                    new
                    {
                        lg.Key.donvi_id,
                        lg.Key.ten_dv,
                        tyle_ccdv = Math.Round((double)(lg.Sum(l => l.ok_khonghen_ccdv) + lg.Sum(l => l.ok_cohen_ccdv)) * 100 / lg.Sum(l => l.tong_pct), 2),
                        unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                    });
            foreach (Unit ttvt in listDoiVT)
            {
                List<dynamic> points = new List<dynamic>();
                var tmp = list_ccdv.Where(item => item.donvi_id == ttvt.donvi_id);
                foreach (var item in tmp)
                {
                    points.Add(new List<dynamic> { item.tyle_ccdv, item.unix_date });
                }

                data.Add(new { target = ttvt.ten_dv, datapoints = points });
            }
            return data;
        }

        public dynamic getCCDV_TTG(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getCCDV_TTG_TTVT(cable, unit, startime, endtime);
            }
            else
            {
                data = getCCDV_TTG_DoiVT(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getCCDV_TTG_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getCCDV_TTG_TTVT_date(cable, unit, startime, endtime);
            }
            else
            {
                data = getCCDV_TTG_DoiVT_date(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getCCDV_DTG(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getCCDV_DTG_TTVT(cable, unit, startime, endtime);
            }
            else
            {
                data = getCCDV_DTG_DoiVT(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getCCDV_DTG_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getCCDV_DTG_TTVT_date(cable, unit, startime, endtime);
            }
            else
            {
                data = getCCDV_DTG_DoiVT_date(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getCCDV_TL(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getCCDV_TL_TTVT(cable, unit, startime, endtime);
            } else
            {
                data = getCCDV_TL_DoiVT(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getCCDV_TL_date(int cable, int unit, string startime, string endtime)
        {
            List<dynamic> data = new List<dynamic>();
            if (unit == 0)
            {
                data = getCCDV_TL_TTVT_date(cable, unit, startime, endtime);
            }
            else
            {
                data = getCCDV_TL_DoiVT_date(cable, unit, startime, endtime);
            }
            return data;
        }

        public dynamic getCCDV(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var date = m_common.convertToString(rq);
            switch ((int)rq.scopedVars.type.value)
            {
                case 1: //Đúng giờ
                    if (rq.targets[0].data.graph == "line")
                    {
                        data = getCCDV_DTG_date((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    }
                    else
                    {
                        data = getCCDV_DTG((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    }                    
                    break;
                case 2: //Trễ giờ
                    if (rq.targets[0].data.graph == "line")
                    {
                        data = getCCDV_TTG_date((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    }
                    else
                    {
                        data = getCCDV_TTG((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    }
                    break;
                case 3: //Tỷ lệ
                    if (rq.targets[0].data.graph == "line")
                    {
                        data = getCCDV_TL_date((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    }
                    else if(rq.targets[0].data.graph == "pie")
                    {
                        data = getCCDV_DTG((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    } else
                    {
                        data = getCCDV_TL((int)rq.scopedVars.cable.value, (int)rq.scopedVars.unit.value, date.Item1, date.Item2);
                    }                  
                    break;
            }
            return data;
        }
    }
}
