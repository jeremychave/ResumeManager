using Microsoft.AspNetCore.Mvc;

namespace ResumeManagerWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
