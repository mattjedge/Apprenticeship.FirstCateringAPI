using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FirstCateringAPI.BusinessLogic.Contracts;
using FirstCateringAPI.BusinessLogic.Implementations;
using FirstCateringAPI.Core.Context;
using FirstCateringAPI.DataAccess.Contracts;
using FirstCateringAPI.DataAccess.Implementations;
using FirstCateringAPI.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace FirstCateringAPI
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
            var securityKey = Configuration.GetValue<string>("AppSettings:Secrets:SecurityKey");
            var validIssuer = Configuration.GetValue<string>("AppSettings:Secrets:ValidIssuer");
            var validAudience = Configuration.GetValue<string>("AppSettings:Secrets:ValidAudience");

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = validIssuer,
                    ValidAudience = validAudience,
                    IssuerSigningKey = symmetricSecurityKey
                };
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddApiVersioning(setup =>
            {
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.DefaultApiVersion = new ApiVersion(1, 0);
                setup.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
                setup.ReportApiVersions = true;
            });

            services.AddSwaggerGen(setup =>
            {
                setup.SwaggerDoc(
                    "FirstCateringLtd", new Info
                    {
                        Title = "First Catering Ltd API",
                        Description = "A common web service for First Catering Ltd that allows Bows Formula One employees to use their cards in the existing kiosks to register and top up with money."
                    });
            });

            var mappingConfig = new MapperConfiguration(config =>
            {
                config.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddDbContext<FirstCateringContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("CateringDatabase")));

            services.AddScoped<IBaseLogic, BaseLogic>();
            services.AddScoped<IRepository, Repository>();

            services.AddScoped<IAuthLogic, AuthLogic>();
            services.AddScoped<IEmployeesLogic, EmployeesLogic>();
            services.AddScoped<IMembershipCardLogic, MembershipCardLogic>();

            services.AddScoped<IEmployeesRepo, EmployeesRepo>();
            services.AddScoped<IMembershipCardsRepo, MembershipCardsRepo>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Unexpected fault");
                    });
                });
            }

            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint("/swagger/FirstCateringLtd/swagger.json", "FirstCateringLtd");
                setupAction.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
