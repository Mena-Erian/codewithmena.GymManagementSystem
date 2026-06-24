using codewithmena.GymManagementSystem.DAL.Configurations.Base;
using codewithmena.GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace codewithmena.GymManagementSystem.DAL.Configurations
{
    public class PlanConfiguration : BaseEntityConfiguration<Plan, int>
    {
        public override void Configure(EntityTypeBuilder<Plan> builder)
        {
            // Apply base config (Id PK, CreatedAt, UpdatedAt, IsDeleted)
            base.Configure(builder);

            // Plan-specific config
            builder.Property(p => p.Name)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(p => p.Description)
                   .HasMaxLength(200);

            builder.Property(p => p.Price)
                   .HasPrecision(10, 2)
                   .IsRequired();

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CHK_Plan_DurationCheck", "[DurationDays] Between 1 and 365");
            });
        }
    }
}

