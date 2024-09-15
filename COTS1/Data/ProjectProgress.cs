using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class ProjectProgress
{
    public int ProjectProgressId { get; set; }

    public int? ProjectId { get; set; }

    public decimal Progress { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public virtual Project? Project { get; set; }
}
