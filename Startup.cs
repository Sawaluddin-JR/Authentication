using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using authentication.Data;
using authentication.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace authentication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddDbContext<UserContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("Default")));

            services.AddControllers();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<JwtService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(options => options
               .WithOrigins(new[] { "http://localhost:5173", "http://localhost:8080", "http://localhost:4200" })
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials()
           );

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}