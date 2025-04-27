
using Projekt.Models;
using Projekt.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System;
using Projekt.Data;
using Projekt.Models.Email;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace Projekt.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public AccountController(UserManager<Users> userManager,
                             SignInManager<Users> signInManager,
                             RoleManager<IdentityRole> roleManager,
                             AppDbContext context,
                             EmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _emailService = emailService;

        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Nieprawidłowa próba logowania.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Zostałeś pomyślnie zalogowany.";
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Nieprawidłowa próba logowania.");
            }

            return View(model);
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new Users
            {
                FullName = model.Name,
                UserName = model.Email,
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
                NormalizedUserName = model.Email.ToUpper(),
                Role = "User" // Domyślnie "User", ale admin może zmienić na "Employee"
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }
                if (!await _roleManager.RoleExistsAsync("Employee"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Employee"));
                }
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["Message"] = "Rejestracja zakończona sukcesem. Zaloguj się.";
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }


        //public IActionResult VerifyEmail()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> VerifyEmail(VerifyEmail model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.FindByEmailAsync(model.Email);
        //        if (user == null)
        //        {
        //            ModelState.AddModelError("", "Podany email nie istnieje.");
        //            return View(model);
        //        }

        //        return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
        //    }

        //    return View(model);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Message"] = "Zostałeś pomyślnie wylogowany.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("Użytkownik nie znaleziony.");
                }

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    TempData["Message"] = "Hasło zmieniono pomyślnie.";
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    // Dodaj logowanie dla debugowania
                    Console.WriteLine($"Błąd zmiany hasła: {error.Description}");
                }
            }
            else
            {
                // Loguj błędy walidacji
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Błąd walidacji: {error.ErrorMessage}");
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult ResetPassword(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("ForgtPassword");
            }
            return View(new ResetPasswordModel { UserId = userId });
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (result.Succeeded)
                {
                    TempData["Message"] = "Hasło zostało zmiennione pomyśnie. Możesz sie zalogować.";
                    return RedirectToAction("Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
           if (ModelState.IsValid)
            {
               var user = await _userManager.FindByEmailAsync(model.Email);
               if (user == null)
               { ModelState.AddModelError("", "Podany emial nie istnieje."); return NotFound(); }
               var code = new Random().Next(100000, 999999).ToString();
                var resetCode = new PasswordResetCodeModel
                {
                    UserId = user.Id,
                    Code = code,
                    CreatedAt = DateTime.Now,
                    EndTime = DateTime.Now.AddMinutes(30),
                    UseAt = null
                };
                _context.PasswordResetCodes.Add(resetCode);
                await _context.SaveChangesAsync();
                var emailBody = $@"
                <h2>Resetowanie hasła</h2>
                <p>Otrzymaliśmy prośbę o zresetowanie hasła. Twój kod weryfikacyjny to:</p>
                <p><strong>{code}</strong></p>
                <p>Wpisz ten kod na stronie, aby zmienić hasło. Kod jest ważny do {resetCode.EndTime.ToString("g")}.</p>";
                await _emailService.SendEmailAsync(user.Email, "Resetowanie hasła - Kod weryfikacyjny", emailBody);
                return RedirectToAction("VerifyCode", new {email = model.Email});
            }
           return View(model);
        }
        [HttpGet]
        public IActionResult VerifyCode(string email)
        {
            return View(new VerifyCodeModel { Email = email});
        }
        [HttpPost]
        public async Task<IActionResult> VerifyCode(VerifyCodeModel model)
        {
            if (ModelState.IsValid)
            {
                var resetCode = _context.PasswordResetCodes
                    .Where(c => c.Code == model.Code && c.User.Email.Equals(model.Email) && c.UseAt == null && c.EndTime > DateTime.Now)
                    .FirstOrDefault();
                if (resetCode == null)
                {
                    ModelState.AddModelError("", "Kod jest nieprawidłowy");
                    return View(model);
                }
                resetCode.UseAt = DateTime.Now;
                _context.Update(resetCode);
                await _context.SaveChangesAsync();
                var user = await _userManager.FindByEmailAsync(model.Email);
                return RedirectToAction("ResetPassword", new { userId = user.Id });
            }
            return View(model) ;
        }


    }
}
