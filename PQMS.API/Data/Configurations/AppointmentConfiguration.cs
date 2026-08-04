using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PQMS.API.Models;

namespace PQMS.API.Data.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");

        builder.HasOne(a => a.Patient)
               .WithMany(p => p.Appointments)
               .HasForeignKey(a => a.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.AppointmentDate);
        builder.HasIndex(a => a.Status);

        builder.Property(a => a.Reason)
               .HasMaxLength(100);

        builder.Property(a => a.Status)
               .HasMaxLength(20)
               .HasDefaultValue("Scheduled");

        builder.Property(a => a.Notes)
               .HasMaxLength(500);

        builder.Property(a => a.CreatedAt)
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
    }
}
