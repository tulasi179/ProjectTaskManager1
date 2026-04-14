using Projecttaskmanager.Models;

namespace ProjectTaskManager.MockData;

public static class TaskDependencyMockData
{
    public static List<TaskDependency> GetDependencies() =>
    [
        new TaskDependency 
        { 
            TaskId = 1, 
            DependentTaskId = 2 
        },
        new TaskDependency 
        { 
            TaskId = 1, 
            DependentTaskId = 3 
        },
        new TaskDependency 
        { 
            TaskId = 2, 
            DependentTaskId = 4 
        }
    ];

    public static List<TaskDependency> GetDependentTasksById(int taskId) =>
        GetDependencies().Where(d => d.TaskId == taskId).ToList();

    public static TaskDependency GetSingleDependency(int taskId, int dependentTaskId) =>
        GetDependencies().First(d => d.TaskId == taskId && d.DependentTaskId == dependentTaskId);
}