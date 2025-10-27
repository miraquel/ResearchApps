using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ResearchApps.Common.Constants;
using ResearchApps.Domain;

namespace ResearchApps.Web.Pages.Setup
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly RoleManager<AppIdentityRole> _roleManager;

        public IndexModel(UserManager<AppIdentityUser> userManager, RoleManager<AppIdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public bool Success { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Create Admin Role if not exists
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                adminRole = new AppIdentityRole { Name = "Admin" };
                await _roleManager.CreateAsync(adminRole);

                // Assign all permissions to Admin role
                var permissions = PermissionConstants.GetAllPermissions();
                foreach (var permission in permissions)
                {
                    await _roleManager.AddClaimAsync(adminRole, new System.Security.Claims.Claim("permission", permission));
                }
            }

            // Create Admin User
            var user = new AppIdentityUser
            {
                UserName = Input.Username,
                Email = Input.Email,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return Page();
            }

            // Assign Admin role to user
            await _userManager.AddToRoleAsync(user, "Admin");

            Success = true;
            return Page();
        }
    }
}
