using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Projecttaskmanager.Controllers;
using Projecttaskmanager.Services;
using Projecttaskmanager.DTOs;
using Projecttaskmanager.Models;
using ProjectTaskManager.MockData;

namespace ProjectTaskManager.Tests.Systems.Controllers;

public class TaskControllerTests
{
    //inject fake logged-in user 
    private static void SetUser(TaskController controller, int userId, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    // get tasks (Admin only)
    [Fact]
    public async Task GetTasks_ShouldReturn200_WithTaskList()
    {
        // Arrange
        var taskService = new Mock<ITaskService>();
        taskService.Setup(s => s.GetAllTasksAsync()).ReturnsAsync(TaskMockData.GetTasks());
        var controller = new TaskController(taskService.Object);

        // Act
        var result = await controller.GetTasks();

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().BeAssignableTo<List<ProjectTasks>>()
                .Which.Should().HaveCount(5);
    }
    // get task with id (Admin only)
    [Fact]
    public async Task GetTask_ShouldReturn200_WithTask()
    {
        // Arrange
        var taskService = new Mock<ITaskService>();
        var fakeTask = TaskMockData.GetSingleTask(1);
        taskService.Setup(s => s.GetTasksByIdAsync(1)).ReturnsAsync(fakeTask);
        var controller = new TaskController(taskService.Object);

        // Act
        var result = await controller.GetTask(1);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().BeOfType<ProjectTasks>()
                .Which.Title.Should().Be(fakeTask.Title);
    }
    //get tasks with id or current id (User only)
    [Fact]
    public async Task GetMyTasks_ShouldReturn200_WithCurrentUserTasks()
    {
        // Arrange
        var taskService = new Mock<ITaskService>();
        var userTasks = TaskMockData.GetTasksByUser(2); // tasks assigned to user 2
        taskService.Setup(s => s.GetTasksByUserIdAsync(2)).ReturnsAsync(userTasks);
        var controller = new TaskController(taskService.Object);
        SetUser(controller, 2, "User"); // logged in as user 2

        // Act
        var result = await controller.GetMyTasks();

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
    }

    // get all tasks with project id - Admin sees all tasks

    [Fact]
    public async Task GetTasksByProject_AsAdmin_ShouldReturn200_WithAllTasks()
    {
        // Arrange
        var taskService = new Mock<ITaskService>();
        var projectTasks = TaskMockData.GetTasksByProject(1); // 3 tasks in project 1
        taskService.Setup(s => s.GetTasksByProjectId(1)).ReturnsAsync(projectTasks);
        var controller = new TaskController(taskService.Object);
        SetUser(controller, 1, "Admin");

        // Act
        var result = await controller.GetTasksByProject(1);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().BeAssignableTo<List<ProjectTasks>>()
                .Which.Should().HaveCount(3); // all 3 tasks in project 1
    }

    // ─────────────────────────────────────────────
    // GET /task/project/{id} - User sees only assigned tasks
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetTasksByProject_AsUser_ShouldReturn200_WithOnlyAssignedTasks()
    {
        // Arrange
        var taskService = new Mock<ITaskService>();
        var projectTasks = TaskMockData.GetTasksByProject(1); // returns tasks for project 1
        taskService.Setup(s => s.GetTasksByProjectId(1)).ReturnsAsync(projectTasks);
        var controller = new TaskController(taskService.Object);
        SetUser(controller, 2, "User"); // user 2 — only sees their own tasks

        // Act
        var result = await controller.GetTasksByProject(1);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<List<ProjectTasks>>()
                .Which.Should().OnlyContain(t => t.AssigneeId == 2);
    }

    // ─────────────────────────────────────────────
    // GET /task/project/{id} - No tasks found
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetTasksByProject_NoTasks_ShouldReturn404()
    {
        // Arrange
        var taskService = new Mock<ITaskService>();
        taskService.Setup(s => s.GetTasksByProjectId(99)).ReturnsAsync(new List<ProjectTasks>());
        var controller = new TaskController(taskService.Object);
        SetUser(controller, 1, "Admin");

        // Act
        var result = await controller.GetTasksByProject(99);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>()
              .Which.Value.Should().Be("No tasks found");
    }

    // ─────────────────────────────────────────────
    // POST /task (Admin only)
    // ─────────────────────────────────────────────

    [Fact]
    public async Task CreateTask_ShouldReturn200_WithTaskResponse()
    {
        // Arrange
        var taskService = new Mock<ITaskService>();
        var dto = new TaskRequestDto
        {
            ProjectId   = 1,
            Title       = "New Task",
            Description = "Do something",
            AssigneeId  = 2
        };
        var created = new ProjectTasks
        {
            Id          = 10,
            ProjectId   = dto.ProjectId,
            Title       = dto.Title,
            Description = dto.Description,
            Status      = "Pending",
            AssigneeId  = dto.AssigneeId
        };
        taskService.Setup(s => s.AddTasksAsync(It.IsAny<ProjectTasks>())).ReturnsAsync(created);
        var controller = new TaskController(taskService.Object);

        // Act
        var result = await controller.CreateTask(dto);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        var response = ok.Value.Should().BeOfType<TaskResponseDto>().Subject;
        response.Title.Should().Be("New Task");
        response.Id.Should().Be(10);
        response.Status.Should().Be("Pending");
    }

    // ─────────────────────────────────────────────
    // PATCH /task/{id}/status - User valid transition
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateTaskStatus_AsUser_ValidTransition_ShouldReturn200()
    {
        // Arrange
        var taskService = new Mock<ITaskService>();
        var existing = new ProjectTasks { Id = 1, Status = "Pending", AssigneeId = 2 };
        taskService.Setup(s => s.GetTasksByIdAsync(1)).ReturnsAsync(existing);
        taskService.Setup(s => s.GetBlockingTasksAsync(1)).ReturnsAsync(new List<ProjectTasks>());
        taskService.Setup(s => s.UpdateTaskAsync(1, It.IsAny<ProjectTasks>())).ReturnsAsync(true);
        var controller = new TaskController(taskService.Object);
        SetUser(controller, 2, "User"); // user 2 — assigned to this task

        // Act
        var result = await controller.UpdateTaskStatus(1, new TaskStatusUpdateDto { Status = "InProgress" });

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be("Task status updated successfully");
    }

    // ─────────────────────────────────────────────
    // PATCH /task/{id}/status - User not assigned → 403
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateTaskStatus_AsUser_NotAssigned_ShouldReturn403()
    {
        // Arrange
        var taskService = new Mock<ITaskService>();
        var existing = new ProjectTasks { Id = 1, Status = "Pending", AssigneeId = 3 }; // assigned to user 3
        taskService.Setup(s => s.GetTasksByIdAsync(1)).ReturnsAsync(existing);
        taskService.Setup(s => s.GetBlockingTasksAsync(1)).ReturnsAsync(new List<ProjectTasks>());
        var controller = new TaskController(taskService.Object);
        SetUser(controller, 2, "User"); // user 2 trying to update user 3's task

        // Act
        var result = await controller.UpdateTaskStatus(1, new TaskStatusUpdateDto { Status = "InProgress" });

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }

    // ─────────────────────────────────────────────
    // PATCH /task/{id}/status - Blocked by another task → 400
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateTaskStatus_WithBlockingTasks_ShouldReturn400()
    {
        // Arrange
        var taskService = new Mock<ITaskService>();
        var existing = new ProjectTasks { Id = 1, Status = "Pending", AssigneeId = 2 };
        var blockingTask = new ProjectTasks { Id = 2, Title = "Blocking Task", Status = "Pending" };
        taskService.Setup(s => s.GetTasksByIdAsync(1)).ReturnsAsync(existing);
        taskService.Setup(s => s.GetBlockingTasksAsync(1)).ReturnsAsync(new List<ProjectTasks> { blockingTask });
        var controller = new TaskController(taskService.Object);
        SetUser(controller, 2, "Admin");

        // Act
        var result = await controller.UpdateTaskStatus(1, new TaskStatusUpdateDto { Status = "InProgress" });

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
              .Which.Value.Should().Be("Cannot update status. The following tasks must be completed first: Blocking Task");
    }

    // ─────────────────────────────────────────────
    // PATCH /task/{id}/status - Invalid transition → 400
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateTaskStatus_AsUser_InvalidTransition_ShouldReturn400()
    {
        // Arrange
        var taskService = new Mock<ITaskService>();
        var existing = new ProjectTasks { Id = 1, Status = "Pending", AssigneeId = 2 };
        taskService.Setup(s => s.GetTasksByIdAsync(1)).ReturnsAsync(existing);
        taskService.Setup(s => s.GetBlockingTasksAsync(1)).ReturnsAsync(new List<ProjectTasks>());
        var controller = new TaskController(taskService.Object);
        SetUser(controller, 2, "User");

        // Act — Pending → Completed is invalid, must go Pending → InProgress first
        var result = await controller.UpdateTaskStatus(1, new TaskStatusUpdateDto { Status = "Completed" });

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // ─────────────────────────────────────────────
    // PUT /task/{id} (Admin only)
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateTask_ShouldReturn200_WithSuccessMessage()
    {
        // Arrange
        var taskService = new Mock<ITaskService>();
        var existing = TaskMockData.GetSingleTask(1);
        taskService.Setup(s => s.GetTasksByIdAsync(1)).ReturnsAsync(existing);
        taskService.Setup(s => s.UpdateTaskAsync(1, It.IsAny<ProjectTasks>())).ReturnsAsync(true);
        var controller = new TaskController(taskService.Object);
        var dto = new TaskRequestDto { Title = "Updated Title", Description = "Updated", AssigneeId = 3 };

        // Act
        var result = await controller.UpdateTask(1, dto);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be("Task updated successfully");
    }

    // ─────────────────────────────────────────────
    // DELETE /task/{id} (Admin only)
    // ─────────────────────────────────────────────

    [Fact]
    public async Task DeleteTask_ShouldReturn200_WithSuccessMessage()
    {
        // Arrange
        var taskService = new Mock<ITaskService>();
        taskService.Setup(s => s.DeleteTaskAsync(1)).ReturnsAsync(true);
        var controller = new TaskController(taskService.Object);

        // Act
        var result = await controller.DeleteTask(1);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be("Task deleted successfully");
    }
}