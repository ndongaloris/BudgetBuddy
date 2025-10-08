// BudgetBuddy/Models/Transaction.cs

using System;
using System.ComponentModel.DataAnnotations;

namespace BudgetBuddy.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public bool IsIncome { get; set; } // True for income, False for expense

        // Foreign keys
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int AccountId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public Account Account { get; set; } = null!;
    }
}
