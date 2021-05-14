using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ServerConvert.controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : Controller
    {
        [HttpGet] //should return 200 ok. Used for "Test connection" on the datasource config page.
        public dynamic Get() { return "success"; }
    }
}