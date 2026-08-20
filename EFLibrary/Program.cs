using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SimpleLibraryEF.Models;
using System.Data.Common;

namespace SimpleLibraryEF;

class Program
{

	public DbConnectionStringBuilder GetConnectionStringBuilder()
	{
		var builder = new DbConnectionStringBuilder();
		builder["Server"] = "(localdb)\\MSSQLLocalDB";
		builder["Database"] = "Itsalright";
		builder["User Id"] = "itsoksa1234";
		builder["Password"] = "itsoksa1234$#@!";
		builder["TrustServerCertificate"] = "True";
		builder["MultipleActiveResultSets"] = "true";

		return builder;
	}

	static async Task Main()
    {
        var _DbConnectionStringBuilder = new DbConnectionStringBuilder();
        

		var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseSqlServer("Server = (localdb)\\MSSQLLocalDB; Database = Itsalright; User Id = itsoksa1234; Password = itsoksa1234$#@!;TrustServerCertificate=True;MultipleActiveResultSets=true")
            .Options;

        using var db = new LibraryContext(options);

        // Apply any pending migrations (creates the database/tables if needed)
        db.Database.Migrate();

        // ---- CREATE ----
        var author = new Author { Name = "J.R.R. Tolkien" };
        var book = new Book { Title = "The Hobbit", Author = author };

        db.Authors.Add(author);
        db.Books.Add(book);
        await db.SaveChangesAsync();

        Console.WriteLine($"Added author '{author.Name}' with book '{book.Title}'");

        // ---- READ ----
        // Include() pulls the related Author together with each Book
        var allBooks = await db.Books
            .Include(b => b.Author)
            .ToListAsync();

        Console.WriteLine("\nAll books:");
        foreach (var b in allBooks)
            Console.WriteLine($"  '{b.Title}' by {b.Author.Name}");
    }
}
