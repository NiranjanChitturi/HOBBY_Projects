using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MPHMS.Domain.Entities.Goals;

namespace MPHMS.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Maps GoalCategory entity.
    /// </summary>
    public class GoalCategoryConfiguration : IEntityTypeConfiguration<GoalCategory>
    {
        public void Configure(EntityTypeBuilder<GoalCategory> builder)
        {
            builder.ToTable("GoalCategories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.DisplayOrder)
                   .IsRequired();

            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        }
    }
}
