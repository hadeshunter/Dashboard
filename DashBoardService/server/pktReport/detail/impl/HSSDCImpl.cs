using ClassModel.model.cable;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.common;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.pktReport.detail.impl
{
    public class HSSDCImpl : IHSSDC
    {
        private ICommon m_common;
        private IConfiguration m_configuration;
        private IPktReport m_pktReport;
        public HSSDCImpl(ICommon common, IConfiguration configuration, IPktReport pktReport)
        {
            m_configuration = configuration;
            m_common = common;
            m_pktReport = pktReport;
        }

        private dynamic getHSSDCD(RqGrafana rq)
        {
            List<UsingPerformance> data = new List<UsingPerformance>();
            data = m_pktReport.executeHSSDCD(rq);
            return data;
        }

        private dynamic getHSSDCQ(RqGrafana rq)
        {
            List<UsingPerformance> data = new List<UsingPerformance>();
            data = m_pktReport.executeHSSDCQ(rq);
            return data;
        }

        private dynamic getCapGoc_DSD(List<UsingPerformance> list, DateTime dngay)
        {
            List<dynamic> data = new List<dynamic>();
            foreach (UsingPerformance item in list)
            {
                List<dynamic> points = new List<dynamic>();
                points.Add(new List<dynamic> { item.dungluong_capgoc_sudung, m_common.convertDayToUnix(01, dngay.Month, dngay.Year) });
                data.Add(new { target = item.ttvt, datapoints = points });
            }
            return data;
        }

        private dynamic getCapGoc_TL(List<UsingPerformance> list, DateTime dngay)
        {
            List<dynamic> data = new List<dynamic>();
            foreach (UsingPerformance item in list)
            {
                List<dynamic> points = new List<dynamic>();
                points.Add(new List<dynamic> { item.hssd_cap_goc * 100, m_common.convertDayToUnix(01, dngay.Month, dngay.Year) });
                data.Add(new { target = item.ttvt, datapoints = points });
            }
            return data;
        }

        private dynamic getCapPhoi_DSD(List<UsingPerformance> list, DateTime dngay)
        {
            List<dynamic> data = new List<dynamic>();
            foreach (UsingPerformance item in list)
            {
                List<dynamic> points = new List<dynamic>();
                points.Add(new List<dynamic> { item.dungluong_capphoi_sudung, m_common.convertDayToUnix(01, dngay.Month, dngay.Year) });
                data.Add(new { target = item.ttvt, datapoints = points });
            }
            return data;
        }

        private dynamic getCapPhoi_TL(List<UsingPerformance> list, DateTime dngay)
        {
            List<dynamic> data = new List<dynamic>();
            foreach (UsingPerformance item in list)
            {
                List<dynamic> points = new List<dynamic>();
                points.Add(new List<dynamic> { item.hssd_cap_phoi * 100, m_common.convertDayToUnix(01, dngay.Month, dngay.Year) });
                data.Add(new { target = item.ttvt, datapoints = points });
            }
            return data;
        }

        private dynamic getPON_DSD(List<UsingPerformance> list, DateTime dngay)
        {
            List<dynamic> data = new List<dynamic>();
            foreach (UsingPerformance item in list)
            {
                List<dynamic> points = new List<dynamic>();
                points.Add(new List<dynamic> { item.dungluong_pon_sudung, m_common.convertDayToUnix(01, dngay.Month, dngay.Year) });
                data.Add(new { target = item.ttvt, datapoints = points });
            }
            return data;
        }

        private dynamic getPON_TL(List<UsingPerformance> list, DateTime dngay)
        {
            List<dynamic> data = new List<dynamic>();
            foreach (UsingPerformance item in list)
            {
                List<dynamic> points = new List<dynamic>();
                points.Add(new List<dynamic> { item.hssd_pon * 100, m_common.convertDayToUnix(01, dngay.Month, dngay.Year) });
                data.Add(new { target = item.ttvt, datapoints = points });
            }
            return data;
        }

        public dynamic getHSSDC(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            List<UsingPerformance> list = new List<UsingPerformance>();
            DateTime dngay = Convert.ToDateTime(rq.range.to);
            switch ((int)rq.scopedVars.cable.value)
            {
                case -1: //All
                    list = new List<UsingPerformance>();
                    List<UsingPerformance> list1 = getHSSDCD(rq);
                    List<UsingPerformance> list2 = getHSSDCQ(rq);
                    List<Unit> listTTVT = m_common.getListTTVT();
                    foreach (Unit unit in listTTVT)
                    {
                        var item1 = list1.FirstOrDefault(i => i.ttvt_id == unit.donvi_id);
                        var item2 = list2.FirstOrDefault(i => i.ttvt_id == unit.donvi_id);
                        UsingPerformance combine = new UsingPerformance();
                        combine.ttvt_id = unit.donvi_id;
                        combine.ttvt = unit.ten_dv;
                        combine.dungluong_capgoc = item1.dungluong_capgoc + item2.dungluong_capgoc;
                        combine.dungluong_capgoc_sudung = item1.dungluong_capgoc_sudung + item2.dungluong_capgoc_sudung;
                        combine.dungluong_capgoc_trong = item1.dungluong_capgoc_trong + item2.dungluong_capgoc_trong;
                        combine.hssd_cap_goc = Math.Round((double)(item1.dungluong_capgoc_sudung + item2.dungluong_capgoc_sudung) / (item1.dungluong_capgoc + item2.dungluong_capgoc),4);
                        combine.dungluong_capphoi = item1.dungluong_capphoi + item2.dungluong_capphoi;
                        combine.dungluong_capphoi_sudung = item1.dungluong_capphoi_sudung + item2.dungluong_capphoi_sudung;
                        combine.dungluong_capphoi_trong = item1.dungluong_capphoi_trong + item2.dungluong_capphoi_trong;
                        combine.hssd_cap_phoi = Math.Round((double)(item1.dungluong_capphoi_sudung + item2.dungluong_capphoi_sudung) / (item1.dungluong_capphoi + item2.dungluong_capphoi), 4);
                        combine.dungluong_pon = item1.dungluong_pon + item2.dungluong_pon;
                        combine.dungluong_pon_sudung = item1.dungluong_pon_sudung + item2.dungluong_pon_sudung;
                        combine.hssd_pon = Math.Round((double)(item1.dungluong_pon_sudung + item2.dungluong_pon_sudung) / (item1.dungluong_pon + item2.dungluong_pon), 4);
                        list.Add(combine);
                    }
                    break;
                case 0: //HSSDCD
                    list = new List<UsingPerformance>();
                    list = getHSSDCD(rq);
                    break;
                case 1: //HSSDCQ
                    list = new List<UsingPerformance>();
                    list = getHSSDCQ(rq);
                    break;
            }

            if (rq.targets[0].data.graph == "pie")
            {
                switch ((int)rq.scopedVars.type.value)
                {
                    case 7: //Cap goc
                        data = getCapGoc_DSD(list, dngay);
                        break;
                    case 8: //Cap phoi
                        data = getCapPhoi_DSD(list, dngay);
                        break;
                    case 9: //PON
                        data = getPON_DSD(list, dngay);
                        break;
                }
            }
            else
            {
                switch ((int)rq.scopedVars.type.value)
                {
                    case 7: //Cap goc
                        data = getCapGoc_TL(list, dngay);
                        break;
                    case 8: //Cap phoi
                        data = getCapPhoi_TL(list, dngay);
                        break;
                    case 9: //PON
                        data = getPON_TL(list, dngay);
                        break;
                }
            }
            
            return data;
        }
    }
}
