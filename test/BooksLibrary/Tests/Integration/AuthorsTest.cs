using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Xunit;
using BooksLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;

namespace BooksLibrary.Tests.Integration
{
    public class AuthorsIntegrationTest : IntegrationTestsBase<Startup>
    {

        [Fact]
        public async void TestGetAuthors()
        {

            // Arrange
            var clientId = "cl-key-b";
            var ip = "::1";

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, "api/authors");
            request.Headers.Add("X-ClientId", clientId);
            request.Headers.Add("X-Real-IP", ip);

            var response = await this.Client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            
            string raw = await response.Content.ReadAsStringAsync();
            List<AuthorDto> outputModel = JsonConvert.DeserializeObject<List<AuthorDto>>(raw);

            // Assert
            Assert.Equal(6, outputModel.Count);
        }
    }
}
