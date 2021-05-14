using System;
using System.Collections.Generic;
using ClassModel.model.bsc;
using ClassModel.model.respond;
using ClassModel.model.RqGrafana;
using DashBoardService.server.bcs;
using DashBoardService.server.common;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardService.controllers.i8Mobile
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class I8MobileAppController : Controller
    {
        private ICommon m_common;
        private II8MobileApp m_i8MobileApp;
        public I8MobileAppController(ICommon common, II8MobileApp i8MobileApp)
        {
            m_common = common;
            m_i8MobileApp = i8MobileApp;
        }

        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }

        [HttpPost("query")]
        public dynamic query([FromBody] RqGrafana rq)
        {
            DataRespond data = new DataRespond();
            try
            {
                List<dynamic> response = m_i8MobileApp.getI8MobileApp(rq);
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