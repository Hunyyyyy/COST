using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class SentTasksList
{
    public int TaskId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public string? Priority { get; set; }

    public string? Status { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Note { get; set; }

    public int? ProjectId { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
}
