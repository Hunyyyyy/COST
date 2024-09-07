using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class TaskNotification
{
    public int NotificationId { get; set; }

    public int? UserId { get; set; }

    public int? TaskId { get; set; }

    public string? NotificationMessage { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Task? Task { get; set; }

    public virtual User? User { get; set; }
}
