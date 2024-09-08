using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();

    public virtual ICollection<SaveTasks> TaskAssignedToNavigations { get; set; } = new List<SaveTasks>();

    public virtual ICollection<SaveTasks> TaskCreatedByNavigations { get; set; } = new List<SaveTasks>();

    public virtual ICollection<TaskNotification> TaskNotifications { get; set; } = new List<TaskNotification>();

    public virtual ICollection<TaskProgress> TaskProgresses { get; set; } = new List<TaskProgress>();

    public virtual UserLogin? UserLogin { get; set; }
}
