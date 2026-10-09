using System;
using System.Collections.Generic;

namespace GameStore.Models.DB;

public partial class Promotion
{
    public int PromotionId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Type { get; set; } = null!;

    public decimal DiscountPercent { get; set; }

    public int GameId { get; set; }

    public string? PublisherId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User? Publisher { get; set; }
}
