using System;
using System.Collections.Generic;

namespace GameStore.Models.DB;

public partial class Pointhistory
{
    public int PointHistoryId { get; set; }

    public string UserId { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int Points { get; set; }

    public string? Description { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
