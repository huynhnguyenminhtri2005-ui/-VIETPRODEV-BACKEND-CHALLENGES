using System;
using System.Collections.Generic;

namespace TestAPIChallenges.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public int? RoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual Role? Role { get; set; }

    public virtual UserAuth? UserAuth { get; set; }

    public bool? IsActive { get; set; }
}
