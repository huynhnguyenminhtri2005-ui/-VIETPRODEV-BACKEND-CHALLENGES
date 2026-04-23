using System;
using System.Collections.Generic;

namespace TestAPIChallenges.Models;

public partial class Class
{
    public int ClassId { get; set; }

    public int? CourseId { get; set; }

    public string ClassCode { get; set; } = null!;

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
