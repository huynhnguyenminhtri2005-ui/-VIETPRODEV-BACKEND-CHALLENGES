using System;
using System.Collections.Generic;

namespace TestAPIChallenges.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public string? Description { get; set; }

    public int? Credits { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
