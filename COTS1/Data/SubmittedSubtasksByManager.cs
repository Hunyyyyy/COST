using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class SubmittedSubtasksByManager
{
    public int SubmissionId { get; set; }

    public int SubtaskId { get; set; }

    public int TaskId { get; set; }

    public int ProjectId { get; set; }

    public int UserId { get; set; }

    public DateTime SubmittedAt { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public string? FilePath { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual Subtask Subtask { get; set; } = null!;

    public virtual SaveTask Task { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
