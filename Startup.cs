using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using VSTSWebApi.Helpers;

namespace VSTSWebApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(config =>
            {
                config.InputFormatters.Add(new PlainTextInputFormatter());
            });
            services.AddSwaggerGen(c =>
            {
            //    ApiKeyScheme scheme = new ApiKeyScheme()
            //    {
            //        Type = "apiKey",
            //        In = "header",
            //        Name = "x-auth-token"
            //    };
                //c.AddSecurityDefinition("VstsPatSecurity", scheme);

                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                //c.OperationFilter<AddApiKeyOperationsFilter>();
                c.OperationFilter<CustomSwaggerDataOperationFilter>();
                c.OperationFilter<CustomParameterTypeOperationFilter>();
                c.SchemaFilter<CustomSwaggerDataSchemaFilter>();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("default", builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            //             services.AddAuthorization(options=>{
            // options.AddPolicy()
            //             });
            //services.AddSingleton<IAuthorizationHandler, VstsPatAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("default");

            //app.UseMiddleware<ApiKeyMiddleware>();            

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}