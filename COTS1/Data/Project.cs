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

    public int? CreatedBy { get; set; }

    public int? Progress { get; set; }

    public virtual ICollection<AssignedSubtask> AssignedSubtasks { get; set; } = new List<AssignedSubtask>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? Manager { get; set; }

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();

    public virtual ICollection<ProjectNotification> ProjectNotifications { get; set; } = new List<ProjectNotification>();

    public virtual ICollection<ProjectProgress> ProjectProgresses { get; set; } = new List<ProjectProgress>();

    public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();

    public virtual ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

    public virtual ICollection<SaveTask> SaveTasks { get; set; } = new List<SaveTask>();

    public virtual ICollection<SentTasksList> SentTasksLists { get; set; } = new List<SentTasksList>();

    public virtual ICollection<SubmittedSubtask> SubmittedSubtasks { get; set; } = new List<SubmittedSubtask>();

    public virtual ICollection<SubmittedSubtasksByManager> SubmittedSubtasksByManagers { get; set; } = new List<SubmittedSubtasksByManager>();

    public virtual ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();
}
