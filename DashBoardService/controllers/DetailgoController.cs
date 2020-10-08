using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.bsc;
using ClassModel.model.respond;
using ClassModel.model.RqGrafana;
using DashBoardApi.server.bcs;
using DashBoardApi.server.common;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardApi.controllers
{
    [Route("api/[controller]")]
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

        [HttpGet("getDetailgo")]
        public dynamic getDetailgo([FromBody] RqGrafana rq)
        {
            DataRespond data = new DataRespond();
            try
            {
                data.success = true;
                data.message = "success";
                data.data = m_detailgo.execureDetailgo(rq);
            }
            catch(Exception e)
            {
                data.success = false;
                data.message = e.Message;
                data.error = e;
            }
            return data;
        }

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
                    List<dynamic> col = new List<dynamic>
                    {                        new { text = "Đơn vị", type = "string" },                        new { text = "TTVT Phát triển mới Fiber", type = "string" },                         new { text = "Công tác lắp đặt Fiber", type = "string" },                        new { text = "Hủy Fiber", type = "number" },                         new { text = "Thực tăng/ PTM", type = "number" },                        new { text = "Hủy/PTM", type = "number" }                    };
                    List<dynamic> row = new List<dynamic>();
                    foreach (var ele in result)
                    {
                        foreach (var target in rq.targets)
                        {
                            if (ele.ten_tt.Contains(target.target))
                            {
                                row.Add(new List<dynamic> { ele.unix_time, ele.ten_tt, ele.ten_dv, ele.sl_login, ele.ty_le });
                            }
                        };
                    };

                    x = new List<dynamic> {                        new {                                columns = col,                                rows = row,                                type = "table"                            }                    };
                }

                return x;            }            catch (Exception e)            {                datarp.error = e;            }            return datarp;        }
    }
}