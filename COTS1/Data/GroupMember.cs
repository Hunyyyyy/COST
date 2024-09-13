using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class GroupMember
{
    public int GroupMemberId { get; set; }

    public int? GroupId { get; set; }

    public int? UserId { get; set; }

    public virtual Group? Group { get; set; }

    public virtual User? User { get; set; }
}
