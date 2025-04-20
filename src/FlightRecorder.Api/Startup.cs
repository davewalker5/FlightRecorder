using FlightRecorder.Api.Entities;
using FlightRecorder.Api.Interfaces;
using FlightRecorder.Api.Services;
using FlightRecorder.BusinessLogic.Api;
using FlightRecorder.BusinessLogic.Api.AeroDataBox;
using FlightRecorder.BusinessLogic.Config;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.BusinessLogic.Logging;
using FlightRecorder.Data;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

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
            var connectionString = Configuration.GetConnectionString("FlightRecorderDB");
            services.AddScoped<FlightRecorderDbContext>();
            services.AddDbContextPool<FlightRecorderDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });
            services.AddScoped<FlightRecorderFactory>();


            Console.WriteLine(Configuration.GetConnectionString("FlightRecorderDB"));

            // Read the configuration file
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Configure strongly typed application settings
            IConfigurationSection section = configuration.GetSection("ApplicationSettings");
            services.Configure<FlightRecorderApplicationSettings>(section);
            var settings = section.Get<FlightRecorderApplicationSettings>();
            ApiKeyResolver.ResolveAllApiKeys(settings!);

            // Get the version number and application title
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(assembly.Location);
            var title = $"Flight Recorder API v{info.FileVersion}";

            // Create the file logger and log the startup messages
            var logger = new FileLogger();
            logger.Initialise(settings.LogFile, settings.MinimumLogLevel);
            logger.LogMessage(Severity.Info, new string('=', 80));
            logger.LogMessage(Severity.Info, title);

            // Log the connection string
            var message = $"Database connection string = {connectionString}";
            logger.LogMessage(Severity.Info, message);

            // Register the logger with the DI framework
            services.AddSingleton<IFlightRecorderLogger>(x => logger);

            // Add the sightings exporter hosted service
            services.AddSingleton<IBackgroundQueue<SightingsExportWorkItem>, BackgroundQueue<SightingsExportWorkItem>>();
            services.AddHostedService<SightingsExportService>();

            // Add the airports exporter hosted service
            services.AddSingleton<IBackgroundQueue<AirportsExportWorkItem>, BackgroundQueue<AirportsExportWorkItem>>();
            services.AddHostedService<AirportsExportService>();

            // Add the report exporter hosted service
            services.AddSingleton<IBackgroundQueue<ReportExportWorkItem>, BackgroundQueue<ReportExportWorkItem>>();
            services.AddHostedService<ReportExportService>();

            // Get the API key and the endpoint URLs used by the external APIs
            var apiKey = settings.ApiServiceKeys.Find(x => x.Service == ApiServiceType.AeroDataBox).Key;
            var flightEndpoint = settings.ApiEndpoints.Find(x => x.EndpointType == ApiEndpointType.Flight).Url;
            var airportEndpoint = settings.ApiEndpoints.Find(x => x.EndpointType == ApiEndpointType.Airport).Url;
            var aircraftEndpoint = settings.ApiEndpoints.Find(x => x.EndpointType == ApiEndpointType.Aircraft).Url;

            // Configure the HTTP client used by the external APIs
            services.AddSingleton<IFlightRecorderHttpClient>(x => FlightRecorderHttpClient.Instance);

            // Add the external API data lookup services
            services.AddScoped<IFlightsApi>(x =>
                new AeroDataBoxFlightsApi(
                    logger: x.GetRequiredService<IFlightRecorderLogger>(),
                    client: x.GetRequiredService<IFlightRecorderHttpClient>(),
                    url: flightEndpoint,
                    key: apiKey)
            );

            services.AddScoped<IAirportsApi>(x =>
                new AeroDataBoxAirportsApi(
                    logger: x.GetRequiredService<IFlightRecorderLogger>(),
                    client: x.GetRequiredService<IFlightRecorderHttpClient>(),
                    url: airportEndpoint,
                    key: apiKey)
            );

            services.AddScoped<IAircraftApi>(x =>
                new AeroDataBoxAircraftApi(
                    logger: x.GetRequiredService<IFlightRecorderLogger>(),
                    client: x.GetRequiredService<IFlightRecorderHttpClient>(),
                    url: aircraftEndpoint,
                    key: apiKey)
            );

            // Configure JWT
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
