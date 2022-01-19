using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using UserAPI.Host.IntegrationTests.Common.Model;

namespace UserAPI.Host.IntegrationTests.Scenario.Base
{
    public abstract class Scenario
    {
        protected readonly Config Config;
        protected readonly HttpClient HttpClient;
        protected readonly IConfiguration Configuration;

        protected HttpClient AuthorizedHttpClient(string bearer) => new()
        {
            BaseAddress = new Uri(Config.Api.UserAPI),
            DefaultRequestHeaders =
            {
                { "Authorization", $"Bearer {bearer}" }
            }
        };

        public Scenario()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json")
                .Build();

            Config = new Config();
            Configuration.Bind(Config);

            HttpClient = new HttpClient
            {
                BaseAddress = new Uri(Config.Api.UserAPI)
            };
        }

        protected Task<(HttpStatusCode code, string strResponse)> SendAsJson(string path, string jwt = null)
        {
            if (!string.IsNullOrEmpty(jwt))
            {
                return SendAsJsonInternal(path, string.Empty, AuthorizedHttpClient(jwt));
            }
            
            return SendAsJsonInternal(path, string.Empty, HttpClient);
        }
        
        protected Task<(HttpStatusCode code, string strResponse, TResponse response)> SendAsJson<TRequest,
            TResponse>(string path, TRequest request, string jwt = null)
        {
            if (!string.IsNullOrEmpty(jwt))
            {
                return SendAsJsonInternal<TRequest, TResponse>(path, request, AuthorizedHttpClient(jwt));
            }
            
            return SendAsJsonInternal<TRequest, TResponse>(path, request, HttpClient);
        }

        protected Task<(HttpStatusCode code, string strResponse)> SendAsJson<TRequest>(string path,
            TRequest request, string jwt = null)
        {
            if (!string.IsNullOrEmpty(jwt))
            {
                return SendAsJsonInternal(path, request, AuthorizedHttpClient(jwt));
            }
            
            return SendAsJsonInternal(path, request, HttpClient);
        }
        
        private async Task<(HttpStatusCode code, string strResponse, TResponse response)> SendAsJsonInternal<TRequest,
            TResponse>(string path, TRequest request, HttpClient httpClient)
        {
            var httpResponse = await SendAsJsonInternal(path, request, httpClient);

            return (httpResponse.code, httpResponse.strResponse,
                JsonConvert.DeserializeObject<TResponse>(httpResponse.strResponse));
        }

        private async Task<(HttpStatusCode code, string strResponse)> SendAsJsonInternal<TRequest>(string path,
            TRequest request, HttpClient httpClient)
        {
            await TestContext.Out.WriteLineAsync($"Request:\n{JsonConvert.SerializeObject(request)}");
            await TestContext.Out.WriteLineAsync($"Sending request to: {httpClient.BaseAddress}");

            var httpResponse = await httpClient.PostAsJsonAsync(path, request);
            var httpResponseMessage = await httpResponse.Content.ReadAsStringAsync();

            await TestContext.Out.WriteLineAsync(
                $"Request finished with: '{httpResponse.StatusCode}'. Response:\n{httpResponseMessage}");

            return (httpResponse.StatusCode, httpResponseMessage);
        }
    }
}