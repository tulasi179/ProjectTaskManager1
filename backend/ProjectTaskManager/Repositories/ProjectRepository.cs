using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.Data;
using Projecttaskmanager.Models;
using Projecttaskmanager.Repositories;

namespace Projecttaskmanager.Repositories;

public class ProjectRepository(AppDbContext context) : IProjectRepository
{
    public async Task<List<Project>> GetAllAsync()
        => await context.project.ToListAsync();

    public async Task<Project?> GetByIdAsync(int id)
        => await context.project
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

    // Used by DeleteProjectAsync — loads tasks + both dependency directions
    public async Task<Project?> GetWithTasksAndDepsAsync(int id)
        => await context.project
            .Include(p => p.Tasks)
                .ThenInclude(t => t.Dependencies)
            .Include(p => p.Tasks)
                .ThenInclude(t => t.Dependents)
            .FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Project> AddAsync(Project project)
    {
        context.project.Add(project);
        await context.SaveChangesAsync();
        return project;
    }

    public async Task<bool> UpdateAsync(int id, Project project)
    {
        var pro = await context.project.FindAsync(id);
        if (pro == null)
            throw new KeyNotFoundException($"Project with Id {id} was not found.");

        pro.Name = project.Name;
        pro.OwnerId = project.OwnerId;
        pro.Description = project.Description;
        pro.EndDate = project.EndDate;

        await context.SaveChangesAsync();
        return true;
    }

    public async Task DeleteAsync(Project project)
    {
        // 1. Delete TaskDependencies first
        foreach (var task in project.Tasks)
        {
            context.dependent.RemoveRange(task.Dependencies);
            context.dependent.RemoveRange(task.Dependents);
        }

        // 2. Delete Tasks
        context.tasks.RemoveRange(project.Tasks);

        // 3. Delete Project
        context.project.Remove(project);
    }

    public async Task SaveChangesAsync()
        => await context.SaveChangesAsync();
}