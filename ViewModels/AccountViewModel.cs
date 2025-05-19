using System;
using System.ComponentModel.DataAnnotations;

namespace GiftGenieAPIWebApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = default!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = default!;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = default!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;
    }
}
