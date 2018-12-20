using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
      services.AddMvc()
        .AddRazorPagesOptions(options => {
          options.Conventions.AuthorizeFolder("/", "PermissionNormal");
          options.Conventions.AuthorizeFolder("/Controlling", "PermissionAdvanced");
          options.Conventions.AuthorizeFolder("/Users", "PermissionHigh");
          // Set all permissions for the pages
          options.Conventions.AllowAnonymousToPage("/Index");
          options.Conventions.AllowAnonymousToPage("/Error");
        })
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      // Cookie Authentication
      services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
        options.LoginPath = "/Index";
        options.AccessDeniedPath = "/Error";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
      });

      // Roles Authorization
      services.AddAuthorization(options => {
        options.DefaultPolicy = new AuthorizationPolicyBuilder("Cookies").RequireAuthenticatedUser().Build();
        options.AddPolicy("PermissionNormal", policy => policy.RequireRole("Normal", "Advanced", "High"));
        options.AddPolicy("PermissionAdvanced", policy => policy.RequireRole("Advanced", "High"));
        options.AddPolicy("PermissionHigh", policy => policy.RequireRole("High"));
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

      var cookieOptions = new CookieAuthenticationOptions();
      cookieOptions.Cookie.HttpOnly = false;
      app.UseAuthentication();

      app.UseMvc();
    }
  }
}
