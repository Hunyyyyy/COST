using System;
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

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectMember> ProjectMembers { get; set; }

    public virtual DbSet<ProjectNotification> ProjectNotifications { get; set; }

    public virtual DbSet<ProjectProgress> ProjectProgresses { get; set; }

    public virtual DbSet<ProjectTask> ProjectTasks { get; set; }

    public virtual DbSet<ProjectUser> ProjectUsers { get; set; }

    public virtual DbSet<Reminder> Reminders { get; set; }

    public virtual DbSet<SaveTask> SaveTasks { get; set; }

    public virtual DbSet<SaveTasksReminder> SaveTasksReminders { get; set; }

    public virtual DbSet<SentTasksList> SentTasksLists { get; set; }

    public virtual DbSet<SubmittedSubtask> SubmittedSubtasks { get; set; }

    public virtual DbSet<SubmittedSubtasksByManager> SubmittedSubtasksByManagers { get; set; }

    public virtual DbSet<Subtask> Subtasks { get; set; }

    public virtual DbSet<SubtaskProgress> SubtaskProgresses { get; set; }

    public virtual DbSet<TaskNotification> TaskNotifications { get; set; }

    public virtual DbSet<TaskProgress> TaskProgresses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-R74JRM89\\HUY;Initial Catalog=TestNhiemVu;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_CI_AS");

        modelBuilder.Entity<AssignedSubtask>(entity =>
        {
            entity.HasKey(e => e.AssignedSubtaskId).HasName("PK__Assigned__FEE319FFFB521838");

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
                .HasConstraintName("FK__AssignedS__Assig__6B24EA82");

            entity.HasOne(d => d.Subtask).WithMany(p => p.AssignedSubtasks)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__AssignedS__Subta__6A30C649");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Groups__149AF36AF2BE4767");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GroupName).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Groups__CreatedB__6E01572D");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => e.GroupMemberId).HasName("PK__GroupMem__34481292BF07BC27");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__GroupMemb__Group__6C190EBB");

            entity.HasOne(d => d.User).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__GroupMemb__UserI__6D0D32F4");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABEF049B5AC1C");

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
                .HasConstraintName("FK__Projects__Manage__73BA3083");
        });

        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(e => e.ProjectMemberId).HasName("PK__ProjectM__E4E9981C4490FA2F");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectMe__Proje__6EF57B66");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ProjectMe__UserI__6FE99F9F");
        });

        modelBuilder.Entity<ProjectNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__ProjectN__20CF2E128A6CB36D");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectNotifications)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectNo__Proje__70DDC3D8");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectNotifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ProjectNo__UserI__71D1E811");
        });

        modelBuilder.Entity<ProjectProgress>(entity =>
        {
            entity.HasKey(e => e.ProjectProgressId).HasName("PK__ProjectP__9766AC670F139F37");

            entity.ToTable("ProjectProgress");

            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Progress).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectProgresses)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectPr__Proje__72C60C4A");
        });

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(e => e.ProjectTaskId).HasName("PK__ProjectT__71C01D044B0CA000");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectTa__Proje__74AE54BC");

            entity.HasOne(d => d.Task).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectTa__TaskI__75A278F5");
        });

        modelBuilder.Entity<ProjectUser>(entity =>
        {
            entity.HasKey(e => e.ProjectUserId).HasName("PK__ProjectU__4F7A4900F0A1B6B4");

            entity.HasIndex(e => e.ProjectId, "IX_ProjectUsers_ProjectId").IsUnique();

            entity.Property(e => e.Role).HasMaxLength(50);

            entity.HasOne(d => d.Project).WithOne(p => p.ProjectUser)
                .HasForeignKey<ProjectUser>(d => d.ProjectId)
                .HasConstraintName("FK_ProjectUsers_Projects");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectUs__UserI__76969D2E");
        });

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasKey(e => e.ReminderId).HasName("PK__Reminder__01A83087577DA793");

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
                .HasConstraintName("FK__Reminders__UserI__787EE5A0");
        });

        modelBuilder.Entity<SaveTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__SaveTask__7C6949B1103C95E2");

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
                .HasConstraintName("FK__SaveTasks__Assig__7A672E12");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SaveTaskCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SaveTasks__Creat__7B5B524B");

            entity.HasOne(d => d.Project).WithMany(p => p.SaveTasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__SaveTasks__Proje__7C4F7684");
        });

        modelBuilder.Entity<SaveTasksReminder>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__SaveTask__7C6949B1DFC3C93F");

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
                .HasConstraintName("FK__SaveTasks__Assig__7D439ABD");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SaveTasksReminderCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SaveTasks__Creat__7E37BEF6");

            entity.HasOne(d => d.Project).WithMany(p => p.SaveTasksReminders)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SaveTasksReminder_Projects");
        });

        modelBuilder.Entity<SentTasksList>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__SentTask__7C6949B1B4D678FD");

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
                .HasConstraintName("FK__SentTasks__Creat__00200768");
        });

        modelBuilder.Entity<SubmittedSubtask>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submitte__449EE125EF3AF077");

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đang xem xét");
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Project).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__Proje__02FC7413");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__Subta__01142BA1");

            entity.HasOne(d => d.Task).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__TaskI__02084FDA");

            entity.HasOne(d => d.User).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__UserI__03F0984C");
        });

        modelBuilder.Entity<SubmittedSubtasksByManager>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submitte__449EE12501B7841A");

            entity.ToTable("SubmittedSubtasksByManager");

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đ? duy?t");
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Project).WithMany(p => p.SubmittedSubtasksByManagers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__Proje__07C12930");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubmittedSubtasksByManagers)
                .HasForeignKey(d => d.SubtaskId)
                .HasConstraintName("FK__Submitted__Subta__05D8E0BE");

            entity.HasOne(d => d.Task).WithMany(p => p.SubmittedSubtasksByManagers)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__TaskI__06CD04F7");

            entity.HasOne(d => d.User).WithMany(p => p.SubmittedSubtasksByManagers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__UserI__08B54D69");
        });

        modelBuilder.Entity<Subtask>(entity =>
        {
            entity.HasKey(e => e.SubtaskId).HasName("PK__Subtasks__E087179660F43EA6");

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
                .HasConstraintName("FK__Subtasks__Assign__0C85DE4D");

            entity.HasOne(d => d.Project).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Subtasks__Projec__0D7A0286");

            entity.HasOne(d => d.Task).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Subtasks__TaskId__0B91BA14");
        });

        modelBuilder.Entity<SubtaskProgress>(entity =>
        {
            entity.HasKey(e => e.SubtaskProgressId).HasName("PK__SubtaskP__F29F4FF871C3F368");

            entity.ToTable("SubtaskProgress");

            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Progress).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.SubtaskProgresses)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__SubtaskPr__Assig__0A9D95DB");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubtaskProgresses)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__SubtaskPr__Subta__09A971A2");
        });

        modelBuilder.Entity<TaskNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__TaskNoti__20CF2E12F6FF64B7");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.Task).WithMany(p => p.TaskNotifications)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TaskNotif__TaskI__0F624AF8");

            entity.HasOne(d => d.User).WithMany(p => p.TaskNotifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__TaskNotif__UserI__0E6E26BF");
        });

        modelBuilder.Entity<TaskProgress>(entity =>
        {
            entity.HasKey(e => e.TaskProgressId).HasName("PK__TaskProg__C944EC697C0B485B");

            entity.ToTable("TaskProgress");

            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Progress).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskProgresses)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TaskProgr__TaskI__10566F31");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CE30A286B");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PasswordSalt).HasMaxLength(256);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserLogi__1788CC4C6A71BF2E");

            entity.ToTable("UserLogin");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.FailedLoginAttempts).HasDefaultValue(0);
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PasswordSalt).HasMaxLength(256);

            entity.HasOne(d => d.User).WithOne(p => p.UserLogin)
                .HasForeignKey<UserLogin>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserLogin__UserI__114A936A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
