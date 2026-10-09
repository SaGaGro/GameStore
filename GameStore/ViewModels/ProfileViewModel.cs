using System.ComponentModel.DataAnnotations;

namespace GameStore.ViewModels
{
    public class ProfileViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อผู้ใช้")]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "กรุณากรอกอีเมล")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อที่แสดง")]
        [MaxLength(100)]
        public string DisplayName { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(50)]
        public string? Country { get; set; }

        public string? ProfileImage { get; set; }

        public int RoleId { get; set; }

        public DateTime CreatedAt { get; set; }

        // สถิติ
        public int GamesOwned { get; set; }
        public int WishlistCount { get; set; }
        public int ReviewCount { get; set; }
        public int PointsBalance { get; set; }
    }
}
