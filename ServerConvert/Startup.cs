using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClassModel.connnection.sql;
using ClassModel.model.timer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServerConvert.listener;
using ServerConvert.listener.impl;
using ServerConvert.service.i8mobileapp;
using ServerConvert.service.i8mobileapp.impl;
using ServerConvert.service.timer.impl;

namespace ServerConvert
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

            services.AddTransient<II8MobileAcceptance, I8MobileAcceptanceImpl>();
            services.AddTransient<II8MobileApp, I8MobileAppImpl>();
            services.AddTransient<ITimeChange, TimeChangeImpl>();
            //timer
            services.AddTransient<IDatabaseSubscription, InventoryDatabaseSubscription>();
            services.AddSignalR();
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
