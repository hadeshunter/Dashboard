using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerConvert.service.i8mobileapp
{
    public interface II8MobileAcceptance
    {
        dynamic getI8MobileAcceptance(string starttime, string endtime);
        void onChangeI8MobileAcceptance();
    }
}
