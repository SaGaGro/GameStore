using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GameStore.ViewModels
{
    public class PublisherRequestViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อค่ายเกม")]
        [MaxLength(200)]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรายละเอียด")]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "กรุณากรอกช่องทางติดต่อ")]
        [MaxLength(500)]
        public string ContactInfo { get; set; }

        public IFormFile? LogoImage { get; set; }
    }
}
