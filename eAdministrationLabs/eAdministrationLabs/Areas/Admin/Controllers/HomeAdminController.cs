using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using eAdministrationLabs.Models;
//using X.PagedList;
using Microsoft.AspNetCore.Authorization;


namespace eAdministrationLabs.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    ////[Authorize(Roles = "Admin, Manager, Technician, Staff")]
    [Authorize(Policy = "AdminOnly")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class HomeAdminController : Controller
    {


        private readonly EAdministrationLabsContext _context;

        public HomeAdminController(EAdministrationLabsContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            // L?y t?ng s? User
            int totalUsers = _context.Users.Count();

            // L?y t?ng s? l??ng Request ?ã nh?n
            int totalRequests = _context.Requests.Count();

            // L?y t?ng s? l??ng Request hoàn thành (tr?ng thái "Complete")
            int completedRequests = _context.HistoryRequests
                .Count(hr => hr.StatusRequest.StatusName == "Complete");

            // Truy?n d? li?u sang view
            ViewData["TotalUsers"] = totalUsers;
            ViewData["TotalRequests"] = totalRequests;
            ViewData["CompletedRequests"] = completedRequests;

            return View();
        }
    }
}
