using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TaskManager.Core;

namespace TaskManager.Controllers;

public class RoleController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Policy = Constants.Policies.RequireAdmin)]
    public IActionResult Admin()
    {
        return View();
    }

    [Authorize(Policy = Constants.Policies.RequireManager)]
    //[Authorize(Roles = $"{Constant.Roles.Administrator},{Constants.Roles.Manager}")]
    public IActionResult Manager()
    {
        return View();
    }

}

