using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class SubtaskProgress
{
    public int SubtaskProgressId { get; set; }

    public int? SubtaskId { get; set; }

    public int? AssignedTo { get; set; }

    public decimal Progress { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual Subtask? Subtask { get; set; }
}
