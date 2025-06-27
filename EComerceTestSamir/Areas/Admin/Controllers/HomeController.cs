using Microsoft.AspNetCore.Mvc;

namespace EComerceTestSamir.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        } 
        public IActionResult NotFoundPage()
        {
            return View();
        }
    }
}
