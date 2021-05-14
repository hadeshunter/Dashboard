using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.respond;
using ClassModel.model.RqGrafana;
using DashBoardService.server.bcs;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardService.controllers.combohome
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class ComboHomeController : Controller
    {
        private IComboHome m_combohome;
        public ComboHomeController(IComboHome combohome)
        {
            m_combohome = combohome;
        }

        [HttpGet("test")]
        public dynamic test() { return "test"; }

        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }

        [HttpPost("query")]
        public dynamic query([FromBody] RqGrafana rq)
        {
            DataRespond datarp = new DataRespond();
            try
            {
                List<dynamic> response = m_combohome.getComboHome(rq);
                return response;
            }
            catch (Exception e)
            {
                datarp.error = e;
            }
            return datarp;
        }

        [HttpPost("annotations")] //should return annotations.
        public IActionResult GetAnnotations() { return Ok(); }
    }
}