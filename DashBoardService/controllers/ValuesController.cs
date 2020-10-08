using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.model.bsc;
using ClassModel.model.RqGrafana;
using ClassModel.respond;
using DashBoardService.schedule;
using DashBoardService.server.bcs;
using DashBoardService.server.common;
using DashBoardService.server.origanization;
using Microsoft.AspNetCore.Mvc;

namespace DashBoardService.Controllers
{
    [Route("api/dashboard/[controller]")]    [ApiController]    public class ValuesController : ControllerBase    {        private ICommon m_common;        private IBsc m_bsc;        private IOrganization m_organization;        private II8MobileApp m_i8MobileApp;

        //Tạo metrics cho HttpPost("Search")
        private static readonly Dictionary<string, I8MobileApp> metrics = new Dictionary<string, I8MobileApp>        {            { "Tân Bình", new I8MobileApp() },            { "Đội Viễn Thông Phú Thọ Hòa", new I8MobileApp() },            { "Đội Viễn Thông Kỳ Hòa", new I8MobileApp() },            { "Đội Viễn Thông Tân Bình", new I8MobileApp() },            { "Đội Viễn Thông Âu Cơ", new I8MobileApp() }        };        public ValuesController(ICommon _common, IBsc _bsc, IOrganization organization, II8MobileApp i8Mobile)        {            m_common = _common;            m_bsc = _bsc;            m_organization = organization;            m_i8MobileApp = i8Mobile;        }        [HttpGet("test")]        public dynamic test()
        {
            return "tesst";
        }        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }

        //[HttpPost("search")]
        //public IActionResult Search()
        //{
        //    //var result = m_i8MobileApp.getById(tb.i8mbid);
        //    //var jsonResult = JsonConvert.SerializeObject(result);

        //    return Ok();
        //}
        [HttpPost("search")] //used by the find metric options on the query tab in panels.
        public IActionResult Search()        {            return Ok(metrics.Keys);        }


        //Định dạng dùng tạo table nhiều cột hàng.
        [HttpPost("query")]        public dynamic query([FromBody] RqGrafana rq)        {            DataRespond datarp = new DataRespond();            try            {                List<dynamic> x = new List<dynamic>();
                if (rq.targets[0].type == "timeserie")
                {
                    List<BscRespond> result = m_bsc.testQuery(rq);
                    foreach (var e in rq.targets)
                    {
                        List<dynamic> points = new List<dynamic>();
                        foreach (var ele in result)
                        {
                            if (ele.ten_dv.Contains(e.target))
                            {
                                ele.unix_time = m_common.ConvertToUnix(ele.ngay);
                                points.Add(new List<dynamic> { ele.sl_login, ele.unix_time });
                            }
                        }
                       
                        x.Add(new { target = e.target, datapoints = points });
                    }
                }
                else if (rq.targets[0].type == "table")
                {
                    List<BscRespond> result = m_bsc.testQuery(rq);
                    foreach (var ele in result)
                    {
                        ele.unix_time = m_common.ConvertToUnix(ele.ngay);
                    }
                    List<dynamic> col = new List<dynamic>
                    {                        new { text = "Thời gian", type = "time"}, //Phải khai báo type = "time" để cột thời gian nhận unixtime mili giây                        new { text = "Tên trung tâm", type = "string"}, //String thì ko hiện lên chart                        new { text = "Tên đơn vị", type = "string"},                        new { text = "Số lượng login", type = "number"}, //Number hiện sô liệu lên chart                        new { text = "Tỷ lệ NV", type = "number"}                    };
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
                }                                return x;            }            catch (Exception e)            {                datarp.error = e;            }            return datarp;        }

        ////Định dạng chỉ có 2 cột time và value, vẽ multibar-graph
        //[HttpPost("query")]
        //public dynamic query()
        //{
        //    DataRespond datarp = new DataRespond();
        //    try
        //    {
        //        Object[] tb1 = new Object[] {
        //                new { time = 1450754160000, tanbinh = 460},
        //                new { time = 1450828800000, tanbinh = 410},
        //                new { time = 1450915200000, tanbinh = 368}
        //        };

        //        Object[] tb2 = new Object[] {
        //                 new { time = 1450754160000, cholon = 699},
        //                 new { time = 1450828800000, cholon = 483},
        //                 new { time = 1450915200000, cholon = 409}
        //        };
        //        Object[] tb3 = new Object[] {
        //                 new { time = 1450754160000, binhchanh = 510},
        //                 new { time = 1450828800000, binhchanh = 417},
        //                 new { time = 1450915200000, binhchanh = 347}
        //        };
        //        var x = new List<dynamic> { tb1, tb2, tb3 };
        //        return x;

        //    }
        //    catch (Exception e)
        //    {
        //        datarp.error = e;
        //    }
        //    return datarp;
        //}

        [HttpPost("annotations")] //should return annotations.
        public IActionResult GetAnnotations() { return Ok(); }        [HttpGet("getDetailDataReal")]        public dynamic getDetailDataReal()        {            BscRequest x = new BscRequest();            return m_bsc.execureDetailDataReal(x);        }        [HttpGet("getOrganization")]        public dynamic getOrganization()        {            DataRespond data = new DataRespond();            try            {                data.success = true;
                //Không đủ quyền truy cập Kiemsoat
                //data.data = m_organization.execureOrganization(); 
                data.data = m_i8MobileApp.getAll();            }            catch (Exception e)            {                data.success = false;                data.message = e.Message;                data.error = e;            }            return data;        }

        //[HttpGet("getBsc")]
        //public dynamic getBsc([FromBody] BscRequest bscRequest)
        //{
        //    DataRespond data = new DataRespond();
        //    try
        //    {
        //        data.success = true;
        //        data.data = m_bsc.execureI8MobileApp(bscRequest);
        //    }
        //    catch (Exception e)
        //    {
        //        data.success = false;
        //        data.message = e.Message;
        //        data.error = e;
        //    }
        //    return data;
        //}

        [HttpPost("execureI8MobileApp")]
        public dynamic execureI8MobileApp([FromBody] RqGrafana data)
        {
            List<BscRespond> result = m_bsc.testQuery(data);
            foreach (var ele in result)
            {
                ele.unix_time = m_common.ConvertToUnix(ele.ngay);
            }

            return result;
        }

    }
}
