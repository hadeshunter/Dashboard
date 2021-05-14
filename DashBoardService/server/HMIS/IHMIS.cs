using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.RqGrafana;

namespace DashBoardService.server.HMIS
{
    public interface IHMIS
    {
        dynamic executeHMIS(RqGrafana rq);
        dynamic getHMIS(RqGrafana rq);
    }
}
