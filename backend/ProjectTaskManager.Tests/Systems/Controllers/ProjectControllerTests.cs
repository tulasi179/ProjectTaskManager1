using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Projecttaskmanager.Controllers;
using Projecttaskmanager.Services;
using Projecttaskmanager.Models;
using Projecttaskmanager.DTOs;
using ProjectTaskManager.MockData;

namespace ProjectTaskManager.Tests.Systems.Controllers;

public class ProjectControllerTests
{
    // ─────────────────────────────────────────────
    // GET /project
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetProjects_ShouldReturn200_WithProjectList()
    {
        // Arrange
        var projectService = new Mock<IProjectService>();
        projectService.Setup(s => s.GetAllProjectsAsync()).ReturnsAsync(ProjectMockData.GetProjects());
        var controller = new ProjectController(projectService.Object);

        // Act
        var result = await controller.GetProject();

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().BeAssignableTo<List<Project>>()
                .Which.Should().HaveCount(3);
    }

    // ─────────────────────────────────────────────
    // GET /project/{id}
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetProjectById_ShouldReturn200_WithProject()
    {
        // Arrange
        var projectService = new Mock<IProjectService>();
        var fakeProject = ProjectMockData.GetSingleProject(1);
        projectService.Setup(s => s.GetProjectByIdAsync(1)).ReturnsAsync(fakeProject);
        var controller = new ProjectController(projectService.Object);

        // Act
        var result = await controller.GetProject(1);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().BeOfType<Project>()
                .Which.Name.Should().Be("TaskManager");
    }

    // ─────────────────────────────────────────────
    // POST /project (Admin only)
    // ─────────────────────────────────────────────

    [Fact]
    public async Task CreateProject_ShouldReturn200_WithProjectResponse()
    {
        // Arrange
        var projectService = new Mock<IProjectService>();
        var dto = new ProjectRequestDTOs
        {
            Name        = "New Project",
            OwnerId     = 1,
            Description = "Test description",
            StartDate   = new DateTime(2026, 1, 1),
            EndDate     = new DateTime(2026, 6, 1)
        };
        var created = new Project
        {
            Id          = 10,
            Name        = dto.Name,
            OwnerId     = dto.OwnerId,
            Description = dto.Description,
            StartDate   = dto.StartDate,
            EndDate     = dto.EndDate
        };
        projectService.Setup(s => s.AddProjectAsync(It.IsAny<Project>())).ReturnsAsync(created);
        var controller = new ProjectController(projectService.Object);

        // Act
        var result = await controller.CreateProject(dto);

        // Assert
        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        var response = ok.Value.Should().BeOfType<ProjectResponseDTOs>().Subject;
        response.Name.Should().Be("New Project");
        response.Id.Should().Be(10);
    }

    // ─────────────────────────────────────────────
    // PUT /project/{id} (Admin only)
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateProject_ShouldReturn200_WithSuccessMessage()
    {
        // Arrange
        var projectService = new Mock<IProjectService>();
        var dto = new ProjectRequestDTOs
        {
            Name        = "Updated Project",
            OwnerId     = 1,
            Description = "Updated description",
            StartDate   = new DateTime(2026, 1, 1),
            EndDate     = new DateTime(2026, 6, 1)
        };
        projectService.Setup(s => s.UpdateProjectAsync(1, It.IsAny<Project>())).ReturnsAsync(true);
        var controller = new ProjectController(projectService.Object);

        // Act
        var result = await controller.UpdateProject(1, dto);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().Be("Project updated successfully");
    }

    // ─────────────────────────────────────────────
    // DELETE /project/{id} (Admin only)
    // ─────────────────────────────────────────────

    [Fact]
    public async Task DeleteProject_ShouldReturn200_WithSuccessMessage()
    {
        // Arrange
        var projectService = new Mock<IProjectService>();
        projectService.Setup(s => s.DeleteProjectAsync(1)).ReturnsAsync(true);
        var controller = new ProjectController(projectService.Object);

        // Act
        var result = await controller.DeleteProject(1);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
        ok.Value.Should().Be("Project deleted successfully");
    }
}