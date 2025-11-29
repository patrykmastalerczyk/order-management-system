using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Interfaces;

namespace OrderManagementSystem.Infrastructure.Data;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
    }

    public async Task<User?> GetByUsernameAndPasswordAsync(string username, string password)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password && u.IsActive);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.Where(u => u.IsActive).ToListAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        if (existingUser == null) return false;

        existingUser.UpdateProfile(user.FullName);
        if (user.IsActive != existingUser.IsActive)
        {
            if (user.IsActive)
                existingUser.Activate();
            else
                existingUser.Deactivate();
        }
        if (user.Password != existingUser.Password)
        {
            existingUser.ChangePassword(user.Password);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return false;

        user.Deactivate();
        await _context.SaveChangesAsync();
        return true;
    }
}
