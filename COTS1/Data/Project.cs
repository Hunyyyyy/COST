using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class Project
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    public int? ManagerId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Progress { get; set; }

    public virtual User? Manager { get; set; }

    public virtual ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

    public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();

    public virtual ICollection<SaveTask> SaveTasks { get; set; } = new List<SaveTask>();

    public virtual ICollection<SaveTasksReminder> SaveTasksReminders { get; set; } = new List<SaveTasksReminder>();

    public virtual ICollection<SubmittedSubtask> SubmittedSubtasks { get; set; } = new List<SubmittedSubtask>();

    public virtual ICollection<SubmittedSubtasksByManager> SubmittedSubtasksByManagers { get; set; } = new List<SubmittedSubtasksByManager>();

    public virtual ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();
}
