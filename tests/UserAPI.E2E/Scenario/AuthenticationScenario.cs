using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using UserAPI.Contracts.Request;
using UserAPI.Contracts.Response;
using UserAPI.Host.IntegrationTests.Common.Builder;

namespace UserAPI.Host.IntegrationTests.Scenario
{
    public class AuthenticationScenario : Base.Scenario
    {
        [Test]
        public async Task Should_SuccessfullyAuthenticateUserByUsername_When_ValidUsernameAndPasswordPassed()
        {
            var authenticationRequest = new AuthenticationRequestBuilder()
                .WithLogin(Config.TestData.ValidUser.Username)
                .WithPassword(Config.TestData.ValidUser.Password)
                .Build();

            var authResponse =
                await SendAsJson<AuthenticateUserRequest, AuthenticateUserResponse>("users/authenticate",
                    authenticationRequest);
            
            Assert.AreEqual(HttpStatusCode.OK, authResponse.code);
            Assert.NotNull(authResponse.response.Jwt);
            Assert.NotNull(authResponse.response.RefreshToken);
        }
        
        [Test]
        public async Task Should_SuccessfullyAuthenticateUserByEmail_When_ValidEmailAndPasswordPassed()
        {
            var authenticationRequest = new AuthenticationRequestBuilder()
                .WithLogin(Config.TestData.ValidUser.Email)
                .WithPassword(Config.TestData.ValidUser.Password)
                .Build();

            var authResponse =
                await SendAsJson<AuthenticateUserRequest, AuthenticateUserResponse>("users/authenticate",
                    authenticationRequest);
            
            Assert.AreEqual(HttpStatusCode.OK, authResponse.code);
            Assert.NotNull(authResponse.response.Jwt);
            Assert.NotNull(authResponse.response.RefreshToken);
        }
        
        [Test]
        public async Task Should_SuccessfullyAuthenticateUserRightAfterRegistration_When_ValidCredentialsPassed()
        {
            var registrationRequest = new RegistrationRequestBuilder().Build();
            var regResponse =
                await SendAsJson<RegisterUserRequest, RegisterUserResponse>("users/register", registrationRequest);
            
            Assert.AreEqual(HttpStatusCode.OK, regResponse.code);
            Assert.NotNull(regResponse.response.UserId);

            var authenticationRequest = new AuthenticationRequestBuilder()
                .WithLogin(registrationRequest.Email)
                .WithPassword(registrationRequest.Password)
                .Build();

            var authResponse =
                await SendAsJson<AuthenticateUserRequest, AuthenticateUserResponse>("users/authenticate",
                    authenticationRequest);
            
            Assert.AreEqual(HttpStatusCode.OK, authResponse.code);
            Assert.NotNull(authResponse.response.Jwt);
            Assert.NotNull(authResponse.response.RefreshToken);
        }
        
        [Test]
        public async Task Should_FailAuthenticationByEmail_When_ValidEmailAndInvalidPasswordPassed()
        {
            var authenticationRequest = new AuthenticationRequestBuilder()
                .WithLogin(Config.TestData.ValidUser.Email)
                .WithPassword(Config.TestData.ValidUser.Password + "dummyStr123!")
                .Build();

            var authResponse = await SendAsJson("users/authenticate", authenticationRequest);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, authResponse.code);
            Assert.IsTrue(authResponse.strResponse.Contains("Invalid Credentials"));
        }
        
        [Test]
        public async Task Should_FailAuthenticationByUsername_When_ValidUsernameAndInvalidPasswordPassed()
        {
            var authenticationRequest = new AuthenticationRequestBuilder()
                .WithLogin(Config.TestData.ValidUser.Username)
                .WithPassword(Config.TestData.ValidUser.Password + "dummyStr123!")
                .Build();

            var authResponse = await SendAsJson("users/authenticate", authenticationRequest);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, authResponse.code);
            Assert.IsTrue(authResponse.strResponse.Contains("Invalid Credentials"));
        }
    }
}