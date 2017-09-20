using System;
using System.Collections.Generic;

namespace dotnet_core_rest.Entities
{
    public static class BookLibraryContextExtension
    {

        public static void EnsureSeedDataForContext(this BookLibraryContext context)
        {
            context.Authors.RemoveRange(context.Authors);
            context.SaveChanges();

            var authors = new List<Author>()
            {
                new Author()
                {
                     Id = new Guid("25320c5e-f58a-4b1f-b63a-8ee07a840bdf"),
                     FirstName = "Stephen",
                     LastName = "King",
                     Genre = "Horror",
                     DateOfBirth = new DateTimeOffset(new DateTime(1947, 9, 21))
                },
                new Author()
                {
                     Id = new Guid("76053df4-6687-4353-8937-b45556748abe"),
                     FirstName = "George",
                     LastName = "RR Martin",
                     Genre = "Fantasy",
                     DateOfBirth = new DateTimeOffset(new DateTime(1948, 9, 20))
                },
                new Author()
                {
                     Id = new Guid("412c3012-d891-4f5e-9613-ff7aa63e6bb3"),
                     FirstName = "Neil",
                     LastName = "Gaiman",
                     Genre = "Fantasy",
                     DateOfBirth = new DateTimeOffset(new DateTime(1960, 11, 10))
                },
                new Author()
                {
                     Id = new Guid("578359b7-1967-41d6-8b87-64ab7605587e"),
                     FirstName = "Tom",
                     LastName = "Lanoye",
                     Genre = "Various",
                     DateOfBirth = new DateTimeOffset(new DateTime(1958, 8, 27))
                },
                new Author()
                {
                     Id = new Guid("f74d6899-9ed2-4137-9876-66b070553f8f"),
                     FirstName = "Douglas",
                     LastName = "Adams",
                     Genre = "Science fiction",
                     DateOfBirth = new DateTimeOffset(new DateTime(1952, 3, 11))
                },
                new Author()
                {
                     Id = new Guid("a1da1d8e-1988-4634-b538-a01709477b77"),
                     FirstName = "Jens",
                     LastName = "Lapidus",
                     Genre = "Thriller",
                     DateOfBirth = new DateTimeOffset(new DateTime(1974, 5, 24))
                }
            };

            context.Authors.AddRange(authors);
            context.SaveChanges();
        }
    } 
}