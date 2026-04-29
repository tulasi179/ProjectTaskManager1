using Projecttaskmanager.Models;

namespace Projecttaskmanager.Repositories;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<Project?> GetWithTasksAndDepsAsync(int id);
    Task<Project> AddAsync(Project project);
    Task<bool> UpdateAsync(int id, Project project);
    Task DeleteAsync(Project project);
    Task SaveChangesAsync();
}
