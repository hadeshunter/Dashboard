using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestingListening.service.ccdv;
using TestingListening.service.test;

namespace TestingListening.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private ITest m_test;
        private ICcdv_Dung_Tg m_ccdv_Dung_Tg;
        public ValuesController(ITest _test, ICcdv_Dung_Tg _ccdv_Dung_Tg)
        {
            m_test = _test;
            m_ccdv_Dung_Tg = _ccdv_Dung_Tg;
        }
        // GET api/values
        [HttpGet("getall")]
        public dynamic Get()
        {
            try
            {
                m_ccdv_Dung_Tg.onChange();
                return true;
            }
            catch(Exception e)
            {
                return e;
            }
            
        }

    }
}
