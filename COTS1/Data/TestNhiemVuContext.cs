﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace COTS1.Data;

public partial class TestNhiemVuContext : DbContext
{
    public TestNhiemVuContext()
    {
    }

    public TestNhiemVuContext(DbContextOptions<TestNhiemVuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AssignedSubtask> AssignedSubtasks { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectUser> ProjectUsers { get; set; }

    public virtual DbSet<Reminder> Reminders { get; set; }

    public virtual DbSet<SaveTask> SaveTasks { get; set; }

    public virtual DbSet<SaveTasksReminder> SaveTasksReminders { get; set; }

    public virtual DbSet<SentTasksList> SentTasksLists { get; set; }

    public virtual DbSet<SubmittedSubtask> SubmittedSubtasks { get; set; }

    public virtual DbSet<Subtask> Subtasks { get; set; }

    public virtual DbSet<SubtaskProgress> SubtaskProgresses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-R74JRM89\\HUY;Initial Catalog=TestNhiemVu;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_CI_AS");

        modelBuilder.Entity<AssignedSubtask>(entity =>
        {
            entity.HasKey(e => e.AssignedSubtaskId).HasName("PK__Assigned__FEE319FF24D26DB0");

            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chưa nh?n");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.AssignedSubtasks)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__AssignedS__Assig__5FB337D6");

            entity.HasOne(d => d.Subtask).WithMany(p => p.AssignedSubtasks)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__AssignedS__Subta__5EBF139D");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABEF0C1496EC4");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Progress).HasDefaultValue(0);
            entity.Property(e => e.ProjectName).HasMaxLength(200);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chưa b?t đ?u");

            entity.HasOne(d => d.Manager).WithMany(p => p.Projects)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__Projects__Manage__60A75C0F");
        });

        modelBuilder.Entity<ProjectUser>(entity =>
        {
            entity.HasKey(e => e.ProjectUserId).HasName("PK__ProjectU__4F7A490089710D2B");

            entity.HasIndex(e => new { e.ProjectId, e.UserId }, "UC_ProjectUsers").IsUnique();

            entity.Property(e => e.Role).HasMaxLength(50);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectUsers)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_ProjectUsers_Projects");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectUs__UserI__619B8048");
        });

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasKey(e => e.ReminderId).HasName("PK__Reminder__01A8308739EDA9E3");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsAcknowledged).HasDefaultValue(false);
            entity.Property(e => e.ProjectName).HasMaxLength(200);
            entity.Property(e => e.ReminderDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(200);

            entity.HasOne(d => d.Project).WithMany(p => p.Reminders)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK_Reminders_Projects");

            entity.HasOne(d => d.User).WithMany(p => p.Reminders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reminders__UserI__6383C8BA");
        });

        modelBuilder.Entity<SaveTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__SaveTask__7C6949B16321D5B8");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Priority).HasMaxLength(50);
            entity.Property(e => e.Progress).HasDefaultValue(0);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đang ch?");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.SaveTaskAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__SaveTasks__Assig__656C112C");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SaveTaskCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SaveTasks__Creat__66603565");

            entity.HasOne(d => d.Project).WithMany(p => p.SaveTasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__SaveTasks__Proje__6754599E");
        });

        modelBuilder.Entity<SaveTasksReminder>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__SaveTask__7C6949B16FF5EA1F");

            entity.ToTable("SaveTasksReminder");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Priority).HasMaxLength(50);
            entity.Property(e => e.ReminderDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.SaveTasksReminderAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__SaveTasks__Assig__68487DD7");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SaveTasksReminderCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SaveTasks__Creat__693CA210");

            entity.HasOne(d => d.Project).WithMany(p => p.SaveTasksReminders)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SaveTasksReminder_Projects");
        });

        modelBuilder.Entity<SentTasksList>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__SentTask__7C6949B1DF26C9D9");

            entity.ToTable("SentTasksList");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(256);
            entity.Property(e => e.Priority).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đang ch?");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SentTasksLists)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SentTasks__Creat__6B24EA82");
        });

        modelBuilder.Entity<SubmittedSubtask>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submitte__449EE1257669BCDE");

            entity.Property(e => e.ApprovedBy).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đang xem xét");
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Project).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__Proje__6C190EBB");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__Subta__6E01572D");

            entity.HasOne(d => d.Task).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__TaskI__6EF57B66");

            entity.HasOne(d => d.User).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__UserI__6D0D32F4");
        });

        modelBuilder.Entity<Subtask>(entity =>
        {
            entity.HasKey(e => e.SubtaskId).HasName("PK__Subtasks__E0871796DFC955B7");

            entity.Property(e => e.Assigner).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chưa nh?n");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__Subtasks__Assign__73BA3083");

            entity.HasOne(d => d.Project).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Subtasks__Projec__74AE54BC");

            entity.HasOne(d => d.Task).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Subtasks__TaskId__72C60C4A");
        });

        modelBuilder.Entity<SubtaskProgress>(entity =>
        {
            entity.HasKey(e => e.SubtaskProgressId).HasName("PK__SubtaskP__F29F4FF84041E725");

            entity.ToTable("SubtaskProgress");

            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Progress).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.SubtaskProgresses)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__SubtaskPr__Assig__71D1E811");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubtaskProgresses)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__SubtaskPr__Subta__70DDC3D8");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CD38F7E54");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PasswordSalt).HasMaxLength(256);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
