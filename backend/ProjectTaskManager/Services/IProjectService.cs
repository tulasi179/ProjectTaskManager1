using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Services
{
    public interface IProjectService
    {
        Task<List<Project>> GetAllProjectsAsync();
        Task<Project?> GetProjectByIdAsync(int Id);
        Task<Project> AddProjectAsync(Project project);
        Task<bool> UpdateProjectAsync(int id, Project project);
        Task<bool> DeleteProjectAsync(int id);

    }
}