using FlightRecorder.Client.ApiClient;
using FlightRecorder.Mvc.Controllers;
using FlightRecorder.Client.Interfaces;
using FlightRecorder.Mvc.Wizard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FlightRecorder.Mvc.Api;
using FlightRecorder.Entities.Interfaces;
using FlightRecorder.Entities.Config;
using FlightRecorder.Entities.Attributes;

namespace FlightRecorder.Mvc
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
            services.AddControllersWithViews();

            // Configure automapper
            services.AddAutoMapper(typeof(Startup));

            // Set up the configuration reader
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            // Read the application settings section
            IConfigurationSection section = configuration.GetSection("ApplicationSettings");
            var settings = section.Get<FlightRecorderApplicationSettings>();
            services.AddSingleton<FlightRecorderApplicationSettings>(settings);

            // The authentication token provider needs to access session via the context
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthenticationTokenProvider, AuthenticationTokenProvider>();

            // Configure the cache
            services.AddMemoryCache();
            services.AddSingleton<ICacheWrapper>(s => new CacheWrapper(new MemoryCacheOptions()));
            
            // Configure the client APIs
            services.AddSingleton<IFlightRecorderHttpClient>(provider => FlightRecorderHttpClient.Instance);
            services.AddSingleton<IAuthenticationClient, AuthenticationClient>();
            services.AddSingleton<IAirlineClient, AirlineClient>();
            services.AddSingleton<IAircraftClient, AircraftClient>();
            services.AddSingleton<ICountriesClient, CountriesClient>();
            services.AddSingleton<IFlightClient, FlightClient>();
            services.AddSingleton<ILocationClient, LocationClient>();
            services.AddSingleton<IManufacturerClient, ManufacturerClient>();
            services.AddSingleton<IModelClient, ModelClient>();
            services.AddSingleton<ISightingClient, SightingClient>();
            services.AddSingleton<ISightingsSearchClient, SightingsSearchClient>();
            services.AddSingleton<IReportsClient, ReportsClient>();
            services.AddSingleton<IExportClient, ExportClient>();
            services.AddSingleton<IUserAttributesClient, UserAttributesClient>();

            // The airports client implements two interfaces. We want to register a singleton for both
            services.AddSingleton<AirportsClient>();
            services.AddSingleton<IAirportsRetriever>(sp => sp.GetRequiredService<AirportsClient>());
            services.AddSingleton<IAirportsClient>(sp => sp.GetRequiredService<AirportsClient>());

            // Configure the sightings wizard
            services.AddScoped<AddSightingWizard>();

            // Configure session state for token storage
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Resolve the settings singleton to see whether we should use the custom error page, even in
                // the development environment
                var settings = app.ApplicationServices.GetService<FlightRecorderApplicationSettings>();
                bool useCustomErrorPage = settings?.UseCustomErrorPageInDevelopment ?? false;

                if (useCustomErrorPage)
                {
                    app.UseExceptionHandler("/Home/Error");
                    app.UseHsts();
                }
                else
                {
                    app.UseDeveloperExceptionPage();
                }      
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            // Set up a class that provides classes that are not managed by the DI container (Attributes)
            // with access to the instances of the cache and HTTP clients that are registered in the
            // ConfigureServices() method
            ServiceAccessor.SetProvider(app.ApplicationServices);

            // JWT authentication with the service is used to authenticate in the UI, so the user data
            // is held in one place (the service database). The login page authenticates with the service
            // and, if successful, stores the JWT token in session. This code segment injects the stored
            // token (if present) into an incoming request
            app.Use(async (context, next) =>
            {
                string token = context.Session.GetString(AuthenticationTokenProvider.TokenSessionKey);
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Append("Authorization", "Bearer " + token);
                }
                await next();
            });

            // Await completion of the pipeline. Once it's done, check the status code and, if it's a
            // 401 Unauthorized, redirect to the login page
            app.Use(async (context, previous) =>
            {
                await previous();
                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    context.Response.Redirect(LoginController.LoginPath);
                }
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
