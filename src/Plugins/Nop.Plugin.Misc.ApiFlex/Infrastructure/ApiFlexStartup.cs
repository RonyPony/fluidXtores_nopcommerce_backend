using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.ApiFlex.Controllers;
using Nop.Plugin.Misc.ApiFlex.JSON.Serializers;
using Nop.Plugin.Misc.ApiFlex.Services;
using Nop.Plugin.Misc.ApiFlex.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Misc.ApiFlex.Helpers;
using Nop.Plugin.Misc.ApiFlex.Factories;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Services.Directory;

namespace Nop.Plugin.Misc.ApiFlex.Infrastructure
{
    internal class ApiFlexStartup : INopStartup
    {
        public int Order => 1;

        public void Configure(IApplicationBuilder app)
        {


            var rewriteOptions = new RewriteOptions()
                .AddRewrite("api/token", "/token", true);

            app.UseRewriter(rewriteOptions);

            app.UseCors(x => x.AllowAnyOrigin()
                             .AllowAnyMethod()
                             .AllowAnyHeader());


            app.MapWhen(context => context.Request.Path.StartsWithSegments(new PathString("/api")),
                a =>
                {

                    a.Use(async (context, next) =>
                    {
                        Console.WriteLine("API Call");
                        context.Request.EnableBuffering();
                        await next();
                    });

                    //a.UseExceptionHandler("/api/error/500/Error");

                    a
                    .UseRouting()
                    .UseAuthentication()
                    .UseAuthorization()
                    .UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });

                }
            );

        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            }).Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });



            addScopes(services);

        }

        private void addScopes(IServiceCollection services)
        {
            services.AddScoped<IJsonFieldsSerializer, JsonFieldsSerializer>();
            services.AddScoped<ICategoryApiService, CategoryApiService>();
            services.AddScoped<ICustomerApiService, CustomerApiService>();
            services.AddScoped<IDTOHelper,DTOHelper>();
            services.AddScoped<IManufacturerApiService,ManufacturerApiService>();
            services.AddScoped<IObjectConverter,ObjectConverter>();
            services.AddScoped<IApiTypeConverter,ApiTypeConverter>();
            services.AddScoped<INewsLetterSubscriptionApiService,NewsLetterSubscriptionApiService>();
            services.AddScoped<IProductApiService,ProductApiService>();
            // services.AddScoped<Factory<Product>>();
        }
    }
}