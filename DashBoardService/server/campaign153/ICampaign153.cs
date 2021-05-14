using ClassModel.model.RqGrafana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.campaign153
{
    public interface ICampaign153
    {
        dynamic getActualPercent(RqGrafana rq);
        dynamic getDetailNew(RqGrafana rq);
        dynamic getDetailConfig(RqGrafana rq);
        dynamic getDetailRemove(RqGrafana rq);
        dynamic getStaticCampaign153(RqGrafana rq);
    }
}
