using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eAdministrationLabs.Dtos.Create;
using eAdministrationLabs.Dtos.Edit;
using eAdministrationLabs.Models;
using Microsoft.AspNetCore.Authorization;

namespace eAdministrationLabs.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/labUsageLog")]
    [Authorize(Policy = "AdminOnly")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class LabUsageLogController : Controller
    {
        private readonly EAdministrationLabsContext _context;

        public LabUsageLogController(EAdministrationLabsContext context)
        {
            _context = context;
        }

        // GET: LabUsageLog
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = _context.LabUsageLogs.Include(l => l.Lab).Include(l => l.User);
            return View(await eAdministrationLabsContext.ToListAsync());
        }

        // GET: LabUsageLog/Details/5
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

        // GET: LabUsageLog/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }

        // POST: LabUsageLog/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LabUsageLogCreateDto createDto)
        {
            if (ModelState.IsValid)
            {
                LabUsageLog createLabUsageLog = new LabUsageLog()
                {
                    LabId = createDto.LabId,
                    UserId = createDto.UserId,
                    Purpose = createDto.Purpose,
                    StartTime = createDto.StartTime,
                    EndTime = createDto.EndTime,
                };

                _context.Add(createLabUsageLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", createDto.LabId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", createDto.UserId);
            return View(createDto);
        }

        //LabId UserId Purpose StartTime EndTime CreatedAt

        // GET: LabUsageLog/Edit/5
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
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", labUsageLog.LabId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", labUsageLog.UserId);
            return View(labUsageLog);
        }

        // POST: LabUsageLog/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LabUsageLogEditDto editDto)
        {
            if (id != editDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var editLabUsageLog = await _context.LabUsageLogs.FindAsync(id);
                    if (editLabUsageLog == null)
                    {
                        return NotFound();
                    }
                    //LabId UserId Purpose StartTime EndTime CreatedAt

                    editLabUsageLog.LabId = editDto.LabId;
                    editLabUsageLog.UserId = editDto.UserId;
                    editLabUsageLog.Purpose = editDto.Purpose;
                    editLabUsageLog.StartTime = editDto.StartTime;
                    editLabUsageLog.EndTime = editDto.EndTime;
                    editLabUsageLog.CreatedAt = editDto.CreatedAt;

                    _context.Update(editLabUsageLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LabUsageLogExists(id))
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
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", editDto.LabId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", editDto.UserId);
            return View(editDto);
        }

        // GET: LabUsageLog/Delete/5
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

        // POST: LabUsageLog/Delete/5
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
