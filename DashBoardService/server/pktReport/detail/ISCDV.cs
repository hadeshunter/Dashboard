using ClassModel.model.RqGrafana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.pktReport.detail
{
    public interface ISCDV
    {
        dynamic getSCDV(RqGrafana rq);
        dynamic getSCDV_DTG(int cable, int unit, string startime, string endtime);
        dynamic getSCDV_TTG(int cable, int unit, string startime, string endtime);
        dynamic getSCDV_TL(int cable, int unit, string startime, string endtime);
    }
}
