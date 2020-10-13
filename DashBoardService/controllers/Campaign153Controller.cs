using System;
using System.Collections.Generic;
using ClassModel.model.bsc;
using ClassModel.model.respond;
using ClassModel.model.RqGrafana;
using DashBoardService.server.common;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardService.controllers
{
    [Route("api/dashboard/[controller]")]    [ApiController]
    public class Campaign153Controller : Controller
    {
        private ICommon m_common;
        public Campaign153Controller(ICommon common)
        {
            m_common = common;
        }

        //Tạo metrics cho HttpPost("Search")
        private static readonly Dictionary<string, I8MobileApp> metrics = new Dictionary<string, I8MobileApp>        {            { "Tân Bình", new I8MobileApp() },            { "Đội Viễn Thông Phú Thọ Hòa", new I8MobileApp() },            { "Đội Viễn Thông Kỳ Hòa", new I8MobileApp() },            { "Đội Viễn Thông Tân Bình", new I8MobileApp() },            { "Đội Viễn Thông Âu Cơ", new I8MobileApp() }        };

        [HttpGet("test")]        public dynamic test()
        {
            return "tesst";
        }

        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }

        [HttpPost("search")] //used by the find metric options on the query tab in panels.
        public IActionResult Search()        {            return Ok(metrics.Keys);        }

        [HttpPost("query")]        public dynamic query([FromBody] RqGrafana rq)
        {
            DataRespond data = new DataRespond();
            try
            {

            }
            catch (Exception e)
            {
                data.success = false;                data.message = e.Message;                data.error = e;
            }
            return data;
        }

        [HttpPost("annotations")] //should return annotations.
        public IActionResult GetAnnotations() { return Ok(); }
    }
}