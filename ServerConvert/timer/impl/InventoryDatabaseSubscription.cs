using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Base.Enums;
using Microsoft.Extensions.Logging;
using ServerConvert.service.i8mobileapp;
using ClassModel.model.timer;
using ClassModel.model.request;
using System.Data;
using System.Globalization;

namespace ServerConvert.listener.impl
{
    public class InventoryDatabaseSubscription : IDatabaseSubscription
    {

        private readonly IConfiguration m_configuration;
        private II8MobileAcceptance m_i8MobileAcceptance;
        private II8MobileApp m_i8MobileApp;
        private ITimeChange m_timeChange;

        public InventoryDatabaseSubscription(IConfiguration _configuration, II8MobileAcceptance _i8MobileAcceptance, II8MobileApp _i8MobileApp, ITimeChange _timeChange)
        {
            m_configuration = _configuration;
            m_i8MobileAcceptance = _i8MobileAcceptance;
            m_i8MobileApp = _i8MobileApp;
            m_timeChange = _timeChange;
        }

        public void Configure()
        {
        //    m_i8MobileAcceptance.onChangeI8MobileAcceptance();
            m_i8MobileApp.onChangeI8MobileApp();
        }
    }
}
