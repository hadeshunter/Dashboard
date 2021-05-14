using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.RqGrafana;

namespace DashBoardService.server.customerSatisfaction.ServiceQuality
{
    public interface IServiceQuality
    {
        dynamic GetData_Dissatisfied_ServiceQuality(RqGrafana rq);
        dynamic GetData_Dissatisfied_ServiceQuality_Grafana(RqGrafana rq);
    }
}
