using System;
using System.Collections.Generic;

namespace startupCompany.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? JobTitle { get; set; }

    public DateTime? JoinDate { get; set; }

    public string? Status { get; set; }

    public string? PerformanceReport { get; set; }

    public string? Role { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<ClientMeeting> ClientMeetings { get; set; } = new List<ClientMeeting>();

    public virtual ICollection<EmployeePerformanceReport> EmployeePerformanceReports { get; set; } = new List<EmployeePerformanceReport>();

    public virtual ICollection<ProjectManagement> ProjectManagements { get; set; } = new List<ProjectManagement>();

    public virtual ICollection<ScheduledMeeting> ScheduledMeetings { get; set; } = new List<ScheduledMeeting>();

    public virtual ICollection<TaskAssignment> TaskAssignments { get; set; } = new List<TaskAssignment>();

    public virtual ICollection<TeamMeeting> TeamMeetings { get; set; } = new List<TeamMeeting>();
}
