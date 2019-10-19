using HHSBoard.Data;
using HHSBoard.Models;
using HHSBoard.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace HHSBoard
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            //Setting the Account Login page
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LoginPath = "/Account/Login";
                // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.LogoutPath = "/Account/Logout";
                // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.AccessDeniedPath = "/Account/AccessDenied";

                options.SlidingExpiration = true;
            });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            CreateRolesandUsersAsync(services).Wait();
            CreateDefaults(services).Wait();
        }

        private async Task CreateRolesandUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "Staff", "User" };

            IdentityResult roleResult;
            foreach (var role in roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private async Task CreateDefaults(IServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Clear defaults in case of update
            db.Defaults.RemoveRange(db.Defaults);
            await db.SaveChangesAsync();

            // Add defaults
            db.Defaults.Add(new Default
            {
                Field = "BussinessRules",
                Value = "<h1 style=\"text-align: left;\">Purpose:</h1><p style=\"text-align: left;\"><br></p><h2 style=\"text-align: left;\">Our CQI Huddle brings our team together daily to:</h2><h3><ul><li style=\"text-align: left;\">Prioritize and work through opportunities for sustained improvement</li></ul><ul><li style=\"text-align: left;\">Review metrics specific to our team that align our work to HHS goals to achieve Best Care for All</li></ul><ul><li style=\"text-align: left;\">Celebrate our successes</li></ul></h3>"
            });

            // Save
            await db.SaveChangesAsync();
        }
    }
}