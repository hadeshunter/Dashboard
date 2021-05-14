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
    public class TLDUTGXLSCController : Controller
    {
        private IPktReport m_pktReport;
        public TLDUTGXLSCController(IPktReport pktReport)
        {
            m_pktReport = pktReport;
        }

        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }

        [HttpPost("search")]
        public IActionResult Search()
        {
            List<dynamic> listCenter = new List<dynamic>();
            listCenter.Add(new { text = "Pie chart - ST Tổng", value = 1 });
            listCenter.Add(new { text = "Pie chart - BH Tổng", value = 2 });
            listCenter.Add(new { text = "Pie chart - ST Quá giờ", value = 3 });
            listCenter.Add(new { text = "Bar chart", value = 4 });
            return Ok(listCenter);
        }

        [HttpPost("query")]
        public dynamic query([FromBody] RqGrafana rq)
        {
            DataRespond data = new DataRespond();
            try
            {
                List<dynamic> response = m_pktReport.getTLDUTGXLSC(rq);
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