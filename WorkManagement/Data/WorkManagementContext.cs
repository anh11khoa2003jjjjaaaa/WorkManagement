using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WorkManagement.Models;

namespace WorkManagement.Data;

public partial class WorkManagementContext : DbContext
{
    public WorkManagementContext()
    {
    }

    public WorkManagementContext(DbContextOptions<WorkManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Otp> Otps { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectTask> ProjectTasks { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<StatusJob> StatusJobs { get; set; }

    public virtual DbSet<TaskEmployee> TaskEmployees { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-S7EK1LR;Database=QuanLyCongViec;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Accounts__3214EC07A0B0777F");

            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Accounts__RoleId__4AB81AF0");

            entity.HasOne(d => d.User).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Accounts__UserId__49C3F6B7");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Departme__3214EC070BC56D86");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Images__3214EC07BD01DDDC");

            entity.Property(e => e.FilePath).HasMaxLength(500);

            entity.HasOne(d => d.Project).WithMany(p => p.Images)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Images__ProjectI__6383C8BA");

            entity.HasOne(d => d.Task).WithMany(p => p.Images)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__Images__TaskId__628FA481");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC07BF9FE3BB");

            entity.Property(e => e.Content).HasMaxLength(500);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__5FB337D6");
        });

        modelBuilder.Entity<Otp>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Otps__3214EC07EC94B133");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpiryAt).HasColumnType("datetime");
            entity.Property(e => e.Otpcode).HasMaxLength(50);

            entity.HasOne(d => d.Account).WithMany(p => p.Otps)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK__Otps__AccountId__66603565");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Position__3214EC072DD9E737");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Projects__3214EC0718591F46");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.StartDate).HasColumnType("datetime");

            entity.HasOne(d => d.Department).WithMany(p => p.Projects)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Projects__Depart__5070F446");

            entity.HasOne(d => d.Manager).WithMany(p => p.Projects)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__Projects__Manage__5165187F");

            entity.HasOne(d => d.Status).WithMany(p => p.Projects)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__Projects__Status__4F7CD00D");
        });

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectT__3214EC07B7258190");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__ProjectTa__Proje__5441852A");

            entity.HasOne(d => d.Status).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK__ProjectTa__Statu__5535A963");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07323CBA8A");

            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<StatusJob>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StatusJo__3214EC07BD782060");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<TaskEmployee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TaskEmpl__3214EC07776830F8");

            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CompletionDate).HasColumnType("datetime");
            entity.Property(e => e.Deadline)
                .HasDefaultValue(new DateTime(2099, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified))
                .HasColumnType("datetime");
            entity.Property(e => e.PenaltyStatus).HasDefaultValue(false);

            entity.HasOne(d => d.Status).WithMany(p => p.TaskEmployees)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("FK_TaskEmployees_StatusJobs");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskEmployees)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__TaskEmplo__TaskI__59063A47");

            entity.HasOne(d => d.User).WithMany(p => p.TaskEmployees)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__TaskEmplo__UserI__59FA5E80");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07CEB466EB");

            entity.Property(e => e.Avatar).HasMaxLength(500);
            entity.Property(e => e.BirthDay).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.IsLeader).HasDefaultValue(false);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");

            entity.HasOne(d => d.Department).WithMany(p => p.Users)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK__Users__Departmen__46E78A0C");

            entity.HasOne(d => d.Position).WithMany(p => p.Users)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK__Users__PositionI__45F365D3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
