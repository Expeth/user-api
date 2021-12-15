using System.Collections.Concurrent;
using Bogus;
using UserAPI.Contracts.Request;

namespace UserAPI.Host.IntegrationTests.Common.Builder
{
    public class RegistrationRequestBuilder
    {
        private readonly ConcurrentDictionary<string, string> _requestProperties;
        private readonly Faker _faker;
        
        public RegistrationRequestBuilder()
        {
            _faker = new Faker();
            _requestProperties = new ConcurrentDictionary<string, string>();
        }

        public RegistrationRequestBuilder WithUsername(string username)
        {
            _requestProperties.AddOrUpdate("username", username, (_, _) => username);
            return this;
        }
        
        public RegistrationRequestBuilder WithEmail(string email)
        {
            _requestProperties.AddOrUpdate("email", email, (_, _) => email);
            return this;
        }
        
        public RegistrationRequestBuilder WithPassword(string password)
        {
            _requestProperties.AddOrUpdate("password", password, (_, _) => password);
            return this;
        }

        public RegisterUserRequest Build()
        {
            var username = _requestProperties.GetOrAdd("username", _faker.Person.UserName);
            var email = _requestProperties.GetOrAdd("email", _faker.Person.Email);
            var password = _requestProperties.GetOrAdd("password", _faker.Internet.Password() + "aF1!");
            var fName = _requestProperties.GetOrAdd("fName", _faker.Person.FirstName);
            var mName = _requestProperties.GetOrAdd("mName", _faker.Person.LastName);
            var lName = _requestProperties.GetOrAdd("lName", _faker.Person.LastName);

            return new RegisterUserRequest(email, username, password, fName, lName, mName);
        }
    }
}