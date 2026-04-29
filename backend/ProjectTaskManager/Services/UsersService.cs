using Microsoft.AspNetCore.Identity;
using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;
using Projecttaskmanager.Repositories;

namespace Projecttaskmanager.Services;

public class UsersServices(IUserRepository repo) : IUsersService
{
    public async Task<List<UserResponce>> GetAllUsersAsync()
        => await repo.GetAllAsync();


    public async Task<UserResponce?> GetUserByIdAsync(int id)
    {
        var result = await repo.GetByIdAsync(id);
        if (result is null)
            throw new KeyNotFoundException($"User with Id {id} was not found.");
        return result;
    }

    public async Task<Users> AddUsersAsync(Users users)
        => await repo.AddAsync(users);

    public async Task<bool> DeleteUsersAysnc(int id)
        => await repo.DeleteAsync(id);

    public async Task<bool> UpdateUserAysnc(int id, UserResponce dto)
        => await repo.UpdateAsync(id, dto);

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await repo.GetEntityByIdAsync(userId);
        if (user is null)
            throw new KeyNotFoundException("User not found.");

        var hasher = new PasswordHasher<Users>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Current password is incorrect.");

        user.PasswordHash = hasher.HashPassword(user, dto.NewPassword);
        await repo.SaveChangesAsync();
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await repo.GetByEmailAsync(dto.Email);
        if (user is null)
            throw new KeyNotFoundException("User not found.");

        var hasher = new PasswordHasher<Users>();
        user.PasswordHash = hasher.HashPassword(user, dto.NewPassword);
        await repo.SaveChangesAsync();
    }
}