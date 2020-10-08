using ClassModel.connnection.reponsitory;
using ClassModel.model.bsc;
using ClassModel.model.RqGrafana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.bcs
{
    public interface IDetail_go:IReponsitory<Detail_go>
    {
        dynamic execureDetailgo(RqGrafana rq);
    }
}
