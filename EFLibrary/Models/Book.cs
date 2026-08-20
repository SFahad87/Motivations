using System.Text.Json.Serialization;

namespace SimpleLibraryEF.Models;

// TABLE 2: Book
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;

    // Foreign key — links each book back to one author
    public int AuthorId { get; set; }

	[JsonIgnore]
	public Author Author { get; set; } = null!;
}
