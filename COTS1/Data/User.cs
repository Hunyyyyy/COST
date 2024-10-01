using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string? PasswordHash { get; set; }

    public string? PasswordSalt { get; set; }

    public virtual ICollection<AssignedSubtask> AssignedSubtasks { get; set; } = new List<AssignedSubtask>();

    public virtual ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();

    public virtual ICollection<SaveTask> SaveTaskAssignedToNavigations { get; set; } = new List<SaveTask>();

    public virtual ICollection<SaveTask> SaveTaskCreatedByNavigations { get; set; } = new List<SaveTask>();

    public virtual ICollection<SaveTasksReminder> SaveTasksReminderAssignedToNavigations { get; set; } = new List<SaveTasksReminder>();

    public virtual ICollection<SaveTasksReminder> SaveTasksReminderCreatedByNavigations { get; set; } = new List<SaveTasksReminder>();

    public virtual ICollection<SentTasksList> SentTasksLists { get; set; } = new List<SentTasksList>();

    public virtual ICollection<SubmittedSubtask> SubmittedSubtasks { get; set; } = new List<SubmittedSubtask>();

    public virtual ICollection<SubmittedSubtasksByManager> SubmittedSubtasksByManagers { get; set; } = new List<SubmittedSubtasksByManager>();

    public virtual ICollection<SubtaskProgress> SubtaskProgresses { get; set; } = new List<SubtaskProgress>();

    public virtual ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();
}
