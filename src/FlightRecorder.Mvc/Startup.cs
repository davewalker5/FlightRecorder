using FlightRecorder.Mvc.Api;
using FlightRecorder.Mvc.Attributes;
using FlightRecorder.Mvc.Configuration;
using FlightRecorder.Mvc.Controllers;
using FlightRecorder.Mvc.Interfaces;
using FlightRecorder.Mvc.Wizard;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

            // Configure strongly typed application settings
            IConfigurationSection section = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(section);

            // The typed HttpClient needs to access session via the context
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Interactions with the REST service are managed via typed HttpClients
            // with "lookup" caching for performance
            services.AddMemoryCache();
            services.AddSingleton<ICacheWrapper>(s => new CacheWrapper(new MemoryCacheOptions()));
            services.AddScoped<AddSightingWizard>();
            services.AddHttpClient<AuthenticationClient>();
            services.AddHttpClient<AirlineClient>();
            services.AddHttpClient<AircraftClient>();
            services.AddHttpClient<AirportsClient>();
            services.AddHttpClient<CountriesClient>();
            services.AddHttpClient<FlightClient>();
            services.AddHttpClient<LocationClient>();
            services.AddHttpClient<ManufacturerClient>();
            services.AddHttpClient<ModelClient>();
            services.AddHttpClient<SightingClient>();
            services.AddHttpClient<SightingsSearchClient>();
            services.AddHttpClient<ReportsClient>();
            services.AddHttpClient<ExportClient>();
            services.AddHttpClient<UserAttributesClient>();

            // Configure session state for token storage
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
            new ServiceAccessor().SetProvider(app.ApplicationServices);

            // JWT authentication with the service is used to authenticate in the UI, so the user data
            // is held in one place (the service database). The login page authenticates with the service
            // and, if successful, stores the JWT token in session. This code segment injects the stored
            // token (if present) into an incoming request
            app.Use(async (context, next) =>
            {
                string token = context.Session.GetString(LoginController.TokenSessionKey);
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
