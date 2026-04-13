using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.Data;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Repositories;

public class TaskRepository(AppDbContext context) : ITaskRepository
{
    public async Task<List<ProjectTasks>> GetAllAsync()
        => await context.tasks.ToListAsync();

    public async Task<ProjectTasks?> GetByIdAsync(int id)
        => await context.tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

    // Used by DeleteTaskAsync — loads both dependency directions
    public async Task<ProjectTasks?> GetWithDependenciesAsync(int id)
        => await context.tasks
            .Include(t => t.Dependencies)
            .Include(t => t.Dependents)
            .FirstOrDefaultAsync(t => t.Id == id);

    public async Task<List<ProjectTasks>> GetByProjectIdAsync(int projectId)
        => await context.tasks
            .Where(t => t.ProjectId == projectId)
            .ToListAsync();

    public async Task<List<ProjectTasks>> GetByUserIdAsync(int userId)
        => await context.tasks
            .Where(t => t.AssigneeId == userId)
            .ToListAsync();

    // Tasks that must complete BEFORE this task (blockers)
    public async Task<List<ProjectTasks>> GetBlockingTasksAsync(int taskId)
        => await context.dependent
            .Where(d => d.DependentTaskId == taskId)
            .Join(context.tasks,
                d => d.TaskId,
                t => t.Id,
                (d, t) => t)
            .ToListAsync();

    // Tasks that are waiting on this task to complete (dependents)
    public async Task<List<ProjectTasks>> GetDependentTasksAsync(int taskId)
        => await context.dependent
            .Where(d => d.TaskId == taskId)
            .Join(context.tasks,
                d => d.DependentTaskId,
                t => t.Id,
                (d, t) => t)
            .ToListAsync();

    public async Task<ProjectTasks> AddAsync(ProjectTasks task)
    {
        context.tasks.Add(task);
        await context.SaveChangesAsync();
        return task;
    }

    public async Task DeleteAsync(ProjectTasks task)
    {
        // 1. Delete dependencies both directions
        context.dependent.RemoveRange(task.Dependencies);
        context.dependent.RemoveRange(task.Dependents);

        // 2. Delete task
        context.tasks.Remove(task);
    }

    public async Task ExecuteUpdateAsync(int id, ProjectTasks tasks)
        => await context.tasks
            .Where(t => t.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.Status, tasks.Status)
                .SetProperty(t => t.AssigneeId, tasks.AssigneeId)
                .SetProperty(t => t.Description, tasks.Description)
                .SetProperty(t => t.ProjectId, tasks.ProjectId)
                .SetProperty(t => t.Title, tasks.Title));

    public async Task SaveChangesAsync()
        => await context.SaveChangesAsync();
}