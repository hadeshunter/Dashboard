using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.RqGrafana;

namespace DashBoardService.server.ThoaitraPCT
{
    public interface IThoaitraPCT
    {
        dynamic GetThoaitraPCTDate(RqGrafana rq);
        dynamic GetThoaitraPCT(string month);
        dynamic CreateTableCCDV_GBDB(string month);
        dynamic CreateTableThoaitra(string month);
        dynamic InsertDataToTableThoaitra(string month);
        dynamic InsertDataToTableCCDV_GBDB(string month);
        dynamic GetDataForGrafana(RqGrafana rq);

    }
}
