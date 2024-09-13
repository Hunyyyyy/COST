using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class ProjectTask
{
    public int ProjectTaskId { get; set; }

    public int? ProjectId { get; set; }

    public int? TaskId { get; set; }

    public virtual Project? Project { get; set; }

    public virtual SaveTask? Task { get; set; }
}
