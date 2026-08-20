using Microsoft.EntityFrameworkCore;
using SimpleLibraryEF.Models;

namespace SimpleLibraryEF;

public class LibraryContext : DbContext
{
    // One DbSet per table
    public DbSet<Author> Authors { get; set; }
    public DbSet<Book> Books { get; set; }

    // This constructor lets the connection string be passed in from outside
    // (Program.cs, a test, or the design-time factory below)
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Author -> Books relationship (one Author has many Books)
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId);

        // Optional: seed a little starter data
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, Name = "George Orwell" , Address = "Steventon, England" },

			new Author { Id = 2, Name = "Jane Austen",  Address = "Motihari, India" }

		);

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "1984", AuthorId = 1 },
			new Book { Id = 2, Title = "Animal Farm", AuthorId = 1 },
            new Book { Id = 3, Title = "Pride and Prejudice", AuthorId = 2 }
        );
    }
}
