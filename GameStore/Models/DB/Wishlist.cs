using System;
using System.Collections.Generic;

namespace GameStore.Models.DB;

public partial class Wishlist
{
    public int WishlistId { get; set; }

    public string UserId { get; set; } = null!;

    public int GameId { get; set; }

    public bool? IsPreRegistered { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
