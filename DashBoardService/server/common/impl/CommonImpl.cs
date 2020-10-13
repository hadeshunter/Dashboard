using System;
using System.Globalization;

namespace DashBoardService.server.common.impl
{
    public class CommonImpl: ICommon
    {
        public CommonImpl()
        {
        }

        public dynamic getTargetList(dynamic data)
        {
            var donvi_cha_id = data.donvi_cha_id.ToString();

            return donvi_cha_id;
        }

        public dynamic convertToUnix(string time)
        {
            DateTime utc = DateTime.ParseExact(time, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToUniversalTime();
            long unix = ((DateTimeOffset)utc).ToUnixTimeMilliseconds();
            return unix;
        }
        public dynamic convertUTCString(string time)
        {
            DateTime utc = DateTime.Parse(time).ToUniversalTime();
            long unix = ((DateTimeOffset)utc).ToUnixTimeMilliseconds();
            return unix;
        }
    }
}
