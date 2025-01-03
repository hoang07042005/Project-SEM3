using eAdministrationLabs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;
using X.PagedList.Extensions;

namespace eAdministrationLabs.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EAdministrationLabsContext _context;

        public HomeController(ILogger<HomeController> logger, EAdministrationLabsContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? page)
        {
            int pageSize = 4;
            int pageNumber = page ?? 1;
            int totalLabs = _context.Labs.Count();
            int totalTechnicalStaff = _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "technicalstaff"))
                .Count();

            ViewData["TotalLab"] = totalLabs;
            ViewData["TotalUsers"] = totalTechnicalStaff;

            var pagedFeedbacks = _context.Feedbacks
                                         .Include(f => f.User)
                                         .OrderBy(f => f.Id)
                                         .ToPagedList(pageNumber, pageSize);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_FeedbackListPartial", pagedFeedbacks);
            }

            return View(pagedFeedbacks);
        }



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
}
