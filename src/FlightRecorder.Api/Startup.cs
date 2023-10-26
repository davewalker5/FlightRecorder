using System.Text;
using FlightRecorder.Api.Entities;
using FlightRecorder.Api.Interfaces;
using FlightRecorder.Api.Services;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.BusinessLogic.Logic;
using FlightRecorder.Data;
using FlightRecorder.DataExchange;
using FlightRecorder.Entities.DataExchange;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace FlightRecorder.Api
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
            services.AddControllers();

            // Configure the flight log DB context and business logic
            services.AddScoped<FlightRecorderDbContext>();
            services.AddDbContextPool<FlightRecorderDbContext>(options =>
            {
                options.UseSqlite(Configuration.GetConnectionString("FlightRecorderDB"));
            });
            services.AddScoped<FlightRecorderFactory>();

            // Add the sightings exporter hosted service
            services.AddSingleton<IBackgroundQueue<SightingsExportWorkItem>, BackgroundQueue<SightingsExportWorkItem>>();
            services.AddHostedService<SightingsExportService>();

            // Add the airports exporter hosted service
            services.AddSingleton<IBackgroundQueue<AirportsExportWorkItem>, BackgroundQueue<AirportsExportWorkItem>>();
            services.AddHostedService<AirportsExportService>();

            // Configure strongly typed application settings
            IConfigurationSection section = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(section);

            // Configure JWT
            AppSettings settings = section.Get<AppSettings>();
            byte[] key = Encoding.ASCII.GetBytes(settings.Secret);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // Configure the user authentication service
            services.AddScoped<IUserService, UserService>();

            // Configure Swagger for API documentation
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Flight Recorder API",
                    Version = "v1"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Configure the Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight Recorder API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
