using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Services;

public interface ITaskDependencyService
{
    Task<List<TaskDependency>> GetDependencies();

    Task<List<TaskDependency>> GetDependentTasksById(int taskId);

     Task<(bool Success, string Message, TaskDependency? Data)> AddDependency(TaskDependency dependency);

    Task<bool> RemoveDependency(int taskId, int dependentTaskId);

    Task<bool> WouldCreateCycle(int taskId, int dependentTaskId);
}
