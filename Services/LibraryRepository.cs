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

        public bool Save()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}