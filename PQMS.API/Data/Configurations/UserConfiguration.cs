using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PQMS.API.Models;

namespace PQMS.API.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.FullName)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(u => u.Email)
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(u => u.PasswordHash)
               .IsRequired();

        builder.Property(u => u.Role)
               .HasMaxLength(20)
               .HasDefaultValue("Patient");

        builder.Property(u => u.CreatedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

        builder.Property(u => u.IsActive)
               .HasDefaultValue(true);
    }
}
