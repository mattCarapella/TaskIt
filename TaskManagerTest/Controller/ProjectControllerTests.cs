using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Controllers;
using TaskManager.Core.Repositories;
using TaskManager.Data;
using TaskManager.Models;
using Xunit;

namespace TaskManagerTest.Controller
{
    public class ProjectControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUOW;
        private readonly Mock<IProjectRepository> _mockRepo;
        private readonly Mock<TaskManagerContext> _mockContext;
        private readonly ProjectsController _controller;
        private readonly TaskManagerContext _context;

        public ProjectControllerTests()
        {
            _mockRepo = new Mock<IProjectRepository>();
            _mockUOW = new Mock<IUnitOfWork>();
            _controller = new ProjectsController(_context, _mockUOW.Object);
        }

        [Fact]
        public void Index_ActionExecutes_ReturnsViewForIndex()
        {
            var result = _controller.Index("", "", "", null);
            Assert.IsType<Task<IActionResult>>(result);
        }
        
        //[Fact]
        //public void Index_ActionExecutes_ReturnsCorrectNumberOfProjects()
        //{
        //    _mockUOW.Setup(uow => uow.ProjectRepository.GetProjects())
        //        .Returns(new List<Project>() { new Project(), new Project });
        //}
    }
}
