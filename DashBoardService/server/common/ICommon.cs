using ClassModel.model.RqGrafana;
using System.Collections.Generic;

namespace DashBoardService.server.common
{
    public interface ICommon
    {
        dynamic getTargetList(dynamic data);
        dynamic convertToUnix(string time);
        dynamic convertUTCString(string time);
    }
}
