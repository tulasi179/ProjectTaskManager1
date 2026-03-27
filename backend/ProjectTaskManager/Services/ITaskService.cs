using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Services
{
    public interface ITaskService
    {
        
    // public int Id { get; set; }

    // public int ProjectId { get; set; }

    // public string Title { get; set; } = string.Empty;

    // public string Description { get; set; } = string.Empty;

    // public string Status { get; set; } = "Pending";

    // public int AssigneeId { get; set; }

    Task<List<ProjectTasks>> GetAllTasksAsync();

    Task<List<ProjectTasks>> GetTasksByProjectId(int id);

    Task<ProjectTasks> GetTasksByIdAsync(int id);

    Task<ProjectTasks> AddTasksAsync(ProjectTasks tasks);
    Task<bool> DeleteTaskAsync(int id);

    Task<bool> UpdateTaskAsync(int id , ProjectTasks tasks);

    Task<List<ProjectTasks>> GetBlockingTasksAsync(int taskId);

    }
}