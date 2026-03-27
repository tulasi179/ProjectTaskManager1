using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Projecttaskmanager.Data;
using Projecttaskmanager.Models;
using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.DTOs;

namespace Projecttaskmanager.Services;

public class UsersServices(AppDbContext context) : IUsersService
{

   public async Task<List<UserResponce>> GetAllUsersAsync()
    => await context.User.Select(c => new UserResponce
    {
        Id = c.Id,       
        Username = c.Username,
        Email = c.Email,
        Role = c.Role,
        IsActive = c.IsActive
    }).ToListAsync();

    public async Task<UserResponce?> GetUserByIdAsync(int id)
    {
       var result = await context.User
        .Where(c => c.Id == id)
        .Select(c => new UserResponce
        {
            Username = c.Username,
            Email = c.Email,
            Role = c.Role
        })
        .FirstOrDefaultAsync();

        if (result is null)
            throw new KeyNotFoundException($"User with Id {id} was not found.");

        return result;
   }
    public async Task<Users> AddUsersAsync(Users users)
    {
         context.User.Add(users);

        await context.SaveChangesAsync();

        return users;
    }

    public async Task<bool>  DeleteUsersAysnc(int id)
    {
        var user = await context.User.FindAsync(id);

        if (user == null)
            throw new KeyNotFoundException($"User with Id {id} was not found.");

        context.User.Remove(user);

        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateUserAysnc(int id, UserResponce dto)
    {
        var existingUser = await context.User.FindAsync(id);

        if (existingUser == null)
            throw new KeyNotFoundException($"User with Id {id} was not found.");

        existingUser.Username = dto.Username;
        existingUser.Email= dto.Email;
       // existingUser.PasswordHash = dto.PasswordHash;
        existingUser.Role = dto.Role;
        existingUser.IsActive = dto.IsActive;
       // existingUser.CreatedAt = DateTime.Now;

        await context.SaveChangesAsync();

        return true;
    }

   public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
{
    var user = await context.User.FindAsync(userId);
    if (user is null)
        throw new KeyNotFoundException("User not found.");

    var hasher = new PasswordHasher<Users>();

    // Verify current password using PasswordHasher
    var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);
    if (result == PasswordVerificationResult.Failed)
        throw new UnauthorizedAccessException("Current password is incorrect.");

    // Hash new password using PasswordHasher
    user.PasswordHash = hasher.HashPassword(user, dto.NewPassword);
    await context.SaveChangesAsync();
}
}