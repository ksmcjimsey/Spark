using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using SparkAuto.Email;

namespace SparkAuto
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
            // We added this so the user gets to choose the cookies or not.
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent, for non-essential 
                // cookies, is needed to use cookies.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //services.AddScoped<IDbInitializer, DbInitializer>();

            // Auto added due to EF selected at startup
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            // Default identity - user account, login, and register
            // If we ever see .AddDefaultUI(UIFramework.Bootstrap4) we can remove as
            // it is built in and will give an error in core 3.1 and later.
            // Changed AddDefaultIdentity to AddIdentity for core 3.1
            //services.AddDefaultIdentity<IdentityUser>()
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddDefaultTokenProviders()     // Added this line as part of core 3.1 for roles and identity
                .AddDefaultUI()     // *** This line is from the internet to fix an error I could not find in the course
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Facebook login - created and app on facebook develoeprs and then got the App ID
            // Needed to install Microsoft.AspNetCore.Authentication.Facebook in NuGet
            // https://developers.facebook.com/apps/177852273566738/settings/basic/
            // Also need to enable the Facebook login: Dashboard --> Quickstart --> add URL
            // Also needs a "Valid OAuth Redirect URIs in the Dashboard --> PRODUCTS --> Settings section
            services.AddAuthentication().AddFacebook(fb =>
            {
                fb.AppId = "177852273566738";
                fb.AppSecret = "4d285ee6eaf4735a21d1974abfd3db01";
            });

            // Add email - calls the contructor and passing in the sendGrid key
            // Creates and EmailOptions object and sets its value to the key
            // EmailOptions is injected in EmailSender class
            services.AddSingleton<IEmailSender, EmailSender>();
            services.Configure<EmailOptions>(Configuration);    // Grabs EmailOptions value out of appsettings.json

            // added nuget package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
            // Then added .AddRazorRuntimeCompilation() to the end of this service entry.
            services.AddRazorPages().AddRazorRuntimeCompilation();

            // No longer needed in core 3 due to endpoints.MapRazorPages() being added
            //services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // This is new for core 3 and later
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

            // No longer needed because of UserEndpoints MapRazorPages
            // app.UseMvc();
        }
    }
}
