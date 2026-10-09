using System;
using System.Collections.Generic;

namespace GameStore.Models.DB;

public partial class Publisherrequest
{
    public int RequestId { get; set; }

    public string UserId { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ContactInfo { get; set; } = null!;

    public string? LogoImage { get; set; }

    public string? Status { get; set; }

    public string? RejectReason { get; set; }

    public string? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime? RequestedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual User? Reviewer { get; set; }
}
