// using Projecttaskmanager.Data;
// using Microsoft.EntityFrameworkCore;
// using Projecttaskmanager.Models;
// namespace Projecttaskmanager.Services;


// public class TaskService(AppDbContext context, INotificationService notificationService): ITaskService
// {

//     public async Task<List<ProjectTasks>> GetAllTasksAsync()
//         => await context.tasks.ToListAsync();

//     public async Task<ProjectTasks?> GetTasksByIdAsync(int id)
//     {
//         var task = await context.tasks.FindAsync(id);
//         return task;
//     }

//       public   async Task<List<ProjectTasks>> GetTasksByProjectId(int id)
//     {
//          return await context.tasks.
//                 Where(t => t.ProjectId==id)
//                 .ToListAsync();
//     }

//     public async Task<ProjectTasks> AddTasksAsync(ProjectTasks tasks)
//     {
//         context.tasks.Add(tasks);
//         await context.SaveChangesAsync();
//         return tasks;
//     }

//     public  async Task<bool> DeleteTaskAsync(int id)
//     {
//          var res = await context.tasks.FindAsync(id);
//          if(res == null)
//          return false;

//          context.tasks.Remove(res);
//          await context.SaveChangesAsync();
//          return true;
//     }
//     public async Task<bool> UpdateTaskAsync(int id, ProjectTasks tasks)
//     {
//         var existing = await context.tasks.FindAsync(id);

//         if(existing == null)
//         return false;

//         existing.AssigneeId = tasks.AssigneeId;
//         existing.Description = tasks.Description;
//         existing.ProjectId = tasks.ProjectId;
//         existing.Status = tasks.Status;
//         existing.Title = tasks.Title;

//         await context.SaveChangesAsync();
//         return true;
//     }

    

// }

using Projecttaskmanager.Data;
using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Services;

public class TaskService(AppDbContext context, INotificationService notificationService) : ITaskService
{
    public async Task<List<ProjectTasks>> GetAllTasksAsync()
        => await context.tasks.ToListAsync();

    public async Task<ProjectTasks> GetTasksByIdAsync(int id)
    {
        var task = await context.tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
            throw new KeyNotFoundException($"Task with Id {id} was not found.");

        return task;
    }
    public async Task<List<ProjectTasks>> GetTasksByProjectId(int id)
        => await context.tasks
            .Where(t => t.ProjectId == id)
            .ToListAsync();

    public async Task<ProjectTasks> AddTasksAsync(ProjectTasks tasks)
    {
        context.tasks.Add(tasks);
        await context.SaveChangesAsync();
        return tasks;
    }

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var res = await context.tasks.FindAsync(id);
        if (res == null) throw new KeyNotFoundException($"Task with Id {id} was not found.");

        context.tasks.Remove(res);
        await context.SaveChangesAsync();
        return true;
    }

    // public async Task<bool> UpdateTaskAsync(int id, ProjectTasks tasks)
    // {
    //     var existing = await context.tasks.FindAsync(id);
    //     if (existing == null) return false;

    //     bool justCompleted = tasks.Status == "Completed" && existing.Status != "Completed";

    //     existing.AssigneeId = tasks.AssigneeId;
    //     existing.Description = tasks.Description;
    //     existing.ProjectId = tasks.ProjectId;
    //     existing.Status = tasks.Status;
    //     existing.Title = tasks.Title;

    //     await context.SaveChangesAsync();

    //     // If task just became Completed, notify assignees of dependent tasks
    //     if (justCompleted)
    //         await NotifyDependentTaskAssignees(existing);

    //     return true;
    // }

    // When task A completes, find all tasks that were waiting on A (DependentTaskId == A)
    // and notify their assignees

   public async Task<List<ProjectTasks>> GetBlockingTasksAsync(int taskId)
    {
        // Find tasks that must complete BEFORE this task can start
        return await context.dependent
            .Where(d => d.DependentTaskId == taskId) 
            .Join(context.tasks,
                d => d.TaskId,
                t => t.Id,
                (d, t) => t)
            .ToListAsync();
    } 
    public async Task<bool> UpdateTaskAsync(int id, ProjectTasks tasks)
    {
        
        var existing = await context.tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
            
        if (existing == null) throw new KeyNotFoundException($"Task with Id {id} was not found.");

        bool justCompleted = tasks.Status == "Completed" && existing.Status != "Completed";

        Console.WriteLine($"DEBUG: justCompleted = {justCompleted}, newStatus = {tasks.Status}, oldStatus = {existing.Status}");

      
        await context.tasks
            .Where(t => t.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.Status, tasks.Status)
                .SetProperty(t => t.AssigneeId, tasks.AssigneeId)
                .SetProperty(t => t.Description, tasks.Description)
                .SetProperty(t => t.ProjectId, tasks.ProjectId)
                .SetProperty(t => t.Title, tasks.Title));

        if (justCompleted)
        {
            existing.Id = id;
            existing.Title = tasks.Title == string.Empty ? existing.Title : tasks.Title;
            Console.WriteLine($"DEBUG: Calling NotifyDependentTaskAssignees for task {id}");
            await NotifyDependentTaskAssignees(existing);
        }

        return true;
    }

private async Task NotifyDependentTaskAssignees(ProjectTasks completedTask)
{
    var dependentTasks = await context.dependent
        .Where(d => d.TaskId == completedTask.Id)
        .Join(context.tasks,
            d => d.DependentTaskId,
            t => t.Id,
            (d, t) => t)
        .ToListAsync();

    Console.WriteLine($"DEBUG: Found {dependentTasks.Count} dependent tasks for task {completedTask.Id}");

    foreach (var depTask in dependentTasks)
    {
        Console.WriteLine($"DEBUG: Notifying user {depTask.AssigneeId} for task {depTask.Title}");
        await notificationService.CreateNotification(
            depTask.AssigneeId,
            $"Task '{completedTask.Title}' has been completed. Your task '{depTask.Title}' can now proceed."
        );
    }
}

public async Task<List<ProjectTasks>> GetTasksByUserIdAsync(int userId)
{
    return await context.tasks
        .Where(t => t.AssigneeId == userId)
        .ToListAsync();
}
}