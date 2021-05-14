using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardService.server.convertdata.tk_khl
{
    public interface ITK_KhongHaiLong_CLPV
    {
        dynamic toDataConvert(string startime, string endtime);
    }
}

