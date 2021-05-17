using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.RqGrafana;

namespace DashBoardService.server.installationInventoryFiber
{
    public interface IInstallationInventoryFiber
    {
        dynamic GetInstallationInventoryFiber(string month);
        dynamic GetInstallationInventoryFiberByDate(string month);
        dynamic CreateTable(string month);
        dynamic InsertDataToTable(string month);
        dynamic GetDataForGrafana(RqGrafana rq);

    }
}
