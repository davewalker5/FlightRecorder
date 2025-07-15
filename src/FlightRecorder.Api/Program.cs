using System.Diagnostics;
using System.Reflection;
using System.Text;
using FlightRecorder.Api.Entities;
using FlightRecorder.Api.Interfaces;
using FlightRecorder.Api.Services;
using FlightRecorder.BusinessLogic.Api;
using FlightRecorder.BusinessLogic.Api.AeroDataBox;
using FlightRecorder.BusinessLogic.Config;
using FlightRecorder.BusinessLogic.Factory;
using FlightRecorder.BusinessLogic.Logging;
using FlightRecorder.Data;
using FlightRecorder.Entities.Attributes;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FlightRecorder.Api
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Read the configuration file
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Configure strongly typed application settings
            IConfigurationSection section = configuration.GetSection("ApplicationSettings");
            builder.Services.Configure<FlightRecorderApplicationSettings>(section);
            var settings = section.Get<FlightRecorderApplicationSettings>();
            ApiKeyResolver.ResolveAllApiKeys(settings!);

            // Configure the flight log DB context and business logic
            var connectionString = configuration.GetConnectionString("FlightRecorderDB");
            builder.Services.AddScoped<FlightRecorderDbContext>();
            builder.Services.AddDbContextPool<FlightRecorderDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

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
            builder.Services.AddSingleton<IFlightRecorderLogger>(x => logger);

            // Configure the business logic
            builder.Services.AddSingleton<FlightRecorderApplicationSettings>(settings);
            builder.Services.AddScoped<FlightRecorderFactory>();
            builder.Services.AddScoped<IAirportsRetriever, AirportsRetrieverService>();
            builder.Services.AddScoped<IUserService, UserService>();

            // Add the sightings exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<SightingsExportWorkItem>, BackgroundQueue<SightingsExportWorkItem>>();
            builder.Services.AddHostedService<SightingsExportService>();

            // Add the airports exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<AirportsExportWorkItem>, BackgroundQueue<AirportsExportWorkItem>>();
            builder.Services.AddHostedService<AirportsExportService>();

            // Add the report exporter hosted service
            builder.Services.AddSingleton<IBackgroundQueue<ReportExportWorkItem>, BackgroundQueue<ReportExportWorkItem>>();
            builder.Services.AddHostedService<ReportExportService>();

            // Get the API key and the endpoint URLs used by the external APIs
            var apiKey = settings.ApiServiceKeys.Find(x => x.Service == ApiServiceType.AeroDataBox).Key;
            var flightEndpoint = settings.ApiEndpoints.Find(x => x.EndpointType == ApiEndpointType.Flight).Url;
            var airportEndpoint = settings.ApiEndpoints.Find(x => x.EndpointType == ApiEndpointType.Airport).Url;
            var aircraftEndpoint = settings.ApiEndpoints.Find(x => x.EndpointType == ApiEndpointType.Aircraft).Url;

            // Configure the HTTP client used by the external APIs
            builder.Services.AddSingleton<IExternalApiHttpClient, ExternalApiHttpClient>(x => ExternalApiHttpClient.Instance);

            // Add the external API data lookup services
            builder.Services.AddScoped<IFlightsApi>(x =>
                new AeroDataBoxFlightsApi(
                    logger: x.GetRequiredService<IFlightRecorderLogger>(),
                    client: x.GetRequiredService<IExternalApiHttpClient>(),
                    url: flightEndpoint,
                    key: apiKey)
            );

            builder.Services.AddScoped<IAirportsApi>(x =>
                new AeroDataBoxAirportsApi(
                    logger: x.GetRequiredService<IFlightRecorderLogger>(),
                    client: x.GetRequiredService<IExternalApiHttpClient>(),
                    url: airportEndpoint,
                    key: apiKey)
            );

            builder.Services.AddScoped<IAircraftApi>(x =>
                new AeroDataBoxAircraftApi(
                    logger: x.GetRequiredService<IFlightRecorderLogger>(),
                    client: x.GetRequiredService<IExternalApiHttpClient>(),
                    url: aircraftEndpoint,
                    key: apiKey)
            );


            // Configure JWT
            byte[] key = Encoding.ASCII.GetBytes(settings!.Secret);
            builder.Services.AddAuthentication(x =>
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

            var app = builder.Build();

            // Set up a class that provides classes that are not managed by the DI container (Attributes)
            // with access to the instances of the cache and HTTP clients that are registered in the
            // ConfigureServices() method
            ServiceAccessor.SetProvider(app.Services);

            // Configure the exception handling middleware to write to the log file
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    // Get an instance of the error handling feature
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = errorFeature?.Error;

                    if (exception != null)
                    {
                        // Log the exception
                        var logger = context.RequestServices.GetRequiredService<IFlightRecorderLogger>();
                        logger.LogMessage(Severity.Error, exception.Message);
                        logger.LogException(exception);

                        // Set a 500 response code and return the exception details in the response
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json";

                        var response = new
                        {
                            Message = exception.Message
                        };

                        await context.Response.WriteAsJsonAsync(response);
                    }
                });
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
