using ClassModel.model.RqGrafana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.pktReport.detail
{
    public interface IXLSC
    {
        dynamic getXLSC(RqGrafana rq);
        dynamic getXLSC_TL(int cable, int unit, string startime, string endtime);
    }
}
