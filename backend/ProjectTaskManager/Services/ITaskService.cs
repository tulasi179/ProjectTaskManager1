using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Services
{
    public interface ITaskService
    {

    Task<List<ProjectTasks>> GetAllTasksAsync();

    Task<List<ProjectTasks>> GetTasksByProjectId(int id);

    Task<ProjectTasks> GetTasksByIdAsync(int id);

    Task<ProjectTasks> AddTasksAsync(ProjectTasks tasks);
    Task<bool> DeleteTaskAsync(int id);

    Task<bool> UpdateTaskAsync(int id , ProjectTasks tasks);

    Task<List<ProjectTasks>> GetBlockingTasksAsync(int taskId);

    Task<List<ProjectTasks>> GetTasksByUserIdAsync(int userId);

    }
}