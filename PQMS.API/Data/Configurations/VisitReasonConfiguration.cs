using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PQMS.API.Models;

namespace PQMS.API.Data.Configurations;

public class VisitReasonConfiguration : IEntityTypeConfiguration<VisitReason>
{
    public void Configure(EntityTypeBuilder<VisitReason> builder)
    {
        builder.ToTable("VisitReasons");

        builder.Property(v => v.Name)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(v => v.IsActive)
               .HasDefaultValue(true);

        // Seed data
        builder.HasData(
            new VisitReason { Id = 1, Name = "General Checkup" },
            new VisitReason { Id = 2, Name = "Follow-up Visit" },
            new VisitReason { Id = 3, Name = "Urgent Care" },
            new VisitReason { Id = 4, Name = "Consultation" },
            new VisitReason { Id = 5, Name = "Lab Results" },
            new VisitReason { Id = 6, Name = "Vaccination" },
            new VisitReason { Id = 7, Name = "Other" }
        );
    }
}
