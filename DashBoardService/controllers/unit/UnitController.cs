using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.organization;
using ClassModel.model.unit;
using DashBoardService.server.origanization;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardService.controllers.unit
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class UnitController : Controller
    {
        private IOrganization m_organization;
        public UnitController(IOrganization organization)
        {
            m_organization = organization;
        }

        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }


        [HttpPost("search")] //used by the find metric options on the query tab in panels.
        public IActionResult Search(UnitRequest rq)
        {
            List<Unit> listOriganization = new List<Unit>();

            List<dynamic> listCenter = new List<dynamic>();
            listCenter.Add(new { text = "VTTP", value = 1 });
            listOriganization = m_organization.getAllCenter(rq);

            foreach (var row in listOriganization)
            {
                listCenter.Add( new { text = row.ten_dv, value = row.donvi_id });
            };
            return Ok(listCenter);
        }

        [HttpPost("tag-keys")]
        public IActionResult getTagKeys()
        {
            List<dynamic> listCenter = new List<dynamic>();
            List<Unit> result = m_organization.getAllTTVT();
            foreach (var row in result)
            {
                listCenter.Add(new { type = "string", text = row.ten_dv});
            }
            return Ok(listCenter);
        }

        [HttpPost("tag-values")]
        public IActionResult getTagValues([FromBody] TagRequest rq)
        {
            List<dynamic> listUnit = new List<dynamic>();
            List<Unit> listCenter = m_organization.getAllTTVT();
            List<Unit> lstUnit = m_organization.getAllDoiVT();
            var tt_id = listCenter.FirstOrDefault(center => center.ten_dv == rq.key).donvi_id;
            List<Unit> result = lstUnit.FindAll(unit => unit.donvi_cha_id == tt_id);
            foreach (var row in result)
            {
                listUnit.Add(new { text = row.ten_dv });
            }
            return Ok(listUnit);
        }
    }
}