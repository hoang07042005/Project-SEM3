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
    [Route("admin/labusagelog")]
    ////[Authorize(Roles = "Admin, Manager, Technician, Staff")]
    [Authorize(Policy = "AdminOnly")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class LabUsageLogController : Controller
    {
        private readonly EAdministrationLabsContext _context;

        public LabUsageLogController(EAdministrationLabsContext context)
        {
            _context = context;
        }

        // GET: Admin/LabUsageLog
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = _context.LabUsageLogs.Include(l => l.Lab).Include(l => l.User);
            return View(await eAdministrationLabsContext.ToListAsync());
        }

        // GET: Admin/LabUsageLog/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labUsageLog = await _context.LabUsageLogs
                .Include(l => l.Lab)
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (labUsageLog == null)
            {
                return NotFound();
            }

            return View(labUsageLog);
        }

        // GET: Admin/LabUsageLog/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Admin/LabUsageLog/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LabId,UserId,Purpose,StartTime,EndTime,CreatedAt")] LabUsageLog labUsageLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(labUsageLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Id", labUsageLog.LabId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", labUsageLog.UserId);
            return View(labUsageLog);
        }

        // GET: Admin/LabUsageLog/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labUsageLog = await _context.LabUsageLogs.FindAsync(id);
            if (labUsageLog == null)
            {
                return NotFound();
            }
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Id", labUsageLog.LabId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", labUsageLog.UserId);
            return View(labUsageLog);
        }

        // POST: Admin/LabUsageLog/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LabId,UserId,Purpose,StartTime,EndTime,CreatedAt")] LabUsageLog labUsageLog)
        {
            if (id != labUsageLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(labUsageLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LabUsageLogExists(labUsageLog.Id))
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
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Id", labUsageLog.LabId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", labUsageLog.UserId);
            return View(labUsageLog);
        }

        // GET: Admin/LabUsageLog/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labUsageLog = await _context.LabUsageLogs
                .Include(l => l.Lab)
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (labUsageLog == null)
            {
                return NotFound();
            }

            return View(labUsageLog);
        }

        // POST: Admin/LabUsageLog/Delete/5
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var labUsageLog = await _context.LabUsageLogs.FindAsync(id);
            if (labUsageLog != null)
            {
                _context.LabUsageLogs.Remove(labUsageLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LabUsageLogExists(int id)
        {
            return _context.LabUsageLogs.Any(e => e.Id == id);
        }
    }
}
