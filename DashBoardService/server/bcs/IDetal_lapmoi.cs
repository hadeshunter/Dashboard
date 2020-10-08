using ClassModel.connnection.reponsitory;
using ClassModel.model.bsc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.bcs
{
    public interface IDetal_lapmoi:IReponsitory<Detal_lapmoi>
    {
        dynamic execureDetailLapmoi(BscRequest bscRequest);
    }
}
