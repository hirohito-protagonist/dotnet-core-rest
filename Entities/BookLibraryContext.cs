using Microsoft.EntityFrameworkCore;

namespace DotNetCoreRest.Entities
{
    public class BookLibraryContext : DbContext
    {

        public BookLibraryContext(DbContextOptions<BookLibraryContext> options) : base(options)
        {
         
        }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }
    }
}