using EVServiceCenterMaintenanceSystem.Models;
using EVServiceCenterMaintenanceSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EVServiceCenterMaintenanceSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(IUserRepository userRepo, IPasswordHasher<User> passwordHasher)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var existing = await _userRepo.GetByUsernameAsync(vm.Username);
            if (existing != null)
            {
                ModelState.AddModelError(nameof(vm.Username), "Tên đăng nhập đã tồn tại");
                return View(vm);
            }

            var user = new User
            {
                Username = vm.Username
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, vm.Password);

            var added = await _userRepo.AddUserAsync(user);
            if (!added)
            {
                ModelState.AddModelError("", "Không thể tạo tài khoản (xung đột)");
                return View(vm);
            }

            // Tự động login sau khi đăng ký (tuỳ ý)
            await SignInUserAsync(user);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _userRepo.GetByUsernameAsync(vm.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(vm);
            }

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, vm.Password);
            if (verify == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(vm);
            }

            await SignInUserAsync(user);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private async Task SignInUserAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            if (!string.IsNullOrEmpty(user.Role))
                claims.Add(new Claim(ClaimTypes.Role, user.Role));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
