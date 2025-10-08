// File: BudgetBuddy/Models/ViewModels/AddExpenseViewModel.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BudgetBuddy.Models.ViewModels
{
    public class AddExpenseViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Account is required")]
        public int AccountId { get; set; }

        // Dropdown options
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Account> Accounts { get; set; } = new List<Account>();
    }
}