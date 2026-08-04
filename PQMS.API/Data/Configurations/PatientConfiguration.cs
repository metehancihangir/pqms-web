using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PQMS.API.Models;

namespace PQMS.API.Data.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasIndex(p => p.FullName);
        builder.HasIndex(p => p.PhoneNumber);
        builder.HasIndex(p => p.DateOfBirth);

        builder.Property(p => p.FullName)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(p => p.PhoneNumber)
               .HasMaxLength(20);

        builder.Property(p => p.CreatedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
    }
}
