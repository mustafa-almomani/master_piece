using System;
using System.Collections.Generic;

namespace startupCompany.Models;

public partial class TaskAssignment
{
    public int TaskId { get; set; }

    public int? ProjectId { get; set; }

    public int? EmployeeId { get; set; }

    public string? TaskDetails { get; set; }

    public string? TaskStatus { get; set; }

    public DateTime? AssignedDate { get; set; }

    public DateTime? DueDate { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ProjectManagement? Project { get; set; }
}
