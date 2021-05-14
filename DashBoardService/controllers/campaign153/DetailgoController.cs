using System;
using System.Collections.Generic;
using System.Linq;
using ClassModel.model.bsc;
using ClassModel.model.respond;
using ClassModel.model.RqGrafana;
using DashBoardService.server.bcs;
using Microsoft.AspNetCore.Mvc;
using DashBoardService.server.common;
using DashBoardService.server.origanization;
using ClassModel.model.unit;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace DashBoardService.controllers.campaign153
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class DetailgoController : Controller
    {
        private ICommon m_common;
        private IDetail_go m_detailgo;
        private IOrganization m_organization;
        public DetailgoController(ICommon common, IDetail_go detailgo, IOrganization organization)
        {
            m_common = common;
            m_detailgo = detailgo;
            m_organization = organization;
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
                List<dynamic> response = m_detailgo.getRemove(rq);
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