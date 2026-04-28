using Projecttaskmanager.Models;

namespace Projecttaskmanager.Repositories;

public interface ITaskRepository
{
    Task<List<ProjectTasks>> GetAllAsync();
    Task<ProjectTasks?> GetByIdAsync(int id);
    Task<ProjectTasks?> GetWithDependenciesAsync(int id);
    Task<List<ProjectTasks>> GetByProjectIdAsync(int projectId);
    Task<List<ProjectTasks>> GetByUserIdAsync(int userId);
    Task<List<ProjectTasks>> GetBlockingTasksAsync(int taskId);
    Task<List<ProjectTasks>> GetDependentTasksAsync(int taskId);
    Task<ProjectTasks> AddAsync(ProjectTasks task);
    Task DeleteAsync(ProjectTasks task);
    Task ExecuteUpdateAsync(int id, ProjectTasks tasks);
    Task SaveChangesAsync();
    
}    