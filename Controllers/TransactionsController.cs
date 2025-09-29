// File: Controllers/TransactionsController.cs

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Financy.Data;
using Financy.Models;
using Microsoft.AspNetCore.Authorization;
using Financy.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Financy.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly FinancyContext _context;
        private readonly UserManager<User> _userManager;

        public TransactionsController(FinancyContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ---------------------------
        // Transactions Index (with filtering, sorting, paging)
        // ---------------------------
        // Example URL:
        // /Transactions?dateRange=month&type=all&categoryId=0&search=&sort=date_desc&page=1&pageSize=10
        public async Task<IActionResult> Index(
            string dateRange = "month",
            string type = "all",
            int? categoryId = null,
            string? search = null,
            string sort = "date_desc",
            int page = 1,
            int pageSize = 10)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            // Base query
            var query = _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.Account)
                .Where(t => t.UserId == user.Id)
                .AsQueryable();

            // Date range filtering
            var now = DateTime.Now;
            DateTime start = DateTime.MinValue, end = DateTime.MaxValue;

            switch ((dateRange ?? "all").ToLower())
            {
                case "today":
                    start = DateTime.Today;
                    end = start.AddDays(1).AddTicks(-1);
                    break;
                case "week":
                    start = DateTime.Today.AddDays(-7);
                    end = DateTime.Today.AddDays(1).AddTicks(-1);
                    break;
                case "month":
                    start = new DateTime(now.Year, now.Month, 1);
                    end = start.AddMonths(1).AddDays(-1);
                    break;
                case "quarter":
                    int currentQuarter = (now.Month - 1) / 3;
                    start = new DateTime(now.Year, currentQuarter * 3 + 1, 1);
                    end = start.AddMonths(3).AddDays(-1);
                    break;
                case "year":
                    start = new DateTime(now.Year, 1, 1);
                    end = start.AddYears(1).AddDays(-1);
                    break;
                default:
                    start = DateTime.MinValue;
                    end = DateTime.MaxValue;
                    break;
            }

            if (dateRange != null && dateRange.ToLower() != "all")
            {
                query = query.Where(t => t.Date >= start && t.Date <= end);
            }

            // Type filter
            if (!string.IsNullOrEmpty(type))
            {
                if (type.ToLower() == "income")
                    query = query.Where(t => t.IsIncome);
                else if (type.ToLower() == "expense")
                    query = query.Where(t => !t.IsIncome);
            }

            // Category filter
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            // Search filter (against Description, Category.Name, Account.Name)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(t =>
                    EF.Functions.Like(t.Description, $"%{s}%") ||
                    (t.Category != null && EF.Functions.Like(t.Category.Name, $"%{s}%")) ||
                    (t.Account != null && EF.Functions.Like(t.Account.Name, $"%{s}%"))
                );
            }

            // Compute totals BEFORE pagination (so metrics reflect active filters)
            var totalIncome = await query.Where(t => t.IsIncome).SumAsync(t => (decimal?)t.Amount) ?? 0M;
            var totalExpenses = await query.Where(t => !t.IsIncome).SumAsync(t => (decimal?)t.Amount) ?? 0M;
            var netBalance = totalIncome - totalExpenses;
            var transactionsCount = await query.CountAsync();

            // Sorting
            query = sort switch
            {
                "date_asc" => query.OrderBy(t => t.Date),
                "amount_asc" => query.OrderBy(t => t.Amount),
                "amount_desc" => query.OrderByDescending(t => t.Amount),
                "category_asc" => query.OrderBy(t => t.Category.Name),
                "category_desc" => query.OrderByDescending(t => t.Category.Name),
                "date_desc" or _ => query.OrderByDescending(t => t.Date),
            };

            // Paging
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);
            var totalPages = (int)Math.Ceiling(transactionsCount / (double)pageSize);

            var pagedTransactions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Provide view state via ViewBag (keeps your template's @model simple)
            ViewBag.TotalIncome = totalIncome;
            ViewBag.TotalExpenses = totalExpenses;
            ViewBag.NetBalance = netBalance;
            ViewBag.TransactionsCount = transactionsCount;

            ViewBag.DateRange = dateRange;
            ViewBag.Type = type;
            ViewBag.CategoryId = categoryId ?? 0;
            ViewBag.Search = search ?? string.Empty;
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;

            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            return View(pagedTransactions);
        }

        // ---------------------------
        // Export filtered transactions to CSV
        // ---------------------------
        [HttpGet]
        public async Task<IActionResult> Export(
            string dateRange = "month",
            string type = "all",
            int? categoryId = null,
            string? search = null,
            string sort = "date_desc")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var query = _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.Account)
                .Where(t => t.UserId == user.Id)
                .AsQueryable();

            // Apply identical filters as Index (dateRange/type/category/search)
            var now = DateTime.Now;
            DateTime start = DateTime.MinValue, end = DateTime.MaxValue;

            switch ((dateRange ?? "all").ToLower())
            {
                case "today":
                    start = DateTime.Today;
                    end = start.AddDays(1).AddTicks(-1);
                    break;
                case "week":
                    start = DateTime.Today.AddDays(-7);
                    end = DateTime.Today.AddDays(1).AddTicks(-1);
                    break;
                case "month":
                    start = new DateTime(now.Year, now.Month, 1);
                    end = start.AddMonths(1).AddDays(-1);
                    break;
                case "quarter":
                    int currentQuarter = (now.Month - 1) / 3;
                    start = new DateTime(now.Year, currentQuarter * 3 + 1, 1);
                    end = start.AddMonths(3).AddDays(-1);
                    break;
                case "year":
                    start = new DateTime(now.Year, 1, 1);
                    end = start.AddYears(1).AddDays(-1);
                    break;
                default:
                    start = DateTime.MinValue;
                    end = DateTime.MaxValue;
                    break;
            }

            if (dateRange != null && dateRange.ToLower() != "all")
            {
                query = query.Where(t => t.Date >= start && t.Date <= end);
            }

            if (!string.IsNullOrEmpty(type))
            {
                if (type.ToLower() == "income")
                    query = query.Where(t => t.IsIncome);
                else if (type.ToLower() == "expense")
                    query = query.Where(t => !t.IsIncome);
            }
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(t =>
                    EF.Functions.Like(t.Description, $"%{s}%") ||
                    (t.Category != null && EF.Functions.Like(t.Category.Name, $"%{s}%")) ||
                    (t.Account != null && EF.Functions.Like(t.Account.Name, $"%{s}%"))
                );
            }

            // Sorting
            query = sort switch
            {
                "date_asc" => query.OrderBy(t => t.Date),
                "amount_asc" => query.OrderBy(t => t.Amount),
                "amount_desc" => query.OrderByDescending(t => t.Amount),
                "category_asc" => query.OrderBy(t => t.Category.Name),
                "category_desc" => query.OrderByDescending(t => t.Category.Name),
                "date_desc" or _ => query.OrderByDescending(t => t.Date),
            };

            var txs = await query.ToListAsync();

            // Build CSV
            var sb = new StringBuilder();
            sb.AppendLine("Id,Date,Type,Category,Account,Description,Amount");

            foreach (var tx in txs)
            {
                var typeLabel = tx.IsIncome ? "Income" : "Expense";
                var dateStr = tx.Date.ToString("yyyy-MM-dd HH:mm");
                var categoryName = tx.Category?.Name?.Replace("\"", "\"\"") ?? "";
                var accountName = tx.Account?.Name?.Replace("\"", "\"\"") ?? "";
                var description = tx.Description?.Replace("\"", "\"\"") ?? "";
                var amount = tx.IsIncome ? tx.Amount : -tx.Amount; // expense negative
                sb.AppendLine($"{tx.Id},\"{dateStr}\",\"{typeLabel}\",\"{categoryName}\",\"{accountName}\",\"{description}\",{amount}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"transactions_{DateTime.Now:yyyyMMddHHmmss}.csv");
        }

        // ---------------------------
        // Add Income (GET)
        // ---------------------------
        [HttpGet]
        public async Task<IActionResult> AddIncome()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var viewModel = new AddIncomeViewModel
            {
                Date = DateTime.Now,
                Categories = await _context.Categories.ToListAsync(),
                Accounts = await _context.Accounts.Where(a => a.UserId == user.Id).ToListAsync()
            };

            ViewBag.IsEdit = false;
            return View(viewModel);
        }

        // ---------------------------
        // Add Income (POST)
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIncome(AddIncomeViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var transaction = new Transaction
                {
                    Description = model.Description,
                    Amount = model.Amount,
                    Date = model.Date,
                    IsIncome = true,
                    UserId = user.Id,
                    CategoryId = model.CategoryId,
                    AccountId = model.AccountId
                };

                // Update the selected account (increase balance)
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Id == model.AccountId && a.UserId == user.Id);
                if (account != null)
                {
                    account.Balance += model.Amount;
                    _context.Update(account);
                }

                _context.Add(transaction);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Income added successfully!";
                return RedirectToAction("Dashboard", "Home");
            }

            model.Categories = await _context.Categories.ToListAsync();
            model.Accounts = await _context.Accounts.Where(a => a.UserId == user.Id).ToListAsync();
            ViewBag.IsEdit = false;
            return View(model);
        }

        // ---------------------------
        // Add Expense (GET)
        // ---------------------------
        [HttpGet]
        public async Task<IActionResult> AddExpense()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var viewModel = new AddExpenseViewModel
            {
                Date = DateTime.Now,
                // exclude Income category so users don't accidentally categorize expenses as Income
                Categories = await _context.Categories
                    .Where(c => !c.Name.ToLower().Equals("income"))
                    .ToListAsync(),
                Accounts = await _context.Accounts.Where(a => a.UserId == user.Id).ToListAsync()
            };

            ViewBag.IsEdit = false;
            return View(viewModel);
        }

        // ---------------------------
        // Add Expense (POST)
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExpense(AddExpenseViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var transaction = new Transaction
                {
                    Description = model.Description,
                    Amount = model.Amount,
                    Date = model.Date,
                    IsIncome = false, // expense
                    UserId = user.Id,
                    CategoryId = model.CategoryId,
                    AccountId = model.AccountId
                };

                // Update the selected account (decrease balance for expense)
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.Id == model.AccountId && a.UserId == user.Id);
                if (account != null)
                {
                    account.Balance -= model.Amount;
                    _context.Update(account);
                }

                _context.Add(transaction);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Expense added successfully!";
                return RedirectToAction("Dashboard", "Home");
            }

            // reload dropdowns and return
            model.Categories = await _context.Categories
                .Where(c => !c.Name.ToLower().Equals("income"))
                .ToListAsync();
            model.Accounts = await _context.Accounts.Where(a => a.UserId == user.Id).ToListAsync();
            ViewBag.IsEdit = false;
            return View(model);
        }

        // ---------------------------
        // Edit Income (GET)
        // ---------------------------
        [HttpGet]
        public async Task<IActionResult> EditIncome(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var income = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id && t.IsIncome);
            if (income == null) return NotFound();

            var viewModel = new AddIncomeViewModel
            {
                Id = income.Id,
                Description = income.Description,
                Amount = income.Amount,
                Date = income.Date,
                CategoryId = income.CategoryId,
                AccountId = income.AccountId,
                Categories = await _context.Categories.ToListAsync(),
                Accounts = await _context.Accounts.Where(a => a.UserId == user.Id).ToListAsync()
            };

            ViewBag.IsEdit = true;
            return View("addIncome", viewModel); // reuse AddIncome view
        }

        // ---------------------------
        // Edit Income (POST)
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditIncome(int id, AddIncomeViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var income = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id && t.IsIncome);
            if (income == null) return NotFound();

            if (ModelState.IsValid)
            {
                // record old values
                var oldAmount = income.Amount;
                var oldAccountId = income.AccountId;

                // update transaction fields
                income.Description = model.Description;
                income.Amount = model.Amount;
                income.Date = model.Date;
                income.CategoryId = model.CategoryId;
                income.AccountId = model.AccountId;

                // adjust accounts depending on change
                if (oldAccountId == model.AccountId)
                {
                    // same account -> apply difference
                    if (model.AccountId != 0)
                    {
                        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == model.AccountId && a.UserId == user.Id);
                        if (account != null)
                        {
                            account.Balance += (model.Amount - oldAmount);
                            _context.Update(account);
                        }
                    }
                }
                else
                {
                    // moved to different account
                    if (oldAccountId != 0)
                    {
                        var oldAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == oldAccountId && a.UserId == user.Id);
                        if (oldAccount != null)
                        {
                            // remove previous income from old account
                            oldAccount.Balance -= oldAmount;
                            _context.Update(oldAccount);
                        }
                    }

                    if (model.AccountId != 0)
                    {
                        var newAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == model.AccountId && a.UserId == user.Id);
                        if (newAccount != null)
                        {
                            newAccount.Balance += model.Amount;
                            _context.Update(newAccount);
                        }
                    }
                }

                _context.Update(income);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Income updated successfully!";
                return RedirectToAction("Dashboard", "Home");
            }

            model.Categories = await _context.Categories.ToListAsync();
            model.Accounts = await _context.Accounts.Where(a => a.UserId == user.Id).ToListAsync();
            ViewBag.IsEdit = true;
            return View("addIncome", model);
        }

        // ---------------------------
        // Edit Expense (GET)
        // ---------------------------
        [HttpGet]
        public async Task<IActionResult> EditExpense(int? id)
        {
            if (id == null) return NotFound();
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var expense = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id && !t.IsIncome);
            if (expense == null) return NotFound();

            var viewModel = new AddExpenseViewModel
            {
                Id = expense.Id,
                Description = expense.Description,
                Amount = expense.Amount,
                Date = expense.Date,
                CategoryId = expense.CategoryId,
                AccountId = expense.AccountId,
                Categories = await _context.Categories
                    .Where(c => !c.Name.ToLower().Equals("income"))
                    .ToListAsync(),
                Accounts = await _context.Accounts.Where(a => a.UserId == user.Id).ToListAsync()
            };

            ViewBag.IsEdit = true;
            return View("addExpense", viewModel); // reuse AddExpense view
        }

        // ---------------------------
        // Edit Expense (POST)
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExpense(int id, AddExpenseViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var expense = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id && !t.IsIncome);
            if (expense == null) return NotFound();

            if (ModelState.IsValid)
            {
                var oldAmount = expense.Amount;
                var oldAccountId = expense.AccountId;

                // update fields
                expense.Description = model.Description;
                expense.Amount = model.Amount;
                expense.Date = model.Date;
                expense.CategoryId = model.CategoryId;
                expense.AccountId = model.AccountId;

                // adjust accounts
                if (oldAccountId == model.AccountId)
                {
                    // same account -> apply difference (expense reduces balance)
                    if (model.AccountId != 0)
                    {
                        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == model.AccountId && a.UserId == user.Id);
                        if (account != null)
                        {
                            // subtract the change (if new amount larger -> subtract more; if smaller -> add back)
                            account.Balance -= (model.Amount - oldAmount);
                            _context.Update(account);
                        }
                    }
                }
                else
                {
                    // revert old expense from previous account
                    if (oldAccountId != 0)
                    {
                        var oldAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == oldAccountId && a.UserId == user.Id);
                        if (oldAccount != null)
                        {
                            oldAccount.Balance += oldAmount; // refund old expense
                            _context.Update(oldAccount);
                        }
                    }

                    // apply new expense to new account
                    if (model.AccountId != 0)
                    {
                        var newAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == model.AccountId && a.UserId == user.Id);
                        if (newAccount != null)
                        {
                            newAccount.Balance -= model.Amount;
                            _context.Update(newAccount);
                        }
                    }
                }

                _context.Update(expense);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Expense updated successfully!";
                return RedirectToAction("Dashboard", "Home");
            }

            model.Categories = await _context.Categories
                .Where(c => !c.Name.ToLower().Equals("income"))
                .ToListAsync();
            model.Accounts = await _context.Accounts.Where(a => a.UserId == user.Id).ToListAsync();
            ViewBag.IsEdit = true;
            return View("addExpense", model);
        }

        // ---------------------------
        // Delete Income (POST only, modal confirmation)
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteIncomeConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var income = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id && t.IsIncome);
            if (income == null) return NotFound();

            // revert income on account
            if (income.AccountId != 0)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == income.AccountId && a.UserId == user.Id);
                if (account != null)
                {
                    account.Balance -= income.Amount;
                    _context.Update(account);
                }
            }

            _context.Transactions.Remove(income);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Income deleted successfully!";
            return RedirectToAction("Dashboard", "Home");
        }

        // ---------------------------
        // Delete Expense (POST only, modal confirmation)
        // ---------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExpenseConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var expense = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id && !t.IsIncome);
            if (expense == null) return NotFound();

            // refund expense back to account
            if (expense.AccountId != 0)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == expense.AccountId && a.UserId == user.Id);
                if (account != null)
                {
                    account.Balance += expense.Amount;
                    _context.Update(account);
                }
            }

            _context.Transactions.Remove(expense);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Expense deleted successfully!";
            return RedirectToAction("Dashboard", "Home");
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
