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

namespace TaskManager.Controllers;

public class FileModelsController : Controller
{
    private readonly TaskManagerContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public FileModelsController(TaskManagerContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<IActionResult> UploadFile(List<IFormFile> files, string description, Guid ticketId)
    {
        var currentUserId = User.Identity.GetUserId();
        if (currentUserId == null)
        {
            return NotFound();
        }
        var ticket = await _unitOfWork.TicketRepository.GetTicket(ticketId);
        if (ticket is null)
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

            var fileModel = new FileModel
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
            _context.Files.Add(fileModel);
            ticket.TicketFiles.Add(fileModel);
            await _unitOfWork.SaveAsync();
        }
        TempData["Message"] = "File successfully uploaded";
        return RedirectToAction("Details", "Tickets", new { id = ticket.TicketId });
    }


    public async Task<IActionResult> Index()
    {
        var taskManagerContext = _context.Files.Include(f => f.Ticket);
        return View(await taskManagerContext.ToListAsync());
    }




    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null || _context.Files == null)
        {
            return NotFound();
        }

        var fileModel = await _context.Files
            .Include(f => f.Ticket)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (fileModel == null)
        {
            return NotFound();
        }

        return View(fileModel);
    }



    public async Task<IActionResult> DownloadFile(Guid? id)
    {
        var file = await _context.Files.FindAsync(id);
        if (file is null)
        {
            return NotFound();
        }
        return File(file.Data, file.FileType, file.Name + file.Extension);
    }


    public async Task<IActionResult> DeleteFileFromDb(Guid id)
    {
        var file = await _context.Files.FindAsync(id);
        if (file is null)
        {
            return NotFound();
        }
        try
        {
            _context.Files.Remove(file);
            await _unitOfWork.SaveAsync();
            return RedirectToAction("Details", "Tickets", new { id = file.TicketId });
        }
        catch (DbUpdateException)
        {
            return RedirectToAction("Details", "Tickets", new { id = file.TicketId });
        }
    }

}
