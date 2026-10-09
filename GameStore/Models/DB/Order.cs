using System;
using System.Collections.Generic;

namespace GameStore.Models.DB;

public partial class Order
{
    public int OrderId { get; set; }

    public string CustomerId { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public decimal? PointsUsed { get; set; }

    public decimal FinalAmount { get; set; }

    public decimal? PointsEarned { get; set; }

    public string? OrderStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User Customer { get; set; } = null!;

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();
}
