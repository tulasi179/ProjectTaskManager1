using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Projecttaskmanager.Models;
using Projecttaskmanager.DTOs;
using Projecttaskmanager.Services;

namespace Projecttaskmanager.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class TaskDependencyController(ITaskDependencyService service) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<List<TaskDependency>>> GetDependencies()
        => Ok(await service.GetDependencies());


    [HttpGet("{taskId}/dependents")]
    public async Task<ActionResult<List<TaskDependency>>> GetDependentTasksById(int taskId)
        => Ok(await service.GetDependentTasksById(taskId));


    [HttpPost] 
    public async Task<ActionResult<TaskDependency>> AddDependency(TaskDependencyRequestDto dto)
    {
        var dependency = new TaskDependency
        {
            TaskId = dto.TaskId,
            DependentTaskId = dto.DependentTaskId
        };
        var (success, message, data) = await service.AddDependency(dependency);
        if (!success)
            return BadRequest(new { message });
        return CreatedAtAction(nameof(GetDependencies), new { taskId = data!.TaskId }, data);
    }

    [HttpDelete("{taskId}/{dependentTaskId}")]
    public async Task<IActionResult> RemoveDependency(int taskId, int dependentTaskId)
    {
        var result = await service.RemoveDependency(taskId, dependentTaskId);
        if (!result)
            return NotFound("Dependency not found");

        return Ok("Dependency removed successfully");
    }
}