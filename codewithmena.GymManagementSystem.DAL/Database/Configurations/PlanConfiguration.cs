using codewithmena.GymManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace codewithmena.GymManagementSystem.Database.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property<string>(p => p.Name)
                 //.HasColumnType("nvarchar(50)")
                 .HasMaxLength(50)
                 .IsRequired();

            builder.Property(p => p.Description)
                 .HasMaxLength(200);

            builder.Property<decimal>(p => p.Price)
                 .HasPrecision(10, 2)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CHK_Plan_DurationCheck", "[DurationDays] Between 1 and 365");
            });


        }
    }
}
