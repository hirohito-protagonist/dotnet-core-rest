using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using DotNetCoreRest.Models;
using DotNetCoreRest.Services;
using DotNetCoreRest.Entities;


namespace DotNetCoreRest.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller 
    {

        private ILibraryRepository _libraryRepository;

        public BooksController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksFromRepository = _libraryRepository.GetBooksForAuthor(authorId);
            var books = Mapper.Map<IEnumerable<BookDto>>(booksFromRepository);

            return Ok(books);
        }

        [HttpGet("{id}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid id)
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

            return Ok(bookForAuthor);
        }

        [HttpPost()]
        public IActionResult CreateBookForAuthor(Guid authorId, [FromBody] BookManipulationDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return StatusCode(422, $"Invalid book data for author {authorId}.");
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
            
            return CreatedAtRoute("GetBookForAuthor", new { authorId = authorId, id = bookToReturn.Id }, bookToReturn);
        }

        [HttpDelete("{id}")]
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

        [HttpPut("{id}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id, [FromBody] BookManipulationDto book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return StatusCode(422, $"Invalid book data for author {authorId}.");
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

        [HttpPatch("{id}")]
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
                return StatusCode(422, $"Invalid book data for author {authorId}.");
            }

            Mapper.Map(bookToPatch, bookFromRepository);

            if (!_libraryRepository.Save())
            {
                return StatusCode(500, $"Patching book {id} for author {authorId} failed on save.");
            }

            return NoContent();
        }
    }
}