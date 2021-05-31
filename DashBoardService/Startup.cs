using ClassModel.connnection.sql;
using DashBoardService.quaz.factory;
using DashBoardService.server.bcs;
using DashBoardService.server.bcs.impl;
using DashBoardService.server.campaign153;
using DashBoardService.server.campaign153.impl;
using DashBoardService.server.common;
using DashBoardService.server.common.impl;
using DashBoardService.server.customerSatisfaction.ServeQuality;
using DashBoardService.server.customerSatisfaction.ServiceQuality;
using DashBoardService.server.installationInventoryFiber;
using DashBoardService.server.ThoaitraNLML;
using DashBoardService.server.ThoaitraPCT;
using DashBoardService.server.HMIS;
using DashBoardService.server.origanization;
using DashBoardService.server.origanization.impl;
using DashBoardService.server.pktReport;
using DashBoardService.server.pktReport.impl;
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
using DashBoardService.server.convertdata.ccdv.impl;
using DashBoardService.server.convertdata.ccdv;
using DashBoardService.server.convertdata.scdv;
using DashBoardService.server.convertdata.scdv.impl;
using DashBoardService.server.pktReport.detail;
using DashBoardService.server.pktReport.detail.impl;
using DashBoardService.server.convertdata.xlsc.impl;
using DashBoardService.server.convertdata.xlsc;
using DashBoardService.server.convertdata.tk_khl;
using DashBoardService.server.convertdata.tk_khl.impl;
using DashBoardService.server.convertdata.tonLDFiber;
using DashBoardService.server.convertdata.tonLDFiber.impl;

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
            services.AddScoped<IBsc, BscImpl>();
            services.AddScoped<II8MobileApp, I8MobileAppImpl>();
            services.AddScoped<II8MobileAcceptance, I8MobileAcceptanceImpl>();
            services.AddScoped<IDirectory, DirectoryImpl>();
            services.AddScoped<ISlCos, SlCosImpl>();
            services.AddScoped<IDetail_Fiber_MyTV, Detail_Fiber_MyTVImpl>();
            services.AddScoped<IDetailDataReal, DetailDataRealImpl>();
            services.AddScoped<IOrganization, OrganizationImpl>();
            services.AddScoped<IDetail_lapmoi, Detail_lapmoiImpl>();
            services.AddScoped<IDetail_go, Detail_goImpl>();
            services.AddScoped<IComboHome, ComboHomeImpl>();
            services.AddScoped<ICampaign153, Campaign153Impl>();
            services.AddScoped<IPktReport, PktReportImpl>();
            services.AddScoped<IServiceQuality, ServiceQualityImpl>();
            services.AddScoped<IServeQuality, ServeQualityImpl>();
            services.AddScoped<IInstallationInventoryFiber, InstallationInventoryFiberImpl>();
            services.AddScoped<IThoaitraNLML, ThoaitraNLMLImpl>();
            services.AddScoped<IThoaitraPCT, ThoaitraPCTImpl>();
            services.AddScoped<IHMIS, HMISImpl>();

            //Server Convert
            services.AddScoped<ICcdvDungThoiGian, CcdvDungThoiGianImpl>();
            services.AddScoped<ISua_Chua_DV_Dung_TG_Quy_Dinh_New, Sua_Chua_DV_Dung_TG_Quy_Dinh_NewImpl>();
            services.AddScoped<ITyLeThoiGianDapUngXuLySuCo, TyLeThoiGianDapUngXuLySuCoImpl>();
            services.AddScoped<ITK_KhongHaiLong_CLDV,TK_KhongHaiLong_CLDVImpl>();
            services.AddScoped<ITK_KhongHaiLong_CLPV, TK_KhongHaiLong_CLPVImpl>();
            services.AddScoped<ITonLapdatFiber, TonLapdatFiberImpl>();

            //SQL server
            services.AddScoped<ICCDV, CCDVImpl>();
            services.AddScoped<ISCDV, SCDVImpl>();
            services.AddScoped<ICLDV, CLDVImpl>();
            services.AddScoped<ICLPV, CLPVImpl>();
            services.AddScoped<IXLSC, XLSCImpl>();
            services.AddScoped<IHSSDC, HSSDCImpl>();
            services.AddScoped<IThoaiTra, ThoaiTraImpl>();
            services.AddScoped<IThoaiTraLydo, ThoaiTraLydoImpl>();
            services.AddScoped<ITonLDFiber, TonLDFiberImpl>();
            services.AddScoped<ILuykeLapgoFiber, LuykeLapgoFiberImpl>();
            services.AddScoped<IMLLBTS, MLLBTSImpl>();
            services.AddScoped<IPktReportSQL, PktReportSQLImpl>();
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
