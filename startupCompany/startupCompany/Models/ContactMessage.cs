using System;
using System.Collections.Generic;

namespace startupCompany.Models;

public partial class ContactMessage
{
    public int MessageId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? MessageText { get; set; }

    public DateTime? MessageDate { get; set; }
}
