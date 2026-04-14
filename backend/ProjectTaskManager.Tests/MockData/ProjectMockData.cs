using Projecttaskmanager.Models;

namespace ProjectTaskManager.MockData;

public class ProjectMockData
{
    public static List<Project> GetProjects() => new()
    {
        new Project 
        {
             Id = 1, 
             Name = "TaskManager",   
             OwnerId = 1, 
             Description = "Task management app",  
             StartDate = new DateTime(2026, 1, 1), 
             EndDate = new DateTime(2026, 6, 1) 
        },
        new Project 
        { 
            Id = 2, 
            Name = "Blog Platform", 
            OwnerId = 1, 
            Description = "Blogging system",      
            StartDate = new DateTime(2026, 2, 1), 
            EndDate = new DateTime(2026, 5, 1) 
        },
        new Project 
        { 
            Id = 3, 
            Name = "E-Commerce",    
            OwnerId = 1, 
            Description = "Online store",         
            StartDate = new DateTime(2026, 3, 1), 
            EndDate = new DateTime(2026, 9, 1) 
        },
    };

    public static Project GetSingleProject(int id) =>
        GetProjects().First(p => p.Id == id);
}