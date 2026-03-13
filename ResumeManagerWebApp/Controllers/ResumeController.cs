using Microsoft.AspNetCore.Mvc;
using ResumeManagerWebApp.Services;

namespace ResumeManagerWebApp.Controllers
{
    public class ResumeController : Controller
    {
        private readonly ResumeApiService _apiService;

        public ResumeController(ResumeApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var resumeList = await _apiService.GetAllResumeAsync();
            return View(resumeList);
        }
    }
}
