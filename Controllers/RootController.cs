using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreRest.Models;

namespace DotNetCoreRest.Controllers
{
    [Route("api")]
    public class RootController : Controller
    {
        private IUrlHelper _urlHelper;

        public RootController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
        {

            if (mediaType == "application/vnd.hateoas+json")
            {
                var links = new List<LinkDto>();

                links.Add(
                  new LinkDto(_urlHelper.Link("GetRoot", new { }),
                  "self",
                  "GET"));

                links.Add(
                 new LinkDto(_urlHelper.Link("GetAuthors", new { }),
                 "authors",
                 "GET"));

                links.Add(
                  new LinkDto(_urlHelper.Link("CreateAuthor", new { }),
                  "create_author",
                  "POST"));

                return Ok(links);
            }
            return NoContent();
        }
    }
}