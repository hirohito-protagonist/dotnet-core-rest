using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreRest.Services;
using DotNetCoreRest.Models;
using DotNetCoreRest.Entities;
using AutoMapper;

namespace DotNetCoreRest.Controllers
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

        [HttpGet("{id}", Name = "GetAuthor")]
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

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorCreationDto author)
        {

            if (author == null)
            {
                return BadRequest();
            }

            var authorEntity = Mapper.Map<Author>(author);

            _libraryRepository.AddAuthor(authorEntity);

            if (!_libraryRepository.Save())
            {
                return StatusCode(500, "A problem happen with save your author");
            }

            var authorReturn = Mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor", new { id = authorReturn.Id }, authorReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(Guid id)
        {
            var authorFromRepository = _libraryRepository.GetAuthor(id);
            if (authorFromRepository == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteAuthor(authorFromRepository);

            if (!_libraryRepository.Save())
            {
                return StatusCode(500, $"Delete {id} author failed on save.");
            }

            return NoContent();
        }
    }
}