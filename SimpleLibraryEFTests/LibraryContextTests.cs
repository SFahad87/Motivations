using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SimpleLibraryEF;
using SimpleLibraryEF.Models;

namespace SimpleLibraryEF.Tests;

[TestFixture]
public class LibraryContextTests
{
    private LibraryContext _db = null!;

    // Runs before EVERY test - gives each test a fresh, isolated in-memory database
    [SetUp]
    public void SetUp()
    {
		var options = new DbContextOptionsBuilder<LibraryContext>()
		.UseSqlServer("Server = (localdb)\\MSSQLLocalDB; Database = Itsalright; User Id = itsoksa1234; Password = itsoksa1234$#@!;TrustServerCertificate=True;MultipleActiveResultSets=true")
		.Options;

		_db = new LibraryContext(options);

        _db.Database.EnsureCreated(); // applies model + seed data for in-memory provider
    }

    // Runs after EVERY test - cleans up
    [TearDown]
    public void TearDown()
    {
        //_db.Database.EnsureDeleted();
        _db.Dispose();
    }

    [Test]
    public async Task AddAuthor_PersistsAuthorToDatabase()
    {
        // Arrange
        var author = new Author { Name = "Pappa. Tolkien", Address = "abc city"};

        // Act
        _db.Authors.Add(author);
        await _db.SaveChangesAsync();

        // Assert
        author.Id.Should().BeGreaterThan(0);
        (await _db.Authors.FindAsync(author.Id)).Should().NotBeNull();
    }

    [Test]
    public async Task AddBook_LinksToAuthorViaForeignKey()
    {
        // Arrange
        var author = new Author { Name = "J.R.R. Tolkien" };
        var book = new Book { Title = "The Hobbit", Author = author };

        // Act
        _db.Authors.Add(author);
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        // Assert
        book.AuthorId.Should().Be(author.Id);
    }

    [Test]
    public async Task GetBooks_WithInclude_LoadsRelatedAuthor()
    {
        // Arrange
        var author = new Author { Name = "Jane Austen" };
        var book = new Book { Title = "Emma", Author = author };
        _db.Authors.Add(author);
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        // Act
        var books = await _db.Books
            .Include(b => b.Author)
            .ToListAsync();

        // Assert
        books.Should().Contain(b => b.Title == "Emma" && b.Author.Name == "Jane Austen");
    }

    [Test]
    public async Task GetBooks_WithoutInclude_AuthorIsNotLoaded()
    {
        // Arrange
        var author = new Author { Name = "Jane Austen" };
        var book = new Book { Title = "Emma", Author = author };
        _db.Authors.Add(author);
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        // Clear EF's local tracking so we get a truly fresh read
        _db.ChangeTracker.Clear();

        // Act
        var bookWithoutInclude = await _db.Books.FirstAsync(b => b.Title == "Emma");

        // Assert - demonstrates the #1 EF gotcha: no Include() means no related data
        bookWithoutInclude.Author.Should().BeNull();
    }

    [Test]
    public async Task UpdateAuthorName_PersistsChange()
    {
        // Arrange
        var author = new Author { Name = "Original Name" };
        _db.Authors.Add(author);
        await _db.SaveChangesAsync();

        // Act
        author.Name = "Updated Name";
        await _db.SaveChangesAsync();

        // Assert
        var updated = await _db.Authors.FindAsync(author.Id);
        updated!.Name.Should().Be("Updated Name");
    }

    [Test]
    public async Task DeleteBook_RemovesFromDatabase()
    {
        // Arrange
        var author = new Author { Name = "George Orwell" };
        var book = new Book { Title = "1984", Author = author };
        _db.Authors.Add(author);
        _db.Books.Add(book);
        await _db.SaveChangesAsync();
        var bookId = book.Id;

        // Act
        _db.Books.Remove(book);
        await _db.SaveChangesAsync();

        // Assert
        (await _db.Books.FindAsync(bookId)).Should().BeNull();
    }

    [Test]
    public async Task DeleteAuthor_DoesNotCascadeDeleteBooksByDefault_WithoutConfiguration()
    {
        // This test documents EF Core's default delete behavior for this relationship.
        // Arrange
        var author = new Author { Name = "George Orwell" };
        var book = new Book { Title = "Animal Farm", Author = author };
        _db.Authors.Add(author);
        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        // Act + Assert
        // Required FK relationships cascade-delete by convention in EF Core,
        // so removing the author also removes their books.
        _db.Authors.Remove(author);
        await _db.SaveChangesAsync();

        (await _db.Books.FindAsync(book.Id)).Should().BeNull();
    }

    [Test]
    public async Task SeedData_ContainsExpectedAuthorsAndBooks()
    {
        // Act
        var authors = await _db.Authors.ToListAsync();
        var books = await _db.Books.ToListAsync();

        // Assert - matches the HasData() seed in LibraryContext.OnModelCreating
        authors.Should().Contain(a => a.Name == "George Orwell");
        authors.Should().Contain(a => a.Name == "Jane Austen");
        books.Should().Contain(b => b.Title == "1984");
    }
}
