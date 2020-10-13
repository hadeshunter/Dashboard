using ClassModel.connnection.reponsitory;
using ClassModel.model.bsc;
using ClassModel.model.organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.origanization
{
    public interface IOrganization:IReponsitory<Organization>
    {
        dynamic execureOrganization();
        dynamic getAllUnit();
        dynamic getAllCenter();
    }
}
