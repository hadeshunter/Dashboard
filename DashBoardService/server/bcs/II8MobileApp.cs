using ClassModel.connnection.reponsitory;
using ClassModel.model.bsc;
using ClassModel.model.RqGrafana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.bcs
{
    public interface II8MobileApp:IReponsitory<I8MobileApp>
    {
        dynamic executeI8MobileApp(RqGrafana rq);
        dynamic getI8MobileApp(RqGrafana rq);
    }
}
