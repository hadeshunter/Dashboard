using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Base.Enums;
using Microsoft.Extensions.Logging;
using TestingListening.service.test;
using TestingListening.service.ccdv;

namespace ServerConvert.listener.impl
{
    public class InventoryDatabaseSubscription : IDatabaseSubscription
    {

        private readonly IConfiguration m_configuration;
        private ITest m_test;
        private ICcdv_Dung_Tg m_ccdv_Dung_Tg;

        public InventoryDatabaseSubscription(IConfiguration _configuration,ITest _test, ICcdv_Dung_Tg _ccdv_Dung_Tg)
        {
            m_configuration = _configuration;
            m_test = _test;
            m_ccdv_Dung_Tg = _ccdv_Dung_Tg;
        }

        public void Configure()
        {
           // m_test.onChange1();
            m_ccdv_Dung_Tg.onChange();
        }
    }
}
