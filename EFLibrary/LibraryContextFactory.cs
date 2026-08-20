using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Data.Common;

namespace SimpleLibraryEF;

// This tells "dotnet ef" how to create a LibraryContext
// when you run migrations commands — without it, the tools
// don't know what connection string to use.
public class LibraryContextFactory : IDesignTimeDbContextFactory<LibraryContext>
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
	
	public LibraryContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LibraryContext>();

		optionsBuilder.UseSqlServer(GetConnectionStringBuilder().ConnectionString);

		//	"Server=localhost;Database=SimpleLibraryDb;Trusted_Connection=True;TrustServerCertificate=True;"
  //      );

        return new LibraryContext(optionsBuilder.Options);
    }
}
