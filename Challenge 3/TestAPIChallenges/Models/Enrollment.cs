using System;
using System.Collections.Generic;

namespace TestAPIChallenges.Models;

public partial class Enrollment
{
    public int EnrollmentId { get; set; }

    public int? UserId { get; set; }

    public int? ClassId { get; set; }

    public DateTime? EnrollDate { get; set; }

    public string? Status { get; set; }

    public virtual Class? Class { get; set; }

    public virtual User? User { get; set; }
}
