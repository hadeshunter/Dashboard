using ClassModel.connnection.oracle;
using ClassModel.connnection.reponsitory.impl;
using ClassModel.connnection.sql;
using ClassModel.model.bsc;
using ClassModel.model.request;
using ClassModel.model.RqGrafana;
using ClassModel.model.testing;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace TestingListening.service.test.impl
{
    public class TestImpl : Reponsitory<TestData>, ITest
    {
        private IConfiguration m_configuration;
        public TestImpl(DataContext context, IConfiguration _configuration) : base(context)
        {
            m_configuration = _configuration;
        }

        //public void onChange1()
        //{
        //    string connStr = m_configuration.GetConnectionString("DefaultConnection1");
        //    var dt = new DataTable();
        //    using (var conn = new SqlConnection(connStr))
        //    {
        //        conn.Open();
        //        using (var cmd = new SqlCommand(@"select endtimeupdate from [dbo].TimeChange where timeid = 1", conn))
        //        {
        //            SqlDependency dependency = new SqlDependency(cmd);
        //            dependency.OnChange += new System.Data.SqlClient.OnChangeEventHandler(onChange2);
        //            SqlDependency.Start(connStr);
        //            cmd.ExecuteReader();
        //        }
        //        conn.Close();
        //    }
        //}
        //public void onChange2(object sender, SqlNotificationEventArgs e)
        //{
        //    //SqlDependency dependency = sender as SqlDependency;
        //    //string connStr = m_configuration.GetConnectionString("DefaultConnection1");
        //    //Test test123 = new Test();
        //    //test123.name = "testing ";
        //    //m_context.Test.Add(test123).State = Microsoft.EntityFrameworkCore.EntityState.Added;
        //    //m_context.SaveChanges();
        //    //onChange1();


        //    string connStr = m_configuration.GetConnectionString("DefaultConnection");
        //    var dt = new DataTable();
        //    using (var conn = new SqlConnection(connStr))
        //    {
        //        conn.Open();
        //        using (var cmd = new SqlCommand(@"insert into Test(name) values ('Testing +"+DateTime.Now.ToString()+"')", conn))
        //        {
        //            dt.Load(cmd.ExecuteReader());
        //        }
        //        conn.Close();
        //    }
        //    onChange1();
        //}

        //public dynamic  Test1()
        //{
        //    string connStr = m_configuration.GetConnectionString("DefaultConnection1");
        //    var dt = new DataTable();
        //    using (var conn = new SqlConnection(connStr))
        //    {
        //        conn.Open();
        //        using (var cmd = new SqlCommand(@"select endtimeupdate from [dbo].TimeChange where timeid = 1", conn))
        //        {
        //            try
        //            {
        //                dt.Load(cmd.ExecuteReader());
        //            }catch(Exception e)
        //            {
        //                return e;
        //            }
 
        //        }
        //    }
        //    return dt;
        //}
    }
}
