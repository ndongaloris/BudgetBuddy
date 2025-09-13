// Models/Account.cs

namespace Financy.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // e.g., "Credit Card", "Bank Account"
        
        // Navigation property
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}