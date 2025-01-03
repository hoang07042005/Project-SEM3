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
    [Route("admin/LabUsageLog")]
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
        public async Task<IActionResult> Index(int? statusFilter)
        {
            ViewBag.StatusOptions = _context.StatusLogs.ToList();
            IQueryable<LabUsageLog> eAdministrationLabsContext = _context.LabUsageLogs.Include(l => l.Lab).Include(l => l.StatusLog).Include(l => l.User);

            if (!statusFilter.HasValue)
            {
                statusFilter = 1; // 1 là ID của trạng thái Pending
            }

            // Lọc theo trạng thái
            eAdministrationLabsContext = eAdministrationLabsContext.Where(h => h.StatusLogId == statusFilter);

            eAdministrationLabsContext = eAdministrationLabsContext
                .OrderBy(h => h.StatusLogId != 1);

            ViewBag.StatusFilter = statusFilter;

            return View(await eAdministrationLabsContext.ToListAsync());
        }


        [HttpPost]
        public JsonResult UpdateStatus(int id, int statusId)
        {
            // Find the LabUsageLog by its ID
            var labUsageLog = _context.LabUsageLogs
                .FirstOrDefault(l => l.Id == id);

            if (labUsageLog != null)
            {
                // Update the status of the LabUsageLog
                labUsageLog.StatusLogId = statusId;

                // Check if the status is "Approve" and update the related Lab's status
                if (statusId == 2 )
                {
                    // Find the related Lab
                    var lab = _context.Labs.FirstOrDefault(l => l.Id == labUsageLog.LabId);

                    if (lab != null)
                    {
                        // Find the "Active" status from the StatusLab table
                        var activeStatus = _context.StatusLabs
                            .FirstOrDefault(s => s.StatusName == "Active");

                        if (activeStatus != null)
                        {
                            // Update the Lab's status to "Active"
                            lab.StatusLabId = activeStatus.Id;
                        }
                    }
                }

                // Check if EndTime has passed and update Lab status to "Inactive"
                if (labUsageLog.EndTime <= DateTime.Now)
                {
                    // Find the related Lab
                    var lab = _context.Labs.FirstOrDefault(l => l.Id == labUsageLog.LabId);

                    if (lab != null)
                    {
                        // Find the "Inactive" status from the StatusLab table
                        var inactiveStatus = _context.StatusLabs
                            .FirstOrDefault(s => s.StatusName == "Inactive");

                        if (inactiveStatus != null)
                        {
                            // Update the Lab's status to "Inactive"
                            lab.StatusLabId = inactiveStatus.Id;
                        }
                    }
                }

                // Save changes to the database
                try
                {
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Trạng thái đã được cập nhật." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Lỗi khi lưu dữ liệu: " + ex.Message });
                }
            }

            return Json(new { success = false, message = "Không tìm thấy LabUsageLog." });
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
                .Include(l => l.StatusLog)
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
            ViewData["StatusLogId"] = new SelectList(_context.StatusLogs, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Admin/LabUsageLog/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LabId,UserId,Purpose,StatusLogId,StartTime,EndTime,CreatedAt")] LabUsageLog labUsageLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(labUsageLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Id", labUsageLog.LabId);
            ViewData["StatusLogId"] = new SelectList(_context.StatusLogs, "Id", "Id", labUsageLog.StatusLogId);
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
            ViewData["StatusLogId"] = new SelectList(_context.StatusLogs, "Id", "Id", labUsageLog.StatusLogId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", labUsageLog.UserId);
            return View(labUsageLog);
        }

        // POST: Admin/LabUsageLog/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LabId,UserId,Purpose,StatusLogId,StartTime,EndTime,CreatedAt")] LabUsageLog labUsageLog)
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
            ViewData["StatusLogId"] = new SelectList(_context.StatusLogs, "Id", "Id", labUsageLog.StatusLogId);
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
                .Include(l => l.StatusLog)
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
