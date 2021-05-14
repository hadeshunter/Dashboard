using ClassModel.convertdata.ccdv;
using ClassModel.model.RqGrafana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.pktReport.detail
{
    public interface ICCDV
    {
        dynamic getCCDV(RqGrafana rq);
        dynamic getCCDV_DTG(int cable, int unit, string startime, string endtime);
        dynamic getCCDV_TTG(int cable, int unit, string startime, string endtime);
        dynamic getCCDV_TL(int cable, int unit, string startime, string endtime);
        dynamic getCCDV_oracle(int cable, string startime, string endtime);
    }
}
