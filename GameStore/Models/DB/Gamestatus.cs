using System;
using System.Collections.Generic;

namespace GameStore.Models.DB;

public partial class Gamestatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
