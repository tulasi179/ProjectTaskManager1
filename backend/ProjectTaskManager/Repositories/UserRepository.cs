using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.Data;
using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;
using Projecttaskmanager.Repositories;

namespace Projecttaskmanager.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<List<UserResponce>> GetAllAsync()
        => await context.User.Select(c => new UserResponce
        {
            Id = c.Id,
            Username = c.Username,
            Email = c.Email,
            Role = c.Role,
            IsActive = c.IsActive
        }).ToListAsync();

    public async Task<UserResponce?> GetByIdAsync(int id)
        => await context.User
            .Where(c => c.Id == id)
            .Select(c => new UserResponce
            {
                Username = c.Username,
                Email = c.Email,
                Role = c.Role
            })
            .FirstOrDefaultAsync();

    // Returns full Users entity — used for password operations
    public async Task<Users?> GetEntityByIdAsync(int id)
        => await context.User.FindAsync(id);

    // Used by AuthService/ResetPassword to look up by email
    public async Task<Users?> GetByEmailAsync(string email)
        => await context.User.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<Users> AddAsync(Users user)
    {
        context.User.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(int id, UserResponce dto)
    {
        var user = await context.User.FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with Id {id} was not found.");

        user.Username = dto.Username;
        user.Email = dto.Email;
        user.Role = dto.Role;
        user.IsActive = dto.IsActive;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await context.User.FindAsync(id);
        if (user == null)
            throw new KeyNotFoundException($"User with Id {id} was not found.");

        context.User.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task SaveChangesAsync()
        => await context.SaveChangesAsync();
}