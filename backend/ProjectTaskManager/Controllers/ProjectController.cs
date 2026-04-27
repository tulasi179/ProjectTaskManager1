using Microsoft.AspNetCore.Mvc;
using Projecttaskmanager.Models;
using Projecttaskmanager.Services;
using Microsoft.AspNetCore.Authorization;
using Projecttaskmanager.DTOs;

namespace Projecttaskmanager.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProjectController(IProjectService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Project>>> GetProject()
        => Ok(await service.GetAllProjectsAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(int id)
    {
        var project = await service.GetProjectByIdAsync(id); //404 error handled in service.
        return Ok(project);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProjectResponseDTOs>> CreateProject(ProjectRequestDTOs dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            OwnerId = dto.OwnerId,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
        var created = await service.AddProjectAsync(project);
        return Ok(new ProjectResponseDTOs
        {
            Id = created.Id,
            Name = created.Name,
            OwnerId = created.OwnerId,
            Description = created.Description, 
            StartDate = created.StartDate,
            EndDate = created.EndDate
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, ProjectRequestDTOs dto)
    {
        var project = new Project
        {
            Name = dto.Name,
            OwnerId = dto.OwnerId,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };
        await service.UpdateProjectAsync(id, project); //404 erro
        return Ok("Project updated successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        await service.DeleteProjectAsync(id); // throws 404 if not found
        return Ok("Project deleted successfully");
    }
}