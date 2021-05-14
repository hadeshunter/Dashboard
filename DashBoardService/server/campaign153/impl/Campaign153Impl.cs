using ClassModel.connnection.sql;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.bcs;
using DashBoardService.server.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.campaign153.impl
{
    public class Campaign153Impl : ICampaign153
    {
        private ICommon m_common;
        private IDetail_lapmoi m_detail_lapmoi;
        private IDetail_go m_detail_go;
        private IDetailDataReal m_detailDataReal;
        public Campaign153Impl(ICommon common, IDetail_lapmoi detail_lapmoi, IDetail_go detail_go, IDetailDataReal detailDataReal)
        {
            m_common = common;
            m_detail_lapmoi = detail_lapmoi;
            m_detail_go = detail_go;
            m_detailDataReal = detailDataReal;
        }

        public dynamic getActualPercent(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            dynamic lst_donviId;
            if ((string)rq.targets[0].data.center == "ttkd")
            {
                lst_donviId = rq.scopedVars.ttkd.value;
            }
            else
            {
                lst_donviId = rq.scopedVars.ttvt.value;
            }
            List<dynamic> total_date = new List<dynamic>();
            List<dynamic> total = new List<dynamic>();
            List<dynamic> total_timeseries = new List<dynamic>();
            List<Unit> listUnit = m_common.getListCenter(rq);
            List<dynamic> configListByDate = m_detailDataReal.getConfigByDate(rq);
            List<dynamic> removeListByDate = m_detailDataReal.getRemoveByDate(rq);
            foreach (var unit in listUnit)
            {
                List<dynamic> data = new List<dynamic>();
                List<dynamic> lst_config = new List<dynamic>(configListByDate.FindAll(r => r.donvi_id == unit.donvi_id));
                List<dynamic> lst_remove = new List<dynamic>(removeListByDate.FindAll(r => r.donvi_id == unit.donvi_id));
                List<dynamic> lst_config_date = new List<dynamic>(lst_config.Select(lg => lg.unix_date));
                List<dynamic> lst_remove_date = new List<dynamic>(lst_remove.Select(lg => lg.unix_date));
                List<dynamic> combine_date = lst_config_date.Union(lst_remove_date).ToList();
                foreach (var date in combine_date)
                {
                    var config_point = lst_config.Where(r => r.unix_date == date).FirstOrDefault();
                    var remove_point = lst_remove.Where(r => r.unix_date == date).FirstOrDefault();
                    int sl_lapdat = 0;
                    int sl_huy = 0;
                    if (config_point != null)
                    {
                        sl_lapdat = config_point.sl_lapdat;
                    }
                    if (remove_point != null)
                    {
                        sl_huy = remove_point.sl_huy;
                    }
                    data.Add(new { unit.donvi_id, unit.ten_dv, sl_lapdat, sl_huy, unix_date = date });
                }
                total_date.Add(combine_date);
                total.Add(data);
            }
            total_date = m_common.getFullList(total_date);
            total = m_detailDataReal.getEqualLengthActual(total, total_date);
            total_timeseries = m_detailDataReal.getTimeseriesActual(total);
            if (rq.targets[0].data.graph == "pie_chart")
            {
                response = total_timeseries.FindAll(r => r.datapoints[0][0] > 0);
            }
            else if (rq.targets[0].data.graph == "column_chart")
            {
                response = total_timeseries;
            }
            else
            {
                List<dynamic> avr_points = m_detailDataReal.getAveragePercent(total);
                List<dynamic> max_points = m_common.getMaxPoints(total_timeseries);
                List<dynamic> min_points = m_common.getMinPoints(total_timeseries);
                response.Add(new { target = "GT MAX", datapoints = max_points });
                response.Add(new { target = "GT trung bình", datapoints = avr_points });
                response.Add(new { target = "GT MIN", datapoints = min_points });
                if ((int)lst_donviId != 1)
                {
                    var unit = listUnit.Where(r => r.donvi_id == (int)lst_donviId).FirstOrDefault();
                    var unit_data = total_timeseries.Where(r => r.target == unit.ten_dv).FirstOrDefault();
                    response.Add(unit_data);
                };
            }
            return response;
        }

        public dynamic getDetailNew(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            dynamic lst_donviId;
            if ((string)rq.targets[0].data.center == "ttkd")
            {
                lst_donviId = rq.scopedVars.ttkd.value;
            }
            else
            {
                lst_donviId = rq.scopedVars.ttvt.value;
            }

            List<dynamic> total = new List<dynamic>();
            List<dynamic> total_date = new List<dynamic>();
            List<dynamic> total_timeseries = new List<dynamic>();
            List<Unit> listUnit = m_common.getListCenter(rq);
            List<dynamic> newListByDate = m_detail_lapmoi.getNewByDate(rq);
            foreach (var unit in listUnit)
            {
                List<dynamic> lst_new = new List<dynamic>(newListByDate.FindAll(r => r.donvi_id == unit.donvi_id));
                List<dynamic> lst_new_date = new List<dynamic>(lst_new.Select(lg => lg.unix_date));
                total_date.Add(lst_new_date);
                total.Add(lst_new);
            }
            total_date = m_common.getFullList(total_date);
            total = m_detail_lapmoi.getEqualLengthNew(listUnit, total, total_date);
            total_timeseries = m_detail_lapmoi.getTimeseriesNew(total);
            if (rq.targets[0].data.graph == "pie_chart" || rq.targets[0].data.graph == "column_chart")
            {
                response = total_timeseries;
            }
            else
            {
                List<dynamic> max_points = m_common.getMaxPoints(total_timeseries);
                List<dynamic> min_points = m_common.getMinPoints(total_timeseries);
                List<dynamic> avr_points = m_common.getAveragePoints(total_timeseries);
                response.Add(new { target = "GT MAX", datapoints = max_points });
                response.Add(new { target = "GT trung bình", datapoints = avr_points });
                response.Add(new { target = "GT MIN", datapoints = min_points });
                if ((int)lst_donviId != 1)
                {
                    var unit = listUnit.Where(r => r.donvi_id == (int)lst_donviId).FirstOrDefault();
                    var unit_data = total_timeseries.Where(r => r.target == unit.ten_dv).FirstOrDefault();
                    response.Add(unit_data);
                };
            }
            return response;
        }

        public dynamic getDetailConfig(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            dynamic lst_donviId;
            if ((string)rq.targets[0].data.center == "ttkd")
            {
                lst_donviId = rq.scopedVars.ttkd.value;
            }
            else
            {
                lst_donviId = rq.scopedVars.ttvt.value;
            }

            List<dynamic> total = new List<dynamic>();
            List<dynamic> total_date = new List<dynamic>();
            List<dynamic> total_timeseries = new List<dynamic>();
            List<Unit> listUnit = m_common.getListCenter(rq);
            List<dynamic> configListByDate = m_detail_lapmoi.getConfigByDate(rq);
            foreach (var unit in listUnit)
            {
                List<dynamic> lst_config = new List<dynamic>(configListByDate.FindAll(r => r.donvi_id == unit.donvi_id));
                List<dynamic> lst_config_date = new List<dynamic>(lst_config.Select(lg => lg.unix_date));
                total_date.Add(lst_config_date);
                total.Add(lst_config);
            }
            total_date = m_common.getFullList(total_date);
            total = m_detail_lapmoi.getEqualLengthConfig(listUnit, total, total_date);
            total_timeseries = m_detail_lapmoi.getTimeseriesConfig(total);
            if (rq.targets[0].data.graph == "pie_chart" || rq.targets[0].data.graph == "column_chart")
            {
                response = total_timeseries;
            }
            else
            {
                List<dynamic> max_points = m_common.getMaxPoints(total_timeseries);
                List<dynamic> min_points = m_common.getMinPoints(total_timeseries);
                List<dynamic> avr_points = m_common.getAveragePoints(total_timeseries);
                response.Add(new { target = "GT MAX", datapoints = max_points });
                response.Add(new { target = "GT trung bình", datapoints = avr_points });
                response.Add(new { target = "GT MIN", datapoints = min_points });
                if ((int)lst_donviId != 1)
                {
                    var unit = listUnit.Where(r => r.donvi_id == (int)lst_donviId).FirstOrDefault();
                    var unit_data = total_timeseries.Where(r => r.target == unit.ten_dv).FirstOrDefault();
                    response.Add(unit_data);
                };
            }
            return response;
        }

        public dynamic getDetailRemove(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            dynamic lst_donviId;
            if ((string)rq.targets[0].data.center == "ttkd")
            {
                lst_donviId = rq.scopedVars.ttkd.value;
            }
            else
            {
                lst_donviId = rq.scopedVars.ttvt.value;
            }
            List<dynamic> total = new List<dynamic>();
            List<dynamic> total_date = new List<dynamic>();
            List<dynamic> total_timeseries = new List<dynamic>();
            List<Unit> listUnit = m_common.getListCenter(rq);
            List<dynamic> removeListByDate = m_detail_go.getRemoveByDate(rq);
            foreach (var unit in listUnit)
            {
                List<dynamic> lst_remove = new List<dynamic>(removeListByDate.FindAll(r => r.donvi_id == unit.donvi_id));
                if (lst_remove.Count() > 0)
                {
                    List<dynamic> lst_remove_date = new List<dynamic>(lst_remove.Select(lg => lg.unix_date));
                    total_date.Add(lst_remove_date);
                    total.Add(lst_remove);
                } else
                {
                    List<dynamic> temp = new List<dynamic> ();
                    temp.Add(new { unit.donvi_id, sl_huy = 0, removeListByDate[0].unix_date });
                    total.Add(temp);
                }
                
            }
            total_date = m_common.getFullList(total_date);
            total = m_detail_go.getEqualLengthRemove(listUnit, total, total_date);
            total_timeseries = m_detail_go.getTimeseriesRemove(total);
            if (rq.targets[0].data.graph == "pie_chart" || rq.targets[0].data.graph == "column_chart")
            {
                response = total_timeseries;
            }
            else
            {
                List<dynamic> max_points = m_common.getMaxPoints(total_timeseries);
                List<dynamic> min_points = m_common.getMinPoints(total_timeseries);
                List<dynamic> avr_points = m_common.getAveragePoints(total_timeseries);
                response.Add(new { target = "GT MAX", datapoints = max_points });
                response.Add(new { target = "GT trung bình", datapoints = avr_points });
                response.Add(new { target = "GT MIN", datapoints = min_points });
                if ((int)lst_donviId != 1)
                {
                    var unit = listUnit.Where(r => r.donvi_id == (int)lst_donviId).FirstOrDefault();
                    var unit_data = total_timeseries.Where(r => r.target == unit.ten_dv).FirstOrDefault();
                    response.Add(unit_data);
                };
            }
            return response;
        }

        public dynamic getStaticCampaign153(RqGrafana rq)
        {
            List<dynamic> response = new List<dynamic>();
            switch ((int)rq.scopedVars.type.value)
            {
                case 1: //Thuc tang
                    response = getActualPercent(rq);
                    break;
                case 2: //Lap moi
                    response = getDetailNew(rq);
                    break;
                case 3: //Huy
                    response = getDetailRemove(rq);
                    break;
                case 4: //Lap dat
                    response = getDetailConfig(rq);
                    break;
            }
            return response;
        }
    }
}
