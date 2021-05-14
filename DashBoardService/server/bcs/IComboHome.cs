using ClassModel.connnection.reponsitory;
using ClassModel.model.bsc;
using ClassModel.model.RqGrafana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.bcs
{
    public interface IComboHome: IReponsitory<ComboHome>
    {
        dynamic executeComboHome(RqGrafana rq);
        dynamic getComboHome(RqGrafana rq);
    }
}
