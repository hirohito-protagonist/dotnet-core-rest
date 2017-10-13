using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DotNetCoreRest.Models;
using DotNetCoreRest.Services;
using DotNetCoreRest.Entities;
using DotNetCoreRest.Helpers;


namespace DotNetCoreRest.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller 
    {

        private ILibraryRepository _libraryRepository;
        private IUrlHelper _urlHelper;

        public BooksController(ILibraryRepository libraryRepository, IUrlHelper urlHelper)
        {
            _libraryRepository = libraryRepository;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetBooksForAuthor")]
        public IActionResult GetBooksForAuthor(Guid authorId, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksFromRepository = _libraryRepository.GetBooksForAuthor(authorId);
            var books = Mapper.Map<IEnumerable<BookDto>>(booksFromRepository);

            if (mediaType == "application/vnd.hateoas+json")
            {
                books = books.Select(book => 
                {
                    return CreateLinksForBook(book);
                });
            }

            return Ok(books);
        }

        [HttpGet("{id}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid id, [FromHeader(Name = "Accept")] string mediaType)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var book = _libraryRepository.GetBookForAuthor(authorId, id);
            if (book == null)
            {
                return NotFound();
            }
            var bookForAuthor = Mapper.Map<BookDto>(book);

            return Ok(mediaType == "application/vnd.hateoas+json" ? CreateLinksForBook(bookForAuthor) : bookForAuthor);
        }

        [HttpPost(Name = "CreateBookForAuthor")]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] BookManipulationDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookToAdd = Mapper.Map<Book>(book);

            _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

            if (!_libraryRepository.Save())
            {
                return StatusCode(500, $"Creating a book for author {authorId} failed on save.");
            }

            var bookToReturn = Mapper.Map<BookDto>(bookToAdd);
            
            return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, id = bookToReturn.Id }, CreateLinksForBook(bookToReturn));
        }

        [HttpDelete("{id}", Name = "DeleteBookForAuthor")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var book = _libraryRepository.GetBookForAuthor(authorId, id);
            if (book == null)
            {
                return NotFound();
            }

            _libraryRepository.DeleteBook(book);

            if (!_libraryRepository.Save())
            {
                return StatusCode(500, $"Deleting book {id} for author {authorId} failed on save.");
            }
            
            return NoContent();
        }

        [HttpPut("{id}", Name = "UpdateBookForAuthor")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] BookManipulationDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookFromRepository = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookFromRepository == null)
            {
                return NotFound();
            }

            Mapper.Map(book, bookFromRepository);


            if (!_libraryRepository.Save())
            {
                return StatusCode(500, $"Updating book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateBookForAuthor")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id, [FromBody] JsonPatchDocument<BookManipulationDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var bookFromRepository = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookFromRepository == null)
            {
                return NotFound();
            }

            var bookToPatch = Mapper.Map<BookManipulationDto>(bookFromRepository);
            patchDoc.ApplyTo(bookToPatch, ModelState);

            TryValidateModel(bookToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            Mapper.Map(bookToPatch, bookFromRepository);

            if (!_libraryRepository.Save())
            {
                return StatusCode(500, $"Patching book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }

        private BookDto CreateLinksForBook(BookDto book)
        {

            book.Links.Add(new LinkDto(_urlHelper.Link("GetBookForAuthor", new { id = book.Id }), "self", "GET"));
            book.Links.Add(new LinkDto(_urlHelper.Link("DeleteBookForAuthor", new { id = book.Id }), "delete_book", "DELETE"));
            book.Links.Add(new LinkDto(_urlHelper.Link("UpdateBookForAuthor", new { id = book.Id }), "update_book", "PUT"));
            book.Links.Add(new LinkDto(_urlHelper.Link("PartiallyUpdateBookForAuthor", new { id = book.Id }), "partially_update_book", "PATCH"));
            return book;
        }
    }
}