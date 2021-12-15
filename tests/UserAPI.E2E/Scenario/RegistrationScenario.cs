using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using UserAPI.Contracts.Request;
using UserAPI.Contracts.Response;
using UserAPI.Host.IntegrationTests.Common.Builder;
using UserAPI.Host.IntegrationTests.Scenarios.Base;

namespace UserAPI.Host.IntegrationTests.Scenarios
{
    public class RegistrationScenario : Scenario
    {
        [Test]
        public async Task Should_SuccessfullyRegisterNewUser_When_ValidDataPassed()
        {
            var registrationRequest = new RegistrationRequestBuilder().Build();

            var regResponse =
                await SendAsJson<RegisterUserRequest, RegisterUserResponse>("users/register", registrationRequest);
            
            Assert.AreEqual(HttpStatusCode.OK, regResponse.code);
            Assert.NotNull(regResponse.response.UserId, "Actual was: " + regResponse.strResponse);
        }

        [Test]
        public async Task Should_FailRegistration_When_ExistingEmailPassed()
        {
            var registrationRequest = new RegistrationRequestBuilder()
                .WithEmail(Config.TestData.ValidUser.Email)
                .Build();

            var regResponse = await SendAsJson("users/register", registrationRequest);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, regResponse.code);
            Assert.IsTrue(regResponse.strResponse.Contains("User should be unique"),
                $"Actual was: {regResponse.strResponse}");
        }
        
        [Test]
        public async Task Should_FailRegistration_When_ExistingUsernamePassed()
        {
            var registrationRequest = new RegistrationRequestBuilder()
                .WithUsername(Config.TestData.ValidUser.Username)
                .Build();

            var regResponse = await SendAsJson("users/register", registrationRequest);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, regResponse.code);
            Assert.IsTrue(regResponse.strResponse.Contains("User should be unique"),
                $"Actual was: {regResponse.strResponse}");
        }
        
        [Test]
        public async Task Should_FailRegistration_When_ExistingUsernameInDifferentCasePassed()
        {
            var username = Config.TestData.ValidUser.Username.ToCharArray();
            username[0] = char.ToUpper(username[0]);
            username[^1] = char.ToUpper(username[^1]);
            
            var registrationRequest = new RegistrationRequestBuilder()
                .WithUsername(new string(username))
                .Build();

            var regResponse = await SendAsJson("users/register", registrationRequest);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, regResponse.code);
            Assert.IsTrue(regResponse.strResponse.Contains("User should be unique"),
                $"Actual was: {regResponse.strResponse}");
        }
        
        [Test]
        public async Task Should_FailRegistration_When_ExistingEmailInDifferentCasePassed()
        {
            var email = Config.TestData.ValidUser.Email.ToCharArray();
            email[0] = char.ToUpper(email[0]);
            email[^1] = char.ToUpper(email[^1]);
            
            var registrationRequest = new RegistrationRequestBuilder()
                .WithEmail(new string(email))
                .Build();

            var regResponse = await SendAsJson("users/register", registrationRequest);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, regResponse.code);
            Assert.IsTrue(regResponse.strResponse.Contains("User should be unique"),
                $"Actual was: {regResponse.strResponse}");
        }

        [Theory]
        [TestCase("notemail.com")]
        [TestCase("notemail@.com")]
        [TestCase("notemail@com")]
        [TestCase("notemail@com.")]
        public async Task Should_FailRegistration_When_InvalidEmailPassed(string email)
        {
            var registrationRequest = new RegistrationRequestBuilder()
                .WithEmail(email)
                .Build();

            var regResponse = await SendAsJson("users/register", registrationRequest);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, regResponse.code);
            Assert.IsTrue(regResponse.strResponse.Contains("The specified condition was not met for 'Email'"),
                $"Actual was: {regResponse.strResponse}");
        }
        
        [Theory]
        [TestCase("123456")]
        [TestCase("123")]
        [TestCase("123!fF")]
        [TestCase("123456789")]
        [TestCase("aD1!#fF")]
        public async Task Should_FailRegistration_When_InvalidPasswordPassed(string password)
        {
            var registrationRequest = new RegistrationRequestBuilder()
                .WithPassword(password)
                .Build();

            var regResponse = await SendAsJson("users/register", registrationRequest);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, regResponse.code);
            Assert.IsTrue(regResponse.strResponse.Contains("The specified condition was not met for 'Password'"),
                $"Actual was: {regResponse.strResponse}");
        }
        
        [Theory]
        [TestCase("_.fdfsdfds")]
        [TestCase("._fdfsdfds")]
        [TestCase("_fdsfds")]
        [TestCase(".fdsfd")]
        [TestCase("fdsfsd..fdsfdsf")]
        [TestCase("fdsfds._fsdfds")]
        [TestCase("fdsfds_.fsdfds")]
        [TestCase("fdsfds__fsdfds")]
        [TestCase("fdsfds_")]
        [TestCase("fdsfds.")]
        public async Task Should_FailRegistration_When_InvalidUsernamePassed(string username)
        {
            var registrationRequest = new RegistrationRequestBuilder()
                .WithUsername(username)
                .Build();

            var regResponse = await SendAsJson("users/register", registrationRequest);
            
            Assert.AreEqual(HttpStatusCode.BadRequest, regResponse.code);
            Assert.IsTrue(regResponse.strResponse.Contains("The specified condition was not met for 'Username'"),
                $"Actual was: {regResponse.strResponse}");
        }
    }
}