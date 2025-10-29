using Flow_Api.Constants;
using System.ComponentModel.DataAnnotations;

namespace Flow_Api.Dtos.Auth.Request
{
    public class RegisterRequestDto
    {
        // Business Information
        [Required(ErrorMessage = "Shop name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Shop name must be between 3 and 100 characters")]
        public string ShopName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Business type is required")]
        public string BusinessType { get; set; } = string.Empty;

        public string? BusinessRegistrationNumber { get; set; }
        public string? TaxNumber { get; set; }

        // Owner Information
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number and one special character")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Address
        [Required(ErrorMessage = "Address is required")]
        public string AddressLine1 { get; set; } = string.Empty;

        public string? AddressLine2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        public string? PostalCode { get; set; }

        // Terms
        [Required(ErrorMessage = "You must agree to terms and conditions")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to terms and conditions")]
        public bool AgreeToTerms { get; set; }
    }
}
