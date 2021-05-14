using ClassModel.connnection.reponsitory;
using ClassModel.model.bsc;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.bcs
{
    public interface IDetail_lapmoi:IReponsitory<Detail_lapmoi>
    {
        dynamic executeDetailLapmoi(RqGrafana rq);
        dynamic getNewByDate(RqGrafana rq);
        dynamic getPTMByDate(RqGrafana rq);
        dynamic getComeBackByDate(RqGrafana rq);
        dynamic getConfigByDate(RqGrafana rq);
        dynamic getEqualLengthNew(List<Unit> listUnit, List<dynamic> list, List<dynamic> list_date);
        dynamic getEqualLengthConfig(List<Unit> listUnit, List<dynamic> list, List<dynamic> list_date);
        dynamic getTimeseriesNew(List<dynamic> list);
        dynamic getTimeseriesConfig(List<dynamic> list);
        dynamic getNew(RqGrafana rq);
    }
}
