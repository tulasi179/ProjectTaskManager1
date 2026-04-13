using Projecttaskmanager.Models;

namespace Projecttaskmanager.Repositories;

public interface ITaskDependencyRepository
{
    Task<List<TaskDependency>> GetAllAsync();
    Task<List<TaskDependency>> GetByTaskIdAsync(int taskId);
    Task<TaskDependency?> GetAsync(int taskId, int dependentTaskId);
    Task<List<int>> GetDependentIdChainAsync(int taskId);
    Task AddAsync(TaskDependency dependency);
    Task RemoveAsync(TaskDependency dependency);
    Task SaveChangesAsync();
}