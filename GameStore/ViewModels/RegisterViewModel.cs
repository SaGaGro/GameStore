using System.ComponentModel.DataAnnotations;

namespace GameStore.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อผู้ใช้")]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "กรุณากรอกอีเมล")]
        [EmailAddress(ErrorMessage = "รูปแบบอีเมลไม่ถูกต้อง")]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "รหัสผ่านต้องมีอย่างน้อย 6 ตัวอักษร")]
        public string Password { get; set; }

        [Required(ErrorMessage = "กรุณายืนยันรหัสผ่าน")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อที่แสดง")]
        [MaxLength(100)]
        public string DisplayName { get; set; }

        [MaxLength(50)]
        public string? Country { get; set; }
    }
}
