using Projecttaskmanager.Models;
using Projecttaskmanager.Repositories;

namespace Projecttaskmanager.Services;

public class TaskService(ITaskRepository repo, INotificationService notificationService) : ITaskService
{

    public async Task<List<ProjectTasks>> GetAllTasksAsync()
        => await repo.GetAllAsync();



    public async Task<ProjectTasks> GetTasksByIdAsync(int id)
    {
        var task = await repo.GetByIdAsync(id);
        if (task is null)
            throw new KeyNotFoundException($"Task with Id {id} was not found.");
        return task;
    
    }

    public async Task<List<ProjectTasks>> GetTasksByProjectId(int id)
        => await repo.GetByProjectIdAsync(id);

    public async Task<ProjectTasks> AddTasksAsync(ProjectTasks tasks)
        => await repo.AddAsync(tasks);

    public async Task<bool> DeleteTaskAsync(int id)
    {
        var task = await repo.GetWithDependenciesAsync(id);
        if (task is null)
            throw new KeyNotFoundException($"Task with Id {id} was not found.");

        await repo.DeleteAsync(task);
        await repo.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProjectTasks>> GetBlockingTasksAsync(int taskId)
        => await repo.GetBlockingTasksAsync(taskId);


    public async Task<bool> UpdateTaskAsync(int id, ProjectTasks tasks)
    {
        var existing = await repo.GetByIdAsync(id);//curr state in the db
        if (existing is null)
            throw new KeyNotFoundException($"Task with Id {id} was not found.");

        bool justCompleted = tasks.Status == "Completed" && existing.Status != "Completed";
        //Console.WriteLine($"DEBUG: justCompleted = {justCompleted}, newStatus = {tasks.Status}, oldStatus = {existing.Status}");
        await repo.ExecuteUpdateAsync(id, tasks);

        if (justCompleted)
        {
            existing.Id = id;
            existing.Title = tasks.Title == string.Empty ? existing.Title : tasks.Title;
            //Console.WriteLine($"DEBUG: Calling NotifyDependentTaskAssignees for task {id}");
            await NotifyDependentTaskAssignees(existing);//when the status chnages checks if they have any dependent tasks and send the noti
        }
        return true;
    }


    public async Task<List<ProjectTasks>> GetTasksByUserIdAsync(int userId)
        => await repo.GetByUserIdAsync(userId);


    private async Task NotifyDependentTaskAssignees(ProjectTasks completedTask)
    {
        var dependentTasks = await repo.GetDependentTasksAsync(completedTask.Id);
       // Console.WriteLine($"DEBUG: Found {dependentTasks.Count} dependent tasks for task {completedTask.Id}");
        foreach (var depTask in dependentTasks)
        {
           // Console.WriteLine($"DEBUG: Notifying user {depTask.AssigneeId} for task {depTask.Title}");
            await notificationService.CreateNotification(
                depTask.AssigneeId,
                $"Task '{completedTask.Title}' has been completed. Your task '{depTask.Title}' can now proceed."
            );
        }
    }
}