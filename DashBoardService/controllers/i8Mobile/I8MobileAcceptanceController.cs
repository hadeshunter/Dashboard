//Push 11/10/2020

using System;
using System.Collections.Generic;
using System.Linq;
using ClassModel.model.bsc;
using ClassModel.model.respond;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.bcs;
using DashBoardService.server.common;
using DashBoardService.server.origanization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DashBoardService.controllers.i8Mobile
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class I8MobileAcceptanceController : Controller
    {
        private ICommon m_common;
        private II8MobileAcceptance m_i8MobileAcceptance;
       
        public I8MobileAcceptanceController(ICommon common, II8MobileAcceptance i8MobileAcceptance)
        {
            m_common = common;
            m_i8MobileAcceptance = i8MobileAcceptance;
        }

        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }

        [HttpPost("query")]
        public dynamic query([FromBody] RqGrafana rq)
        {
            DataRespond data = new DataRespond();
            try
            {
                List<dynamic> response = m_i8MobileAcceptance.getI8MobileAcceptance(rq);
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