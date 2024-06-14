using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ProductsSolution.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
