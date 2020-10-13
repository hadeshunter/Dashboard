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

        ////Tạo metrics cho HttpPost("Search")
        //private static readonly Dictionary<string, I8MobileApp> metrics = new Dictionary<string, I8MobileApp>        //{        //    { "Đội Viễn Thông", new I8MobileApp() },        //};

        [HttpGet("test")]        public dynamic test()
        {
            return "tesst";
        }

        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }

        //[HttpPost("search")] //used by the find metric options on the query tab in panels.
        //public IActionResult Search()        //{        //    return Ok(metrics.Keys);        //}

        [HttpPost("query")]        public dynamic query([FromBody] RqGrafana rq)
        {
            DataRespond data = new DataRespond();
            try
            {
                List<dynamic> x = new List<dynamic>();

                List<UsageResponse> result = m_i8MobileApp.usageStatistic(rq);
                if (rq.targets[0].type == "table")
                {
                    List<dynamic> col = new List<dynamic>
                    {                        new { text = "Đội VT", type = "string"},
                        new { text = "TTVT", type = "string"},                        new { text = "Login", type = "number"},
                        new { text = "Tổng", type = "number"},                        new { text = "Tỷ lệ", type = "number"}                    };
                        List<dynamic> row = new List<dynamic>();
                        foreach (var ele in result)
                        {
                            foreach (var target in rq.targets)
                            {
                                if (ele.ttvt_id == int.Parse(target.target))
                                {
                                    row.Add(new List<dynamic> { ele.ten_dv, ele.ttvt, ele.login, ele.tong, ele.ty_le });
                                }
                            };
                        };

                        x = new List<dynamic> {                        new {                                columns = col,                                rows = row,                                type = "table"                            }                    };
                } else if (rq.targets[0].type == "timeserie")
                {
                    foreach (var e in rq.targets)
                    {
                        List<dynamic> points = new List<dynamic>();
                        foreach (var ele in result)
                        {
                            if (ele.ttvt_id == int.Parse(e.target))
                            {
                               // ele.unix_time = m_common.convertUTCString(rq.range.to);
                                points.Add(new List<dynamic> { ele.login, ele.unix_time });
                            }
                        }

                        x.Add(new { e.target, datapoints = points });
                    }
                }


                return x;
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