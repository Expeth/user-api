using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using UserAPI.Application.Common;
using UserAPI.Application.Common.Abstraction.Factory;
using UserAPI.Application.Common.Abstraction.Repository;
using UserAPI.Application.Common.Abstraction.Service;
using UserAPI.Infrastructure.Abstraction;
using UserAPI.Infrastructure.Factory;
using UserAPI.Infrastructure.Repository;
using UserAPI.Infrastructure.Service;

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
                .AddSingleton<IUserRepository, DbUserRepository>()
                .AddSingleton<IPrivateKeyRepository, PrivateKeyRepository>()
                .AddSingleton<IJwtFactory, JwtFactory>()
                .AddSingleton<ISigningCredentialsFactory, RsaSecurityKeyFactory>()
                .AddSingleton(_ => new MongoClient("mongodb://localhost:27017").GetDatabase("UserAPI"));

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

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
