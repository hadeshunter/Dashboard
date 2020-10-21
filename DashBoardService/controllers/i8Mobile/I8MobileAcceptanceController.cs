//Push 11/10/2020

using System;
using System.Collections.Generic;
using System.Linq;
using ClassModel.model.bsc;
using ClassModel.model.respond;
using ClassModel.model.RqGrafana;
using ClassModel.model.unit;
using DashBoardService.server.bcs;
using DashBoardService.server.common;
using DashBoardService.server.origanization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DashBoardService.controllers.i8Mobile
{
    [Route("api/dashboard/[controller]")]
    [ApiController]
    public class I8MobileAcceptanceController : Controller
    {
        private ICommon m_common;
        private IOrganization m_organization;
        private II8MobileAcceptance m_i8MobileAcceptance;
       
        public I8MobileAcceptanceController(ICommon common, IOrganization organization, II8MobileAcceptance i8MobileAcceptance)
        {
            m_common = common;
            m_organization = organization;
            m_i8MobileAcceptance = i8MobileAcceptance;
        }

        ////Tạo metrics cho HttpPost("Search")
        //private static Dictionary<string, dynamic> metrics = new Dictionary<string, dynamic>        //{        //    { "Đội Viễn thông Bình Hưng Hòa", new { text = "Đội Viễn thông Bình Hưng Hòa", value = 234 } },        //    { "Đội Viễn thông Bình Điền", new { text = "Đội Viễn thông Bình Điền", value = 235 } }        //};

        [HttpGet("test")]        public dynamic test()
        {
            return "tesst";
        }

        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }

        //[HttpPost("search")] //used by the find metric options on the query tab in panels.
        //public IActionResult Search()        //{        //    List<Unit> listOriganization = new List<Unit>();        //    Dictionary<int, string> metrics1 = new Dictionary<int, string>();        //    listOriganization = m_organization.getAllUnit();

        //    foreach (var row in listOriganization)
        //    {
        //        metrics1.Add(row.donvi_id, row.ten_dv);
        //    };        //    return Ok(metrics1.Values);        //}

        [HttpPost("query")]        public dynamic query([FromBody] RqGrafana rq)
        {
            DataRespond data = new DataRespond();
            try
            {
                List<dynamic> x = new List<dynamic>();
                List<I8MobileAcceptance> result = m_i8MobileAcceptance.acceptanceStatistic(rq);
                if (rq.targets[0].type == "table")
                {
                    List<dynamic> col = new List<dynamic>
                    {                        new { text = "Đội VT", type = "string"},
                        new { text = "TTVT", type = "string"},
                        new { text = "Ngày HT", type = "string"},                        new { text = "Số phiếu HT", type = "number"},                        new { text = "Số phiếu HT qua App", type = "number"}                    };
                        List<dynamic> row = new List<dynamic>();
                        foreach (var ele in result)
                        {
                            foreach (var target in rq.targets)
                            {
                                if (ele.donvi_cha_id == int.Parse(target.target))
                                {
                                    row.Add(new List<dynamic> { ele.doi_vt, ele.TTVT, ele.ngay_ht, ele.PCT_CCDV_VA_SCDV_HOAN_TAT, ele.PCT_HOAN_TAT_QUA_MOBILE_APP });
                                }
                            };
                        };

                        x = new List<dynamic> {                        new {                                columns = col,                                rows = row,                                type = "table"                            }                    };
                } else if (rq.targets[0].type == "timeseries")
                {

                    List<I8MobileAcceptance> listByTTVT = new List<I8MobileAcceptance>();                  
                    foreach (var ele in result)
                    {
                        foreach (var target in rq.targets)
                        {
                            if (ele.donvi_cha_id == int.Parse(target.target))
                            {
                                listByTTVT.Add(ele);
                            }
                        };
                    };
                    
                    var listByDate = listByTTVT
                            .OrderBy(ele => ele.ngay_ht)
                            .GroupBy(l => new { l.ngay_ht, l.donvi_id, l.doi_vt, l.donvi_cha_id, l.TTVT })
                            .Select(lg =>
                               new {
                                   lg.Key.donvi_id,
                                   lg.Key.doi_vt,
                                   lg.Key.donvi_cha_id,
                                   lg.Key.TTVT,
                                   lg.Key.ngay_ht,
                                   PCT_CCDV_VA_SCDV_HOAN_TAT = lg.Sum(w => w.PCT_CCDV_VA_SCDV_HOAN_TAT),
                                   PCT_HOAN_TAT_QUA_MOBILE_APP = lg.Sum(w => w.PCT_HOAN_TAT_QUA_MOBILE_APP)
                               });
                    List<Unit> lstUnit = new List<Unit>();
                    lstUnit = m_organization.getAllUnit();
                    List<Unit> lstUnit1 = new List<Unit>();
                    foreach (var unit in lstUnit)
                    {
                        var filter = listByDate.FirstOrDefault(u => u.donvi_id == unit.donvi_id);
                        if (filter != null)
                        {
                            lstUnit1.Add(unit);
                        }
                    }

                    foreach (var unit in lstUnit1)
                    {
                        List<dynamic> points = new List<dynamic>();
                        foreach (var ele in listByDate)
                        {
                            if (ele.donvi_id == unit.donvi_id)
                            {
                                var unix_time = m_common.convertToUnix(ele.ngay_ht);
                                if (rq.targets[0].data.condition == "usaged")
                                {
                                    points.Add(new List<dynamic> { ele.PCT_HOAN_TAT_QUA_MOBILE_APP, unix_time });
                                }
                                else if (rq.targets[0].data.condition == "total")
                                {
                                    points.Add(new List<dynamic> { ele.PCT_CCDV_VA_SCDV_HOAN_TAT, unix_time });
                                }
                            }
                        }
                        x.Add(new { target = unit.ten_dv, datapoints = points });
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