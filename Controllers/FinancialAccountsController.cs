
// File: Financy/Controllers/FinancialAccountsController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Financy.Data;
using Financy.Models;
using Financy.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Financy.Controllers
{
    [Authorize]
    public class FinancialAccountsController : Controller
    {
        private readonly FinancyContext _context;
        private readonly UserManager<User> _userManager;

        public FinancialAccountsController(FinancyContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: FinancialAccounts
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var accounts = await _context.Accounts
                .Where(a => a.UserId == user.Id)
                .OrderBy(a => a.Name)
                .ToListAsync();

            ViewBag.TotalBalance = accounts.Sum(a => a.Balance);
            ViewBag.TotalAccounts = accounts.Count;
            ViewBag.CheckingAccounts = accounts.Count(a => a.Type.ToLower().Contains("checking"));
            ViewBag.CreditCards = accounts.Count(a => a.Type.ToLower().Contains("credit"));

            return View(accounts);
        }

        // GET: FinancialAccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FinancialAccounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var account = new Account
                {
                    Name = model.Name,
                    Type = model.Type,
                    Balance = model.Balance,
                    UserId = user.Id
                };

                _context.Add(account);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Account created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: FinancialAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == user.Id);

            if (account == null) return NotFound();

            return View(account);
        }

        // ===========================
        // EDIT ACCOUNT
        // ===========================
        // GET: FinancialAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);
            if (account == null) return NotFound();

            var model = new AccountViewModel
            {
                Id = account.Id,
                Name = account.Name,
                Type = account.Type,
                Balance = account.Balance
            };

            return View(model);
        }

        // POST: FinancialAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AccountViewModel model)
        {
            if (id != model.Id) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);
                if (account == null) return NotFound();

                account.Name = model.Name;
                account.Type = model.Type;
                account.Balance = model.Balance;

                _context.Update(account);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Account updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // ===========================
        // DELETE ACCOUNT
        // ===========================
        // GET: FinancialAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var account = await _context.Accounts
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == user.Id);

            if (account == null) return NotFound();

            return View(account);
        }

        // POST: FinancialAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);
            if (account == null) return NotFound();

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Account deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
