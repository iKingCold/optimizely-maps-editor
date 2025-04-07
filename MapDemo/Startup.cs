using EPiServer.Cms.Shell;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Framework.Hosting;
using EPiServer.Framework.Web.Resources;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using EPiServer.Web.Hosting;
using EPiServer.Web.Routing;
using System.Diagnostics;
using MapProvider.Lantmateriet.Services;

namespace MapDemo
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostingEnvironment;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration)
        {
            _webHostingEnvironment = webHostingEnvironment;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (_webHostingEnvironment.IsDevelopment())
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));

                // Configure site for add-on development, meaning files for the Maps Editor add-on will be loaded directly from its project folder.
                var uiSolutionFolder = Path.Combine(_webHostingEnvironment.ContentRootPath, @"..\");

                services.Configure<CompositeFileProviderOptions>(c =>
                {
                    c.BasePathFileProviders.Add(new MappingPhysicalFileProvider($"/EPiServer/{MapCore.ServiceCollectionExtensions.Plugin}", string.Empty, Path.Combine(uiSolutionFolder, MapCore.ServiceCollectionExtensions.Plugin)));
                });

                services.Configure<SchedulerOptions>(options => options.Enabled = false)
                    .Configure<ClientResourceOptions>(x => x.Debug = Debugger.IsAttached) // CMS UI debug logging when the debugger is attached
                    .Configure<SchedulerOptions>(x => x.Enabled = false) // Disable scheduled jobs
                    .AddRazorPages();
            }

            services
                .AddCmsAspNetIdentity<ApplicationUser>()
                .AddCms()
                .AddAdminUserRegistration()
                .AddEmbeddedLocalization<Startup>();

            services.AddEditorMapWidget(_configuration);
            //Maybe reconsider how this should work, might want the user to override the settings in the appsettings.json or configure through pipeline?
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapContent();

                endpoints.MapControllerRoute(
                    name: "GetTileImage",
                    pattern: "{controller=Map}/{action=GetTileImage}/");

                endpoints.MapControllerRoute(
                    name: "SearchAutoComplete",
                    pattern: "{controller=Map}/{action=SearchAutoComplete}/");

                endpoints.MapControllerRoute(
                    name: "SearchAddress",
                    pattern: "{controller=Map}/{action=SearchAddress}/");
            });

            app.UseStatusCodePages(context =>
            {
                if (context.HttpContext.Response.HasStarted == false &&
                    context.HttpContext.Response.StatusCode == StatusCodes.Status404NotFound &&
                    context.HttpContext.Request.Path == "/")
                {
                    context.HttpContext.Response.Redirect("/episerver/cms");
                }

                return Task.CompletedTask;
            });
        }
    }
}
