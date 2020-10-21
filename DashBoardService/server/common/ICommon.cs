using ClassModel.model.RqGrafana;
using System;
using System.Collections.Generic;

namespace DashBoardService.server.common
{
    public interface ICommon
    {
        dynamic getTargetList(dynamic data);
        dynamic convertToUnix(DateTime time);
        dynamic convertUTCString(DateTime time);
    }
}
