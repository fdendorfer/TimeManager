using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TimeManager {
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      services.Configure<IdentityOptions>(options => {
        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 10;
      });

      // Cookie Authentication
      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
      // Claims Authorization
      services.AddAuthorization(options => {
        options.AddPolicy("IdPermission", policy => policy.RequireClaim("IdPermission"));
        //options.AddPolicy("PermissionNormal", policy => policy.AddRequirements());
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      } else {
        app.UseExceptionHandler("/Error");
        app.UseHsts();  // Forces HTTPS
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseAuthentication();

      app.UseMvc();
    }
  }
}
