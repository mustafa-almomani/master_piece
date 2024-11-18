using System;
using System.Collections.Generic;

namespace startupCompany.Models;

public partial class CustomerRequest
{
    public int RequestId { get; set; }

    public int? UserId { get; set; }

    public int? ServiceId { get; set; }

    public string? ProjectDetails { get; set; }

    public DateTime? RequestDate { get; set; }

    public string? Status { get; set; }

    public string? Img { get; set; }

    public string? CompanyName { get; set; }

    public string? Address { get; set; }

    public DateTime? MeetingDate { get; set; }

    public virtual ICollection<ProjectManagement> ProjectManagements { get; set; } = new List<ProjectManagement>();

    public virtual Service? Service { get; set; }

    public virtual User? User { get; set; }
}
