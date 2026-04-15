using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Projecttaskmanager.Controllers;
using Projecttaskmanager.Services;
using Projecttaskmanager.Models;
using Projecttaskmanager.DTOs;
using ProjectTaskManager.MockData;


namespace ProjectTaskManager.Tests.Controllers
{
    public class ProjectControllerTests
    {
        private readonly ProjectController _controller;
        private readonly Mock<IProjectService> _mockService;

        public ProjectControllerTests()
        {
            _mockService = new Mock<IProjectService>();
            _controller = new ProjectController(_mockService.Object);
        }

        // Get all projects
        [Fact]
        public async Task GetProject_ReturnsOk()
        {
            var projects = ProjectMockData.GetProjects();

            _mockService.Setup(x => x.GetAllProjectsAsync())
                        .ReturnsAsync(projects);//returns fake data

            var result = await _controller.GetProject();//calls contoller methods

            Assert.IsType<OkObjectResult>(result.Result);//checks if the response is 200 ok
        }

        // Get project by id
        [Fact]
        public async Task GetProjectById_ReturnsOk()
        {
            var project = ProjectMockData.GetSingleProject(1);

            _mockService.Setup(x => x.GetProjectByIdAsync(1))
                        .ReturnsAsync(project);

            var result = await _controller.GetProject(1);

            Assert.IsType<OkObjectResult>(result.Result);
        }

        // Create project
        [Fact]
        public async Task CreateProject_ReturnsOk()
        {
            var dto = new ProjectRequestDTOs
            {
                Name = "Test Project",
                OwnerId = 1,
                Description = "Test Desc",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(10)
            };

            var project = new Project
            {
                Id = 1,
                Name = dto.Name,
                OwnerId = dto.OwnerId,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            _mockService.Setup(x => x.AddProjectAsync(It.IsAny<Project>()))
                        .ReturnsAsync(project);

            var result = await _controller.CreateProject(dto);

            Assert.IsType<OkObjectResult>(result.Result);
        }

        //  Delete project
        [Fact]
        public async Task DeleteProject_ReturnsOk()
        {
            _mockService.Setup(x => x.DeleteProjectAsync(1))
                        .ReturnsAsync(true);

            var result = await _controller.DeleteProject(1);

            Assert.IsType<OkObjectResult>(result);
        }
    }
}