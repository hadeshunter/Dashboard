using ClassModel.connnection.reponsitory;
using ClassModel.model.bsc;
using ClassModel.model.RqGrafana;

namespace DashBoardService.server.bcs
{
    public interface II8MobileAcceptance:IReponsitory<I8MobileAcceptance>
    {
        dynamic executeI8MobileAcceptance(RqGrafana rq);
        dynamic getI8MobileAcceptance(RqGrafana rq);
    }
}
