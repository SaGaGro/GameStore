using System;
using System.Collections.Generic;

namespace GameStore.Models.DB;

public partial class Gameseries
{
    public int SeriesId { get; set; }

    public string SeriesName { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
