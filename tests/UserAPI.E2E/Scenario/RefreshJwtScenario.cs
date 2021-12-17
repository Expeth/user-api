using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using UserAPI.Contracts.Request;
using UserAPI.Contracts.Response;
using UserAPI.Host.IntegrationTests.Common.Builder;

namespace UserAPI.Host.IntegrationTests.Scenario
{
    public class RefreshJwtScenario : Base.Scenario
    {
        /// <summary>
        /// Can be run only once.
        /// To re-run this test, please re-run mongo-db-data-seed container, to populate new valid data
        /// </summary>
        [Test]
        public async Task Should_SuccessfullyRefreshJwt_When_ExpiredJwtAndValidRefreshTokenPassed()
        {
            var expiredJwt = Config.TestData.ValidUser.ExpiredJwt;
            var validRefreshToken = Config.TestData.ValidUser.ValidRefreshToken;

            var refreshJwtRequest = new RefreshJwtRequestBuilder()
                .WithRefreshToken(validRefreshToken)
                .Build();

            var refreshJwtResponse =
                await SendAsJson<RefreshJwtRequest, RefreshJwtResponse>("users/refresh", refreshJwtRequest, expiredJwt);
            
            Assert.AreEqual(HttpStatusCode.OK, refreshJwtResponse.code);
            Assert.NotNull(refreshJwtResponse.response.Jwt);
            Assert.NotNull(refreshJwtResponse.response.RefreshToken);
        }
        
        [Test]
        public async Task Should_FailRefreshJwt_When_ExpiredJwtAndUsedRefreshTokenPassed()
        {
            var expiredJwt = Config.TestData.ValidUser.ExpiredJwt;
            var validRefreshToken = Config.TestData.ValidUser.UsedRefreshToken;

            var refreshJwtRequest = new RefreshJwtRequestBuilder()
                .WithRefreshToken(validRefreshToken)
                .Build();

            var refreshJwtResponse =
                await SendAsJson("users/refresh", refreshJwtRequest, expiredJwt);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, refreshJwtResponse.code);
            Assert.IsTrue(refreshJwtResponse.strResponse.Contains("Invalid refresh token"));
        }
        
        [Test]
        public async Task Should_FailRefreshJwt_When_ExpiredJwtAndDeclinedRefreshTokenPassed()
        {
            var expiredJwt = Config.TestData.ValidUser.ExpiredJwt;
            var validRefreshToken = Config.TestData.ValidUser.DeclinedRefreshToken;

            var refreshJwtRequest = new RefreshJwtRequestBuilder()
                .WithRefreshToken(validRefreshToken)
                .Build();

            var refreshJwtResponse =
                await SendAsJson("users/refresh", refreshJwtRequest, expiredJwt);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, refreshJwtResponse.code);
            Assert.IsTrue(refreshJwtResponse.strResponse.Contains("Invalid refresh token"));
        }
        
        [Test]
        public async Task Should_FailRefreshJwt_When_ExpiredJwtAndExpiredRefreshTokenPassed()
        {
            var expiredJwt = Config.TestData.ValidUser.ExpiredJwt;
            var validRefreshToken = Config.TestData.ValidUser.ExpiredRefreshToken;

            var refreshJwtRequest = new RefreshJwtRequestBuilder()
                .WithRefreshToken(validRefreshToken)
                .Build();

            var refreshJwtResponse =
                await SendAsJson("users/refresh", refreshJwtRequest, expiredJwt);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, refreshJwtResponse.code);
            Assert.IsTrue(refreshJwtResponse.strResponse.Contains("Invalid refresh token"));
        }
        
        [Test]
        public async Task Should_FailRefreshJwt_When_ExpiredJwtAndDifferentUserRefreshTokenPassed()
        {
            var expiredJwt = Config.TestData.ValidUser.ExpiredJwt;
            var validRefreshToken = Config.TestData.ValidUser.DifferentUserRefreshToken;

            var refreshJwtRequest = new RefreshJwtRequestBuilder()
                .WithRefreshToken(validRefreshToken)
                .Build();

            var refreshJwtResponse =
                await SendAsJson("users/refresh", refreshJwtRequest, expiredJwt);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, refreshJwtResponse.code);
            Assert.IsTrue(refreshJwtResponse.strResponse.Contains("Invalid refresh token"));
        }
        
        [Test]
        public async Task Should_FailRefreshJwt_When_NotExpiredJwtAndValidRefreshTokenPassed()
        {
            var notExpiredJwt = Config.TestData.ValidUser.NotExpiredJwt;
            var validRefreshToken = Config.TestData.ValidUser.ValidRefreshToken;

            var refreshJwtRequest = new RefreshJwtRequestBuilder()
                .WithRefreshToken(validRefreshToken)
                .Build();

            var refreshJwtResponse =
                await SendAsJson("users/refresh", refreshJwtRequest, notExpiredJwt);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, refreshJwtResponse.code);
            Assert.IsTrue(refreshJwtResponse.strResponse.Contains("Invalid JWT"));
        }

        [Test]
        public async Task Should_FailRefreshJwt_When_ExpiredJwtAndRefreshTokenDoesntMatchCurrentSessionPassed()
        {
            var expiredJwt = Config.TestData.ValidUser.ExpiredJwt;
            var validRefreshToken = Config.TestData.ValidUser.DoesntMatchSessionRefreshToken;

            var refreshJwtRequest = new RefreshJwtRequestBuilder()
                .WithRefreshToken(validRefreshToken)
                .Build();

            var refreshJwtResponse =
                await SendAsJson("users/refresh", refreshJwtRequest, expiredJwt);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, refreshJwtResponse.code);
            Assert.IsTrue(refreshJwtResponse.strResponse.Contains("Invalid refresh token"));
        }
    }
}