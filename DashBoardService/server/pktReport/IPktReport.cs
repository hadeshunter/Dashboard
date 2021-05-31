using ClassModel.connnection.reponsitory;
using ClassModel.model.pktReport;
using ClassModel.model.RqGrafana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.pktReport
{
    public interface IPktReport
    {
        dynamic executeCCDVDTG(RqGrafana rq);
        dynamic getCCDVDTG(RqGrafana rq);
        dynamic executeSCDVDTG(RqGrafana rq);
        dynamic getSCDVDTG(RqGrafana rq);
        dynamic executeTLDUTGXLSC(RqGrafana rq);
        dynamic getTLDUTGXLSC(RqGrafana rq);
        dynamic executeHSSDCD(RqGrafana rq);
        dynamic getHSSDCD(RqGrafana rq);
        dynamic executeHSSDCQ(RqGrafana rq);
        dynamic getHSSDCQ(RqGrafana rq);
        dynamic executeMegaVNN(string month);
        dynamic getMegaVNN(RqGrafana rq);
        dynamic executeLuykeLapgoFiberDate(RqGrafana rq);
        dynamic executeLuykeLapgoFiber(RqGrafana rq);
        dynamic getLuykeLapgoFiber(RqGrafana rq);
        dynamic executeMLLBTS(RqGrafana rq);
        dynamic getMLLBTS(RqGrafana rq);
        dynamic executeMLLNN(RqGrafana rq);
        dynamic getMLLNN(RqGrafana rq);
        dynamic executeTKMLLBTS(RqGrafana rq, string loai_mang);
        dynamic executeTKMLLBTS_NN(RqGrafana rq, string loai_mang);
        dynamic executePAKHDD(RqGrafana rq);
        dynamic getPAKHDD(RqGrafana rq);
    }
}
