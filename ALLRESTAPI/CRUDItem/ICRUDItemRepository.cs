using ALLRESTAPI.Models;
namespace ALLRESTAPI.CRUDItem
{
	public interface ICRUDItemRepository
	{
		Task<IEnumerable<ALLRESTAPI.Models.CRUDItem>> GetCRUDItems();
	}
}
