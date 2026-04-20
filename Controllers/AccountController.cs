using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectApprovalSystem.Models;
using ProjectApprovalSystem.Models.ViewModels;
using System.Threading.Tasks;

namespace ProjectApprovalSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            // If already logged in, redirect to appropriate dashboard
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToDashboardBasedOnRole();
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);

                    // If returnUrl exists and is valid, use it
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // Redirect based on role - using Redirect for absolute URLs
                    if (roles.Contains("Student"))
                    {
                        return Redirect("/Student/Dashboard");
                    }
                    else if (roles.Contains("Supervisor"))
                    {
                        return Redirect("/Supervisor/Dashboard");
                    }
                    else if (roles.Contains("ModuleLeader") || roles.Contains("SystemAdmin"))
                    {
                        return Redirect("/Admin/Dashboard");
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SimpleLogin(string email = "student@pas.com", string password = "Student@123")
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, true, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);
                var roles = await _userManager.GetRolesAsync(user);
                return Content($"SUCCESS! You are now logged in as {email}. Roles: {string.Join(", ", roles)}. <a href='/Student/Dashboard'>Go to Dashboard</a>", "text/html");
            }

            return Content($"FAILED! Could not login as {email}. Please check credentials.", "text/html");
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserType = model.UserType == "Supervisor" ? UserRole.Supervisor : UserRole.Student,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var role = model.UserType == "Supervisor" ? "Supervisor" : "Student";
                    await _userManager.AddToRoleAsync(user, role);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    if (role == "Student")
                    {
                        return Redirect("/Student/Dashboard");
                    }
                    else
                    {
                        return Redirect("/Supervisor/Dashboard");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult CheckAuth()
        {
            return Content($"Authenticated: {User.Identity.IsAuthenticated}, Name: {User.Identity.Name}");
        }

        private IActionResult RedirectToDashboardBasedOnRole()
        {
            if (User.IsInRole("Student"))
            {
                return Redirect("/Student/Dashboard");
            }
            else if (User.IsInRole("Supervisor"))
            {
                return Redirect("/Supervisor/Dashboard");
            }
            else if (User.IsInRole("ModuleLeader") || User.IsInRole("SystemAdmin"))
            {
                return Redirect("/Admin/Dashboard");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}