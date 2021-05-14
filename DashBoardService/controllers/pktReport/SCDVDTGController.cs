using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.respond;
using ClassModel.model.RqGrafana;
using DashBoardService.server.pktReport;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardService.controllers.pktReport
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class SCDVDTGController : Controller
    {
        private IPktReport m_pktReport;
        public SCDVDTGController(IPktReport pktReport)
        {
            m_pktReport = pktReport;
        }

        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }

        [HttpPost("query")]
        public dynamic query([FromBody] RqGrafana rq)
        {
            DataRespond data = new DataRespond();
            try
            {
                List<dynamic> response = m_pktReport.getSCDVDTG(rq);
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