using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UserAPI.Application.Common.Abstraction.Factory;
using UserAPI.Domain.Entity;
using UserAPI.Infrastructure.Abstraction;
using UserAPI.Infrastructure.Factory;

namespace UserAPI.Infrastructure.UnitTests
{
    public class JwtFactoryTests
    {
        private static readonly string PrivateKey = "-----BEGIN RSA PRIVATE KEY----- MIICXAIBAAKBgQCJA9M8tdgGL4AwBQ8XZc3v14N8zwP0tS7z3BBJuAtkghv+53e2 9wutDU9S58y5QEkKWWXn46mokcKC7sMbeCks9pvtaMJaERZnDTtrP4VkC7VOB0GI HIaIcryg4nWBZYaPKV3j06jO6BUtQ/2MwqE2N9kBfXb18kFAm8C9TgVwLwIDAQAB AoGAE8Cha1cr1XhzmnigPFdI4RLIue1+PIECS9Wl43rM6ah4ML9d2tqyrDgG/4S7 VtmVrhBFSLDhfJPG3ulc51DjXoi9dmgEyivdsaF6edSIBKenoA5FDPddMw8V8zHb lk1ZWL0ki7M4B2viGjHI9Gs0N9xFMn6FjigyocEuNiM0w0ECQQDpfOM7laFPMK5V cv3IIXq+7Aix8Chpvb1mzFmY6ePGnmIDA6IZEN/JM6t2kMRdXiB9kBImF/+AWLlo rp5YiLYjAkEAljm5m6NzKG5aA4ogUyPu8dhzxzK44tj8IuNtcXaVyrP82h/si+NM AcsD+SBQMd8SrgJNiJNLC1tcZLYDm6rwhQJBAI+a1FetbA08r7y2gQg6LziGC8MF JpYCsR8syF6YXBOpDjc0YNpx2nHxaZ/+4gdbATi5B7COSgMyjranz5Q8YWkCQA9e jofG3DRJvfnYut/msD6cB5Rcsx+6VWl4XS0blc2sRnVGiNvzAEa6r4hgbvP7P5z/ 7VDIyQe7bCN9n7bgcUUCQBxAIsDIgbZYG36ITaVhdRkxdE2mckYsq80kwkLUnRCk Hdw2bpoPeADPgQBgzD2xem9mZK3e5DnAh6wAzF3DouE= -----END RSA PRIVATE KEY-----";
        
        private IJwtFactory _jwtFactory;
        private Mock<IPrivateKeyRepository> _privateKeyRepository;

        [SetUp]
        public void Setup()
        {
            _privateKeyRepository = new Mock<IPrivateKeyRepository>();
            _privateKeyRepository.Setup(i => i.GetAsync())
                .ReturnsAsync(PrivateKey);
            
            _jwtFactory = new JwtFactory(new RsaSecurityKeyFactory(_privateKeyRepository.Object));
        }

        [Test]
        public async Task Should_ReturnSessionClaims_When_SessionEntityPassed()
        {
            var sessionId = Guid.NewGuid().ToString();
            var userId = Guid.NewGuid().ToString();
            var date = DateTime.Now;

            var sessionEntity = new SessionEntity(sessionId, date, userId);
            var jwt = await _jwtFactory.CreateAsync(sessionEntity);

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadJwtToken(jwt);

            var sessionIdClaim = securityToken.Claims.FirstOrDefault(i => i.Type == "SessionID")?.Value;
            var userIdClaim = securityToken.Claims.FirstOrDefault(i => i.Type == "UserID")?.Value;
            
            Assert.AreEqual(sessionId, sessionIdClaim);
            Assert.AreEqual(userId, userIdClaim);
        }
    }
}