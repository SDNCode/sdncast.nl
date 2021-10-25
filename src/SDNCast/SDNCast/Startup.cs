using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SDNCast.Services;

namespace SDNCast
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddRazorPages();
            services.AddServerSideBlazor();

            if (string.IsNullOrEmpty(Configuration["AppSettings:AzureStorageConnectionString"]))
            {
                services.AddSingleton<ILiveShowDetailsService, FileSystemLiveShowDetailsService>();
            }
            else
            {
                services.AddSingleton<ILiveShowDetailsService, AzureStorageLiveShowDetailsService>();
            }
            services.AddSingleton<IObjectMapper, SimpleMapper>();
            services.AddScoped<IShowsService, YouTubeShowsService>();
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Security Pointers for reference:
            // https://www.hanselman.com/blog/easily-adding-security-headers-to-your-aspnet-core-web-app-and-getting-an-a-grade

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts(options => options.MaxAge(days: 30));
            app.UseHttpsRedirection();

            app.UseRewriter(new RewriteOptions()
                .AddRedirectToWww()
                .AddIISUrlRewrite(env.ContentRootFileProvider, "urlRewrite.config"));

            //Registered before static files to always set header
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(options => options.NoReferrerWhenDowngrade());

            app.UseCsp(options => options
                .DefaultSources(s => s.Self()
                    .CustomSources("data:")
                    .CustomSources("https:"))
                .StyleSources(s => s.Self()
                    .CustomSources("www.google.com", "platform.twitter.com", "cdn.syndication.twimg.com", "fonts.googleapis.com")
                    .UnsafeInline()
                )
                .ScriptSources(s => s.Self()
                    .CustomSources("www.google.com", "cse.google.com", "cdn.syndication.twimg.com", "platform.twitter.com", "code.jquery.com")
                    .UnsafeInline()
                    .UnsafeEval()
                )
                .ImageSources(s => s.Self()
                    .CustomSources("data:")
                    .CustomSources("i.ytimg.com", "avatars0.githubusercontent.com", "avatars1.githubusercontent.com", "pbs.twimg.com", "reynders.co")
                )
            );

            app.UseStaticFiles();

            //Registered after static files, to set headers for dynamic content.
            app.UseXfo(options => options.SameOrigin());

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            // Permissions-Policy (previously known as Feature-Policy
            // https://scotthelme.co.uk/goodbye-feature-policy-and-hello-permissions-policy/
            // https://jonathancrozier.com/blog/stepping-up-the-security-of-asp-net-core-web-apps-with-security-headers
            app.Use(async (context, next) =>
            {
                // does not appear to work (yet?)
                // https://securityheaders.com/
                context.Response.Headers.Add("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
                await next.Invoke();
            });
        }
    }
}
