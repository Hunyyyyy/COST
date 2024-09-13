using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class Subtask
{
    public int SubtaskId { get; set; }

    public int? TaskId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public int? AssignedTo { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ProjectId { get; set; }

    public virtual ICollection<AssignedSubtask> AssignedSubtasks { get; set; } = new List<AssignedSubtask>();

    public virtual User? AssignedToNavigation { get; set; }

    public virtual Project? Project { get; set; }

    public virtual SaveTask? Task { get; set; }

    public virtual ICollection<TaskProgress> TaskProgresses { get; set; } = new List<TaskProgress>();
}
