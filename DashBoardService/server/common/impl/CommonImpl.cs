using System;
using System.Globalization;

namespace DashBoardService.server.common.impl
{
    public class CommonImpl: ICommon
    {
        public dynamic ConvertToUnix(string time)
        {
            DateTime utc = DateTime.ParseExact(time, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToUniversalTime();
            long unix = ((DateTimeOffset)utc).ToUnixTimeMilliseconds();
            return unix;
        }
    }
}
