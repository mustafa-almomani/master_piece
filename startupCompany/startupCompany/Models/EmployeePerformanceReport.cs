using System;
using System.Collections.Generic;

namespace startupCompany.Models;

public partial class EmployeePerformanceReport
{
    public int ReportId { get; set; }

    public int? EmployeeId { get; set; }

    public string? ReportDetails { get; set; }

    public DateTime? ReportDate { get; set; }

    public int? Completedtask { get; set; }

    public string? PerformanceRating { get; set; }

    public string? Note { get; set; }

    public virtual Employee? Employee { get; set; }
}
