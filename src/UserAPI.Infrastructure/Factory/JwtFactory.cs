using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using UserAPI.Application.Common.Abstraction.Factory;
using UserAPI.Domain.Entity;
using UserAPI.Infrastructure.Abstraction;

namespace UserAPI.Infrastructure.Factory
{
    public class JwtFactory: IJwtFactory
    {
        private readonly ISigningCredentialsFactory _signingCredentialsFactory;

        public JwtFactory(ISigningCredentialsFactory signingCredentialsFactory)
        {
            _signingCredentialsFactory = signingCredentialsFactory;
        }

        public async Task<string> CreateAsync(SessionEntity sessionEntity)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("SessionID", sessionEntity.Id));
            claims.Add(new Claim("UserID", sessionEntity.UserId));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = "UserApiJwtFactory",
                IssuedAt = sessionEntity.CreationTime,
                NotBefore = sessionEntity.CreationTime,
                Expires = sessionEntity.CreationTime + TimeSpan.FromMilliseconds(1),
                SigningCredentials = await _signingCredentialsFactory.CreateAsync()
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}