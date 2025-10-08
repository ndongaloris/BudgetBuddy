// BudgetBuddy/Models/SeedData.cs

using BudgetBuddy.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BudgetBuddy.Models
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new FinancyContext(
                serviceProvider.GetRequiredService<DbContextOptions<FinancyContext>>()))
            {
                // Look for any categories
                if (context.Categories.Any())
                {
                    return;   // DB has been seeded
                }

                // Add default categories
                context.Categories.AddRange(
                    new Category { Name = "House", Color = "#FF6384" },
                    new Category { Name = "Savings", Color = "#36A2EB" },
                    new Category { Name = "Transportation", Color = "#FFCE56" },
                    new Category { Name = "Groceries", Color = "#4BC0C0" },
                    new Category { Name = "Shopping", Color = "#9966FF" },
                    new Category { Name = "Entertainment", Color = "#FF9F40" },
                    new Category { Name = "Income", Color = "#2ecc71" }
                );

                await context.SaveChangesAsync();
            }
        }
    }
}