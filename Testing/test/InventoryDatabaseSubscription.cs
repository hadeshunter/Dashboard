using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Base.Enums;
using Microsoft.Extensions.Logging;

namespace ServerConvert.listener.impl
{
    public class InventoryDatabaseSubscription : IDatabaseSubscription
    {

        private readonly IConfiguration m_configuration;

        public InventoryDatabaseSubscription(IConfiguration _configuration)
        {
            m_configuration = _configuration;
        }

        public void Configure()
        {

            SuscribirseAlos();
        }
        public void SuscribirseAlos()
        {
            string connStr = m_configuration.GetConnectionString("DefaultConnection");
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"select name from [dbo].Test", conn))
                {
                    SqlDependency dependency = new SqlDependency(cmd);
                    dependency.OnChange += new OnChangeEventHandler(onChange);
                    SqlDependency.Start(connStr);
                    cmd.ExecuteReader();
                }
                
            }
        }
        public void onChange(object sender, SqlNotificationEventArgs e)
        {

            SqlDependency dependency = sender as SqlDependency;

            // Notices are only a one shot deal
            // so remove the existing one so a new 
            // one can be added
           
            if (e.Type == SqlNotificationType.Change)
            {
                Console.WriteLine("abc ");
            }
            SuscribirseAlos();
        }
    }
}
