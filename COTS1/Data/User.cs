﻿using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? PasswordSalt { get; set; }

    public virtual ICollection<AssignedSubtask> AssignedSubtaskAssignedToNavigations { get; set; } = new List<AssignedSubtask>();

    public virtual ICollection<AssignedSubtask> AssignedSubtaskMembers { get; set; } = new List<AssignedSubtask>();

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Project> ProjectCreatedByNavigations { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectManagers { get; set; } = new List<Project>();

    public virtual ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();

    public virtual ICollection<ProjectNotification> ProjectNotifications { get; set; } = new List<ProjectNotification>();

    public virtual ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();

    public virtual ICollection<SaveTask> SaveTaskAssignedToNavigations { get; set; } = new List<SaveTask>();

    public virtual ICollection<SaveTask> SaveTaskCreatedByNavigations { get; set; } = new List<SaveTask>();

    public virtual ICollection<SentTasksList> SentTasksLists { get; set; } = new List<SentTasksList>();

    public virtual ICollection<SubmittedSubtask> SubmittedSubtasks { get; set; } = new List<SubmittedSubtask>();

    public virtual ICollection<SubmittedSubtasksByManager> SubmittedSubtasksByManagers { get; set; } = new List<SubmittedSubtasksByManager>();

    public virtual ICollection<SubtaskProgress> SubtaskProgresses { get; set; } = new List<SubtaskProgress>();

    public virtual ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();

    public virtual ICollection<TaskNotification> TaskNotifications { get; set; } = new List<TaskNotification>();

    public virtual UserLogin? UserLogin { get; set; }
}
