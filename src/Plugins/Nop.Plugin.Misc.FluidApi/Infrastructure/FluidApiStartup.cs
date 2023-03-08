

using FluentMigrator.Runner.Processors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Misc.FluidApi.Authorization.Policies;
using Nop.Plugin.Misc.FluidApi.Authorization.Requirements;
using Nop.Plugin.Misc.FluidApi.Converters;
using Nop.Plugin.Misc.FluidApi.Factories;
using Nop.Plugin.Misc.FluidApi.Helpers;
using Nop.Plugin.Misc.FluidApi.JSON.Serializers;
using Nop.Plugin.Misc.FluidApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Nop.Plugin.Misc.FluidApi.Infrastructure
{
	public class FluidApiStartup : INopStartup
	{
		public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
		{
			//var apiConfig = services.Configure<ApiConfiguration>(apiConfigSection);
			//ApiConfiguration appSettings = Singleton<ApiConfiguration>.Instance ?? new ApiConfiguration();
			//var apiConfig = appSettings.Get<ApiConfiguration>().SecurityKey;

			var apiConfigSection = configuration.GetSection("Api");

			if (apiConfigSection != null)
			{
				//var apiConfig = services.ConfigureStartupConfig<ApiConfiguration>(apiConfigSection);
				//var apiConfig = services.ConfigureStartupConfig<ApiConfiguration>(apiConfigSection);
				var apiConfig = DataSettingsManager.LoadSettings();

				if (!string.IsNullOrEmpty(apiConfig.SecurityKey))
				{
					services.AddAuthentication(options =>
					{
						options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
						options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					})
					.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtBearerOptions =>
					{

						jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuerSigningKey = true,
							IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiConfig.SecurityKey)),
							ValidateIssuer = false, // ValidIssuer = "The name of the issuer",
							ValidateAudience = false, // ValidAudience = "The name of the audience",
							ValidateLifetime = true, // validate the expiration and not before values in the token
							ClockSkew = TimeSpan.FromMinutes(apiConfig.AllowedClockSkewInMinutes)
						};
					});
					
					JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
					AddAuthorizationPipeline(services);

					services.AddScoped<IJsonFieldsSerializer, JsonFieldsSerializer>();
					services.AddScoped<ICustomerService, CustomerService>();
					services.AddScoped<ICustomerApiService, CustomerApiService>();
					services.AddScoped<IUrlRecordService, UrlRecordService>();
					services.AddScoped<ICategoryService, CategoryService>();
					services.AddScoped<ICategoryApiService, CategoryApiService>();
					services.AddScoped<ILocalizationService, LocalizationService>();
					services.AddScoped<IPictureService, PictureService>();
					services.AddScoped<IStoreMappingService, StoreMappingService>();
					services.AddScoped<IStoreService, StoreService>();
					services.AddScoped<IDiscountService, DiscountService>();
					services.AddScoped<IAclService, AclService>();
					services.AddScoped<IDTOHelper, DTOHelper>();
					services.AddScoped<IProductAttributeConverter, ProductAttributeConverter>();
					services.AddScoped<IApiTypeConverter, ApiTypeConverter>();
					services.AddScoped<IObjectConverter, ObjectConverter>();
					services.AddScoped<ICustomerRolesHelper, CustomerRolesHelper>();
				}
			}


		}

		public void Configure(IApplicationBuilder app)
		{
			var rewriteOptions = new RewriteOptions()
				.AddRewrite("api/token", "/token", true);

			app.UseRewriter(rewriteOptions);

			app.UseCors(x => x.AllowAnyOrigin()
							 .AllowAnyMethod()
							 .AllowAnyHeader());
			app
				   .UseRouting()

				   .UseAuthentication()
				   .UseAuthorization()
				   .UseEndpoints(endpoints =>
				   {
					   endpoints.MapControllers();
				   });

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
				}
			);
		}

		public int Order => 1;

		private static void AddAuthorizationPipeline(IServiceCollection services)
		{
			services.Configure<KestrelServerOptions>(options =>
			{
				options.AllowSynchronousIO = true;
			}).Configure<IISServerOptions>(options =>
			{
				options.AllowSynchronousIO = true;
			});

			services.AddAuthorization(options =>
			{
				options.AddPolicy(JwtBearerDefaults.AuthenticationScheme,
								  policy =>
								  {
									  policy.Requirements.Add(new ActiveApiPluginRequirement());
									  policy.Requirements.Add(new AuthorizationSchemeRequirement());
									  policy.Requirements.Add(new CustomerRoleRequirement());
									  policy.RequireAuthenticatedUser();
								  });
			});

			services.AddSingleton<IAuthorizationHandler, ActiveApiPluginAuthorizationPolicy>();
			services.AddSingleton<IAuthorizationHandler, ValidSchemeAuthorizationPolicy>();
			services.AddSingleton<IAuthorizationHandler, CustomerRoleAuthorizationPolicy>();

		}
	}
}
