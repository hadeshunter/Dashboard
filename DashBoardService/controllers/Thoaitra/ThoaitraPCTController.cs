using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.RqGrafana;
using DashBoardService.server.ThoaitraPCT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardService.controllers.ThoaitraPCT
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class ThoaitraPCTController : ControllerBase
    {
        IThoaitraPCT thoaitraPCT;
        public ThoaitraPCTController(IThoaitraPCT thoaitraPCT)
        {
            this.thoaitraPCT = thoaitraPCT;
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
            list.Add(new { text = "Tất cả", value = 1 });
            list.Add(new { text = "Đối soát với TTKD", value = 2 });
            list.Add(new { text = "Chỉ TTVT", value = 3 });
            return Ok(list);
        }
        [HttpPost("query")]
        public dynamic query(RqGrafana rq)
        {
            return thoaitraPCT.GetDataForGrafana(rq);
        }

       
    }
}
