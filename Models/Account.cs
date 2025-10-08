
// File: BudgetBuddy/Models/Account.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetBuddy.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Account Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Account Type")]
        public string Type { get; set; } = string.Empty; // Checking, Savings, Credit Card, etc.

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        // Foreign key to User
        public string? UserId { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
