﻿using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class ProjectUser
{
    public int ProjectUserId { get; set; }

    public int ProjectId { get; set; }

    public int UserId { get; set; }

    public string Role { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
