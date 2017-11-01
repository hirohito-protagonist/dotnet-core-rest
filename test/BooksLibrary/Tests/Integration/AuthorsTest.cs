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

        private void AddRequestIpLimitHeaders(HttpRequestMessage request)
        {
            var clientId = "cl-key-b";
            var ip = "::1";

            request.Headers.Add("X-ClientId", clientId);
            request.Headers.Add("X-Real-IP", ip);
        }

        private async Task<AuthorDto> CreateDummyAuthor()
        {
            var content = new StringContent(JsonConvert.SerializeObject(new AuthorCreationDto()),
                                    Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "api/authors");
            request.Content = content;
            this.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);

            string rawAuthor = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<AuthorDto>(rawAuthor);
        }

        [Fact(DisplayName = "it should return collection of authors")]
        public async void TestGetAuthors()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/authors");
            this.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            
            string raw = await response.Content.ReadAsStringAsync();
            List<AuthorDto> outputModel = JsonConvert.DeserializeObject<List<AuthorDto>>(raw);

            Assert.Equal(6, outputModel.Count);
        }

        [Fact(DisplayName = "it should return 404 not found status code when author could not be find by id")]
        public async void TestGetAuthorWithInvalidId()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/authors/123test");
            this.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "it should find author for specific id")]
        public async void TestGetAuthorWithValidId()
        {
            AuthorDto author = await CreateDummyAuthor();

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/authors/{author.Id}");
            this.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


       [Fact(DisplayName = "it should delete author for specific id")]
        public async void TestDeleteAuthorWithValidId()
        {
            AuthorDto author = await CreateDummyAuthor();

            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/authors/{author.Id}");
            this.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

       [Fact(DisplayName = "it should return 404 status code on delete author action when can not find author for specific id")]
        public async void TestDeleteAuthorWithInValidId()
        {

            var request = new HttpRequestMessage(HttpMethod.Delete, "api/authors/123test");
            this.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact(DisplayName = "it should create author and return 201 status code on success")]
        public async void TestAuthorCreation()
        {
            
            var content = new StringContent(JsonConvert.SerializeObject(new AuthorCreationDto()),
                                    Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "api/authors");
            request.Content = content;
            this.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);

            Assert.Equal(201, (int)response.StatusCode);
        }
    }
}
