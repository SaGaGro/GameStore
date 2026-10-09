using System;
using System.Collections.Generic;

namespace GameStore.Models.DB;

public partial class Game
{
    public int GameId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal BasePrice { get; set; }

    public string? CoverImage { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public int StatusId { get; set; }

    public int? SeriesId { get; set; }

    public string? Genre { get; set; }

    public decimal? DiscountPercent { get; set; }

    public bool? IsFree { get; set; }

    public bool? HasTrial { get; set; }

    public int? TrialHours { get; set; }

    public decimal? Rating { get; set; }

    public int? ReviewCount { get; set; }

    public int? SoldCount { get; set; }

    public bool? IsApproved { get; set; }

    public string? PublisherId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? Publisher { get; set; }

    public virtual Gameseries? Series { get; set; }

    public virtual Gamestatus Status { get; set; } = null!;

    public virtual ICollection<Campaigngame> Campaigngames { get; set; } = new List<Campaigngame>();

    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    public virtual ICollection<Userlibrary> Userlibraries { get; set; } = new List<Userlibrary>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
}
