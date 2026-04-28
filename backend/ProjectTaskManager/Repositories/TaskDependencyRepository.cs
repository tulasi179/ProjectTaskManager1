using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.Data;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Repositories;

public class TaskDependencyRepository(AppDbContext context) : ITaskDependencyRepository
{

    public async Task<List<TaskDependency>> GetAllAsync()
        => await context.dependent.ToListAsync();


    public async Task<List<TaskDependency>> GetByTaskIdAsync(int taskId)
        => await context.dependent
            .Where(d => d.TaskId == taskId)
            .ToListAsync();


    // checks if there is is  a dependency relationship exist to delete a dependency
    public async Task<TaskDependency?> GetAsync(int taskId, int dependentTaskId)
        => await context.dependent
            .FirstOrDefaultAsync(d =>
                d.TaskId == taskId &&
                d.DependentTaskId == dependentTaskId);

    // used by cycle detection — returns next dependent IDs in the chain
    public async Task<List<int>> GetDependentIdChainAsync(int taskId)
        => await context.dependent
            .Where(d => d.TaskId == taskId)
            .Select(d => d.DependentTaskId)
            .ToListAsync();

    public async Task AddAsync(TaskDependency dependency)
        => context.dependent.Add(dependency);

    public async Task RemoveAsync(TaskDependency dependency)
        => context.dependent.Remove(dependency);

    public async Task SaveChangesAsync()
        => await context.SaveChangesAsync();
}