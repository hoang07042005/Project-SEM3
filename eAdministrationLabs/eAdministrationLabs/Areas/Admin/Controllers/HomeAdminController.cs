using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using eAdministrationLabs.Models; 
using Microsoft.AspNetCore.Authorization;
using System;

namespace eAdministrationLabs.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
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
            // Lấy tổng số User
            int totalUsers = _context.Users.Count();

           
            int totalRequests = _context.Requests.Count();

          
            int completedRequests = _context.HistoryRequests
                .Count(hr => hr.StatusRequest.StatusName == "Complete");

            
            ViewData["TotalUsers"] = totalUsers;
            ViewData["TotalRequests"] = totalRequests;
            ViewData["CompletedRequests"] = completedRequests;

            return View();
        }
    }
}
