using System;
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
    public class BooksIntegrationTest : IntegrationTestsBase<Startup>
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

        private async Task<BookDto> CreateDummyBookForAuthor(Guid authorId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new BookManipulationDto()
            {
                Title = "test",
                Description = "test"
            }), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "api/authors/" + authorId + "/books");
            request.Content = content;
            this.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);

            string rawBook = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<BookDto>(rawBook);
        }

        [Fact(DisplayName = "it should return collection of books for author")]
        public async void TestGetBooks()
        {
            var author = await CreateDummyAuthor();
            var book1 = await CreateDummyBookForAuthor(author.Id);
            var book2 = await CreateDummyBookForAuthor(author.Id);

            var request = new HttpRequestMessage(HttpMethod.Get, "api/authors/" + author.Id + "/books");
            this.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            
            string raw = await response.Content.ReadAsStringAsync();
            List<BookDto> outputModel = JsonConvert.DeserializeObject<List<BookDto>>(raw);

            Assert.Equal(2, outputModel.Count);
        }
    }
}
