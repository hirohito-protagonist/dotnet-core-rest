using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using dotnet_core_rest.Services;
using dotnet_core_rest.Models;

namespace dotnet_core_rest.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {

        private ILibraryRepository _libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetAuthors()
        {
            var authorsFromRepository = _libraryRepository.GetAuthors();
            var authors = new List<AuthorDto>();

            foreach (var author in authorsFromRepository)
            {
                authors.Add(new AuthorDto(){
                    Id = author.Id,
                    Name = $"{author.FirstName} {author.LastName}",
                    Genre = author.Genre
                });
            }
            return new JsonResult(authors);
        }

    }
}