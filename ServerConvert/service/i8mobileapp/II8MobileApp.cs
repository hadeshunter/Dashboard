using ClassModel.model.request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerConvert.service.i8mobileapp
{
    public interface II8MobileApp
    {
        dynamic getDetailI8MobileApp(string starttime,string endtime);
        void onChangeI8MobileApp();
    }
}
