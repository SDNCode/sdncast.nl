// Copyright (c) .NET Foundation. All rights reserved. 
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
//fanie was here again
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

using sdncast.nl.Formatters;
using sdncast.nl.Services;
using System.Security.Claims;

namespace sdncast.nl
{
    // Force a little update to the App Service 

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddOpenIdConnect(options =>
            {
                options.ClientId = Configuration["Authentication:AzureAd:ClientId"];
                options.Authority = Configuration["Authentication:AzureAd:AADInstance"] + Configuration["Authentication:AzureAd:TenantId"];
                options.ResponseType = OpenIdConnectResponseType.IdToken;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie();

            services.AddAuthorization(options =>
                options.AddPolicy("Admin", policyBuilder =>
                    policyBuilder.RequireClaim(
                        ClaimTypes.Name,
                        Configuration["Authorization:AdminUsers"].Split(',')
                    )
                )
            );

            services.AddMvc(options => options.OutputFormatters.Add(new iCalendarOutputFormatter()))
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizePage("/Admin/Index", "Admin");
                })
                .AddCookieTempDataProvider();


            services.AddCachedWebRoot();
            services.AddSingleton<IStartupFilter, AppStart>();
            services.AddScoped<IShowsService, YouTubeShowsService>();
            services.AddSingleton<IObjectMapper, SimpleMapper>();
            services.AddSingleton<IDeploymentEnvironment, DeploymentEnvironment>();
            services.AddSingleton<IConfigureOptions<ApplicationInsightsServiceOptions>, ApplicationInsightsServiceOptionsSetup>();

            if (string.IsNullOrEmpty(Configuration["AppSettings:AzureStorageConnectionString"]))
            {
                services.AddSingleton<ILiveShowDetailsService, FileSystemLiveShowDetailsService>();
            }
            else
            {
                services.AddSingleton<ILiveShowDetailsService, AzureStorageLiveShowDetailsService>();
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.Use((context, next) => context.Request.Path.StartsWithSegments("/ping")
                ? context.Response.WriteAsync("pong")
                : next()
            );

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Information);
                app.UseExceptionHandler("/error");
            }

            // app.UseHsts();


            app.UseRewriter(new RewriteOptions()
                .AddIISUrlRewrite(env.ContentRootFileProvider, "urlRewrite.config"));

            app.UseStatusCodePages();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
