using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.convertdata.ccdv
{
    public interface ICcdvDungThoiGian
    {
        dynamic toDataConvert(string startime, string endtime);
    }
}
