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
using Xunit;

namespace TaskManagerTest.Controller
{
    public class PNoteControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUOW;
        private readonly Mock<IProjectRepository> _mockRepo;
        private readonly Mock<TaskManagerContext> _mockContext;
        private readonly PNotesController _controller;
        private readonly TaskManagerContext _context;
        public PNoteControllerTests()
        {
            _mockRepo = new Mock<IProjectRepository>();
            _mockUOW = new Mock<IUnitOfWork>();
            _controller = new PNotesController(_context);
        }

        [Fact]
        public void Index_ActionExecutes_ReturnsViewForIndex()
        {
            var result = _controller.Index();
            Assert.IsType<Task<IActionResult>>(result);
        }
    }
}
