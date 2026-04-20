using System.ComponentModel.DataAnnotations;

namespace ProjectApprovalSystem.Models.ViewModels
{
    public class ManageUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "User Role")]
        public string Role { get; set; } = string.Empty;

        [Display(Name = "Set Password (optional)")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}