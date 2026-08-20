using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ALLRESTAPI.CRUDItem
{
	public class CRUDItemRepository : ICRUDItemRepository
	{
		private ALLRESTAPI.Database.AppDbContext dbContext;

		public CRUDItemRepository(ALLRESTAPI.Database.AppDbContext _dbContext)
		{
			this.dbContext = _dbContext;
		}


		public async Task<IEnumerable<ALLRESTAPI.Models.CRUDItem>> GetCRUDItems()
		{
			var cRUDItems = await dbContext.CRUDItem.Where(i=>i.Id > 0).ToListAsync();

			return cRUDItems;
		}
	}
}
