using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using UserAPI.Contracts.Request;
using UserAPI.Contracts.Response;
using UserAPI.Host.IntegrationTests.Common.Builder;

namespace UserAPI.Host.IntegrationTests.Scenario
{
    public class LogoutScenario : Base.Scenario
    {
        [Test]
        public async Task Should_SuccessfullyLogoutUser_When_NotExpiredJwtWithSessionPassed()
        {
            var authenticationRequest = new AuthenticationRequestBuilder()
                .WithLogin(Config.TestData.ValidUser.Username)
                .WithPassword(Config.TestData.ValidUser.Password)
                .Build();

            var authResponse =
                await SendAsJson<AuthenticateUserRequest, AuthenticateUserResponse>("users/authenticate",
                    authenticationRequest);

            var logoutResponse = await SendAsJson("users/logout", authResponse.response.Jwt);
            
            Assert.AreEqual(HttpStatusCode.OK, logoutResponse.code);
        }
        
        [Test]
        public async Task Should_FailLogoutUser_When_ExpiredJwtPassed()
        {
            var expiredJwt = Config.TestData.ValidUser.ExpiredJwt;
            
            var logoutResponse = await SendAsJson("users/logout", expiredJwt);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, logoutResponse.code);
            Assert.IsTrue(logoutResponse.strResponse.Contains("Invalid JWT"));
        }
        
        [Test]
        public async Task Should_FailLogoutUser_When_EndedSessionInJwtPassed()
        {
            var authenticationRequest = new AuthenticationRequestBuilder()
                .WithLogin(Config.TestData.ValidUser.Username)
                .WithPassword(Config.TestData.ValidUser.Password)
                .Build();

            var authResponse =
                await SendAsJson<AuthenticateUserRequest, AuthenticateUserResponse>("users/authenticate",
                    authenticationRequest);

            // Send first logout request to end the session
            await SendAsJson("users/logout", authResponse.response.Jwt);
            
            var logoutResponse = await SendAsJson("users/logout", authResponse.response.Jwt);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, logoutResponse.code);
            Assert.IsTrue(logoutResponse.strResponse.Contains("Invalid session"));
        }
    }
}