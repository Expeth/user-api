using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using UserAPI.Application.Common.Extension;
using UserAPI.Domain.ValueObject;

namespace UserAPI.Host.Extensions
{
    public static class ParsingExtensions
    {
        public static JwtClaims ToClaimsObject(this IEnumerable<Claim> claims)
        {
            var userId = claims.FirstOrDefault(i => i.Type == "UserID").Value;
            var exp = claims.FirstOrDefault(i => i.Type == "exp").Value;
            var iat = claims.FirstOrDefault(i => i.Type == "iat").Value;
            
            return new JwtClaims(userId, exp.FromTimestamp(), iat.FromTimestamp());
        }
    }
}