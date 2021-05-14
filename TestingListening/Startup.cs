using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.connnection.sql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServerConvert.listener;
using ServerConvert.listener.impl;
using TestingListening.service.ccdv;
using TestingListening.service.ccdv.impl;
using TestingListening.service.test;
using TestingListening.service.test.impl;

namespace TestingListening
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

       
        public void ConfigureServices(IServiceCollection services)
        {
            //connect database
            services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //connect database
            services.AddDbContext<TimeContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection1")));

            
            //convert data
            services.AddTransient<ICcdv_Dung_Tg, Ccdv_Dung_TgImpl>();

            services.AddTransient<IDatabaseSubscription, InventoryDatabaseSubscription>();
            services.AddSignalR();


            services.AddScoped<ITest, TestImpl>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDatabaseSubscription databaseSubscription)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            databaseSubscription.Configure();
        }
    }
}
