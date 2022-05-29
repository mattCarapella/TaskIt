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
        var currentUser = await _unitOfWork.UserRepository.GetCurrentUser();

        // returns dictionary with keys ["USER_TICKETS", "DEADLINES"]
        var userTickets = await _unitOfWork.TicketRepository.GetTicketDetailsForUserDashboard(currentUser.Id);
        var ticketCounts = _unitOfWork.TicketRepository.GetTicketCounts(userTickets["USER_TICKETS"], currentUser.Id);
        var projectCount = _unitOfWork.ProjectRepository.GetProjectCountForUser(currentUser.Id);

        IEnumerable<TicketTypeGroup> typeData = from t in userTickets["USER_TICKETS"]
                                                where t.Status != Core.Enums.Enums.Status.COMPLETED
                                                group t by t.TicketType into grp
                                                select new TicketTypeGroup()
                                                {
                                                    Type = grp.Key,
                                                    TypeCount = grp.Count()
                                                };

        IEnumerable<TicketStatusGroup> statusData = from t in userTickets["USER_TICKETS"]
                                                    group t by t.Status into grp
                                                    select new TicketStatusGroup()
                                                    {
                                                        Status = grp.Key,
                                                        StatusCount = grp.Count()
                                                    };

        IEnumerable<TicketPriorityGroup> priorityData = from t in userTickets["USER_TICKETS"]
                                                        where t.Status != Core.Enums.Enums.Status.COMPLETED
                                                        group t by t.Priority into grp
                                                        select new TicketPriorityGroup()
                                                        {
                                                            Priority = grp.Key,
                                                            PriorityCount = grp.Count()
                                                        };

        var priorityCounts = new List<int>();
        var priorityKeys = new List<string>();
        var typeCounts = new List<int>();
        var typeKeys = new List<string>();

        foreach (var item in priorityData)
        {
            priorityCounts.Add(item.PriorityCount);
            priorityKeys.Add(item.Priority.ToString()!);
        }
        foreach (var item in typeData)
        {
            typeCounts.Add(item.TypeCount);
            typeKeys.Add(item.Type.ToString()!);
        }

        if (User.IsInRole(Core.Constants.Roles.Administrator))
        {
            var managerProjectCount = await _unitOfWork.ProjectRepository.GetProjectCountForManager(currentUser.Id);
            var allTickets = await _unitOfWork.TicketRepository.GetTicketDetailsForAdminDashboard(currentUser.Id);
            var allTicketsCount = await _unitOfWork.TicketRepository.GetTicketDetailsCountForAdminDashboard(currentUser.Id);

            var vm = new DashboardViewModel
            {
                CurrentUserName = currentUser.FirstName,
                ProjectCount = projectCount,
                OpenTicketCount = ticketCounts["OPEN"],
                ClosedTicketCount = ticketCounts["CLOSED"],
                UpcomingDeadlines = userTickets["DEADLINES"],
                TicketPriorityGroup = priorityData,
                TicketStatusGroup = statusData,
                TicketTypeGroup = typeData,
                PriorityCounts = priorityCounts,
                TypeCounts = typeCounts,
                PriorityKeys = priorityKeys,
                TypeKeys = typeKeys,
                ManagerProjectCount = managerProjectCount,
                TicketsToAssign = allTickets["TO_ASSIGN"],
                TicketsToReview = allTickets["TO_REVIEW"],
                AssignCount = allTicketsCount["TO_ASSIGN"],
                ReviewCount = allTicketsCount["TO_REVIEW"]
            };
            return View(vm);
        }
        if (User.IsInRole(Core.Constants.Roles.Manager))
        {
            var managerProjectCount = await _unitOfWork.ProjectRepository.GetProjectCountForManager(currentUser.Id);
            var managerTickets = await _unitOfWork.TicketRepository.GetTicketDetailsForManagerDashboard(currentUser.Id);
            var managerTicketCounts = await _unitOfWork.TicketRepository.GetTicketDetailsCountForManagerDashboard(currentUser.Id);

            var vm = new DashboardViewModel
            {
                CurrentUserName = currentUser.FirstName,
                ProjectCount = projectCount,
                OpenTicketCount = ticketCounts["OPEN"],
                ClosedTicketCount = ticketCounts["CLOSED"],
                UpcomingDeadlines = userTickets["DEADLINES"],
                TicketPriorityGroup = priorityData,
                TicketStatusGroup = statusData,
                TicketTypeGroup = typeData,
                PriorityCounts = priorityCounts,
                TypeCounts = typeCounts,
                PriorityKeys = priorityKeys,
                TypeKeys = typeKeys,
                ManagerProjectCount = managerProjectCount,
                TicketsToAssign = managerTickets["TO_ASSIGN"],
                TicketsToReview = managerTickets["TO_REVIEW"],
                AssignCount = managerTicketCounts["TO_ASSIGN"],
                ReviewCount = managerTicketCounts["TO_REVIEW"],
            };
            return View(vm);
        }
        else
        {
            var vm = new DashboardViewModel
            {
                CurrentUserName = currentUser.FirstName,
                ProjectCount = projectCount,
                OpenTicketCount = ticketCounts["OPEN"],
                ClosedTicketCount = ticketCounts["CLOSED"],
                UpcomingDeadlines = userTickets["DEADLINES"],
                TicketPriorityGroup = priorityData,
                TicketStatusGroup = statusData,
                TicketTypeGroup = typeData,
                PriorityCounts = priorityCounts,
                TypeCounts = typeCounts,
                PriorityKeys = priorityKeys,
                TypeKeys = typeKeys,

            };
            return View(vm);
        }
        
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