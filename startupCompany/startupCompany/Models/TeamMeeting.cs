using System;
using System.Collections.Generic;

namespace startupCompany.Models;

public partial class TeamMeeting
{
    public int MeetingId { get; set; }

    public int? EmployeeId { get; set; }

    public string? MeetingAgenda { get; set; }

    public DateTime? MeetingDateTime { get; set; }

    public string? Linkmeeting { get; set; }

    public virtual Employee? Employee { get; set; }
}
