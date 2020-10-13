using System;
using System.Collections.Generic;
using System.Linq;
using ClassModel.model.bsc;
using ClassModel.model.respond;
using ClassModel.model.RqGrafana;
using DashBoardService.server.bcs;
using Microsoft.AspNetCore.Mvc;
using DashBoardService.server.common;

namespace DashBoardService.controllers
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class DetailgoController : Controller
    {
        private ICommon m_common;
        private IDetail_go  m_detailgo;
        public DetailgoController(ICommon common, IDetail_go detailgo)
        {
            m_common = common;
            m_detailgo = detailgo;
        }

        //Tạo metrics cho HttpPost("Search")
        private static readonly Dictionary<string, I8MobileApp> metrics = new Dictionary<string, I8MobileApp>        {            { "Tân Bình", new I8MobileApp() },            { "Đội Viễn Thông Phú Thọ Hòa", new I8MobileApp() },            { "Đội Viễn Thông Kỳ Hòa", new I8MobileApp() },            { "Đội Viễn Thông Tân Bình", new I8MobileApp() },            { "Đội Viễn Thông Âu Cơ", new I8MobileApp() }        };

        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }

        [HttpPost("search")] //used by the find metric options on the query tab in panels.
        public IActionResult Search()        {            return Ok(metrics.Keys);        }

        [HttpPost("query")]        public dynamic query([FromBody] RqGrafana rq)        {            DataRespond datarp = new DataRespond();            try            {                List<dynamic> x = new List<dynamic>();
                if (rq.targets[0].type == "timeserie")
                {
                    //List<Detail_go> result = m_detailgo.execureDetailgo(rq);
                    //foreach (var e in rq.targets)
                    //{
                    //    List<dynamic> points = new List<dynamic>();
                    //    foreach (var ele in result)
                    //    {
                    //        if (ele.ten_dv.Contains(e.target))
                    //        {
                                
                    //            points.Add(new List<dynamic> { ele.sl_login, ele.unix_time });
                    //        }
                    //    }

                    //    x.Add(new { target = e.target, datapoints = points });
                    //}
                }
                else if (rq.targets[0].type == "table")
                {
                    List<Detail_go> result = m_detailgo.execureDetailgo(rq);
                    //List<dynamic> col = new List<dynamic>
                    //{                    //    new { text = "Đơn vị", type = "string" },                    //    new { text = "TTVT Phát triển mới Fiber", type = "number" },
                    //    new { text = "Công tác lắp đặt Fiber", type = "number" },                    //    new { text = "Hủy Fiber", type = "number" },
                    //    new { text = "Thực tăng/ PTM", type = "number" },                    //    new { text = "Hủy/PTM", type = "number" }                    //};
                    //List<dynamic> row = new List<dynamic>();
                    //foreach (var ele in result)
                    //{
                    //    foreach (var target in rq.targets)
                    //    {
                    //        if (ele.ten_dv.Contains(target.target))
                    //        {
                    //            row.Add(new List<dynamic> { ele.ten_dv, ele.ten_dv, ele.sl_login, ele.ty_le });
                    //        }
                    //    };
                    //};

                    //x = new List<dynamic> {                    //    new {                    //            columns = col,                    //            rows = row,                    //            type = "table"                    //        }                    //};
                }

                //return x;            }            catch (Exception e)            {                datarp.error = e;            }            return datarp;        }

        [HttpPost("annotations")] //should return annotations.
        public IActionResult GetAnnotations() { return Ok(); }

        [HttpPost("getDetailgo")]
        public dynamic getDetailgo([FromBody] RqGrafana rq)
        {
            DataRespond data = new DataRespond();
            try
            {
                data.success = true;
                data.message = "success";
                data.data = m_detailgo.execureDetailgo(rq);
            }
            catch (Exception e)
            {
                data.success = false;
                data.message = e.Message;
                data.error = e;
            }
            return data;
        }
    }
}