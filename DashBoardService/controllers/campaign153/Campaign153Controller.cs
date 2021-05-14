using ClassModel.model.respond;
using ClassModel.model.RqGrafana;
using DashBoardService.server.campaign153;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DashBoardService.controllers.campaign153
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class Campaign153Controller : Controller
    {
        private ICampaign153 m_campaign153;
        public Campaign153Controller(ICampaign153 campaign153)
        {
            m_campaign153 = campaign153;
        }

        [HttpGet("test")]
        public dynamic test() { return "test"; }

        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }

        [HttpPost("query")]
        public dynamic query([FromBody] RqGrafana rq)
        {
            DataRespond datarp = new DataRespond();
            try
            {   
                List<dynamic> response = m_campaign153.getStaticCampaign153(rq);
                return response;
            }
            catch (Exception e)
            {
                datarp.error = e;
            }
            return datarp;
        }

        [HttpPost("annotations")] //should return annotations.
        public IActionResult GetAnnotations() { return Ok(); }
    }
}
