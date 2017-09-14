using System;
using System.Collections.Generic;
using dotnet_core_rest.Entities;

namespace dotnet_core_rest.Services
{
    public interface ILibraryRepository
    {
        IEnumerable<Author> GetAuthors();
    }
}