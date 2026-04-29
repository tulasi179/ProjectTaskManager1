using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Repositories;

public interface IUserRepository
{
    Task<List<UserResponce>> GetAllAsync();
    Task<UserResponce?> GetByIdAsync(int id);
    Task<Users?> GetEntityByIdAsync(int id);
    Task<Users?> GetByEmailAsync(string email);
    Task<Users> AddAsync(Users user);
    Task<bool> UpdateAsync(int id, UserResponce dto);
    Task<bool> DeleteAsync(int id);
    Task SaveChangesAsync();
    
}