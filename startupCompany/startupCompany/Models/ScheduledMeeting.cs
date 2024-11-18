using System;
using System.Collections.Generic;

namespace startupCompany.Models;

public partial class ScheduledMeeting
{
    public int ScheduledMeetingId { get; set; }

    public string? MeetingType { get; set; }

    public DateTime? MeetingDateTime { get; set; }

    public string? Participants { get; set; }

    public string? Platform { get; set; }

    public int? AdminId { get; set; }

    public string? Action { get; set; }

    public virtual Employee? Admin { get; set; }
}
