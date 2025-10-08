// BudgetBuddy/Models/User.cs

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BudgetBuddy.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        
        public string ProfileInitials { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public List<Account> Accounts { get; set; } = new List<Account>();
    }
}