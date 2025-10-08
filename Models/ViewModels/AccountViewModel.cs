// File: BudgetBuddy/Models/ViewModels/AccountViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace BudgetBuddy.Models.ViewModels
{
    public class AccountViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Account name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account type is required")]
        public string Type { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative")]
        public decimal Balance { get; set; } = 0;
    }
}