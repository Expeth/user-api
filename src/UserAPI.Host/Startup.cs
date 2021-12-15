using System.Linq;
using System.Security.Cryptography;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using UserAPI.Application.Common;
using UserAPI.Application.Common.Abstraction.Factory;
using UserAPI.Application.Common.Abstraction.Repository;
using UserAPI.Application.Common.Abstraction.Service;
using UserAPI.Infrastructure.Abstraction;
using UserAPI.Infrastructure.Factory;
using UserAPI.Infrastructure.Repository;
using UserAPI.Infrastructure.Service;
using Formatting = Newtonsoft.Json.Formatting;

namespace UserAPI.Host
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
            services.AddMediatR(typeof(IApplication).Assembly)
                .AddMemoryCache()
                .AddValidatorsFromAssembly(typeof(IApplication).Assembly)
                .AddTransient<IPasswordService, Pbkdf2PasswordService>()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IPrivateKeyRepository, PrivateKeyRepository>()
                .AddSingleton<IJwtFactory, JwtFactory>()
                .AddSingleton<ISigningCredentialsFactory, RsaSecurityKeyFactory>()
                .AddSingleton<IRefreshTokenFactory, RefreshTokenFactory>()
                .AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>()
                .AddSingleton(_ =>
                    new MongoClient(Configuration.GetConnectionString("MongoDB")).GetDatabase("UserAPI"));

            // TODO: move to ext project
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                var rsa = RSA.Create();
                rsa.ImportFromPem("-----BEGIN PUBLIC KEY----- MIGeMA0GCSqGSIb3DQEBAQUAA4GMADCBiAKBgGbHjnvrVtk47Ek0vt83BSIlEcmC sLEDliypcE+tqtrtoudkeFafaAsuCrnJq9yc1JYLhbYv5zELrVfT+Z3cOweblkzD rL1R6TxgELnMCvqAcLjX/246N/6RdWS0br0qRaEnsl9Z/QtEMTdMoAE0BKEHZrOp OdTX37DrHomGejnNAgMBAAE= -----END PUBLIC KEY-----");

                options.IncludeErrorDetails = true;
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    ValidIssuer = "JwtFactory",
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = false,
                    ValidateAudience = false,
                    ValidateIssuer = true,
                };
            });

            services.AddHealthChecks()
                .AddCheck("app", () => HealthCheckResult.Healthy());

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserAPI.Host", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UserAPI.Host v1"));
            
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
                // TODO: move to ext project
                endpoints.MapHealthChecks("/healthcheck", new HealthCheckOptions
                {
                    ResponseWriter = (context, report) =>
                    {
                        context.Response.ContentType = "application/json";

                        var json = new JObject(
                            new JProperty("status", report.Status.ToString()),
                            new JProperty("results", new JObject(report.Entries.Select(pair =>
                                new JProperty(pair.Key, new JObject(
                                    new JProperty("status", pair.Value.Status.ToString()),
                                    new JProperty("description", pair.Value.Description),
                                    new JProperty("data", new JObject(pair.Value.Data.Select(
                                        p => new JProperty(p.Key, p.Value))))))))));

                        return context.Response.WriteAsync(
                            json.ToString(Formatting.Indented));
                    }
                });
            });
        }
    }
}
