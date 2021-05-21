using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.installationInventoryFiber;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.common;
using DashBoardService.server.convertdata.tonLDFiber;
using DashBoardService.server.installationInventoryFiber;
using Microsoft.Extensions.Configuration;

namespace DashBoardService.server.pktReport.detail.impl
{
    public class TonLDFiberImpl : ITonLDFiber
    {
        private ICommon m_common;
        private IConfiguration m_configuration;
        private IInstallationInventoryFiber m_tonLDFiber;
        private ITonLapdatFiber m_convert_data;
        public TonLDFiberImpl(ICommon m_common, IConfiguration m_configuration, IInstallationInventoryFiber tonLDFiber, ITonLapdatFiber convert_data)
        {
            this.m_common = m_common;
            this.m_configuration = m_configuration;
            m_tonLDFiber = tonLDFiber;
            m_convert_data = convert_data;
        }

        private dynamic getTonLDFiberDate_Oracle(RqGrafana rq)
        {
            var (startime, endtime) = m_common.convertToString(rq);
            var checkDate = m_convert_data.toDataConvert(startime, endtime);
            List<installationInventoryFiberModel> data = new List<installationInventoryFiberModel>();
            //data = m_tonLDFiber.GetInstallationInventoryFiberByDate(rq);
            if (checkDate == true)
            {
                string connStr = m_configuration.GetConnectionString("DefaultConnection");
                var dt = new DataTable();
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    //@"select * from [dbo].Ccdv_Dung_Tg where ngaycn_bbbg between CONVERT(date,'" + startime + "',103) and CONVERT(date,'" + endtime + "',103)"
                    using (var cmd = new SqlCommand(@"select * from [dbo].TonLDFiber where ngay_yc between CONVERT(DATETIME,'" + startime + "',103) and CONVERT(DATETIME,'" + endtime + "',103)", conn))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            installationInventoryFiberModel row = new installationInventoryFiberModel();
                            row.donvi_id = Convert.ToInt32(rdr["donvi_id"]);
                            row.donvi_cha_id = Convert.ToInt32(rdr["donvi_cha_id"]);
                            row.ten_dv = rdr["ten_dv"].ToString();
                            row.ten_dv_cha = rdr["ten_dv_cha"].ToString();
                            row.ngay_yc = (DateTime)rdr["ngay_yc"];
                            row.tong_fiber = Convert.ToInt32(rdr["tong_fiber"]);
                            row.ton_fiber = Convert.ToInt32(rdr["ton_fiber"]);
                            row.lapdat_fiber = Convert.ToInt32(rdr["lapdat_fiber"]);
                            row.tyle_ton = Convert.ToDouble(rdr["tyle_ton"]);
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

        private dynamic getTonLDFiber_SLTon_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<installationInventoryFiberModel> list = getTonLDFiberDate_Oracle(rq);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_sl_ton = list
                        .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_cha_id, l.ten_dv_cha })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_cha_id,
                                lg.Key.ten_dv_cha,
                                ton_fiber = lg.Sum(l => l.ton_fiber),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_cha_id == ttvt.donvi_id);
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
                List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == donvi_id);
                foreach (Unit doivt in listDoiVT)
                {
                    var list_sl_ton = list
                        .Where(item => item.donvi_id == doivt.donvi_id)
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                ton_fiber = lg.Sum(l => l.ton_fiber),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            });
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_sl_ton)
                    {
                        points.Add(new List<dynamic> { item.ton_fiber, item.unix_date });
                    }

                    data.Add(new { target = doivt.ten_dv, datapoints = points });
                }                
            }
            return data;
        }

        private dynamic getTonLDFiber_SLTon(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<installationInventoryFiberModel> list = getTonLDFiberDate_Oracle(rq);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_sl_ton = list
                        .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_cha_id, l.ten_dv_cha })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_cha_id,
                                lg.Key.ten_dv_cha,
                                ton_fiber = lg.Sum(l => l.ton_fiber),
                                unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_cha_id == ttvt.donvi_id);
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
                List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == donvi_id);
                foreach (Unit doivt in listDoiVT)
                {
                    var list_sl_ton = list
                        .Where(item => item.donvi_id == doivt.donvi_id)
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                ton_fiber = lg.Sum(l => l.ton_fiber),
                                unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            });
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_sl_ton)
                    {
                        points.Add(new List<dynamic> { item.ton_fiber, item.unix_date });
                    }

                    data.Add(new { target = doivt.ten_dv, datapoints = points });
                }
            }
            return data;
        }

        private dynamic getTonLDFiber_SLLD_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<installationInventoryFiberModel> list = getTonLDFiberDate_Oracle(rq);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_sl_ld = list
                        .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_cha_id, l.ten_dv_cha })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_cha_id,
                                lg.Key.ten_dv_cha,
                                lapdat_fiber = lg.Sum(l => l.lapdat_fiber),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_cha_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_sl_ld)
                    {
                        points.Add(new List<dynamic> { item.lapdat_fiber, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == donvi_id);
                foreach (Unit doivt in listDoiVT)
                {
                    var list_sl_ld = list
                        .Where(item => item.donvi_id == doivt.donvi_id)
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                lapdat_fiber = lg.Sum(l => l.lapdat_fiber),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            });
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_sl_ld)
                    {
                        points.Add(new List<dynamic> { item.lapdat_fiber, item.unix_date });
                    }

                    data.Add(new { target = doivt.ten_dv, datapoints = points });
                }
            }
            return data;
        }

        private dynamic getTonLDFiber_SLLD(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<installationInventoryFiberModel> list = getTonLDFiberDate_Oracle(rq);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_sl_ld = list
                        .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_cha_id, l.ten_dv_cha })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_cha_id,
                                lg.Key.ten_dv_cha,
                                lapdat_fiber = lg.Sum(l => l.lapdat_fiber),
                                unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_cha_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_sl_ld)
                    {
                        points.Add(new List<dynamic> { item.lapdat_fiber, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == donvi_id);
                foreach (Unit doivt in listDoiVT)
                {
                    var list_sl_ld = list
                        .Where(item => item.donvi_id == doivt.donvi_id)
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                lapdat_fiber = lg.Sum(l => l.lapdat_fiber),
                                unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            });
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_sl_ld)
                    {
                        points.Add(new List<dynamic> { item.lapdat_fiber, item.unix_date });
                    }

                    data.Add(new { target = doivt.ten_dv, datapoints = points });
                }
            }
            return data;
        }

        private dynamic getTonLDFiber_TL_date(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<installationInventoryFiberModel> list = getTonLDFiberDate_Oracle(rq);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_tyle_ton = list
                        .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_cha_id, l.ten_dv_cha })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_cha_id,
                                lg.Key.ten_dv_cha,
                                tyle_ton = Math.Round((double)lg.Sum(l => l.ton_fiber) * 100 / lg.Sum(l => l.tong_fiber), 4),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_cha_id == ttvt.donvi_id);
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
                List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == donvi_id);
                foreach (Unit doivt in listDoiVT)
                {
                    var list_tyle_ton = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Day, l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                tyle_ton = Math.Round((double)lg.Sum(l => l.ton_fiber) * 100 / lg.Sum(l => l.tong_fiber), 4),
                                unix_date = m_common.convertDayToUnix(lg.Key.Day, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == doivt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_tyle_ton)
                    {
                        points.Add(new List<dynamic> { item.tyle_ton, item.unix_date });
                    }

                    data.Add(new { target = doivt.ten_dv, datapoints = points });
                }                
            }
            return data;
        }

        private dynamic getTonLDFiber_TL(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            var (startime, endtime) = m_common.convertToString(rq);
            DateTime time = DateTime.ParseExact(startime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            List<installationInventoryFiberModel> list = getTonLDFiberDate_Oracle(rq);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_tyle_ton = list
                        .OrderBy(ele => (ele.donvi_cha_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_cha_id, l.ten_dv_cha })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_cha_id,
                                lg.Key.ten_dv_cha,
                                tyle_ton = Math.Round((double)lg.Sum(l => l.ton_fiber) * 100 / lg.Sum(l => l.tong_fiber), 4),
                                unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_cha_id == ttvt.donvi_id);
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
                List<Unit> listDoiVT = m_common.getListDoiVT().FindAll(item => item.donvi_cha_id == donvi_id);
                foreach (Unit doivt in listDoiVT)
                {
                    var list_tyle_ton = list
                        .OrderBy(ele => (ele.donvi_id, ele.ngay_yc))
                        .GroupBy(l => new { l.ngay_yc.Month, l.ngay_yc.Year, l.donvi_id, l.ten_dv })
                        .Select(lg =>
                            new
                            {
                                lg.Key.donvi_id,
                                lg.Key.ten_dv,
                                tyle_ton = Math.Round((double)lg.Sum(l => l.ton_fiber) * 100 / lg.Sum(l => l.tong_fiber), 4),
                                unix_date = time.Year == lg.Key.Year && time.Month == lg.Key.Month && time.Day != 1 ? m_common.convertDayToUnix(time.Day + 1, lg.Key.Month, lg.Key.Year) : m_common.convertDayToUnix(1, lg.Key.Month, lg.Key.Year)
                            })
                         .Where(item => item.donvi_id == doivt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_tyle_ton)
                    {
                        points.Add(new List<dynamic> { item.tyle_ton, item.unix_date });
                    }

                    data.Add(new { target = doivt.ten_dv, datapoints = points });
                }
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
                } else
                {
                    data = getTonLDFiber_TL(rq);
                }                
            }
            return data;
        }
    }
}
