using codewithmena.GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace codewithmena.GymManagementSystem.DAL.DbContexts
{
    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Auto-discovers and applies all IEntityTypeConfiguration classes in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Plan> Plans { get; set; }
    }
}
