using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LabManagement.DAL.Models;

public partial class LabManagementDbContext : DbContext
{
    public LabManagementDbContext()
    {
    }

    public LabManagementDbContext(DbContextOptions<LabManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActivityType> ActivityTypes { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<EventParticipant> EventParticipants { get; set; }

    public virtual DbSet<Lab> Labs { get; set; }

    public virtual DbSet<LabEvent> LabEvents { get; set; }

    public virtual DbSet<LabZone> LabZones { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<SecurityLog> SecurityLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    private string? GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnection"];

        return strConn;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GetConnectionString());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityType>(entity =>
        {
            entity.HasKey(e => e.ActivityTypeId).HasName("PK__activity__D2470C8792B1F20E");

            entity.ToTable("activity_types");

            entity.Property(e => e.ActivityTypeId).HasColumnName("activity_type_id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");

            // Seed data for ActivityTypes
            entity.HasData(
                new ActivityType { ActivityTypeId = 1, Name = "Workshop", Description = "Hands-on training session" },
                new ActivityType { ActivityTypeId = 2, Name = "Seminar", Description = "Educational seminar or lecture" },
                new ActivityType { ActivityTypeId = 3, Name = "Research", Description = "Research activity" },
                new ActivityType { ActivityTypeId = 4, Name = "Experiment", Description = "Laboratory experiment" },
                new ActivityType { ActivityTypeId = 5, Name = "Meeting", Description = "Group meeting or discussion" }
            );
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__bookings__5DE3A5B17836BDD3");

            entity.ToTable("bookings");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasColumnType("decimal(2, 0)")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.Lab).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.LabId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookings__lab_id__7D0E9093");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookings__user_i__7C1A6C5A");

            entity.HasOne(d => d.Zone).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ZoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bookings__zone_i__7E02B4CC");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("PK__equipmen__197068AFC616B45E");

            entity.ToTable("equipment");

            entity.HasIndex(e => e.Code, "UQ__equipmen__357D4CF958F5304E").IsUnique();

            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasColumnType("decimal(2, 0)")
                .HasColumnName("status");

            entity.HasOne(d => d.Lab).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.LabId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__equipment__lab_i__7849DB76");

            // Seed data for Equipment
            entity.HasData(
                new Equipment { EquipmentId = 1, LabId = 1, Name = "Microscope", Code = "EQ-001", Description = "Digital microscope with 1000x magnification", Status = 1 },
                new Equipment { EquipmentId = 2, LabId = 1, Name = "Centrifuge", Code = "EQ-002", Description = "High-speed centrifuge", Status = 1 },
                new Equipment { EquipmentId = 3, LabId = 2, Name = "Computer Station", Code = "EQ-003", Description = "Workstation with development tools", Status = 1 },
                new Equipment { EquipmentId = 4, LabId = 2, Name = "Server Rack", Code = "EQ-004", Description = "Network server infrastructure", Status = 1 }
            );
        });

