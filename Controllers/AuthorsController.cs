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
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public IActionResult GetAuthors(Guid id)
        {
            var authorFromRepository = _libraryRepository.GetAuthor(id);

            if (authorFromRepository == null)
            {
                return NotFound();
            }

            var author = Mapper.Map<AuthorDto>(authorFromRepository);
            return Ok(author);
        }

    }
}