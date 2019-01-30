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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasOne(e => e.MembershipCard)
                .WithOne(x => x.Employee)
                .HasForeignKey<MembershipCard>(x => x.CardId)
                .HasPrincipalKey<Employee>(x => x.CardId);

            modelBuilder.Entity<MembershipCard>()
                .Property(x => x.CurrentBalance)
                .HasColumnType("decimal(10,2)");           
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<MembershipCard> MembershipCards { get; set; }
    }
}
