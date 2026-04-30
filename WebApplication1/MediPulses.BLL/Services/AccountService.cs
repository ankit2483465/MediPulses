using BCrypt.Net;
using MediPulses.BLL.Interfaces;
using MediPulses.DAL.Context;
using MediPulses.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediPulses.BLL.Services;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _db;

    public AccountService(ApplicationDbContext db) => _db = db;

    public Task<bool> EmailExistsAsync(string email) =>
        _db.Users.AnyAsync(u => u.Email == email);

    public async Task CreateUserAsync(User user)
    {
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.Role = "User";
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return null;

        bool isBcryptHash = user.Password != null && user.Password.StartsWith("$2");
        bool passwordValid;

        if (isBcryptHash)
        {
            try { passwordValid = BCrypt.Net.BCrypt.Verify(password, user.Password); }
            catch { return null; }
        }
        else
        {
            passwordValid = user.Password == password;
            if (passwordValid)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                await _db.SaveChangesAsync();
            }
        }

        return passwordValid ? user : null;
    }
}
