// File: Pages/Account/Logout.cshtml.cs

using BudgetBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BudgetBuddy.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<User> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public void OnGet()
        {
            // This will display the logout page with the spinner
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            if (_signInManager.IsSignedIn(User))
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");
            }

            return LocalRedirect("~/");
        }

        public IActionResult OnPostCancel()
        {
            return LocalRedirect("~/");
        }
    }
}