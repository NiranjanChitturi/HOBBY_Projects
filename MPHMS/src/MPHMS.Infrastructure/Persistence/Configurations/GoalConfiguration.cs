using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MPHMS.Domain.Entities.Goals;

namespace MPHMS.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Maps Goal entity.
    /// </summary>
    public class GoalConfiguration : IEntityTypeConfiguration<Goal>
    {
        public void Configure(EntityTypeBuilder<Goal> builder)
        {
            builder.ToTable("Goals");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(x => x.Description)
                   .HasMaxLength(1000);

            builder.Property(x => x.Priority).IsRequired();
            builder.Property(x => x.Status).IsRequired();

            // Relationships
       //      builder.HasOne(x => x.User)
       //             .WithMany()
       //             .HasForeignKey(x => x.UserId)
       //             .OnDelete(DeleteBehavior.Restrict);
builder.Property(x => x.UserId)
       .IsRequired();

            builder.HasOne(x => x.Category)
                   .WithMany()
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Audit
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        }
    }
}