        modelBuilder.Entity<EventParticipant>(entity =>
        {
            entity.HasKey(e => new { e.EventId, e.UserId }).HasName("PK__event_pa__C8EB1457D657AA72");

            entity.ToTable("event_participants");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Role)
                .HasColumnType("decimal(2, 0)")
                .HasColumnName("role");

            entity.HasOne(d => d.Event).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__event_par__event__078C1F06");

            entity.HasOne(d => d.User).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__event_par__user___0880433F");
        });

        modelBuilder.Entity<Lab>(entity =>
        {
            entity.HasKey(e => e.LabId).HasName("PK__labs__66DE64DB381C94E8");

            entity.ToTable("labs");

            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("location");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");

            entity.HasOne(d => d.Manager).WithMany(p => p.Labs)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__labs__manager_id__6FB49575");

            // Seed data for Labs
            entity.HasData(
                new Lab { LabId = 1, Name = "Biology Lab", ManagerId = 2, Location = "Building A - Floor 2", Description = "Laboratory for biology experiments and research" },
                new Lab { LabId = 2, Name = "Computer Lab", ManagerId = 2, Location = "Building B - Floor 3", Description = "Computer lab with 30 workstations" },
                new Lab { LabId = 3, Name = "Chemistry Lab", ManagerId = 3, Location = "Building A - Floor 1", Description = "Chemistry laboratory with safety equipment" }
            );
        });

        modelBuilder.Entity<LabEvent>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__lab_even__2370F7275CD0E23F");

            entity.ToTable("lab_events");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.ActivityTypeId).HasColumnName("activity_type_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.OrganizerId).HasColumnName("organizer_id");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasColumnType("decimal(2, 0)")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.ActivityType).WithMany(p => p.LabEvents)
                .HasForeignKey(d => d.ActivityTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lab_event__activ__03BB8E22");

            entity.HasOne(d => d.Lab).WithMany(p => p.LabEvents)
                .HasForeignKey(d => d.LabId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lab_event__lab_i__01D345B0");

            entity.HasOne(d => d.Organizer).WithMany(p => p.LabEvents)
                .HasForeignKey(d => d.OrganizerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lab_event__organ__04AFB25B");

            entity.HasOne(d => d.Zone).WithMany(p => p.LabEvents)
                .HasForeignKey(d => d.ZoneId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lab_event__zone___02C769E9");
        });

        modelBuilder.Entity<LabZone>(entity =>
        {
            entity.HasKey(e => e.ZoneId).HasName("PK__lab_zone__80B401DFE18A342C");

            entity.ToTable("lab_zones");

            entity.Property(e => e.ZoneId).HasColumnName("zone_id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");

            entity.HasOne(d => d.Lab).WithMany(p => p.LabZones)
                .HasForeignKey(d => d.LabId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lab_zones__lab_i__72910220");

            // Seed data for LabZones
            entity.HasData(
                new LabZone { ZoneId = 1, LabId = 1, Name = "Zone A", Description = "Main experiment area" },
                new LabZone { ZoneId = 2, LabId = 1, Name = "Zone B", Description = "Storage and preparation area" },
                new LabZone { ZoneId = 3, LabId = 2, Name = "Zone A", Description = "Development stations" },
                new LabZone { ZoneId = 4, LabId = 2, Name = "Zone B", Description = "Server room" },
                new LabZone { ZoneId = 5, LabId = 3, Name = "Zone A", Description = "Chemical storage" }
            );
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__notifica__E059842F312DB8D7");

            entity.ToTable("notifications");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.Message)
                .HasColumnType("text")
                .HasColumnName("message");
            entity.Property(e => e.RecipientId).HasColumnName("recipient_id");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("sent_at");

            entity.HasOne(d => d.Event).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__notificat__event__12FDD1B2");

            entity.HasOne(d => d.Recipient).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.RecipientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__notificat__recip__1209AD79");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__reports__779B7C581E1EA1F5");

            entity.ToTable("reports");

            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.GeneratedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("generated_at");
            entity.Property(e => e.GeneratedBy).HasColumnName("generated_by");
            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.ReportType)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("report_type");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

            entity.HasOne(d => d.GeneratedByNavigation).WithMany(p => p.Reports)
                .HasForeignKey(d => d.GeneratedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__reports__generat__16CE6296");

            entity.HasOne(d => d.Lab).WithMany(p => p.Reports)
                .HasForeignKey(d => d.LabId)
                .HasConstraintName("FK__reports__lab_id__17C286CF");

            entity.HasOne(d => d.Zone).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ZoneId)
                .HasConstraintName("FK__reports__zone_id__18B6AB08");
        });

        modelBuilder.Entity<SecurityLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__security__9E2397E0366827BF");

            entity.ToTable("security_logs");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.Action)
                .HasColumnType("decimal(2, 0)")
                .HasColumnName("action");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("photo_url");
            entity.Property(e => e.SecurityId).HasColumnName("security_id");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp");

            entity.HasOne(d => d.Event).WithMany(p => p.SecurityLogs)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__security___event__0C50D423");

            entity.HasOne(d => d.Security).WithMany(p => p.SecurityLogs)
                .HasForeignKey(d => d.SecurityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__security___secur__0D44F85C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370FBC272336");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164598EED90").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasColumnType("decimal(2, 0)")
                .HasColumnName("role");

            // Seed data for Users
            // Note: In production, use proper password hashing (e.g., BCrypt)
            entity.HasData(
                new User { UserId = 1, Name = "Admin User", Email = "admin@lab.com", PasswordHash = "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", Role = 1, CreatedAt = new DateTime(2025, 1, 1) },
                new User { UserId = 2, Name = "School Manager", Email = "schoolmanager@lab.com", PasswordHash = "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", Role = 2, CreatedAt = new DateTime(2025, 1, 1) },
                new User { UserId = 3, Name = "Lab Manager", Email = "manager@lab.com", PasswordHash = "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", Role = 3, CreatedAt = new DateTime(2025, 1, 1) },
                new User { UserId = 4, Name = "Security Staff", Email = "security@lab.com", PasswordHash = "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", Role = 4, CreatedAt = new DateTime(2025, 1, 1) },
                new User { UserId = 5, Name = "Student User", Email = "student@lab.com", PasswordHash = "$2y$10$hHPvRxU0fb3iOGs2z2VeEuEZ0UTfBS/L6LINEkQ5uElUYJXpbSFsC", Role = 5, CreatedAt = new DateTime(2025, 1, 1) }
            );
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}