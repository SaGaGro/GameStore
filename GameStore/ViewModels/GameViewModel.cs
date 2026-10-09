using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GameStore.ViewModels
{
    public class GameViewModel
    {
        public int GameId { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อเกม")]
        [MaxLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรายละเอียด")]
        [MaxLength(3000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "กรุณากรอกราคา")]
        [Range(0, 99999, ErrorMessage = "ราคาต้องอยู่ระหว่าง 0 - 99,999")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "กรุณาเลือกหมวดหมู่")]
        [MaxLength(100)]
        public string Genre { get; set; }

        public IFormFile? CoverImage { get; set; }

        public string? ExistingCoverImage { get; set; }

        public bool IsFree { get; set; }

        public bool HasTrial { get; set; }

        [Range(1, 100, ErrorMessage = "เวลาทดลองต้องอยู่ระหว่าง 1 - 100 ชั่วโมง")]
        public int? TrialHours { get; set; }

        public decimal? TrialDiscount { get; set; }

        [MaxLength(100)]
        public string? SeriesName { get; set; }

        public int? SeriesOrder { get; set; }

        [Required(ErrorMessage = "กรุณาเลือกวันวางจำหน่าย")]
        [DataType(DataType.Date)]
        public DateTime ReleasedAt { get; set; } = DateTime.Now;
    }
}
