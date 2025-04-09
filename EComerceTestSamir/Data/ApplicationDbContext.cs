using EComerceTestSamir.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EComerceTestSamir.Data
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ECommerceTest;Integrated Security=True;TrustServerCertificate=True");

        }
    }
}
