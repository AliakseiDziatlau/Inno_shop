using UserControl.Core.Entities;

namespace UserControl.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetAllAsync(int page, int pageSize);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<User?> GetByPasswordResetTokenAsync(string token);
    Task<User?> GetByConfirmationTokenAsync(string token);
}