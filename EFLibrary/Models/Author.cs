namespace SimpleLibraryEF.Models;

// TABLE 1: Author
public class Author
{
    public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Address { get; set; } = string.Empty;
	public string Signature { get; set; } = string.Empty;

	// One author can have many books
	public ICollection<Book> Books { get; set; } = new List<Book>();
}
