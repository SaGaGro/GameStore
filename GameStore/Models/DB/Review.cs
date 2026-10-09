using System;
using System.Collections.Generic;

namespace GameStore.Models.DB;

public partial class Review
{
    public int ReviewId { get; set; }

    public string UserId { get; set; } = null!;

    public int GameId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
