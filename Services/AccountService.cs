
// File: Services/AccountService.cs

using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using BudgetBuddy.Data;
using BudgetBuddy.Models;
using BudgetBuddy.Models.ViewModels;

namespace BudgetBuddy.Services
{
    public class AccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly FinancyContext _context;
        private readonly ILogger<AccountService> _logger;

        public AccountService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            FinancyContext context,
            ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        // ==============================================================
        // REGISTER LOCAL USER
        // ==============================================================
        public async Task<IdentityResult> RegisterLocalAsync(RegisterViewModel model)
        {
            _logger.LogInformation("Registering new user: {Email}", model.Email);

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                ProfileInitials = GetInitials(model.FullName)
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Create default Main Account
                _context.Accounts.Add(new Account
                {
                    Name = "Main Account",
                    Type = "Checking",
                    Balance = 0,
                    UserId = user.Id
                });

                await _context.SaveChangesAsync();

                // Sign in immediately after register
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User {Email} registered and signed in successfully.", model.Email);
            }
            else
            {
                foreach (var error in result.Errors)
                    _logger.LogWarning("Registration error for {Email}: {Error}", model.Email, error.Description);
            }

            return result;
        }

        // ==============================================================
        // LOGIN USER (LOCAL)
        // ==============================================================
        public async Task<SignInResult> PasswordSignInAsync(LoginViewModel model)
        {
            _logger.LogInformation("Attempting login for {Email}", model.Email);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
                _logger.LogInformation("Login successful for {Email}", model.Email);
            else if (result.IsLockedOut)
                _logger.LogWarning("User {Email} account is locked out.", model.Email);
            else
                _logger.LogWarning("Login failed for {Email}", model.Email);

            return result;
        }

        // ==============================================================
        // LOGOUT USER
        // ==============================================================
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
        }

        // ==============================================================
        // EXTERNAL LOGIN SUPPORT
        // ==============================================================
        public async Task<ExternalLoginInfo?> GetExternalLoginInfoAsync()
        {
            return await _signInManager.GetExternalLoginInfoAsync();
        }

        public async Task<IdentityResult> CreateExternalUserAsync(ExternalLoginConfirmationViewModel model)
        {
            var info = await GetExternalLoginInfoAsync();
            if (info == null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "External login information not available."
                });
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                ProfileInitials = GetInitials(model.FullName),
                ProfileImageUrl = model.ProfileImageUrl
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded) return result;

            result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded) return result;

            _context.Accounts.Add(new Account
            {
                Name = "Main Account",
                Type = "Checking",
                Balance = 0,
                UserId = user.Id
            });
            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User {Email} created an account using {Provider}.", user.Email, info.LoginProvider);

            return result;
        }

        // ==============================================================
        // HELPER: Generate Initials
        // ==============================================================
        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "U";
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0][0].ToString().ToUpper();
            return (parts[0][0].ToString() + parts[^1][0].ToString()).ToUpper();
        }
    }
}
