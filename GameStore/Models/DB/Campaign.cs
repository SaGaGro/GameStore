using System;
using System.Collections.Generic;

namespace GameStore.Models.DB;

public partial class Campaign
{
    public int CampaignId { get; set; }

    public string CampaignName { get; set; } = null!;

    public decimal DiscountPercentage { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Campaigngame> Campaigngames { get; set; } = new List<Campaigngame>();
}
