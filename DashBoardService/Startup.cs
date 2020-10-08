using ClassModel.connnection.sql;
using DashBoardService.quaz.factory;
using DashBoardService.server.bcs;
using DashBoardService.server.bcs.impl;
using DashBoardService.server.common;
using DashBoardService.server.common.impl;
using DashBoardService.server.origanization;
using DashBoardService.server.origanization.impl;
using DashBoardServicve.server.bcs.impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using ReportSoftWare.model;
using ReportSoftWare.schedule;

namespace DashBoardService
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

            services.AddScoped<ICommon, CommonImpl>();
            services.AddScoped<IBsc,BscImpl>();
            services.AddScoped<II8MobileApp,I8MobileAppImpl>();
            services.AddScoped<II8MobileAcceptance,I8MobileAcceptanceImpl>();
            services.AddScoped<IDirectory, DirectoryImpl>();
            services.AddScoped<ISlCos, SlCosImpl>();
            services.AddScoped<IDetail_Fiber_MyTV, Detail_Fiber_MyTVImpl>();
            services.AddScoped<IDetailDataReal, DetailDataRealImpl>();
            services.AddScoped<IOrganization, OrganizationImpl>();
            services.AddScoped<IDetal_lapmoi, Detal_lapmoiImpl>();
            services.AddScoped<IDetail_go, Detail_goImpl>();

            // Add Quartz services
            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            // Add our job
            services.AddSingleton<HelloWorldJob>();
            services.AddSingleton(new JobSchedule(jobType: typeof(HelloWorldJob),
                cronExpression: "* * * */2 * ?")); // run every 2 day

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder
                                     .AllowAnyOrigin()
                                     .AllowAnyMethod()
                                     .AllowAnyHeader()
                                     .AllowCredentials());
            app.UseAuthentication();
            app.UseDeveloperExceptionPage();//filter user
            app.UseMvcWithDefaultRoute();///filter user
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
