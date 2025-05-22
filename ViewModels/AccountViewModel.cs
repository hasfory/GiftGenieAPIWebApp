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
        [CustomValidation(typeof(RegisterViewModel), nameof(ValidateBirthdate))]
        public DateTime Birthdate { get; set; }

        public static ValidationResult? ValidateBirthdate(DateTime birthdate, ValidationContext context)
        {
            if (birthdate > DateTime.Today)
                return new ValidationResult("Birthdate cannot be in the future.");
            return ValidationResult.Success;
        }
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