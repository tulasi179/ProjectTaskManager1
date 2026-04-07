using Microsoft.AspNetCore.Mvc;
using Projecttaskmanager.Models;
using Projecttaskmanager.Services;
using Projecttaskmanager.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Projecttaskmanager.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TaskController(ITaskService service) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<ProjectTasks>>> GetTasks()
        => Ok(await service.GetAllTasksAsync());

    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectTasks>> GetTask(int id)
    {
      var task = await service.GetTasksByIdAsync(id);
       return Ok(task);
    }


    [Authorize(Roles = "User")]
    [HttpGet("my")]
    public async Task<ActionResult> GetMyTasks()
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var tasks = await service.GetTasksByUserIdAsync(currentUserId);
        return Ok(new { data = tasks });
    }
    // Both Admin and User can view tasks by project
    // but User only sees tasks assigned to them
    [HttpGet("project/{id}")]
    public async Task<ActionResult<List<ProjectTasks>>> GetTasksByProject(int id)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        //Get the logged-in user’s ID from the JWT token
        //It looks inside the JWT token and finds a specific claim -> User.FindFirstValue(ClaimTypes.NameIdentifier)
        //ClaimTypes.NameIdentifier  This is a standard claim type for: User ID
               var isAdmin = User.IsInRole("Admin");

        var tasks = await service.GetTasksByProjectId(id);

        // Filter to only assigned tasks if User role
        if (!isAdmin)
            tasks = tasks.Where(t => t.AssigneeId == currentUserId).ToList();

        if (!tasks.Any())
            return NotFound("No tasks found");

        return Ok(tasks);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
  public async Task<ActionResult<TaskResponseDto>> CreateTask(TaskRequestDto dto)
        {
            var task = new ProjectTasks
            {
                ProjectId = dto.ProjectId,
                Title = dto.Title,
                Description = dto.Description,
                AssigneeId = dto.AssigneeId
            };
            var created = await service.AddTasksAsync(task);
            return Ok(new TaskResponseDto
            {
                Id = created.Id,
                ProjectId = created.ProjectId,
                Title = created.Title,
                Description = created.Description,
                Status = created.Status,
                AssigneeId = created.AssigneeId
            });
        }

    // Admin can update everything, User can only update status of their own task
  
[HttpPatch("{id}/status")]
public async Task<IActionResult> UpdateTaskStatus(int id, TaskStatusUpdateDto dto)
{
    var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var isAdmin = User.IsInRole("Admin");

    var existing = await service.GetTasksByIdAsync(id); // throws 404 if not found

    if (!isAdmin && existing.AssigneeId != currentUserId)
        return Forbid();

    var blockingTasks = await service.GetBlockingTasksAsync(id);
    if (blockingTasks.Any(t => t.Status != "Completed"))
    {
        var pendingTitles = blockingTasks
            .Where(t => t.Status != "Completed")
            .Select(t => t.Title);
        return BadRequest($"Cannot update status. The following tasks must be completed first: {string.Join(", ", pendingTitles)}");
    }

    if (!isAdmin)
    {
        var allowedTransitions = new Dictionary<string, string>
        {
            { "Pending", "InProgress" },
            { "InProgress", "Completed" }
        };

        if (!allowedTransitions.TryGetValue(existing.Status, out var allowedNext)
            || dto.Status != allowedNext)
            return BadRequest($"Invalid transition. '{existing.Status}' can only move to '{allowedTransitions.GetValueOrDefault(existing.Status)}'.");
    }

    existing.Status = dto.Status;
    await service.UpdateTaskAsync(id, existing); // ← removed result check
    return Ok("Task status updated successfully");
}



[Authorize(Roles = "Admin")]
[HttpPut("{id}")]
public async Task<IActionResult> UpdateTask(int id, TaskRequestDto dto)
{
    var existing = await service.GetTasksByIdAsync(id);
    existing.Title = dto.Title;
    existing.Description = dto.Description;
    existing.AssigneeId = dto.AssigneeId;
    await service.UpdateTaskAsync(id, existing);
    return Ok("Task updated successfully");
}


    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
       await service.DeleteTaskAsync(id);
        return Ok("Task deleted successfully");
    }
}