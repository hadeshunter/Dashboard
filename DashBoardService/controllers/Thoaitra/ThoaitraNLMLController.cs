using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.RqGrafana;
using DashBoardService.server.ThoaitraNLML;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardService.controllers.ThoaitraNLML
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class ThoaitraNLMLController : ControllerBase
    {
        IThoaitraNLML thoaitraNLML;
        public ThoaitraNLMLController(IThoaitraNLML thoaitraNLML)
        {
            this.thoaitraNLML = thoaitraNLML;
        }
        [HttpGet]
        public dynamic get()
        {
            return Ok();
        }
        [HttpPost("search")]
        public dynamic search()
        {
            List<dynamic> list = new List<dynamic>();
            list.Add(new { text = "Table", value = 1 });
            return Ok(list);
        }
        [HttpPost("query")]
        public dynamic query(RqGrafana rq)
        {
            return thoaitraNLML.GetDataForGrafana(rq);
            
        }
    }
}
