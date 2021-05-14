using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.RqGrafana;

namespace DashBoardService.server.ThoaitraNLML
{
    public interface IThoaitraNLML
    {
        dynamic GetThoaitraNLML(string month);
        dynamic CreateTable(string month);
        dynamic InsertDataToTable(string month);
        dynamic GetDataForGrafana(RqGrafana rq);
    }
}
