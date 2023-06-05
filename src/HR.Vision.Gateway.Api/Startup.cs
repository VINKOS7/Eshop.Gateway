using E.Shop.Gateway.Api.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace E.Shop.Gateway.Api
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
            services.Configure<AuthoritySettings>(Configuration.GetSection("Authority"));
            
            services.AddControllers()
                .AddNewtonsoftJson();
            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.SetIsOriginAllowedToAllowWildcardSubdomains();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                        builder.AllowCredentials();
                        builder.WithOrigins(
                            "chrome-extension://*",
                            "http://localhost:3103",
                            "http://localhost:3104",
                            "http://localhost:3000",
                            "http://localhost:3900",
                            "https://qa.app.hr.vision",
                            "https://app.hr.vision");
                    });
            });
            
            services.AddSignalR();
            
            var authoritySettings = Configuration.GetSection("Authority").Get<AuthoritySettings>();

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options => {
                    options.Authority = authoritySettings.Url;
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                    
                    options.RequireHttpsMetadata = false;
                });

            services
                .AddOcelot();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.UseWebSockets();
            
            app.UseOcelot().Wait();
        }
    }
}