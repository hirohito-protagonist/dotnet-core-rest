using System;
using System.Collections.Generic;
using System.Linq;
using DotNetCoreRest.Entities;
using DotNetCoreRest.Helpers;

namespace DotNetCoreRest.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        private BookLibraryContext _context;


        public LibraryRepository(BookLibraryContext context)
        {
            _context = context;
        }

        public IEnumerable<Author> GetAuthors(AuthorResourceParameters authorResourceParameters)
        {
            return _context.Authors
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .Skip(authorResourceParameters.PageSize * (authorResourceParameters.PageNumber - 1))
                .Take(authorResourceParameters.PageSize)
                .ToList();
        }

        public Author GetAuthor(Guid Id)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == Id);
        }

        public void AddAuthor(Author author)
        {
            author.Id = Guid.NewGuid();
            _context.Authors.Add(author);
        }

        public void DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
        }

        public bool AuthorExists(Guid authorId)
        {
            return _context.Authors.Any(a => a.Id == authorId);
        }

        public IEnumerable<Book> GetBooksForAuthor(Guid authorId)
        {
            return _context.Books.Where(b => b.AuthorId == authorId).OrderBy(b => b.Title).ToList();
        }

        public Book GetBookForAuthor(Guid authorId, Guid id)
        {
            return _context.Books.Where(b => b.AuthorId == authorId && b.Id == id).FirstOrDefault();
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public void AddBookForAuthor(Guid authorId, Book book)
        {
            var author = GetAuthor(authorId);
            if (author != null)
            {
                if (book.Id == null)
                {
                    book.Id = Guid.NewGuid();
                }
                author.Books.Add(book);
            }
        }

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}