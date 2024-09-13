using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class AssignedSubtask
{
    public int AssignedSubtaskId { get; set; }

    public int? SubtaskId { get; set; }

    public int? AssignedTo { get; set; }

    public string? Status { get; set; }

    public DateTime? AssignedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? ProjectId { get; set; }

    public int? MemberId { get; set; }

    public int? TaskId { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual User? Member { get; set; }

    public virtual Project? Project { get; set; }

    public virtual Subtask? Subtask { get; set; }

    public virtual SaveTask? Task { get; set; }
}
