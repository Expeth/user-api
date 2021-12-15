using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace UserAPI.Host.Extensions
{
    public static class AuthServiceCollectionExtensions
    {
        private static readonly string CfgSectionPath = "jwt:publicKeyFileLocation";
        
        public static IServiceCollection AddBearerAuthentication(this IServiceCollection serviceCollection,
            IConfiguration cfg)
        {
            var filePath = cfg.GetSection(CfgSectionPath).Value;
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("There are no pem file for public key.");
            }

            // TODO: Try to find better solution
            var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText(filePath));
            
            serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new RsaSecurityKey(rsa),
                        ValidIssuer = "UserApiJwtFactory",
                        RequireSignedTokens = true,
                        RequireExpirationTime = true,
                        ValidateLifetime = false,
                        ValidateAudience = false,
                        ValidateIssuer = true,
                    };
                });
            
            return serviceCollection;
        }
    }
}