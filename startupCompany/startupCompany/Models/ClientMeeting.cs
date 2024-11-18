using System;
using System.Collections.Generic;

namespace startupCompany.Models;

public partial class ClientMeeting
{
    public int MeetingId { get; set; }

    public int? AdminId { get; set; }

    public string? ClientName { get; set; }

    public string? ClientEmail { get; set; }

    public string? ClientPhone { get; set; }

    public string? MeetingPlatform { get; set; }

    public DateTime? MeetingDateTime { get; set; }

    public string? MeetingLink { get; set; }

    public string? Subjectmeet { get; set; }

    public virtual Employee? Admin { get; set; }
}
