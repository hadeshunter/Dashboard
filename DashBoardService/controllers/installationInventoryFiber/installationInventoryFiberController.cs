using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.RqGrafana;
using DashBoardService.server.installationInventoryFiber;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardService.controllers.installationInventoryFiber
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class installationInventoryFiberController : ControllerBase
    {
        IInstallationInventoryFiber fiber;
        public installationInventoryFiberController(IInstallationInventoryFiber fiber)
        {
            this.fiber = fiber;
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
            list.Add(new { text = "Bar Chart", value = 1 });
            return Ok(list);
        }
        [HttpPost("query")]
        public dynamic query(RqGrafana rq)
        {
            return fiber.GetDataForGrafana(rq);
        }
    }
}
