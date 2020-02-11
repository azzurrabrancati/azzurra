using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SecuredWebApi
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
            services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
                .AddAzureADBearer(options => Configuration.Bind("AzureAd", options));


            //Configure Authentication Service
            services.AddAuthentication(sharedoptions =>
            {
                sharedoptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Audience = Configuration.GetSection("AzureAd")["ClientId"];
                options.Authority = $"{Configuration.GetSection("AzureAd")["Instance"]}{Configuration.GetSection("AzureAd")["TenantId"]}";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudiences = new List<string> { Configuration.GetSection("AzureAd")["AppIDURL"], Configuration.GetSection("AzureAd")["ClientId"] },
                    // set the following issuers if you want to support both V1 and V2 issuers
                    ValidIssuers = new List<string> {
                                $"{Configuration.GetSection("AzureAd")["Issuer"]}{Configuration.GetSection("AzureAd")["TenantId"]}",
                                $"{Configuration.GetSection("AzureAd")["Issuer"]}{Configuration.GetSection("AzureAd")["TenantId"]}{"/v2.0"}"
                            }
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddControllers();
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

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
