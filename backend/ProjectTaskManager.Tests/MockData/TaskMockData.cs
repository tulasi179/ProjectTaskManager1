using Projecttaskmanager.Models;

namespace ProjectTaskManager.MockData;

public class TaskMockData
{
    public static List<ProjectTasks> GetTasks() => new()
    {
        new ProjectTasks 
        { 
            Id = 1, 
            ProjectId = 1, 
            Title = "Create Tables", 
            Description = "Setup DB tables",  
            Status = "Pending",  
             AssigneeId = 2 
        },
        new ProjectTasks 
        {
             Id = 2,
             ProjectId = 1,
              Title = "Build APIs",  
              Description = "Build REST APIs",
              Status = "InProgress",
               AssigneeId = 3 
        },
        new ProjectTasks 
        {
             Id = 3,
              ProjectId = 1, 
              Title = "Authentication",  
              Description = "JWT Auth setup",    
              Status = "Completed",  
              AssigneeId = 2 
        },
        new ProjectTasks 
        {
             Id = 4,
             ProjectId = 2, 
             Title = "Blog UI",         
             Description = "Design blog pages", 
             Status = "Pending",    
             AssigneeId = 3 
        },
        new ProjectTasks 
        {
             Id = 5, 
             ProjectId = 2, 
             Title = "Blog API",        
             Description = "Blog endpoints",    
             Status = "InProgress", 
             AssigneeId = 2 
        },
    };

    public static List<ProjectTasks> GetTasksByUser(int userId) =>
        GetTasks().Where(t => t.AssigneeId == userId).ToList();

    public static List<ProjectTasks> GetTasksByProject(int projectId) =>
        GetTasks().Where(t => t.ProjectId == projectId).ToList();

    public static ProjectTasks GetSingleTask(int id) =>
        GetTasks().First(t => t.Id == id);
}