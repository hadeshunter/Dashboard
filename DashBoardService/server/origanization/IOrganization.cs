using ClassModel.connnection.reponsitory;
using ClassModel.model.bsc;
using ClassModel.model.organization;
using ClassModel.model.unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.origanization
{
    public interface IOrganization:IReponsitory<Organization>
    {
        dynamic execureOrganization();
        dynamic getAllDoiVT();
        dynamic getAllCenter(UnitRequest rq);
        dynamic getAllTTVT();
        dynamic getAllTTKD();
    }
}
