using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.bsc;
using ClassModel.model.respond;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.bcs;
using DashBoardService.server.common;
using DashBoardService.server.origanization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DashBoardService.controllers.campaign153
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class ActualPercentController : Controller
    {
        private ICommon m_common;
        private IDetail_lapmoi m_detail_lapmoi;
        private IDetailDataReal m_detailDataReal;
        private IOrganization m_organization;
        public ActualPercentController(ICommon common, IOrganization organization, IDetail_lapmoi detail_lapmoi, IDetailDataReal detailDataReal)
        {
            m_common = common;
            m_detail_lapmoi = detail_lapmoi;
            m_detailDataReal = detailDataReal;
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
                List<dynamic> response = m_detailDataReal.getActualPercent(rq);
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