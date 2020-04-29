using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using IdentityServer.Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using IdentityServer.Infrastructure.Middleware;
using Serilog;
using IdentityServer.Infrastructure.Services;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            services.AddControllersWithViews();
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default")));

            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppIdentityDbContext>()
                    .AddDefaultTokenProviders();

            // this adds the operational data from DB (codes, tokens, consents)
            services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddOperationalStore(options =>
                  {
                      options.ConfigureDbContext = builder => builder.UseSqlServer(Configuration.GetConnectionString("Default"));
                  })
                  .AddInMemoryIdentityResources(Config.GetIdentityResources())
                  .AddInMemoryApiResources(Config.GetApiResources())
                  .AddInMemoryClients(Config.GetClients())
                  .AddAspNetIdentity<AppUser>();

            services.AddTransient<IProfileService, IdentityClaimsProfileService>();


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
               .AddCookie("Cookies")
               .AddOpenIdConnect("oidc", options =>
               {
                   options.Authority = "http://localhost:44393";
                   options.RequireHttpsMetadata = false;
                   options.ClientId = "angular_southindianvillage_admin";
                   //options.ClientSecret = "secret";
                   options.ResponseType = "code id_token token";
                   options.Scope.Add("openid");
                   options.SaveTokens = true;
               });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthentication();
            
            app.UseCors(options => options.WithOrigins("*"));
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseIdentityServer();
            app.UseCors(options => options.WithOrigins("http://localhost:4001").AllowAnyMethod().AllowAnyHeader());

        }
    }
}
