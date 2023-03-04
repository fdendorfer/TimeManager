using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.Formula.Atp;
using TimeManager.Database;

namespace TimeManager
{
  public class Startup
  {

    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      // Adds Accept-Compression Header to HTTP requests / Adds compression for brotli, gzip and all needed mime types by default
      services.AddResponseCompression(options => {
        options.EnableForHttps = true;
      });
      
      services.AddMvc().AddRazorPagesOptions(options =>
        {
          // Set all permissions for the pages
          options.Conventions.AllowAnonymousToPage("/Index");
          options.Conventions.AllowAnonymousToPage("/Error");

          options.Conventions.AuthorizePage("/OwnTimes", "PermissionNormal");

          options.Conventions.AuthorizePage("/Controlling", "PermissionManager");

          options.Conventions.AuthorizePage("/Users", "PermissionAdmin");
        }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      // Cross-site request blocking
      services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

      // Cookie Authentication
      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
      {
        options.LoginPath = "/Index";
        options.AccessDeniedPath = "/Shared/Error";
        //options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
      });

      // Roles Authorization
      services.AddAuthorization(options =>
      {
        options.DefaultPolicy = new AuthorizationPolicyBuilder("Cookies").RequireAuthenticatedUser().Build();
        options.AddPolicy("PermissionNormal", policy => policy.RequireRole("Normal", "Manager", "Admin"));
        options.AddPolicy("PermissionManager", policy => policy.RequireRole("Manager", "Admin"));
        options.AddPolicy("PermissionAdmin", policy => policy.RequireRole("Admin"));
      });

      // Database service
      services.AddDbContext<DatabaseContext>(options =>
      {
        options.UseSqlServer(ConfigurationBinder.GetValue<string>(Configuration, "ConnectionString"));
        options.EnableSensitiveDataLogging(true);
      });

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      } else
      {
        app.UseExceptionHandler("/Shared/Error");
        app.UseHsts();  // Forces HTTPS
      }

      app.UseResponseCompression();
      app.UseStaticFiles(new StaticFileOptions
      {
        OnPrepareResponse = ctx =>
        {
          ctx.Context.Response.Headers.Add("cache-control", "public, max-age=31536000");
        }
      });

      var cookieOptions = new CookieAuthenticationOptions();
      cookieOptions.Cookie.HttpOnly = false;
      app.UseAuthentication();

      app.UseMvc();
    }
  }
}
