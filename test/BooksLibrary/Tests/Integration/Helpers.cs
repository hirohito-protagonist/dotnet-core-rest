using System;
using System.Net.Http;

namespace BooksLibrary.Tests.Integration
{
    public class Helpers
    {
        public void AddRequestIpLimitHeaders(HttpRequestMessage request)
        {
            var clientId = "cl-key-b";
            var ip = "::1";

            request.Headers.Add("X-ClientId", clientId);
            request.Headers.Add("X-Real-IP", ip);
        }
    }
}