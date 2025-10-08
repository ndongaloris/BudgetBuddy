// File: Pages/Account/Register.cshtml.cs

using BudgetBuddy.Data;
using BudgetBuddy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BudgetBuddy.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly FinancyContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            FinancyContext context,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string ReturnUrl { get; set; } = string.Empty;

        [TempData]
        public string SuccessMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required(ErrorMessage = "Full name is required")]
            [Display(Name = "Full Name")]
            [StringLength(100, ErrorMessage = "Full name must be between {2} and {1} characters long", MinimumLength = 2)]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, ErrorMessage = "Password must be at least {2} and max {1} characters long", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please confirm your password")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "Passwords do not match")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            _logger.LogInformation("Registration page loaded");
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            _logger.LogInformation("Registration POST received");
            
            // Check if Input is null (model binding failed)
            if (Input == null)
            {
                _logger.LogError("Input model is NULL - model binding failed completely");
                ModelState.AddModelError(string.Empty, "Form data is invalid. Please try again.");
                Input = new InputModel(); // Initialize to avoid null reference
                return Page();
            }

            _logger.LogInformation("Input values - FullName: {FullName}, Email: {Email}, Password: [HIDDEN], ConfirmPassword: [HIDDEN]", 
                Input.FullName, Input.Email);

            // Log model state before validation
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid on POST");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state?.Errors?.Count > 0)
                    {
                        _logger.LogWarning("ModelState error for {Key}: {Errors}", 
                            key, string.Join(", ", state.Errors.Select(e => e.ErrorMessage)));
                    }
                }
            }

            if (ModelState.IsValid)
            {
                _logger.LogInformation("Starting registration process for {Email}", Input.Email);

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(Input.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "A user with this email already exists.");
                    _logger.LogWarning("Registration failed: User with email {Email} already exists", Input.Email);
                    return Page();
                }

                _logger.LogInformation("Creating new user object");
                var user = new User
                {
                    FullName = Input.FullName.Trim(),
                    ProfileInitials = GetInitials(Input.FullName),
                    CreatedAt = DateTime.UtcNow
                };

                try
                {
                    _logger.LogInformation("Setting username and email");
                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    user.Email = Input.Email;
                    user.EmailConfirmed = true; // For demo purposes - skip email confirmation

                    _logger.LogInformation("Creating user in database");
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User {Email} created successfully", Input.Email);

                        // Create default account for the user
                        try
                        {
                            _logger.LogInformation("Creating default account for user");
                            var userAccount = new BudgetBuddy.Models.Account
                            {
                                Name = "Main Account",
                                Type = "Checking",
                                Balance = 0,
                                UserId = user.Id
                            };
                            
                            _context.Accounts.Add(userAccount);
                            await _context.SaveChangesAsync();
                            _logger.LogInformation("Default account created for user {Email}", Input.Email);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to create default account for user {Email}", Input.Email);
                            // Continue even if account creation fails
                        }

                        // Sign in the user
                        _logger.LogInformation("Signing in user after registration");
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User {Email} signed in after registration", Input.Email);

                        SuccessMessage = "Account created successfully! Redirecting to dashboard...";
                        
                        // Small delay to show success message, then redirect
                        await Task.Delay(2000);
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        _logger.LogWarning("User creation failed with {ErrorCount} errors", result.Errors.Count());
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                            _logger.LogWarning("Registration error for {Email}: {Code} - {Description}", 
                                Input.Email, error.Code, error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception during user registration for {Email}", Input.Email);
                    ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
                }
            }
            else
            {
                _logger.LogWarning("Registration model state invalid for {Email}. Error count: {ErrorCount}", 
                    Input.Email, ModelState.ErrorCount);
                
                // Log all validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Validation errors: {Errors}", string.Join("; ", errors));
            }

            // If we got this far, something failed, redisplay form
            _logger.LogInformation("Registration failed - returning to registration page");
            return Page();
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "U";
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0][0].ToString().ToUpper();
            return (parts[0][0].ToString() + parts[^1][0].ToString()).ToUpper();
        }
    }
}