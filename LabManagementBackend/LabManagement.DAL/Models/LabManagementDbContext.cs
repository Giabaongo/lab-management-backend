using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace LabManagement.DAL.Models;

public partial class LabManagementDbContext : DbContext
{
    public LabManagementDbContext(DbContextOptions<LabManagementDbContext> options)
        : base(options)
    {
    }

    // REMOVED: GetConnectionString and OnConfiguring methods
    // DbContext configuration is now properly handled via Dependency Injection in Program.cs
    // This follows .NET best practices and works correctly in containerized environments

    public virtual DbSet<ActivityType> ActivityTypes { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<EventParticipants> EventParticipants { get; set; }

    public virtual DbSet<LabEvent> LabEvents { get; set; }

    public virtual DbSet<LabZone> LabZones { get; set; }

    public virtual DbSet<Lab> Labs { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<SecurityLog> SecurityLogs { get; set; }

    public virtual DbSet<UserDepartment> UserDepartments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActivityType>(entity =>
        {
            entity.HasKey(e => e.ActivityTypeId).HasName("PK__activity__D2470C8792B1F20E");

            entity.ToTable("activity_types");

            entity.Property(e => e.ActivityTypeId).HasColumnName("activity_type_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__bookings__5DE3A5B17836BDD3");

            entity.ToTable("bookings");

            entity.HasIndex(e => e.LabId, "IX_bookings_lab_id");

            entity.HasIndex(e => e.UserId, "IX_bookings_user_id");

            entity.HasIndex(e => e.ZoneId, "IX_bookings_zone_id");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.EndTime)
                .HasPrecision(3)
                .HasColumnName("end_time");
            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasColumnName("row_version");
            entity.Property(e => e.StartTime)
                .HasPrecision(3)
                .HasColumnName("start_time");
            entity.Property(e => e.Status).HasColumnName("status");
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

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__departme__C2232422");

            entity.ToTable("departments");

            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsPublic).HasColumnName("is_public");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("PK__equipmen__197068AFC616B45E");

            entity.ToTable("equipment");

            entity.HasIndex(e => e.LabId, "IX_equipment_lab_id");

            entity.HasIndex(e => e.Code, "UQ__equipmen__357D4CF958F5304E").IsUnique();

            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Lab).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.LabId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__equipment__lab_i__7849DB76");
        });

        modelBuilder.Entity<EventParticipants>(entity =>
        {
            entity.HasKey(e => new { e.EventId, e.UserId }).HasName("PK__event_pa__C8EB1457D657AA72");

            entity.ToTable("event_participants");

            entity.HasIndex(e => e.UserId, "IX_event_participants_user_id");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Role).HasColumnName("role");

            entity.HasOne(d => d.Event).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__event_par__event__078C1F06");

            entity.HasOne(d => d.User).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__event_par__user___0880433F");
        });

        modelBuilder.Entity<LabEvent>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__lab_even__2370F7275CD0E23F");

            entity.ToTable("lab_events");

            entity.HasIndex(e => e.ActivityTypeId, "IX_lab_events_activity_type_id");

            entity.HasIndex(e => e.LabId, "IX_lab_events_lab_id");

            entity.HasIndex(e => e.OrganizerId, "IX_lab_events_organizer_id");

            entity.HasIndex(e => e.ZoneId, "IX_lab_events_zone_id");

            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.ActivityTypeId).HasColumnName("activity_type_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndTime)
                .HasPrecision(3)
                .HasColumnName("end_time");
            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.OrganizerId).HasColumnName("organizer_id");
            entity.Property(e => e.IsHighPriority)
                .HasDefaultValue(false)
                .HasColumnName("is_high_priority");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasColumnName("row_version");
            entity.Property(e => e.StartTime)
                .HasPrecision(3)
                .HasColumnName("start_time");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
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

            entity.HasIndex(e => e.LabId, "IX_lab_zones_lab_id");

            entity.Property(e => e.ZoneId).HasColumnName("zone_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.Lab).WithMany(p => p.LabZones)
                .HasForeignKey(d => d.LabId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__lab_zones__lab_i__72910220");
        });

        modelBuilder.Entity<Lab>(entity =>
        {
            entity.HasKey(e => e.LabId).HasName("PK__labs__66DE64DB381C94E8");

            entity.ToTable("labs");

            entity.HasIndex(e => e.DepartmentId, "IX_labs_department_id");

            entity.HasIndex(e => e.ManagerId, "IX_labs_manager_id");

            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.DepartmentId)
                .HasDefaultValue(1)
                .HasColumnName("department_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsOpen)
                .HasDefaultValue(false)
                .HasColumnName("is_open");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(1)
                .HasColumnName("status");

            entity.HasOne(d => d.Department).WithMany(p => p.Labs)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__labs__departmen__6EC0713C");

            entity.HasOne(d => d.Manager).WithMany(p => p.Labs)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__labs__manager_id__6FB49575");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__notifica__E059842F312DB8D7");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.EventId, "IX_notifications_event_id");

            entity.HasIndex(e => e.RecipientId, "IX_notifications_recipient_id");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.RecipientId).HasColumnName("recipient_id");
            entity.Property(e => e.SentAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
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

            entity.HasIndex(e => e.LabId, "IX_reports_lab_id");

            entity.HasIndex(e => e.UserId, "IX_reports_user_id");

            entity.HasIndex(e => e.ZoneId, "IX_reports_zone_id");

            entity.Property(e => e.ReportId).HasColumnName("report_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.GeneratedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("generated_at");
            entity.Property(e => e.LabId).HasColumnName("lab_id");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(1000)
                .HasColumnName("photo_url");
            entity.Property(e => e.ReportType)
                .HasMaxLength(100)
                .HasColumnName("report_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ZoneId).HasColumnName("zone_id");

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

            entity.HasIndex(e => e.EventId, "IX_security_logs_event_id");

            entity.HasIndex(e => e.SecurityId, "IX_security_logs_security_id");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.ActionType).HasColumnName("action_type");
            entity.Property(e => e.EventId).HasColumnName("event_id");
            entity.Property(e => e.LoggedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("logged_at");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(255)
                .HasColumnName("photo_url");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasColumnName("row_version");
            entity.Property(e => e.SecurityId).HasColumnName("security_id");

            entity.HasOne(d => d.Event).WithMany(p => p.SecurityLogs)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__security___event__0C50D423");

            entity.HasOne(d => d.Security).WithMany(p => p.SecurityLogs)
                .HasForeignKey(d => d.SecurityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__security___secur__0D44F85C");
        });

        modelBuilder.Entity<UserDepartment>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.DepartmentId }).HasName("PK__user_dep__1EDFFB19");

            entity.ToTable("user_departments");

            entity.HasIndex(e => e.DepartmentId, "IX_user_departments_department_id");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Status)
                .HasDefaultValue(0) // 0 = Pending
                .HasColumnName("status");

            entity.HasOne(d => d.Department).WithMany(p => p.UserDepartments)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__user_depa__depar__74AE549C");

            entity.HasOne(d => d.User).WithMany(p => p.UserDepartments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__user_depa__user___73BA3083");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370FBC272336");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164598EED90").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(128)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role).HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
