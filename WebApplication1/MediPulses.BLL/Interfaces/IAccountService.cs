using MediPulses.DAL.Entities;

namespace MediPulses.BLL.Interfaces;

public interface IAccountService
{
    Task<bool> EmailExistsAsync(string email);
    Task CreateUserAsync(User user);
    Task<User?> AuthenticateAsync(string email, string password);
}
