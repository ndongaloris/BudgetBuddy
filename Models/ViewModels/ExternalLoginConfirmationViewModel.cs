
// Models/ViewModels/ExternalLoginConfirmationViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace Financy.Models.ViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Profile Image URL")]
        public string? ProfileImageUrl { get; set; }
    }
}
