using System.Collections.Generic;
using ClassModel.model.RqGrafana;
using DashBoardService.server.customerSatisfaction.ServeQuality;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardService.controllers.customerSatisfaction
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class ServeQualityController : ControllerBase
    {
        IServeQuality clpv;
        public ServeQualityController(IServeQuality clpv)
        {
            this.clpv = clpv;
        }
        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }

        [HttpPost("annotations")] //should return annotations.
        public dynamic GetAnnotations() { return Ok(); }

        [HttpPost("search")]
        public IActionResult Search() {
            List<dynamic> list = new List<dynamic>();
            list.Add(new { text = "Bar Chart", value = 1 });
            list.Add(new { text = "Pie Chart", value = 2 });
            return Ok(list); 
        }

        [HttpPost("query")]
        public dynamic query([FromBody] RqGrafana rq)
        {
            return clpv.GetData_Dissatisfied_ServeQuality_Grafana(rq);
        }
    }
}
