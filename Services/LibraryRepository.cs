using System;
using System.Collections.Generic;
using System.Linq;
using dotnet_core_rest.Entities;

namespace dotnet_core_rest.Services
{
    public class LibraryRepository : ILibraryRepository
    {
        public IEnumerable<Author> GetAuthors()
        {
            var authors = new List<Author>(){
                new Author()
                {
                     Id = new Guid("25320c5e-f58a-4b1f-b63a-8ee07a840bdf"),
                     FirstName = "Stephen",
                     LastName = "King",
                     Genre = "Horror",
                     DateOfBirth = new DateTimeOffset(new DateTime(1947, 9, 21))
        
                }
            };

            return authors;
        }

        public Author GetAuthor(Guid Id)
        {
            return GetAuthors().FirstOrDefault(a => a.Id == Id);
        }

    }
}