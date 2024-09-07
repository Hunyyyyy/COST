using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class TaskProgress
{
    public int ProgressId { get; set; }

    public int? SubtaskId { get; set; }

    public int? UserId { get; set; }

    public string? Status { get; set; }

    public DateTime? ProgressDate { get; set; }

    public string? Notes { get; set; }

    public virtual Subtask? Subtask { get; set; }

    public virtual User? User { get; set; }
}
