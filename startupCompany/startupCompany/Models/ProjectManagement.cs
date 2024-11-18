using System;
using System.Collections.Generic;

namespace startupCompany.Models;

public partial class ProjectManagement
{
    public int ProjectId { get; set; }

    public int? RequestId { get; set; }

    public int? AdminId { get; set; }

    public string? ProjectDetails { get; set; }

    public string? ProjectStatus { get; set; }

    public string? Task { get; set; }

    public string? Progress { get; set; }

    public DateTime? DueDate { get; set; }

    public virtual Employee? Admin { get; set; }

    public virtual User? AdminNavigation { get; set; }

    public virtual CustomerRequest? Request { get; set; }

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();
}
