using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApiCore.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Localization;

namespace WebApiCore
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
        public const string SecretKey = "needtogetthisfromenvironment";
        public readonly SymmetricSecurityKey SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer();
            services.AddDbContext<Database>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), opt =>
                {                    
                });
            });
            
            services.AddMvcWithPolicy();
            services.AddJwtOptions(Configuration, SigningKey);            
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,  ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("pt-BR", "pt-BR")                
            });
            
            app.UseJwtBearerAuthenticationWithOptions(Configuration, SigningKey);
            app.UseMvc();
        }
    }
}
