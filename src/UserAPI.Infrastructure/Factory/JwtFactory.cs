using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserAPI.Application.Common.Abstraction.Factory;
using UserAPI.Domain.Entity;
using UserAPI.Infrastructure.Abstraction;
using UserAPI.Infrastructure.Model;

namespace UserAPI.Infrastructure.Factory
{
    public class JwtFactory: IJwtFactory
    {
        private readonly ISigningCredentialsFactory _signingCredentialsFactory;
        private readonly JwtCfg _jwtConfig;

        public JwtFactory(ISigningCredentialsFactory signingCredentialsFactory, IOptions<JwtCfg> cfg)
        {
            _signingCredentialsFactory = signingCredentialsFactory;
            _jwtConfig = cfg.Value;
        }

        public async Task<string> CreateAsync(SessionEntity sessionEntity)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("SessionID", sessionEntity.Id));
            claims.Add(new Claim("UserID", sessionEntity.UserId));

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = "UserApiJwtFactory",
                IssuedAt = now,
                NotBefore = now,
                Expires = now + _jwtConfig.Lifetime,
                SigningCredentials = await _signingCredentialsFactory.CreateAsync()
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}