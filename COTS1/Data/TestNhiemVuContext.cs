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

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectMember> ProjectMembers { get; set; }

    public virtual DbSet<ProjectNotification> ProjectNotifications { get; set; }

    public virtual DbSet<ProjectTask> ProjectTasks { get; set; }

    public virtual DbSet<ProjectUser> ProjectUsers { get; set; }

    public virtual DbSet<SaveTask> SaveTasks { get; set; }

    public virtual DbSet<SentTasksList> SentTasksLists { get; set; }

    public virtual DbSet<Subtask> Subtasks { get; set; }

    public virtual DbSet<TaskNotification> TaskNotifications { get; set; }

    public virtual DbSet<TaskProgress> TaskProgresses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-R74JRM89\\HUY;Initial Catalog=TestNhiemVu;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__Groups__149AF36A82CC9CBB");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.GroupName).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Groups)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Groups__CreatedB__4222D4EF");
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasKey(e => e.GroupMemberId).HasName("PK__GroupMem__34481292103F5E27");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__GroupMemb__Group__45F365D3");

            entity.HasOne(d => d.User).WithMany(p => p.GroupMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__GroupMemb__UserI__46E78A0C");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__761ABEF089C7D1D8");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ProjectName).HasMaxLength(200);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chua b?t d?u");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProjectCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Projects__Create__70DDC3D8");

            entity.HasOne(d => d.Manager).WithMany(p => p.ProjectManagers)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK__Projects__Manage__5BE2A6F2");
        });

        modelBuilder.Entity<ProjectMember>(entity =>
        {
            entity.HasKey(e => e.ProjectMemberId).HasName("PK__ProjectM__E4E9981C5FDCFFFB");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectMe__Proje__5FB337D6");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectMembers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ProjectMe__UserI__60A75C0F");
        });

        modelBuilder.Entity<ProjectNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__ProjectN__20CF2E12EE5139EA");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectNotifications)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectNo__Proje__6754599E");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectNotifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__ProjectNo__UserI__68487DD7");
        });

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(e => e.ProjectTaskId).HasName("PK__ProjectT__71C01D04BB1FAB7A");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectTa__Proje__6383C8BA");

            entity.HasOne(d => d.Task).WithMany(p => p.ProjectTasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProjectTa__TaskI__6477ECF3");
        });

        modelBuilder.Entity<ProjectUser>(entity =>
        {
            entity.HasKey(e => e.ProjectUserId).HasName("PK__ProjectU__4F7A4900FC3E3E47");

            entity.Property(e => e.Role).HasMaxLength(50);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectUsers)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectUs__Proje__02FC7413");

            entity.HasOne(d => d.User).WithMany(p => p.ProjectUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProjectUs__UserI__03F0984C");
        });

        modelBuilder.Entity<SaveTask>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Tasks__7C6949B14588A461");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(256);
            entity.Property(e => e.Priority).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Ðang ch?");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.SaveTaskAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__Tasks__AssignedT__2C3393D0");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SaveTaskCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Tasks__CreatedBy__2D27B809");

            entity.HasOne(d => d.Project).WithMany(p => p.SaveTasks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__SaveTasks__Proje__6FE99F9F");
        });

        modelBuilder.Entity<SentTasksList>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__SentTask__7C6949B191512D41");

            entity.ToTable("SentTasksList");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Note).HasMaxLength(256);
            entity.Property(e => e.Priority).HasMaxLength(50);
            entity.Property(e => e.ProjectId).HasColumnName("ProjectID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Ðang ch?");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.SentTasksLists)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__SentTasks__Creat__6E01572D");

            entity.HasOne(d => d.Project).WithMany(p => p.SentTasksLists)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__SentTasks__Proje__73BA3083");
        });

        modelBuilder.Entity<Subtask>(entity =>
        {
            entity.HasKey(e => e.SubtaskId).HasName("PK__Subtasks__E0871796E0EC8E71");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Chua nh?n");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__Subtasks__Assign__32E0915F");

            entity.HasOne(d => d.Task).WithMany(p => p.Subtasks)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_Subtasks_SaveTasks");
        });

        modelBuilder.Entity<TaskNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__TaskNoti__20CF2E12E7663FC3");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.Task).WithMany(p => p.TaskNotifications)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK_TaskNotifications_SaveTasks");

            entity.HasOne(d => d.User).WithMany(p => p.TaskNotifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__TaskNotif__UserI__3B75D760");
        });

        modelBuilder.Entity<TaskProgress>(entity =>
        {
            entity.HasKey(e => e.ProgressId).HasName("PK__TaskProg__BAE29CA539F67C71");

            entity.ToTable("TaskProgress");

            entity.Property(e => e.ProgressDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Subtask).WithMany(p => p.TaskProgresses)
                .HasForeignKey(d => d.SubtaskId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TaskProgr__Subta__36B12243");

            entity.HasOne(d => d.User).WithMany(p => p.TaskProgresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__TaskProgr__UserI__37A5467C");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C88D05740");

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
            entity.HasKey(e => e.UserId).HasName("PK__UserLogi__1788CC4CF7DBC50B");

            entity.ToTable("UserLogin");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.FailedLoginAttempts).HasDefaultValue(0);
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PasswordSalt).HasMaxLength(256);

            entity.HasOne(d => d.User).WithOne(p => p.UserLogin)
                .HasForeignKey<UserLogin>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserLogin__UserI__276EDEB3");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
