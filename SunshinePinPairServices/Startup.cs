using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace SunshinePinPairServices
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //MvcOptions.EnableEndpointRouting = false;
            services.AddMvc(options => { options.EnableEndpointRouting = false; });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            //services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            /*
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            */
            /*
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
            */
            //app.UseWelcomePage();
            //app.UseFileServer();
            var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), @"Pages")),
                RequestPath = new PathString("")
            });
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });
            //app.UseMvcWithDefaultRoute();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                        name: "Default",
                        template: "{controller=Home}/{action=Index}/");
            });

            app.Run(async (context) =>
            {
                //var msg = Configuration["placeholder"];
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync("⠄⠄⠄⠄⠄⡇⠄⠄⠄⠄⠄⢠⡀⠄⠄⢀⡬⠛⠁⠄⠄⠄⠄⠄⠄⠄⠉⠻⣿⣿⣿⣽⣿⣿⣿⣿⣿⣿⣿⣿⣧⠄⠄⠙⢦\n ⠄⠄⠄⠄⠄⡇⠄⠄⠄⠄⢰⠼⠙⢀⡴⠋⠄⠄⠄⠄⠄⠄⠄⠄⠄⡠⠖⠄⠄⠙⠿⢿⣿⣿⣿⣿⣿⣿⣿⣿⣯⣀⡀⠄⠄⠄⡀\n ⠄⠄⠄⠄⠄⡇⠄⠄⠄⠄⠄⠄⡴⠋⠄⠄⠄⠄⠄⠄⠄⠄⠄⢠⠞⠄⠄⠄⠄⠄⠄⠄⠄⠄⠉⠉⠉⠙⠋⠙⠋⠙⠻⠦⠤⣤⣼⣆⣀⣀⣀⣀⡀\n ⠄⠄⠄⠄⠄⢷⠄⠄⠄⠄⢠⠞⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⡰⠃⡄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠉⠉⠉\n ⠄⠄⠄⠄⠄⢸⡀⠄⠄⢠⠏⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⣰⠁⣸⠁⠄⠄⠄⠄⠄⠄⠄⠄⢀⠄⠄⡄\n ⠄⠄⠄⠄⠄⢀⣧⠄⢠⠏⠄⠄⠄⠄⠄⠄⠄⠄⠄⢀⢾⠃⡜⡿⠄⠄⠄⠄⠄⠄⠄⠄⣠⠋⢀⣼⠁⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠸⢾⣀⣠\n ⠄⠄⠄⢀⣠⢌⣦⢀⡏⠄⡄⠄⠄⢠⠃⠄⠄⠐⣶⡁⡞⡼⠄⣇⠄⠄⠄⠄⠄⠄⠄⡴⠁⢠⠎⢸⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⢈⠝⠁\n ⠄⢀⠞⠉⠄⠄⠹⡼⠄⣼⠁⠄⠄⡏⠄⠄⢀⡞⠄⠈⣷⠇⠄⢻⠄⠄⠄⠄⠄⢐⣞⣀⣰⣃⣀⣸⠄⢀⠇⠄⠄⠄⠄⠄⠄⠄⠄⠄⠁⠄⠄⠄⠄⠄⢀\n ⢰⠋⠄⠄⠄⠄⢀⡇⢠⡏⠄⠄⢸⠄⠄⢀⠎⠄⠄⠄⡇⠄⠄⢸⡀⠄⠄⠄⢠⢾⢁⡜⠁⠄⠄⢸⠄⣸⠄⠄⠄⠄⠄⠄⡀⠄⠄⠄⠄⠄⠄⠄⢀⡴⠃\n ⡞⠄⠄⠄⠄⠄⢸⠄⡞⡷⠄⠄⡟⠄⢘⡟⠛⠷⠶⣤⣅⠄⠄⠄⣇⠄⠄⢠⠋⡧⠊⠄⠄⠄⠄⢸⢀⠇⠄⠄⠄⠄⠄⢰⠁⠄⠄⠄⠄⠄⢀⡴⠋\n ⢹⠄⠄⠄⠄⠄⡾⢰⠃⡇⠄⠄⡇⠄⡜⢀⣠⣤⠶⠞⠛⠁⠄⠄⠘⡄⡰⠃⠘⠱⣾⣟⡛⠛⠛⠛⡟⠂⠄⠄⠄⠄⠄⡎⠄⠄⠄⠄⣀⠴⡋\n ⠈⢳⠄⠄⠄⠄⡇⡼⠄⢻⠄⢠⡇⢸⠁⠈⠁⠄⠄⠄⠄⠄⠄⠄⠄⠈⠁⠄⠄⠄⠄⠙⠿⣶⣄⡰⠇⠄⠄⠄⠄⠄⡼⠄⠄⠄⡠⢾⣿⣆⢳\n ⣀⣬⠿⠷⠦⠤⣷⣇⡠⠾⡄⢸⣇⢸⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⢹⠛⠂⠄⠄⠄⠄⣰⠁⠄⣀⣬⣷⠞⠛⠙⠛⢧⣤⣀\n ⠉⠄⠄⠄⠄⠄⢻⠄⠄⠄⢧⢸⢸⠘⡇⠄⠄⠄⠄⠄⠄⠄⠄⣠⠄⠄⠄⠄⠄⠄⠄⠄⠄⢠⠃⠄⠄⠄⠄⠄⣰⠃⣶⢉⠜⠋⠄⠄⠄⠄⠄⠄⠄⠈⢳\n ⠄⠄⠄⢀⣤⡀⢸⠄⠄⠄⠈⢿⠄⠄⣿⣆⠄⠄⠄⠄⠄⠄⠄⡟⣧⠄⠄⠄⠄⠄⠄⠄⡴⠃⠄⠄⠄⡠⠊⡰⡗⠋⡰⡼⠃⠄⠄⠄⠄⠄⠄⠄⠄⠄⢨\n ⠄⢀⡔⠉⠄⠙⢦⠄⠄⠄⠄⢸⡀⢰⡏⠈⠳⣄⠄⠄⠄⠄⠄⠉⠁⠄⠄⠄⠄⠄⢀⡞⠁⠄⢀⣤⠎⠄⡔⣡⠃⢰⡇⣹⠄⠄⠄⠄⠄⠄⠄⠄⠄⠄⢼\n ⣰⠋⠄⢀⣴⣖⠒⠓⡆⠄⠄⠈⣇⣿⣿⠄⢸⠹⡷⢄⠄⠄⠄⠄⠄⠄⠄⠄⠄⣠⠋⠄⢀⣴⣿⠏⠠⠊⣴⡏⢠⢻⡇⢹⡆⠄⠄⠄⠄⠄⣷⠄⠄⣰⠋\n ⡇⠄⣰⣿⣿⣿⠒⠋⣛⣄⠄⠄⣹⢸⠈⢧⠸⡄⠹⣄⠙⠶⢶⣶⣶⣶⣶⡶⢾⠃⠄⡴⢋⡾⠋⣀⡴⣞⡝⡰⠃⠄⡇⡸⠉⠳⣄⣀⣀⣀⣿⣦⠞⠁\n ⢷⢰⣿⣿⣿⣿⠄⠈⠁⠈⡇⠄⡏⢸⠄⠈⠓⢧⣀⣈⣤⡤⠖⠛⠉⠁⠄⢡⠃⢠⡞⠓⠚⠓⠚⣳⡞⠈⠘⠁⠄⠄⢹⡇⠄⠄⠄⠈⠉⠁⠸⣷⣀⣀⣀\n ⢸⣿⣿⣿⣿⣿⠄⣿⣁⠜⠁⢸⡇⢸⣄⣀⡀⠘⢦⡀⠄⠄⠄⠄⠄⠄⢀⠏⡴⡻⠄⠄⠄⠠⣎⠹⡄⠄⢀⣀⣤⣤⣀⠁⠄⠄⠄⠄⠄⠄⠄⠈⢻⣿⣿\n ⢸⣿⣿⣿⣿⣿⡇⢸⠄⠄⢀⡴⡇⠈⡇⠈⣩⠗⠒⣵⠆⠄⠄⠄⠄⠄⢸⡞⢰⠃⢀⠄⣀⡰⠟⠒⠒⡿⠉⠄⠄⠄⠈⠑⣄⠄⠄⠄⠄⠄⠄⠄⠈⢿⣿\n ⣿⣿⣿⣿⣿⣿⣷⠎⠄⢠⠏⠄⠹⣄⢣⢠⠃⠄⠄⢤⠤⠄⠄⠠⠤⢶⡏⠄⡎⢠⠞⠋⠁⠄⠄⠄⣸⠁⠄⠄⠄⠄⠄⠄⠈⣧⠄⠄⠄⠄⠄⠄⠄⠄⠻\n ⣿⣿⣿⣿⣿⣿⣃⡀⢠⠏⠄⠄⠄⠄⣨⠇⠄⣠⠴⠚⠁⠄⠄⠄⠄⠈⡇⢰⠃⠄⠄⠄⠄⠄⠄⢰⠇⠄⠄⠄⠄⠄⠄⠄⠄⢹⡀\n ⣿⣿⣿⣿⣿⡿⢉⣇⡎⠄⠄⠄⠄⢰⠇⠄⢨⠇⠄⠄⠄⠄⠄⠄⠄⠄⠘⢾⡀⠄⠄⠄⠄⠄⠄⡞⢀⠄⠄⠄⠄⠄⠄⠄⠄⢸⡇\n");
            });

        }
    }
}
