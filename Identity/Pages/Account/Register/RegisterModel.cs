using System.ComponentModel.DataAnnotations;

namespace Identity.Pages.Account.Register
{
    public class RegisterModel
    {
        [Required]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [MinLength(8)]
        public string? Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }

        public string? ReturnUrl { get; set; }
        public string? Button { get; set; }
    }
}
