using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.convertdata.tonLDFiber
{
    public interface ITonLapdatFiber
    {
        dynamic toDataConvert(string startime, string endtime);
    }
}
