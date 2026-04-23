using System;
using System.Collections.Generic;

namespace TestAPIChallenges.Models;

public partial class UserAuth
{
    public int AuthId { get; set; }

    public int? UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? LastLogin { get; set; }

    public virtual User? User { get; set; }
}
