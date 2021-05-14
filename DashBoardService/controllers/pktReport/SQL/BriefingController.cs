using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.convertdata.ccdv;
using ClassModel.model.respond;
using ClassModel.model.RqGrafana;
using DashBoardService.server.common;
using DashBoardService.server.pktReport;
using DashBoardService.server.pktReport.detail;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardService.controllers.pktReport.SQL
{
    [Route("api/dashboard/sql/[controller]")]
    [ApiController]
    public class BriefingController : Controller
    {
        private ICommon m_common;
        private IPktReportSQL m_pktReport;
        private ICCDV m_ccdv;
        public BriefingController(ICommon common,IPktReportSQL pktReport, ICCDV ccdv)
        {
            m_ccdv = ccdv;
            m_pktReport = pktReport;
            m_common = common;
        }

        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }

        [HttpPost("query")]
        public dynamic query([FromBody] RqGrafana rq)
        {
            DataRespond data = new DataRespond();
            try
            {
                var response = m_pktReport.getStatic(rq);
                return response;
            }
            catch (Exception e)
            {
                data.success = false;
                data.message = e.Message;
                data.error = e;
            }
            return data;
        }

        [HttpPost("annotations")] //should return annotations.
        public IActionResult GetAnnotations() { return Ok(); }
    }
}