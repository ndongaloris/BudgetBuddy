
// Models/Category.cs

namespace BudgetBuddy.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty; // For UI display

        // Navigation property
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}