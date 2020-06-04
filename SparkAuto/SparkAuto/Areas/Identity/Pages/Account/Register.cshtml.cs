using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SparkAuto.Data;
using SparkAuto.Models;
using SparkAuto.Utility;

namespace SparkAuto.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        // We add these fields - Get the role used and access to db
        private readonly RoleManager<IdentityRole> _roleManager;    // Needs to be added to the pipeline like EF.
        private readonly ApplicationDbContext _db;
        

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            
            // We had to add our new fields above to the constructor
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;

            // New passed in values need to be assigned to our fields we created above.
            _db = db;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        // Leave this in - new for core 3.1
        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        // object (model) to hold the input data from the user registration 
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public String Name { get; set; }

            public String Address { get; set; }

            public String City { get; set; }

            public String PostalCode { get; set; }

            [Required]
            public String PhoneNumber { get; set; }

            // Check box to set as admin - only shows when user is logged in as an Admin user.
            public bool IsAdmin { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            // New for core 3.1 - please leave this line in.
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }


        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            // New for core 3.1 - Leave external logins in place
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // We created our own class with constants to use.  Change IdentityUser to 
                // ApplicationUser.  We added more fields to the model and db so we need
                // to pass them in.
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Name = Input.Name,
                    Address = Input.Address,
                    City = Input.City,
                    PostalCode = Input.PostalCode,
                    PhoneNumber = Input.PhoneNumber
                };

                // Create roles if they exist.
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    // We added this section - if admin user does not exist we want to create it.
                    if (!await _roleManager.RoleExistsAsync(SD.AdminEndUser) )
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));
                    }
                    // We added this section - if customer user does not exist we want to create it.
                    if (!await _roleManager.RoleExistsAsync(SD.CustomerEndUser))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser));
                    }

                    // Asign the new role to the user - always admin user right now
                    //await _userManager.AddToRoleAsync(user, SD.AdminEndUser);
                    // Asign the new role to the user - always type user right now
                    //await _userManager.AddToRoleAsync(user, SD.CustomerEndUser);
                    //_logger.LogInformation("User created a new account with password.");
                    // Modify one more time now that the form has an isAdmin flag
                    if (Input.IsAdmin)
                    {
                        // Asign the new role to the user - admin user
                        await _userManager.AddToRoleAsync(user, SD.AdminEndUser);

                        // Leave these lines - changed slightly in core 3.1 
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = user.Id, code = code },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        // We want to show the user list after adding a new Admin
                        return RedirectToPage("/Users/Index");
                    }
                    else
                    {
                        // Asign the new role to the user - customer user
                        await _userManager.AddToRoleAsync(user, SD.CustomerEndUser);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    _logger.LogInformation("User created a new account with password.");

                    // Added for core 3.1
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
