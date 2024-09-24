using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class Reminder
{
    public int ReminderId { get; set; }

    public int ProjectId { get; set; }

    public int UserId { get; set; }

    public string ReminderContent { get; set; } = null!;

    public DateTime ReminderDate { get; set; }

    public bool? IsAcknowledged { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? DaysRemaining { get; set; }

    public string? ProjectName { get; set; }

    public string? Status { get; set; }

    public string? FullName { get; set; }

    public int? TaskId { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
