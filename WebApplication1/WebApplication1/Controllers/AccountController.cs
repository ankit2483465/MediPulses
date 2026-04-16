using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net; // Requires BCrypt.Net-Next NuGet package
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;

        public AccountController(ApplicationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public IActionResult Signup() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(User user)
        {
            if (!ModelState.IsValid) return View(user);

            // 1. Check if user exists
            if (await _db.Users.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already in use.");
                return View(user);
            }

            // 2. Hash the password before saving
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Role = "User";

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            // 3. Find user by email first
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            // 4. Verify password — support both BCrypt-hashed and legacy plain-text passwords
            bool passwordValid = false;
            if (user != null)
            {
                bool isBcryptHash = user.Password != null && user.Password.StartsWith("$2");
                if (isBcryptHash)
                {
                    try { passwordValid = BCrypt.Net.BCrypt.Verify(password, user.Password); }
                    catch { passwordValid = false; }
                }
                else
                {
                    // Legacy plain-text: compare directly, then upgrade to BCrypt
                    passwordValid = user.Password == password;
                    if (passwordValid)
                    {
                        user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                        await _db.SaveChangesAsync();
                    }
                }
            }

            if (passwordValid && user != null)
            {
                var tokenString = GenerateJwtToken(user);

                Response.Cookies.Append("JWTToken", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(2)
                });

                // Write to session so HomeController can read the user name
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserRole", user.Role ?? "User");

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
            // 5. Pull secret from configuration
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Secret"]);

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