// using Microsoft.EntityFrameworkCore;

// using Projecttaskmanager.Data;
// using Projecttaskmanager.Models;

// namespace Projecttaskmanager.Services;

// public class TaskDependencyService(AppDbContext context) : ITaskDependencyService
// {
//     public async Task<List<TaskDependency>> GetDependencies()
//         => await context.dependent.ToListAsync();

//     public async Task<List<TaskDependency>> GetDependentTasksById(int taskId)
//         => await context.dependent
//             .Where(d => d.TaskId == taskId)
//             .ToListAsync();

//     public async Task<TaskDependency> AddDependency(TaskDependency dependency)
//     {
//         context.dependent.Add(dependency);
//         await context.SaveChangesAsync();
//         return dependency;
       
//     }

//     public async Task<bool> RemoveDependency(int taskId, int dependentTaskId)
//     {
//         var dependency = await context.dependent
//             .FirstOrDefaultAsync(d =>
//                 d.TaskId == taskId &&
//                 d.DependentTaskId == dependentTaskId);

//         if (dependency == null)
//             return false;

//         context.dependent.Remove(dependency);
//         await context.SaveChangesAsync();

//         return true;
//         // throw new NotImplementedException();
//     }
// }




using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.Data;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Services;

public class TaskDependencyService(AppDbContext context) : ITaskDependencyService
{
    public async Task<List<TaskDependency>> GetDependencies()
        => await context.dependent.ToListAsync();

    public async Task<List<TaskDependency>> GetDependentTasksById(int taskId)
        => await context.dependent
            .Where(d => d.TaskId == taskId)
            .ToListAsync();

    public async Task<(bool Success, string Message, TaskDependency? Data)> AddDependency(TaskDependency dependency)
    {
        // Prevent self-dependency
        if (dependency.TaskId == dependency.DependentTaskId)
            return (false, "A task cannot depend on itself.", null);

        // Check for circular dependency
        bool hasCycle = await WouldCreateCycle(dependency.TaskId, dependency.DependentTaskId);
        if (hasCycle)
            return (false, "Adding this dependency would create a circular dependency.", null);

        context.dependent.Add(dependency);
        await context.SaveChangesAsync();
        return (true, "Dependency added successfully.", dependency);
    }

    public async Task<bool> RemoveDependency(int taskId, int dependentTaskId)
    {
        var dependency = await context.dependent
            .FirstOrDefaultAsync(d =>
                d.TaskId == taskId &&
                d.DependentTaskId == dependentTaskId);

        if (dependency == null)
            return false;

        context.dependent.Remove(dependency);
        await context.SaveChangesAsync();
        return true;
    }

    // Checks if making TaskId depend on DependentTaskId would create a cycle
    // starting from DependentTaskId, can we reach TaskId by following dependencies?
    // If yes adding this link would close the loop → cycle detected
    private async Task<bool> WouldCreateCycle(int taskId, int dependentTaskId)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();

        queue.Enqueue(dependentTaskId);

        while (queue.Count > 0)
        {
            int current = queue.Dequeue();

            if (current == taskId)
                return true; // cycle found

            if (!visited.Add(current))
                continue; // already visited

            // Get all tasks that 'current' depends on and follow the chain
            var nextDeps = await context.dependent
                .Where(d => d.TaskId == current)
                .Select(d => d.DependentTaskId)
                .ToListAsync();

            foreach (var next in nextDeps)
                queue.Enqueue(next);
        }

        return false;
    }
}