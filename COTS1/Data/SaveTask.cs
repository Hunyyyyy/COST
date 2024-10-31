using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class SaveTask
{
    public int TaskId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public string? Priority { get; set; }

    public string? Status { get; set; }

    public int? AssignedTo { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? ProjectId { get; set; }

    public int? Progress { get; set; }

    public string? Note { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ICollection<SubmittedSubtask> SubmittedSubtasks { get; set; } = new List<SubmittedSubtask>();

    public virtual ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();
}
