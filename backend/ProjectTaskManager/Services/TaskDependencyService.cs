using Projecttaskmanager.Models;
using Projecttaskmanager.Repositories;

namespace Projecttaskmanager.Services;

public class TaskDependencyService(ITaskDependencyRepository repo) : ITaskDependencyService
{

    public async Task<List<TaskDependency>> GetDependencies()
        => await repo.GetAllAsync();


    public async Task<List<TaskDependency>> GetDependentTasksById(int taskId)
        => await repo.GetByTaskIdAsync(taskId);


    public async Task<(bool Success, string Message, TaskDependency? Data)> AddDependency(TaskDependency dependency)
    {
        // prevent self-dependency
        if (dependency.TaskId == dependency.DependentTaskId)
            return (false, "A task cannot depend on itself.", null);

        // check for circular dependency
        bool hasCycle = await WouldCreateCycle(dependency.TaskId, dependency.DependentTaskId);
        if (hasCycle)
            return (false, "Adding this dependency would create a circular dependency.", null);

        await repo.AddAsync(dependency);
        await repo.SaveChangesAsync();
        return (true, "Dependency added successfully.", dependency);
    }

    public async Task<bool> RemoveDependency(int taskId, int dependentTaskId)
    {
        var dependency = await repo.GetAsync(taskId, dependentTaskId);
        if (dependency == null)
            return false;

        await repo.RemoveAsync(dependency);
        await repo.SaveChangesAsync();
        return true;
    }

//BFS 
    public async Task<bool> WouldCreateCycle(int taskId, int dependentTaskId)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();
        queue.Enqueue(dependentTaskId);
        while (queue.Count > 0)
        {
            int current = queue.Dequeue();
            if (current == taskId)
                return true;//self cycle/loop
            if (!visited.Add(current))
                continue;
          
            var nextDeps = await repo.GetDependentIdChainAsync(current);
            foreach (var next in nextDeps)
                queue.Enqueue(next);
        }
        return false;
    }
}