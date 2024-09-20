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
<<<<<<< HEAD
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-BDUKE70U\\SQLEXPRESS01;Initial Catalog=TestNhiemVu;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
=======
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-R74JRM89\\HUY;Initial Catalog=TestNhiemVu;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
>>>>>>> 703f596acaadb5b2aac6f76d9b104b12d7fc423d

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssignedSubtask>(entity =>
        {
            entity.HasKey(e => e.AssignedSubtaskId).HasName("PK__Assigned__FEE319FF7922160F");

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
                .HasConstraintName("FK__AssignedS__Assig__55F4C372");

            entity.HasOne(d => d.Subtask).WithMany(p => p.AssignedSubtasks)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__AssignedS__Subta__55009F39");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Groups__149AF36A7E77C136");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GroupName).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Groups__CreatedB__3D5E1FD2");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => e.GroupMemberId).HasName("PK__GroupMem__3448129273A1C61A");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__GroupMemb__Group__412EB0B6");

            entity.HasOne(d => d.User).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__GroupMemb__UserI__4222D4EF");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABEF071C15089");

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
                .HasConstraintName("FK__Projects__Manage__45F365D3");
        });

        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(e => e.ProjectMemberId).HasName("PK__ProjectM__E4E9981CF0DACDC0");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectMe__Proje__49C3F6B7");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ProjectMe__UserI__4AB81AF0");
        });

        modelBuilder.Entity<ProjectNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__ProjectN__20CF2E1264ABF864");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectNotifications)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectNo__Proje__5165187F");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectNotifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ProjectNo__UserI__52593CB8");
        });

        modelBuilder.Entity<ProjectProgress>(entity =>
        {
            entity.HasKey(e => e.ProjectProgressId).HasName("PK__ProjectP__9766AC678CF25503");

            entity.ToTable("ProjectProgress");

            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Progress).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectProgresses)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectPr__Proje__6B24EA82");
        });

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(e => e.ProjectTaskId).HasName("PK__ProjectT__71C01D04E75CB2F4");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectTa__Proje__4F47C5E3");

            entity.HasOne(d => d.Task).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectTa__TaskI__503BEA1C");
        });

        modelBuilder.Entity<ProjectUser>(entity =>
        {
            entity.HasKey(e => e.ProjectUserId).HasName("PK__ProjectU__4F7A49003D892596");

            entity.Property(e => e.Role).HasMaxLength(50);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectUsers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectUs__Proje__5AEE82B9");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectUs__UserI__5BE2A6F2");
        });

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasKey(e => e.ReminderId).HasName("PK__Reminder__01A83087B9826B8F");

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
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reminders__Proje__2334397B");

            entity.HasOne(d => d.User).WithMany(p => p.Reminders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reminders__UserI__24285DB4");
        });

        modelBuilder.Entity<SaveTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__SaveTask__7C6949B18515055B");

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
                .HasConstraintName("FK__SaveTasks__Assig__3D2915A8");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SaveTaskCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SaveTasks__Creat__3E1D39E1");

            entity.HasOne(d => d.Project).WithMany(p => p.SaveTasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__SaveTasks__Proje__40058253");
        });

        modelBuilder.Entity<SaveTasksReminder>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__SaveTask__7C6949B137F3F12C");

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
                .HasConstraintName("FK__SaveTasks__Assig__2704CA5F");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SaveTasksReminderCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SaveTasks__Creat__27F8EE98");

            entity.HasOne(d => d.Project).WithMany(p => p.SaveTasksReminders)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__SaveTasks__Proje__29E1370A");
        });

        modelBuilder.Entity<SentTasksList>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__SentTask__7C6949B1C4877BB0");

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
                .HasConstraintName("FK__SentTasks__Creat__14270015");
        });

        modelBuilder.Entity<SubmittedSubtask>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submitte__449EE1258820E79E");

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đang xem xét");
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Project).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__Proje__10216507");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__Subta__0E391C95");

            entity.HasOne(d => d.Task).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__TaskI__0F2D40CE");

            entity.HasOne(d => d.User).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__UserI__11158940");
        });

        modelBuilder.Entity<SubmittedSubtasksByManager>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submitte__449EE12583BDC46F");

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
                .HasConstraintName("FK__Submitted__Proje__1D7B6025");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubmittedSubtasksByManagers)
                .HasForeignKey(d => d.SubtaskId)
                .HasConstraintName("FK__Submitted__Subta__1B9317B3");

            entity.HasOne(d => d.Task).WithMany(p => p.SubmittedSubtasksByManagers)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__TaskI__1C873BEC");

            entity.HasOne(d => d.User).WithMany(p => p.SubmittedSubtasksByManagers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__UserI__1E6F845E");
        });

        modelBuilder.Entity<Subtask>(entity =>
        {
            entity.HasKey(e => e.SubtaskId).HasName("PK__Subtasks__E0871796BD30A0DE");

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
                .HasConstraintName("FK__Subtasks__Assign__45BE5BA9");

            entity.HasOne(d => d.Project).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Subtasks__Projec__7849DB76");

            entity.HasOne(d => d.Task).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Subtasks__TaskId__43D61337");
        });

        modelBuilder.Entity<SubtaskProgress>(entity =>
        {
            entity.HasKey(e => e.SubtaskProgressId).HasName("PK__SubtaskP__F29F4FF86E5FCCD5");

            entity.ToTable("SubtaskProgress");

            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Progress).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.SubtaskProgresses)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__SubtaskPr__Assig__5CA1C101");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubtaskProgresses)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__SubtaskPr__Subta__5BAD9CC8");
        });

        modelBuilder.Entity<TaskNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__TaskNoti__20CF2E1234048C6B");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.Task).WithMany(p => p.TaskNotifications)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TaskNotif__TaskI__4A8310C6");

            entity.HasOne(d => d.User).WithMany(p => p.TaskNotifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__TaskNotif__UserI__498EEC8D");
        });

        modelBuilder.Entity<TaskProgress>(entity =>
        {
            entity.HasKey(e => e.TaskProgressId).HasName("PK__TaskProg__C944EC69A8D5E2F0");

            entity.ToTable("TaskProgress");

            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Progress).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskProgresses)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TaskProgr__TaskI__6166761E");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C901051F2");

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
            entity.HasKey(e => e.UserId).HasName("PK__UserLogi__1788CC4C453AE00B");

            entity.ToTable("UserLogin");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.FailedLoginAttempts).HasDefaultValue(0);
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PasswordSalt).HasMaxLength(256);

            entity.HasOne(d => d.User).WithOne(p => p.UserLogin)
                .HasForeignKey<UserLogin>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserLogin__UserI__398D8EEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
