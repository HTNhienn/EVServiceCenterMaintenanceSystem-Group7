using System.ComponentModel.DataAnnotations;

namespace EVServiceCenterMaintenanceSystem.Models
{
    public class RegisterViewModel
    {
        [Required]
        [MinLength(3)]
        public string Username { get; set; } = default!;

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = default!;
    }
}
