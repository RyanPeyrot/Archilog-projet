using ArchiLibrary.Data;
using ArchiLibrary.Models;
using ArchiLog.Models;
using Microsoft.EntityFrameworkCore;

namespace ArchiLog.Data
{
    public class ArchiLogDbContext : BaseDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Server=tcp:archilogjara.database.windows.net,1433;Initial Catalog=archilogjara;Persist Security Info=False;User ID=CloudSA487b67b3;Password=Jara1234;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Car> Cars { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
