using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eAdministrationLabs.Models;
using Microsoft.AspNetCore.Authorization;

namespace eAdministrationLabs.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/historyrequest")]
    ////[Authorize(Roles = "Admin, Manager, Technician, Staff")]
    [Authorize(Policy = "AdminOnly")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class HistoryRequestController : Controller
    {
        private readonly EAdministrationLabsContext _context;

        public HistoryRequestController(EAdministrationLabsContext context)
        {
            _context = context;
        }

        // GET: Admin/HistoryRequest
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = _context.HistoryRequests.Include(h => h.Request).Include(h => h.StatusRequest).Include(h => h.User);
            return View(await eAdministrationLabsContext.ToListAsync());
        }

        // GET: Admin/HistoryRequest/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historyRequest = await _context.HistoryRequests
                .Include(h => h.Request)
                .Include(h => h.StatusRequest)
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historyRequest == null)
            {
                return NotFound();
            }

            return View(historyRequest);
        }

        // GET: Admin/HistoryRequest/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Id");
            ViewData["StatusRequestId"] = new SelectList(_context.StatusRequests, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Admin/HistoryRequest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,RequestId,StatusRequestId,ChangedBy,ChangedAt,Notes")] HistoryRequest historyRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(historyRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Id", historyRequest.RequestId);
            ViewData["StatusRequestId"] = new SelectList(_context.StatusRequests, "Id", "Id", historyRequest.StatusRequestId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", historyRequest.UserId);
            return View(historyRequest);
        }

        // GET: Admin/HistoryRequest/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historyRequest = await _context.HistoryRequests.FindAsync(id);
            if (historyRequest == null)
            {
                return NotFound();
            }
            ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Id", historyRequest.RequestId);
            ViewData["StatusRequestId"] = new SelectList(_context.StatusRequests, "Id", "Id", historyRequest.StatusRequestId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", historyRequest.UserId);
            return View(historyRequest);
        }

        // POST: Admin/HistoryRequest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,RequestId,StatusRequestId,ChangedBy,ChangedAt,Notes")] HistoryRequest historyRequest)
        {
            if (id != historyRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(historyRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoryRequestExists(historyRequest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Id", historyRequest.RequestId);
            ViewData["StatusRequestId"] = new SelectList(_context.StatusRequests, "Id", "Id", historyRequest.StatusRequestId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", historyRequest.UserId);
            return View(historyRequest);
        }

        // GET: Admin/HistoryRequest/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historyRequest = await _context.HistoryRequests
                .Include(h => h.Request)
                .Include(h => h.StatusRequest)
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historyRequest == null)
            {
                return NotFound();
            }

            return View(historyRequest);
        }

        // POST: Admin/HistoryRequest/Delete/5
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var historyRequest = await _context.HistoryRequests.FindAsync(id);
            if (historyRequest != null)
            {
                _context.HistoryRequests.Remove(historyRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoryRequestExists(int id)
        {
            return _context.HistoryRequests.Any(e => e.Id == id);
        }
    }
}
