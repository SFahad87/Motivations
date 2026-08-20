namespace ALLRESTAPI.Models
{
	public class CRUDItem
	{
		public int Id { get; set; }
		public string ItemName { get; set; } = null!; // FIX: nullable initializer required when Nullable is enabled
		public DateTime CreateDate { get; set; }
		public DateTime LastUpdateDate { get; set; }
	}
}
