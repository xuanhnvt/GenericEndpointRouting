using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericEndpointRouting.Extensions;
using GenericEndpointRouting.Middlewares;
using GenericEndpointRouting.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GenericEndpointRouting
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
            });
            services.AddScoped<ISlugService, SlugService>();
            services.AddRazorPages()
                .AddNewtonsoftJson();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            #region Use middleware to redirect selected endpoint

            app.UseMiddleware<EndpointRedirectingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapFallback("{genericSlug}", context => Task.CompletedTask).WithDisplayName("GenericEndpoint");
            });

            #endregion

            // comment above region and uncomment below to invoke specific page by slug inside request delegate
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapRazorPages();
            //    endpoints.MapFallbackToGenericPage();
            //});
        }
    }
}
