using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BooksLibrary.Services;
using BooksLibrary.Models;
using BooksLibrary.Entities;
using BooksLibrary.Helpers;
using AutoMapper;

namespace BooksLibrary.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {

        private ILibraryRepository _libraryRepository;
        private IUrlHelper _urlHelper;

        public AuthorsController(ILibraryRepository libraryRepository, IUrlHelper urlHelper)
        {
            _libraryRepository = libraryRepository;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetAuthors")]
        public IActionResult GetAuthors(AuthorResourceParameters authorResourceParameters, [FromHeader(Name = "Accept")] string mediaType)
        {

            var authorsFromRepository = _libraryRepository.GetAuthors(authorResourceParameters);
            var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepository);

            if (mediaType == "application/vnd.hateoas+json")
            {
                authors = authors.Select(author =>
                {
                    return CreateLinksForAuthor(author);
                });
            }

            return Ok(authors);
        }

        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthors(Guid id, [FromHeader(Name = "Accept")] string mediaType)
        {
            var authorFromRepository = _libraryRepository.GetAuthor(id);

            if (authorFromRepository == null)
            {
                return NotFound();
            }

            var author = Mapper.Map<AuthorDto>(authorFromRepository);
            return Ok(mediaType == "application/vnd.hateoas+json" ? CreateLinksForAuthor(author) : author);
        }

        [HttpPost(Name = "CreateAuthor")]
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
                throw new Exception("A problem happen with save your author");
            }

            var authorReturn = Mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor", new { id = authorReturn.Id }, authorReturn);
        }

        [HttpDelete("{id}", Name = "DeleteAuthor")]
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
                throw new Exception( $"Delete {id} author failed on save.");
            }

            return NoContent();
        }

        private AuthorDto CreateLinksForAuthor(AuthorDto author)
        {

            author.Links.Add(new LinkDto(_urlHelper.Link("GetAuthor", new { id = author.Id }), "self", "GET"));
            author.Links.Add(new LinkDto(_urlHelper.Link("DeleteAuthor", new { id = author.Id }), "delete_author", "DELETE"));
            author.Links.Add(new LinkDto(_urlHelper.Link("CreateAuthor", new {}), "create_author", "POST"));
            return author;
        }
    }
}