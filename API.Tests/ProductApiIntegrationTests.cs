using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace API.Tests
{
    public class ProductApiIntegrationTests :
        IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProductApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ShouldReturnSuccess()
        {
            var response = await _client.PostAsync(
                "/api/v1/Auth/login?username=admin&password=admin123",
                null);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized()
        {
            var response = await _client.PostAsync(
                "/api/v1/Auth/login?username=test&password=123",
                null);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Products_WithoutToken_ShouldReturn401()
        {
            var response =
                await _client.GetAsync("/api/v1/Products");

            Assert.Equal(HttpStatusCode.Unauthorized,
                response.StatusCode);
        }
    }
}