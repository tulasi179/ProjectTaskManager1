using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.Data;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Services;

public class ProjectService(AppDbContext context) :IProjectService
{
    /*
    Task<List<Project>> GetAllProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int Id);
        Task<Project> AddProjectAsync(Project project);
        Task<bool> UpdateProjectAsync(int id, Project project);
        Task<bool> DeleteProjetcAsync(int id);
    */

    public async Task<List<Project>> GetAllProjectsAsync()
        => await context.project.ToListAsync();

    public async Task<Project?> GetProjectByIdAsync(int id)
    {
         var project = await context.project
        .AsNoTracking()
        .FirstOrDefaultAsync(p => p.Id == id);
        if (project is null)
            throw new KeyNotFoundException($"Project with Id {id} was not found.");
        return project;
    }

    public  async Task<Project> AddProjectAsync(Project project)
    {
         context.project.Add(project);
         await context.SaveChangesAsync();
         return project;
    }

    public async Task<bool> UpdateProjectAsync(int id, Project project)
    {
        var pro = await context.project.FindAsync(id);

        if(pro==null)
          throw new KeyNotFoundException($"Project with Id {id} was not found.");

        pro.Name = project.Name;
        pro.OwnerId = project.OwnerId;
        pro.Description = project.Description;
        pro.EndDate = project.EndDate;
         
         await context.SaveChangesAsync();
         return true;
    }

    public async Task<bool> DeleteProjectAsync(int id)
{
    var pro = await context.project
        .Include(p => p.Tasks)
            .ThenInclude(t => t.Dependencies)  // Include task dependencies
        .Include(p => p.Tasks)
            .ThenInclude(t => t.Dependents)    // Include task dependents
        .FirstOrDefaultAsync(p => p.Id == id);

    if (pro == null)
        throw new KeyNotFoundException($"Project with Id {id} was not found.");

    // 1. Delete TaskDependencies first (they reference tasks)
    foreach (var task in pro.Tasks)
    {
        context.dependent.RemoveRange(task.Dependencies);
        context.dependent.RemoveRange(task.Dependents);
    }

    // 2. Delete Tasks
    context.tasks.RemoveRange(pro.Tasks);

    // 3. Delete Project
    context.project.Remove(pro);

    await context.SaveChangesAsync();
    return true;
}

}