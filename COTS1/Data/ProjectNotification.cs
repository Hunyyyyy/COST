using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class ProjectNotification
{
    public int NotificationId { get; set; }

    public int? ProjectId { get; set; }

    public int? UserId { get; set; }

    public string? NotificationMessage { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Project? Project { get; set; }

    public virtual User? User { get; set; }
}
