using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.BTS;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.common;
using Microsoft.Extensions.Configuration;

namespace DashBoardService.server.pktReport.detail.impl
{
    public class MLLBTSImpl : IMLLBTS
    {
        private ICommon m_common;
        private IConfiguration m_configuration;
        private IPktReport m_pktReport;
        public MLLBTSImpl(ICommon m_common, IConfiguration m_configuration, IPktReport pktReport)
        {
            this.m_common = m_common;
            this.m_configuration = m_configuration;
            m_pktReport = pktReport;
        }
        
        public dynamic getMLLBTS_NN(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            string loai_mang = "2G";
            switch ((int)rq.scopedVars.type.value)
            {
                case 1014: //2G
                    loai_mang = "2G";
                    break;
                case 1015: //3G
                    loai_mang = "3G";
                    break;
                case 1016: //4G
                    loai_mang = "4G";
                    break;
            }
            List<tk_mll_bts_nn> list_oracle = m_pktReport.executeTKMLLBTS_NN(rq, loai_mang);
            var list_loi = list_oracle
                .GroupBy(l => new { l.ten_loi })
                .Select(lg =>
                    new
                    {
                        lg.Key.ten_loi
                    });
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                List<dynamic> list_nn = new List<dynamic>();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_mllbts = list_oracle
                        .OrderBy(ele => (ele.donvi_id, ele.thang_tk))
                        .Select(lg =>
                            new
                            {
                                lg.donvi_id,
                                lg.ten_loi,
                                lg.sl_loi,
                                lg.thang_tk,
                                unix_date = m_common.convertDayToUnix(1, lg.thang_tk.Month, lg.thang_tk.Year)
                            })
                         .Where(item => item.donvi_id == ttvt.donvi_id);
                    var tmp = list_mllbts
                        .OrderBy(ele => (ele.ten_loi, ele.thang_tk))
                        .GroupBy(l => new { l.unix_date, l.ten_loi })
                        .Select(lg =>
                            new
                            {
                                lg.Key.ten_loi,
                                sl_loi = lg.Sum(item => item.sl_loi),
                                lg.Key.unix_date
                            });
                    list_nn = new List<dynamic>(tmp);
                }

                foreach (var loi in list_loi)
                {
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_nn.FindAll(item => item.ten_loi == loi.ten_loi))
                    {
                        points.Add(new List<dynamic> { item.sl_loi, item.unix_date });
                    }

                    data.Add(new { target = loi.ten_loi, datapoints = points });
                }
                
            }
            else
            {
                List<dynamic> list_nn = new List<dynamic>();
                int donvi_id = (int)rq.scopedVars.unit.value;
                string ten_dv = (string)rq.scopedVars.unit.text;
                var list_mllbts = list_oracle
                        .OrderBy(ele => (ele.donvi_id, ele.thang_tk))
                        .Select(lg =>
                            new
                            {
                                lg.donvi_id,
                                lg.ten_loi,
                                lg.sl_loi,
                                lg.thang_tk,
                                unix_date = m_common.convertDayToUnix(1, lg.thang_tk.Month, lg.thang_tk.Year)
                            })
                         .Where(item => item.donvi_id == donvi_id);
                var tmp = list_mllbts
                        .OrderBy(ele => (ele.ten_loi, ele.thang_tk))
                        .GroupBy(l => new { l.unix_date, l.ten_loi })
                        .Select(lg =>
                            new
                            {
                                lg.Key.ten_loi,
                                sl_loi = lg.Sum(item => item.sl_loi),
                                lg.Key.unix_date
                            });
                list_nn = new List<dynamic>(tmp);
                foreach (var loi in list_loi)
                {
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_nn.FindAll(item => item.ten_loi == loi.ten_loi))
                    {
                        points.Add(new List<dynamic> { item.sl_loi, item.unix_date });
                    }

                    data.Add(new { target = loi.ten_loi, datapoints = points });
                }
            }
            return data;
        }

        public dynamic getMLLBTS_TG(RqGrafana rq)
        {
            List<dynamic> data = new List<dynamic>();
            string loai_mang = "2G";
            switch ((int)rq.scopedVars.type.value)
            {
                case 1014: //2G
                    loai_mang = "2G";
                    break;
                case 1015: //3G
                    loai_mang = "3G";
                    break;
                case 1016: //4G
                    loai_mang = "4G";
                    break;
            }
            List<tk_mll_bts> list_oracle = m_pktReport.executeTKMLLBTS(rq, loai_mang);
            if ((int)rq.scopedVars.unit.value == 0)
            {
                List<Unit> listTTVT = m_common.getListTTVT();
                List<dynamic> list_nn = new List<dynamic>();
                foreach (Unit ttvt in listTTVT)
                {
                    var list_mllbts_tg = list_oracle
                        .OrderBy(ele => (ele.donvi_id, ele.thang_tk))
                        .Select(lg =>
                            new
                            {
                                lg.donvi_id,
                                lg.tb_tg_mll,
                                unix_date = m_common.convertDayToUnix(1, lg.thang_tk.Month, lg.thang_tk.Year)
                            })
                         .Where(item => item.donvi_id == ttvt.donvi_id);
                    List<dynamic> points = new List<dynamic>();
                    foreach (var item in list_mllbts_tg)
                    {
                        points.Add(new List<dynamic> { item.tb_tg_mll, item.unix_date });
                    }

                    data.Add(new { target = ttvt.ten_dv, datapoints = points });
                }
            }
            else
            {
                int donvi_id = (int)rq.scopedVars.unit.value;
                string ten_dv = (string)rq.scopedVars.unit.text;
                var list_mllbts_tg = list_oracle
                        .OrderBy(ele => (ele.donvi_id, ele.thang_tk))
                        .Select(lg =>
                            new
                            {
                                lg.donvi_id,
                                lg.tb_tg_mll,
                                unix_date = m_common.convertDayToUnix(1, lg.thang_tk.Month, lg.thang_tk.Year)
                            })
                         .Where(item => item.donvi_id == donvi_id);

                List<dynamic> points = new List<dynamic>();
                foreach (var item in list_mllbts_tg)
                {
                    points.Add(new List<dynamic> { item.tb_tg_mll, item.unix_date });
                }

                data.Add(new { target = ten_dv, datapoints = points });
            }
            return data;
        }
    }
}
