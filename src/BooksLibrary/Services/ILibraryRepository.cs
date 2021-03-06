using System;
using System.Collections.Generic;
using BooksLibrary.Entities;
using BooksLibrary.Helpers;

namespace BooksLibrary.Services
{
    public interface ILibraryRepository
    {
        IEnumerable<Author> GetAuthors(AuthorResourceParameters authorResourceParameters);
        Author GetAuthor(Guid Id);
        void AddAuthor(Author author);
        void DeleteAuthor(Author author);
        bool AuthorExists(Guid Id);
        IEnumerable<Book> GetBooksForAuthor(Guid authorId);
        Book GetBookForAuthor(Guid authorId, Guid author);
        void AddBookForAuthor(Guid authorId, Book book);
        void DeleteBook(Book book);
        bool Save();
    }
}