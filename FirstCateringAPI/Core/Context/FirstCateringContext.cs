using FirstCateringAPI.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FirstCateringAPI.Core.Context
{
    public class FirstCateringContext : DbContext
    {
        public FirstCateringContext(DbContextOptions<FirstCateringContext> options): base(options)
        {
            Database.Migrate();
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<MembershipCard> MembershipCards { get; set; }
    }
}
