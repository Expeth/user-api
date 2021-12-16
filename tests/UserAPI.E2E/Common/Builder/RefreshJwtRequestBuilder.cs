using System;
using System.Collections.Concurrent;
using Bogus;
using UserAPI.Contracts.Request;

namespace UserAPI.Host.IntegrationTests.Common.Builder
{
    public class RefreshJwtRequestBuilder
    {
        private readonly ConcurrentDictionary<string, string> _requestProperties;
        
        public RefreshJwtRequestBuilder()
        {
            _requestProperties = new ConcurrentDictionary<string, string>();
        }

        public RefreshJwtRequestBuilder WithRefreshToken(string token)
        {
            _requestProperties.AddOrUpdate("token", token, (_, _) => token);
            return this;
        }

        public RefreshJwtRequest Build()
        {
            var token = _requestProperties.GetOrAdd("token", Guid.NewGuid().ToString);

            return new RefreshJwtRequest(token);
        }
    }
}