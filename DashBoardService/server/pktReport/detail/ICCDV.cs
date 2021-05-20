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
    }
}
