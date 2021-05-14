using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.RqGrafana;

namespace DashBoardService.server.customerSatisfaction.ServeQuality
{
    public interface IServeQuality
    {
        dynamic GetData_Dissatisfied_ServeQuality(RqGrafana rq);
        dynamic GetData_Dissatisfied_ServeQuality_Grafana(RqGrafana rq);
    }

}
