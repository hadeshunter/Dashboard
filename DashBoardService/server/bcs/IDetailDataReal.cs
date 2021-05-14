using ClassModel.connnection.reponsitory;
using ClassModel.model.bsc;
using ClassModel.model.realIncrease;
using ClassModel.model.RqGrafana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.bcs
{
    public interface IDetailDataReal : IReponsitory<DetailDataReal>
    {
        dynamic getConfigByDate(RqGrafana rq);
        dynamic getRemoveByDate(RqGrafana rq);
        dynamic getActualPercent(RqGrafana rq);
        dynamic getEqualLengthActual(List<dynamic> list, List<dynamic> list_date);
        dynamic getAveragePercent(List<dynamic> list);
        dynamic getTimeseriesActual(List<dynamic> list);
    }
}
