using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VehicleLeasing.Data;
using VehicleLeasing.Models;
using VehicleLeasing.Services;
using VehicleLeasing.Util;
using VehicleLeasing.Hubs;

namespace VehicleLeasing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = GetConnectionStringFromEnv()
                ?? builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<Driver>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentityServer()
                .AddApiAuthorization<Driver, ApplicationDbContext>();

            builder.Services.AddAuthentication()
                .AddIdentityServerJwt();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddSignalR();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
                options.Password.RequiredUniqueChars = 1;
            });

            // https://github.com/dotnet/core/blob/main/release-notes/6.0/known-issues.md#spa-template-issues-with-individual-authentication-when-running-in-development
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.Configure<JwtBearerOptions>(
                    IdentityServerJwtConstants.IdentityServerJwtBearerScheme,
                    options =>
                    {
                        options.Authority = "https://localhost:44422";
                    });
            }

            ConfigureAppServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            // Apply migrations early
            var applyMigrations = Environment.GetEnvironmentVariable("APPLY_MIGRATIONS");
            if (new string[] { "1", "TRUE" }.Contains(applyMigrations?.ToUpper()))
            {
                
                using (var scope = app.Services.CreateScope())
                {
                    ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("Applying migrations");

                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    dbContext.Database.Migrate();
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
            app.MapRazorPages();
            app.MapHub<LeasesHub>("/leasesHub");

            app.MapFallbackToFile("index.html");

            app.Run();
        }

        public static void ConfigureAppServices(IServiceCollection services, ConfigurationManager configurationManager)
        {
            services.AddTransient<IVehicleService, VehicleService>();
            services.AddTransient<ILeasingService, LeasingService>();
            services.AddSingleton<IServerTime, DefaultServerTime>();

            services.Configure<AppConfig>(configurationManager.GetSection("AppConfig"));
        }

        public static string? GetConnectionStringFromEnv()
        {
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var database = Environment.GetEnvironmentVariable("DB_DATABASE") ?? "postgres";
            var user = Environment.GetEnvironmentVariable("DB_USER");
            var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

            if (!string.IsNullOrWhiteSpace(dbHost) &&
                !string.IsNullOrWhiteSpace(dbPort) &&
                !string.IsNullOrEmpty(user) &&
                !string.IsNullOrEmpty(password))
            {
                return $"Server={dbHost};Port={dbPort};Database={database};User Id={user};Password={password};";
            }

            return null;
        }
    }
}