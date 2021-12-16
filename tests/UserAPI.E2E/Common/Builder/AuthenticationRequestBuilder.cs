using System.Collections.Concurrent;
using Bogus;
using UserAPI.Contracts.Request;

namespace UserAPI.Host.IntegrationTests.Common.Builder
{
    public class AuthenticationRequestBuilder
    {
        private readonly ConcurrentDictionary<string, string> _requestProperties;
        private readonly Faker _faker;
        
        public AuthenticationRequestBuilder()
        {
            _faker = new Faker();
            _requestProperties = new ConcurrentDictionary<string, string>();
        }

        public AuthenticationRequestBuilder WithLogin(string login)
        {
            _requestProperties.AddOrUpdate("login", login, (_, _) => login);
            return this;
        }

        public AuthenticationRequestBuilder WithPassword(string password)
        {
            _requestProperties.AddOrUpdate("password", password, (_, _) => password);
            return this;
        }

        public AuthenticateUserRequest Build()
        {
            var login = _requestProperties.GetOrAdd("login", _faker.Person.UserName);
            var password = _requestProperties.GetOrAdd("password", _faker.Internet.Password() + "aF1!");

            return new AuthenticateUserRequest(login,password);
        }
    }
}