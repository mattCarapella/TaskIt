using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public ErrorController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    [Route("/error/{statuscode}")]
    public IActionResult Index(int statuscode)
    {
        switch (statuscode)
        {
            case 404:
                return View("/Views/Shared/Errors/404.cshtml");
            case 500:
                return View("/Views/Shared/Errors/500.cshtml");
            default:
                return View("Error");
        }
    }

}

