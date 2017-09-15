using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using dotnet_core_rest.Services;
using dotnet_core_rest.Models;
using AutoMapper;

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
            var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepository);
            return new JsonResult(authors);
        }

    }
}