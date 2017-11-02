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
        [Fact(DisplayName = "it should return collection of books for author")]
        public async void TestGetBooks()
        {
            var author = await this.CreateDummyAuthor();
            var book1 = await this.CreateDummyBookForAuthor(author.Id);
            var book2 = await this.CreateDummyBookForAuthor(author.Id);

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/authors/{author.Id}/books");
            this.TestHelpers.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            
            string raw = await response.Content.ReadAsStringAsync();
            List<BookDto> outputModel = JsonConvert.DeserializeObject<List<BookDto>>(raw);

            Assert.Equal(2, outputModel.Count);
        }
      
        [Fact(DisplayName = "it should delete book for defined id")]
        public async void TestDeletedBookForValidId()
        {
            var author = await this.CreateDummyAuthor();
            var book = await this.CreateDummyBookForAuthor(author.Id);

            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/authors/{author.Id}/books/{book.Id}");
            this.TestHelpers.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);
            
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact(DisplayName = "it should check if the book is in the system before delete")]
        public async void TestDeletedBookForInValidId()
        {
            var author = await this.CreateDummyAuthor();

            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/authors/{author.Id}/books/123test");
            this.TestHelpers.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Fact(DisplayName = "it should validated book data before it will be add to system")]
        public async void TestValidationBookCreation()
        {
            var author = await this.CreateDummyAuthor();

            var content = new StringContent(JsonConvert.SerializeObject(new BookManipulationDto()), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"api/authors/{author.Id}/books");
            request.Content = content;
            this.TestHelpers.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);
            
            Assert.Equal(422, (int)response.StatusCode);
        }

        [Fact(DisplayName = "it should update existing book")]
        public async void TestUpdateBook()
        {
            var author = await this.CreateDummyAuthor();
            var book = await this.CreateDummyBookForAuthor(author.Id);
            var serializedBook = new StringContent(JsonConvert.SerializeObject(new BookManipulationDto()
            {
                Title = "Updated title",
                Description = "Updated description"
            }), Encoding.UTF8, "application/json");

            var updateRequest = new HttpRequestMessage(HttpMethod.Put, $"api/authors/{author.Id}/books/{book.Id}");
            updateRequest.Content = serializedBook;
            this.TestHelpers.AddRequestIpLimitHeaders(updateRequest);

            var updateResponse = await this.Client.SendAsync(updateRequest);

            Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/authors/{author.Id}/books/{book.Id}");
            this.TestHelpers.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);

            response.EnsureSuccessStatusCode();
            
            string raw = await response.Content.ReadAsStringAsync();
            BookDto updatedBook = JsonConvert.DeserializeObject<BookDto>(raw);

            Assert.Equal("Updated title", updatedBook.Title);
            Assert.Equal("Updated description", updatedBook.Description);
        }
    }
}
