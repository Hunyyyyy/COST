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
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-VE716R02\\SQLEXPRESS;Initial Catalog=TestNhiemVu;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Vietnamese_CI_AS");

        modelBuilder.Entity<AssignedSubtask>(entity =>
        {
            entity.HasKey(e => e.AssignedSubtaskId).HasName("PK__Assigned__FEE319FFDD05C361");

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
                .HasConstraintName("FK__AssignedS__Assig__10566F31");

            entity.HasOne(d => d.Subtask).WithMany(p => p.AssignedSubtasks)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__AssignedS__Subta__0F624AF8");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Groups__149AF36A9022CE77");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GroupName).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Groups__CreatedB__1332DBDC");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => e.GroupMemberId).HasName("PK__GroupMem__3448129223EDEA85");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__GroupMemb__Group__114A936A");

            entity.HasOne(d => d.User).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__GroupMemb__UserI__123EB7A3");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABEF0A2B6A0EA");

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
                .HasConstraintName("FK__Projects__Manage__18EBB532");
        });

        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(e => e.ProjectMemberId).HasName("PK__ProjectM__E4E9981C525AE3C1");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectMe__Proje__14270015");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ProjectMe__UserI__151B244E");
        });

        modelBuilder.Entity<ProjectNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__ProjectN__20CF2E12E427899F");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectNotifications)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectNo__Proje__160F4887");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectNotifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ProjectNo__UserI__17036CC0");
        });

        modelBuilder.Entity<ProjectProgress>(entity =>
        {
            entity.HasKey(e => e.ProjectProgressId).HasName("PK__ProjectP__9766AC675F012495");

            entity.ToTable("ProjectProgress");

            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Progress).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectProgresses)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectPr__Proje__17F790F9");
        });

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(e => e.ProjectTaskId).HasName("PK__ProjectT__71C01D040F1B0419");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectTa__Proje__19DFD96B");

            entity.HasOne(d => d.Task).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectTa__TaskI__1AD3FDA4");
        });

        modelBuilder.Entity<ProjectUser>(entity =>
        {
            entity.HasKey(e => e.ProjectUserId).HasName("PK__ProjectU__4F7A49009C13F635");

            entity.HasIndex(e => e.ProjectId, "IX_ProjectUsers_ProjectId").IsUnique();

            entity.Property(e => e.Role).HasMaxLength(50);

            entity.HasOne(d => d.Project).WithOne(p => p.ProjectUser)
                .HasForeignKey<ProjectUser>(d => d.ProjectId)
                .HasConstraintName("FK_ProjectUsers_Projects");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectUs__UserI__1BC821DD");
        });

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasKey(e => e.ReminderId).HasName("PK__Reminder__01A830872906E6A8");

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
                .HasConstraintName("FK__Reminders__UserI__1DB06A4F");
        });

        modelBuilder.Entity<SaveTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__SaveTask__7C6949B13AE99C4A");

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
                .HasConstraintName("FK__SaveTasks__Assig__1F98B2C1");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SaveTaskCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SaveTasks__Creat__208CD6FA");

            entity.HasOne(d => d.Project).WithMany(p => p.SaveTasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__SaveTasks__Proje__2180FB33");
        });

        modelBuilder.Entity<SaveTasksReminder>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__SaveTask__7C6949B14CC5D94D");

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
                .HasConstraintName("FK__SaveTasks__Assig__22751F6C");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SaveTasksReminderCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SaveTasks__Creat__236943A5");

            entity.HasOne(d => d.Project).WithMany(p => p.SaveTasksReminders)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SaveTasksReminder_Projects");
        });

        modelBuilder.Entity<SentTasksList>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__SentTask__7C6949B1FC72E1FB");

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
                .HasConstraintName("FK__SentTasks__Creat__25518C17");
        });

        modelBuilder.Entity<SubmittedSubtask>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submitte__449EE125BECCCF40");

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Đang xem xét");
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Project).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__Proje__282DF8C2");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__Subta__2645B050");

            entity.HasOne(d => d.Task).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__TaskI__2739D489");

            entity.HasOne(d => d.User).WithMany(p => p.SubmittedSubtasks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__UserI__29221CFB");
        });

        modelBuilder.Entity<SubmittedSubtasksByManager>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__Submitte__449EE12516B40ADE");

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
                .HasConstraintName("FK__Submitted__Proje__2CF2ADDF");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubmittedSubtasksByManagers)
                .HasForeignKey(d => d.SubtaskId)
                .HasConstraintName("FK__Submitted__Subta__2B0A656D");

            entity.HasOne(d => d.Task).WithMany(p => p.SubmittedSubtasksByManagers)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__TaskI__2BFE89A6");

            entity.HasOne(d => d.User).WithMany(p => p.SubmittedSubtasksByManagers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submitted__UserI__2DE6D218");
        });

        modelBuilder.Entity<Subtask>(entity =>
        {
            entity.HasKey(e => e.SubtaskId).HasName("PK__Subtasks__E08717966D00F6AC");

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
                .HasConstraintName("FK__Subtasks__Assign__31B762FC");

            entity.HasOne(d => d.Project).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__Subtasks__Projec__32AB8735");

            entity.HasOne(d => d.Task).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Subtasks__TaskId__30C33EC3");
        });

        modelBuilder.Entity<SubtaskProgress>(entity =>
        {
            entity.HasKey(e => e.SubtaskProgressId).HasName("PK__SubtaskP__F29F4FF82120BCBA");

            entity.ToTable("SubtaskProgress");

            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Progress).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.SubtaskProgresses)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__SubtaskPr__Assig__2FCF1A8A");

            entity.HasOne(d => d.Subtask).WithMany(p => p.SubtaskProgresses)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__SubtaskPr__Subta__2EDAF651");
        });

        modelBuilder.Entity<TaskNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__TaskNoti__20CF2E129004F09B");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.Task).WithMany(p => p.TaskNotifications)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TaskNotif__TaskI__3493CFA7");

            entity.HasOne(d => d.User).WithMany(p => p.TaskNotifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__TaskNotif__UserI__339FAB6E");
        });

        modelBuilder.Entity<TaskProgress>(entity =>
        {
            entity.HasKey(e => e.TaskProgressId).HasName("PK__TaskProg__C944EC690DE99AAB");

            entity.ToTable("TaskProgress");

            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Progress).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskProgresses)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TaskProgr__TaskI__3587F3E0");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C11987132");

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
            entity.HasKey(e => e.UserId).HasName("PK__UserLogi__1788CC4C89320614");

            entity.ToTable("UserLogin");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.FailedLoginAttempts).HasDefaultValue(0);
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PasswordSalt).HasMaxLength(256);

            entity.HasOne(d => d.User).WithOne(p => p.UserLogin)
                .HasForeignKey<UserLogin>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserLogin__UserI__367C1819");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
