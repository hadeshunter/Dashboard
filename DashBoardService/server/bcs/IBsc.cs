using ClassModel.model.bsc;
using ClassModel.model.RqGrafana;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashBoardApi.server.bcs
{
    public interface IBsc
    {
        dynamic execureI8MobileApp(BscRequest bscRequest);
        dynamic execureI8NghiemThu(BscRequest bscRequest);
        dynamic execureDbThueBao(BscRequest bscRequest);
        dynamic execureDataIncrese(BscRequest bscRequest);
        dynamic execureDetailFiberMyTV(BscRequest bscRequest);
        dynamic execureDetailDataReal(BscRequest bscRequest);
        dynamic testQuery(RqGrafana rq);
    }
}
