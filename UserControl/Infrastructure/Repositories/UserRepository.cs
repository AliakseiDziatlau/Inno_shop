using Microsoft.EntityFrameworkCore;
using UserControl.Core.Entities;
using UserControl.Core.Interfaces;
using UserControl.Infrastructure.Persistence;

namespace UserControl.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.user.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.user.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetAllAsync(int page, int pageSize)
    {
        return await _context.user
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.user.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.user.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.user.Remove(user);
        await _context.SaveChangesAsync();
    }
    
    public async Task<User?> GetByPasswordResetTokenAsync(string token)
    {
        return await _context.user.SingleOrDefaultAsync(u => u.PasswordResetToken == token);
    }
    
    public async Task<User?> GetByConfirmationTokenAsync(string token)
    {
        return await _context.user.SingleOrDefaultAsync(u => u.ConfirmationToken == token);
    }
}