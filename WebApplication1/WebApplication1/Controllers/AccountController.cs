using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _config;

        public AccountController(IAccountService accountService, IConfiguration config)
        {
            _accountService = accountService;
            _config = config;
        }

        public IActionResult Signup() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(User user)
        {
            if (!ModelState.IsValid) return View(user);

            if (await _accountService.EmailExistsAsync(user.Email))
            {
                ModelState.AddModelError("Email", "Email already in use.");
                return View(user);
            }

            await _accountService.CreateUserAsync(user);
            return RedirectToAction("Login");
        }

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _accountService.AuthenticateAsync(email, password);

            if (user != null)
            {
                var tokenString = GenerateJwtToken(user);

                Response.Cookies.Append("JWTToken", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(2)
                });

                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserRole", user.Role ?? "User");
                HttpContext.Session.SetInt32("UserId", user.UserId);

                return user.Role == "Admin"
                    ? RedirectToAction("Index", "Admin")
                    : RedirectToAction("Index", "Home");
            }

            ViewBag.Message = "Invalid Email or Password";
            return View();
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role ?? "User")
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("JWTToken");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}
