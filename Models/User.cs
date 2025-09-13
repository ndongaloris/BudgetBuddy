
// File: Models/User.cs

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Financy.Models
{
    public class User : IdentityUser
    {
        [Required]
        [PersonalData]
        public string FullName { get; set; } = string.Empty;

        [PersonalData]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [PersonalData]
        public string? ProfileInitials { get; set; }

        [PersonalData]
        public string? ProfileImageUrl { get; set; }

        // Navigation properties
        public List<Transaction> Transactions { get; set; } = new();
    }
}
