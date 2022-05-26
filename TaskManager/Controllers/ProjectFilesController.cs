using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Repositories;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager
{
    public class ProjectFilesController : Controller
    {
        private readonly TaskManagerContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public ProjectFilesController(TaskManagerContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }


        [HttpPost]
        public async Task<IActionResult> UploadFile(List<IFormFile> files, string description, Guid projectId)
        {
            var currentUserId = User.Identity.GetUserId();
            if (currentUserId == null)
            {
                return NotFound();
            }
            var project = await _unitOfWork.ProjectRepository.GetProject(projectId);
            if (project is null)
            {
                return NotFound();
            }

            foreach (var file in files)
            {
                var filename = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                var size = file.Length;

                if (size > 2097152)
                {
                    return Problem("File too large.");
                }

                var fileModel = new ProjectFile
                {
                    CreatedAt = DateTime.Now,
                    FileType = file.ContentType,
                    Extension = extension,
                    Name = filename,
                    Description = description,
                    FileSize = size,
                    UploadedByUserId = currentUserId
                };
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    fileModel.Data = dataStream.ToArray();
                }
                _context.ProjectFiles.Add(fileModel);
                project.ProjectFiles.Add(fileModel);
                await _unitOfWork.SaveAsync();
            }
            TempData["Message"] = "File successfully uploaded";
            return RedirectToAction("Details", "Projects", new { id = project.Id });
        }


        public async Task<IActionResult> DownloadFile(Guid? id)
        {
            var file = await _context.ProjectFiles.FindAsync(id);
            if (file is null)
            {
                return NotFound();
            }
            return File(file.Data, file.FileType, file.Name + file.Extension);
        }


        public async Task<IActionResult> DeleteFileFromDb(Guid id)
        {
            var file = await _context.ProjectFiles.FindAsync(id);
            if (file is null)
            {
                return NotFound();
            }
            try
            {
                _context.ProjectFiles.Remove(file);
                await _unitOfWork.SaveAsync();
                return RedirectToAction("Details", "Projects", new { id = file.ProjectId });
            }
            catch (DbUpdateException)
            {
                return RedirectToAction("Details", "Projects", new { id = file.ProjectId });
            }
        }





        // GET: ProjectFiles
        public async Task<IActionResult> Index()
        {
            var taskManagerContext = _context.ProjectFiles.Include(p => p.Project).Include(p => p.UploadedByUser);
            return View(await taskManagerContext.ToListAsync());
        }

    }
}
