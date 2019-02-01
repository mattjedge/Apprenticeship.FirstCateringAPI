using System;
using System.IO;
using System.Reflection;
using System.Text;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,                                     

                    ValidIssuer = validIssuer,
                    ValidAudience = validAudience,
                    IssuerSigningKey = symmetricSecurityKey,
                   
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
                    "FirstCateringLtdV1", new Info
                    {
                        Title = "First Catering Ltd API",
                        Description = "A common web service for First Catering Ltd that allows Bows Formula One employees " +
                                         "to use their cards in the existing kiosks to register and top up with money."    +
                                         Environment.NewLine + Environment.NewLine +
                                         "All endpoints begin with https://localhost:44340/api/" +                                         
                                         Environment.NewLine + Environment.NewLine +
                                         "To access the API you must first send your username and password with a basic authorization header to Auth/Token. " +
                                         "This will generate a JwtBearer token (with a 15 minute expiration) that you must send in the authorization header with each subsequent request." +
                                         Environment.NewLine + Environment.NewLine +
                                         "To deal with a user first scanning their card, begin by calling MembershipCards/{CardId}/Verify, " +
                                         "which will check whether the card exists in the system. If it does, go on to call Employees/{EmployeeID}/Login; otherwise, " +
                                         "Employees/Register will register a new employee and their membership card into the system." +
                                         Environment.NewLine + Environment.NewLine +
                                         "When accessing resources, you can specify whether you want the response body to include " +
                                         "HATEOAS navigational links by including the hateoas header - application/vnd.catering.hateoas+json." +
                                         "Responses will otherwise be returned in JSON."
                    });

                //enable swagger comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                setup.IncludeXmlComments(xmlPath);
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
            services.AddScoped<IEncryption, Encryption>();

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
                        await context.Response.WriteAsync("Unexpected server error");
                    });
                });
            }
            app.UseSwagger();
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint("/swagger/FirstCateringLtdV1/swagger.json", "FirstCateringLtdV1");
                setupAction.RoutePrefix = String.Empty;
                setupAction.SupportedSubmitMethods();
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}