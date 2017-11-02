using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BooksLibrary.Models;

namespace BooksLibrary.Tests.Integration
{
    public class IntegrationTestsBase<TStartup> : IDisposable
        where TStartup : class
    {
        private readonly TestServer server;
        

        public IntegrationTestsBase()
        {
            var host = new WebHostBuilder()
                            .UseStartup<TStartup>()
                            .ConfigureServices(ConfigureServices);

            this.server = new TestServer(host);
            this.Client = this.server.CreateClient();
            this.TestHelpers = new Helpers();
        }

        public HttpClient Client { get; }

        public Helpers TestHelpers { get; }

        public void Dispose()
        {
            this.Client.Dispose();
            this.server.Dispose();
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        { }

        public async Task<AuthorDto> CreateDummyAuthor()
        {
            var content = new StringContent(JsonConvert.SerializeObject(new AuthorCreationDto()),
                                    Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "api/authors");
            request.Content = content;
            this.TestHelpers.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);

            string rawAuthor = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<AuthorDto>(rawAuthor);
        }

        public async Task<BookDto> CreateDummyBookForAuthor(Guid authorId, string title = "test", string description = "test")
        {
            var content = new StringContent(JsonConvert.SerializeObject(new BookManipulationDto()
            {
                Title = title,
                Description = description,
            }), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"api/authors/{authorId}/books");
            request.Content = content;
            this.TestHelpers.AddRequestIpLimitHeaders(request);

            var response = await this.Client.SendAsync(request);

            string rawBook = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<BookDto>(rawBook);
        }

    }
}
