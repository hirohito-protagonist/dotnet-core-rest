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
            var response = await this.Client.GetAsync("api/authors");

            response.EnsureSuccessStatusCode();
            
            string raw = await response.Content.ReadAsStringAsync();
            List<AuthorDto> outputModel = JsonConvert.DeserializeObject<List<AuthorDto>>(raw);

            Assert.Equal(6, outputModel.Count);
        }
    }
}
