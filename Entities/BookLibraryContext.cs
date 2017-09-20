using Microsoft.EntityFrameworkCore;

namespace dotnet_core_rest.Entities
{
    public class BookLibraryContext : DbContext
    {

        public BookLibraryContext(DbContextOptions<BookLibraryContext> options) : base(options)
        {
         
        }

        public DbSet<Author> Authors { get; set; }
    }
}