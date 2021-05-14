using ClassModel.connnection.reponsitory;
using ClassModel.model.bsc;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using System.Collections.Generic;

namespace DashBoardService.server.bcs
{
    public interface IDetail_go:IReponsitory<Detail_go>
    {
        dynamic execureDetailgo(RqGrafana rq);
        dynamic getRemoveByDate(RqGrafana rq);
        dynamic getEqualLengthRemove(List<Unit> listUnit, List<dynamic> list, List<dynamic> list_date);
        dynamic getTimeseriesRemove(List<dynamic> list);
        dynamic getRemove(RqGrafana rq);
    }
}
