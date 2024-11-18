using System;
using System.Collections.Generic;

namespace startupCompany.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? TotalAmount { get; set; }

    public string? AmountPaid { get; set; }

    public int? RequestId { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual User? User { get; set; }
}
