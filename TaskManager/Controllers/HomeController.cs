using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TaskManager.Areas.Identity.Data;
using TaskManager.Core.Repositories;
using TaskManager.Core.ViewModels;
using TaskManager.Core.ViewModels.Dashboard;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TaskManagerContext _context;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, TaskManagerContext context)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IActionResult> Index(int? id=null)
    {
        var currentUserId = User.Identity.GetUserId();
        var ticketAssignmentList = await _unitOfWork.TicketAssignmentRepository.GetTicketAssignmentsForUser(currentUserId);
        var userTickets = from t in ticketAssignmentList select t.Ticket;

        //var priorityGroups = (from t in userTickets
        //                      where t.Status != Core.Enums.Enums.Status.COMPLETED
        //                      group t by t.Priority into priorityGroup
        //                      select new
        //                      {
        //                          Priority = priorityGroup.Key,
        //                          PriorityCount = priorityGroup.Count()
        //                      }).ToList();


        //var typeGroups = (from t in userTickets
        //                  where t.Status != Core.Enums.Enums.Status.COMPLETED
        //                  group t by t.TicketType into typeGroup
        //                  select new
        //                  {
        //                      Type = typeGroup.Key,
        //                      TypeCount = typeGroup.Count()
        //                  }).ToList();


        IEnumerable<TicketTypeGroup> typeData = from t in userTickets
                                                where t.Status != Core.Enums.Enums.Status.COMPLETED
                                                group t by t.TicketType into grp
                                                select new TicketTypeGroup()
                                                {
                                                    Type = grp.Key,
                                                    TypeCount = grp.Count()
                                                };

        IEnumerable<TicketStatusGroup> statusData = from t in userTickets
                                                    group t by t.Status into grp
                                                    select new TicketStatusGroup()
                                                    {
                                                        Status = grp.Key,
                                                        StatusCount = grp.Count()
                                                    };

        IEnumerable<TicketPriorityGroup> priorityData = from t in userTickets
                                                        where t.Status != Core.Enums.Enums.Status.COMPLETED
                                                        group t by t.Priority into grp
                                                        select new TicketPriorityGroup()
                                                        {
                                                            Priority = grp.Key,
                                                            PriorityCount = grp.Count()
                                                        };

        var priorityCounts = new List<int>();
        var typeCounts = new List<int>();
        foreach (var item in priorityData)
        {
            priorityCounts.Add(item.PriorityCount);
        }
        foreach (var item in typeData)
        {
            typeCounts.Add(item.TypeCount);
        }

        var upcomingDeadlines = await _unitOfWork.TicketAssignmentRepository.GetUpcomingTicketDeadlines(currentUserId);
        var projectCount = _unitOfWork.ProjectAssignmentRepository.GetUserProjectCount(currentUserId);
        var openTicketCount = _unitOfWork.TicketAssignmentRepository.GetUserOpenTicketCount(currentUserId);
        var closedTicketCount = _unitOfWork.TicketAssignmentRepository.GetUserClosedTicketCount(currentUserId);
        var user = await _unitOfWork.UserRepository.GetCurrentUser();

        var managerProjects = new List<Project>();
        var managerTickets = new List<Ticket>();
        var toAssign = new List<Ticket>();
        var toReview = new List<Ticket>();

        if (User.IsInRole(Core.Constants.Roles.Manager)) {
            managerProjects = await _unitOfWork.ProjectRepository.GetProjectsForManager(currentUserId);
            managerTickets = await _unitOfWork.TicketRepository.GetTicketsForManagersProjects(currentUserId);
            toAssign = managerTickets.Where(t => t.Status == Core.Enums.Enums.Status.TODO).ToList();
            toReview = managerTickets.Where(t => t.Status == Core.Enums.Enums.Status.SUBMITTED).ToList();
        }

        var vm = new DashboardViewModel
        {
            ProjectCount = projectCount,
            OpenTicketCount = openTicketCount,
            ClosedTicketCount = closedTicketCount,
            UpcomingDeadlines = upcomingDeadlines,
            TicketPriorityGroup = priorityData,
            TicketStatusGroup = statusData,
            TicketTypeGroup = typeData,
            CurrentUserName = user.FirstName,
            PriorityCounts = priorityCounts,
            TypeCounts = typeCounts,
            ManagerProjects = managerProjects,
            ManagerTicketsToAssign = toAssign,
            ManagerTicketsToReview = toReview
        };

        return View(vm);



        //var priorityStatusGroups = (from t in userTickets
        //                          group t by new { t.Priority, t.Status } into priorityGroup
        //                          select new
        //                          {
        //                              Priority = priorityGroup.Key.Priority,
        //                              Status = priorityGroup.Key.Status,
        //                              PriorityCount = priorityGroup.Count()
        //                          }).ToList();

        //var highCount = priorityGroups[0].PriorityCount;
        //var lowCount = priorityGroups[1].PriorityCount;
        //var medCount = priorityGroups[2].PriorityCount;
        //var featureCount = typeGroups[0].TypeCount;
        //var bugCount = typeGroups[1].TypeCount;
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
