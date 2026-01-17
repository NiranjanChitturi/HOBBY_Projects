using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MPHMS.Domain.Entities.Habits;

namespace MPHMS.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Maps SkipReason lookup entity.
    /// </summary>
    public class SkipReasonConfiguration : IEntityTypeConfiguration<SkipReason>
    {
        public void Configure(EntityTypeBuilder<SkipReason> builder)
        {
            builder.ToTable("SkipReasons");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(x => x.Description)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.IsSystemDefined)
                   .HasDefaultValue(true);

            builder.Property(x => x.IsDeleted)
                   .HasDefaultValue(false);

            builder.HasIndex(x => x.Code)
                   .IsUnique();
        }
    }
}
