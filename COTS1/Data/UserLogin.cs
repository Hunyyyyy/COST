using System;
using System.Collections.Generic;

namespace COTS1.Data;

public partial class UserLogin
{
    public int UserId { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? PasswordSalt { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public int? FailedLoginAttempts { get; set; }

    public virtual User User { get; set; } = null!;
}
