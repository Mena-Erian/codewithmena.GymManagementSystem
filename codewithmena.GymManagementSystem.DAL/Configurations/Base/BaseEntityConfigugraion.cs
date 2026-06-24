using codewithmena.GymManagementSystem.DAL.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace codewithmena.GymManagementSystem.DAL.Configurations.Base
{
    public abstract class BaseEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // CreatedAt — defaults to current timestamp on INSERT
            builder.Property(e => e.CreatedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            // UpdatedAt — nullable, set manually on update
            builder.Property(e => e.UpdatedAt)
                   .IsRequired(false);

            // IsDeleted — soft-delete flag, default false
            builder.Property(e => e.IsDeleted)
                   .HasDefaultValue(false)
                   .IsRequired();
        }
    }
}

