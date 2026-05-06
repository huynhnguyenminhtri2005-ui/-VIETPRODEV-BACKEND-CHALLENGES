using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TestAPIChallenges.Models;

public partial class CourseManagementContext : DbContext
{
    public CourseManagementContext()
    {
    }

    public CourseManagementContext(DbContextOptions<CourseManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAuth> UserAuths { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=CourseManagement;Username=postgres;Password=123456");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("classes_pkey");

            entity.ToTable("classes");

            entity.HasIndex(e => e.ClassCode, "classes_class_code_key").IsUnique();

            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.ClassCode)
                .HasMaxLength(50)
                .HasColumnName("class_code");
            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.Course).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("classes_course_id_fkey");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("courses_pkey");

            entity.ToTable("courses");

            entity.Property(e => e.CourseId).HasColumnName("course_id");
            entity.Property(e => e.CourseName)
                .HasMaxLength(200)
                .HasColumnName("course_name");
            entity.Property(e => e.Credits).HasColumnName("credits");
            entity.Property(e => e.Description).HasColumnName("description");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("enrollments_pkey");

            entity.ToTable("enrollments");

            entity.HasIndex(e => new { e.UserId, e.ClassId }, "unique_enrollment").IsUnique();

            entity.Property(e => e.EnrollmentId).HasColumnName("enrollment_id");
            entity.Property(e => e.ClassId).HasColumnName("class_id");
            entity.Property(e => e.EnrollDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("enroll_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Active'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Class).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("enrollments_class_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("enrollments_user_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.RoleName, "roles_role_name_key").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "idx_user_email");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_role_id_fkey");
        });

        modelBuilder.Entity<UserAuth>(entity =>
        {
            entity.HasKey(e => e.AuthId).HasName("user_auth_pkey");

            entity.ToTable("user_auth");

            entity.HasIndex(e => e.Username, "idx_auth_username");

            entity.HasIndex(e => e.UserId, "user_auth_user_id_key").IsUnique();

            entity.HasIndex(e => e.Username, "user_auth_username_key").IsUnique();

            entity.Property(e => e.AuthId).HasColumnName("auth_id");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.User).WithOne(p => p.UserAuth)
                .HasForeignKey<UserAuth>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("user_auth_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
