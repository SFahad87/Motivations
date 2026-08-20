using SimpleLibraryEF.Models;

namespace ALLRESTAPI.CRUDItem
{
	public interface IAuthorService
	{
		public Task<bool> AddAuthor(Author author);
	}
}
