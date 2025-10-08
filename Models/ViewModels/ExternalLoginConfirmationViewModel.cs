// File: BudgetBuddy/Models/ViewModels/ExternalLoginConfirmationViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace BudgetBuddy.Models.ViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }
    }
}