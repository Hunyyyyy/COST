﻿using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class TaskProgress
{
    public int TaskProgressId { get; set; }

    public int? TaskId { get; set; }

    public decimal Progress { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public virtual SaveTask? Task { get; set; }
}
