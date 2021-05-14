using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.RqGrafana;
using ClassModel.model.respond;
using DashBoardService.server.pktReport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClassModel.model.pktReport;

namespace DashBoardService.controllers.pktReport
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class LuykeLapgoFiberController : Controller
    {
        private IPktReport m_pktReport;
        public LuykeLapgoFiberController(IPktReport pktReport)
        {
            m_pktReport = pktReport;
        }
        [HttpGet]
        public dynamic get()
        {
            return Ok();
        }
        [HttpPost("search")]
        public dynamic search()
        {
            List<dynamic> list = new List<dynamic>();
            list.Add(new { text = "Bar Chart", value = 1 });
            return Ok(list);
        }
        [HttpPost("query")]
        public dynamic query([FromBody] RqGrafana rq)
        {
            return m_pktReport.getLuykeLapgoFiber(rq);
        }
    }
}
