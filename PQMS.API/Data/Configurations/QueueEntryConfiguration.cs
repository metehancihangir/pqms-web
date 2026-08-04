using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PQMS.API.Models;

namespace PQMS.API.Data.Configurations;

public class QueueEntryConfiguration : IEntityTypeConfiguration<QueueEntry>
{
    public void Configure(EntityTypeBuilder<QueueEntry> builder)
    {
        builder.ToTable("QueueEntries");

        builder.HasOne(q => q.Patient)
               .WithMany(p => p.QueueEntries)
               .HasForeignKey(q => q.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(q => q.QueueDate);
        builder.HasIndex(q => q.Status);

        builder.Property(q => q.QueueNumber)
               .HasMaxLength(15)
               .IsRequired();

        builder.Property(q => q.Status)
               .HasMaxLength(20)
               .HasDefaultValue("Waiting");

        builder.Property(q => q.CheckInType)
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(q => q.VisitReason)
               .HasMaxLength(100);

        builder.Property(q => q.AdditionalInfo)
               .HasMaxLength(500);

        builder.Property(q => q.CheckInTime)
               .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
    }
}
