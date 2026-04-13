using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.Data;
using Projecttaskmanager.Models;
using Projecttaskmanager.Repositories;

namespace Projecttaskmanager.Services;

public class ProjectService(IProjectRepository repo) :IProjectService
{
    public async Task<List<Project>> GetAllProjectsAsync()
        => await repo.GetAllAsync();
    public async Task<Project?> GetProjectByIdAsync(int id)
    {
         var project = await repo.GetByIdAsync(id);
        if (project is null)
            throw new KeyNotFoundException($"Project with Id {id} was not found.");
        return project;
    }

    public  async Task<Project> AddProjectAsync(Project project)
         => await repo.AddAsync(project);

    public async Task<bool> UpdateProjectAsync(int id, Project project)
          => await repo.UpdateAsync(id, project);

    public async Task<bool> DeleteProjectAsync(int id)
    {
        var pro = await repo.GetWithTasksAndDepsAsync(id);
        if (pro == null)
            throw new KeyNotFoundException($"Project with Id {id} was not found.");

        await repo.DeleteAsync(pro);
        await repo.SaveChangesAsync();
        return true;
    }

}