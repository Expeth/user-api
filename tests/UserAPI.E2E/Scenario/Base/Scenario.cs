using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using UserAPI.Host.IntegrationTests.Common.Model;

namespace UserAPI.Host.IntegrationTests.Scenarios.Base
{
    public abstract class Scenario
    {
        protected readonly Config Config;
        protected readonly HttpClient HttpClient;
        protected readonly IConfiguration Configuration;

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

        protected async Task<(HttpStatusCode code, string strResponse, TResponse response)> SendAsJson<TRequest,
            TResponse>(string path, TRequest request)
        {
            var httpResponse = await SendAsJson(path, request);

            return (httpResponse.code, httpResponse.strResponse,
                JsonConvert.DeserializeObject<TResponse>(httpResponse.strResponse));
        }

        protected async Task<(HttpStatusCode code, string strResponse)> SendAsJson<TRequest>(string path,
            TRequest request)
        {
            await TestContext.Out.WriteLineAsync($"Request:\n{JsonConvert.SerializeObject(request)}");
            await TestContext.Out.WriteLineAsync($"Sending request to: {HttpClient.BaseAddress}");

            var httpResponse = await HttpClient.PostAsJsonAsync(path, request);
            var httpResponseMessage = await httpResponse.Content.ReadAsStringAsync();

            await TestContext.Out.WriteLineAsync(
                $"Request finished with: '{httpResponse.StatusCode}'. Response:\n{httpResponseMessage}");

            return (httpResponse.StatusCode, httpResponseMessage);
        }
    }
}