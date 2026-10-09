using System;
using System.Collections.Generic;

namespace GameStore.Models.DB;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? Email { get; set; }

    public int RoleId { get; set; }

    public decimal? WalletPoints { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? DisplayName { get; set; }

    public string? ProfileImage { get; set; }

    public string? Phone { get; set; }

    public string? Country { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    public virtual ICollection<Userlibrary> Userlibraries { get; set; } = new List<Userlibrary>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    public virtual ICollection<Publisherrequest> PublisherRequests { get; set; } = new List<Publisherrequest>();

    public virtual ICollection<Publisherrequest> ReviewedRequests { get; set; } = new List<Publisherrequest>();

    public virtual ICollection<Pointhistory> Pointhistories { get; set; } = new List<Pointhistory>();

    public virtual Role Role { get; set; } = null!;
}
