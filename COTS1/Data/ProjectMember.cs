using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class ProjectMember
{
    public int ProjectMemberId { get; set; }

    public int? ProjectId { get; set; }

    public int? UserId { get; set; }

    public virtual Project? Project { get; set; }

    public virtual User? User { get; set; }
}
