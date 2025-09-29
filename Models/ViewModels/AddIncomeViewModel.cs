
// File: Financy/Models/ViewModels/AddIncomeViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace Financy.Models.ViewModels
{
    public class AddIncomeViewModel
    {
        public int Id { get; set; } // Needed for editing

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Account is required")]
        [Display(Name = "Account")]
        public int AccountId { get; set; }

        // Dropdown options
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Account> Accounts { get; set; } = new List<Account>();
    }
}
