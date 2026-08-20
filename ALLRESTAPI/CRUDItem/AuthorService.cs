using SimpleLibraryEF;
using SimpleLibraryEF.Models;

namespace ALLRESTAPI.CRUDItem
{
	public class AuthorService : IAuthorService
	{
		private LibraryContext _dblc = null!;
		public AuthorService(LibraryContext lc) 
		{
			_dblc = lc; 
		}
		
		public async Task<bool> AddAuthor(Author _author) 
		{
			_dblc.Authors.Add(_author);
			await _dblc.SaveChangesAsync();
			return true;
		}
	}
}