using ALLRESTAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ALLRESTAPI.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<ALLRESTAPI.Models.CRUDItem> CRUDItem { get; set; }
   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ALLRESTAPI.Models.CRUDItem>(entity =>
        {
            entity.HasKey(ci => ci.Id);
            entity.Property(ci => ci.ItemName).IsRequired().HasMaxLength(100);
            entity.Property(ci => ci.CreateDate).IsRequired().HasMaxLength(255);
			entity.Property(ci => ci.LastUpdateDate).IsRequired().HasMaxLength(255);
			entity.HasIndex(ci => ci.Id).IsUnique();
        });
    }
}
