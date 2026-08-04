using Microsoft.EntityFrameworkCore;
using PQMS.API.Models;

namespace PQMS.API.Data;

public class PqmsDbContext : DbContext
{
    public PqmsDbContext(DbContextOptions<PqmsDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<QueueEntry> QueueEntries { get; set; }
    public DbSet<VisitReason> VisitReasons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // utf8mb4 karakter seti her tablo için
        modelBuilder.HasCharSet("utf8mb4");

        // Tüm Fluent API konfigürasyonlarını bu assembly'den otomatik uygula
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PqmsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
