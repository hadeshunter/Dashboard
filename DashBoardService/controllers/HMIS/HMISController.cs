using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.RqGrafana;
using DashBoardService.server.HMIS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardService.controllers.HMIS
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class HMISController : ControllerBase
    {
        IHMIS m_HMIS;
        public HMISController(IHMIS HMIS)
        {
            m_HMIS = HMIS;
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
            list.Add(new { text = "SL đã triển khai HMIS", value = 2 });
            list.Add(new { text = "SL chưa triển khai HMIS", value = 3 });
            list.Add(new { text = "Đã triển khai và chưa triển khai", value = 4 });
            return Ok(list);
        }
        [HttpPost("query")]
        public dynamic query(RqGrafana rq)
        {
            return m_HMIS.getHMIS(rq);
        }
    }
}
