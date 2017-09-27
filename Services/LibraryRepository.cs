using System;
using System.Collections.Generic;
using System.Linq;
using dotnet_core_rest.Entities;

namespace dotnet_core_rest.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        private BookLibraryContext _context;


        public LibraryRepository(BookLibraryContext context)
        {
            _context = context;
        }

        public IEnumerable<Author> GetAuthors()
        {
            return _context.Authors.OrderBy(a => a.FirstName).ThenBy(a => a.LastName).ToList();
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

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}