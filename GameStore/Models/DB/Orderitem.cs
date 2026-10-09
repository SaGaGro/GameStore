using System;
using System.Collections.Generic;

namespace GameStore.Models.DB;

public partial class Orderitem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int GameId { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? DiscountAmount { get; set; }

    public decimal NetPrice { get; set; }

    public string? DiscountNote { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
