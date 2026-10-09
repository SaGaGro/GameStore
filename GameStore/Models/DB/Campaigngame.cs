using System;
using System.Collections.Generic;

namespace GameStore.Models.DB;

public partial class Campaigngame
{
    public int CampaignGameId { get; set; }

    public int CampaignId { get; set; }

    public int GameId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Campaign Campaign { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;
}
