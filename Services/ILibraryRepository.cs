using System;
using System.Collections.Generic;
using DotNetCoreRest.Entities;

namespace DotNetCoreRest.Services
{
    public interface ILibraryRepository
    {
        IEnumerable<Author> GetAuthors();
        Author GetAuthor(Guid Id);
        void AddAuthor(Author author);
        void DeleteAuthor(Author author);
        bool AuthorExists(Guid Id);
        IEnumerable<Book> GetBooksForAuthor(Guid authorId);
        Book GetBookForAuthor(Guid authorId, Guid author);
        void DeleteBook(Book book);
        bool Save();
    }
}