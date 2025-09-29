
// File: Financy/Models/ViewModels/AccountViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace Financy.Models.ViewModels
{
    public class AccountViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Account Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Account Type")]
        public string Type { get; set; } = string.Empty;

        [Display(Name = "Initial Balance")]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; } = 0;
    }
}
