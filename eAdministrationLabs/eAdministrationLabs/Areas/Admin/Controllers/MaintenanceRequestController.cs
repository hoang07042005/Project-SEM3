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
    [Route("admin/maintenanceRequest")]
    [Authorize(Policy = "AdminOnly")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class MaintenanceRequestController : Controller
    {
        private readonly EAdministrationLabsContext _context;

        public MaintenanceRequestController(EAdministrationLabsContext context)
        {
            _context = context;
        }

        // GET: MaintenanceRequest
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = _context.MaintenanceRequests.Include(m => m.Computer).Include(m => m.Lab).Include(m => m.RequestedByNavigation);
            return View(await eAdministrationLabsContext.ToListAsync());
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var maintenanceRequest = _context.MaintenanceRequests.FirstOrDefault(c => c.Id == id);

            if (maintenanceRequest != null)
            {
                maintenanceRequest.Status = status;
                _context.SaveChanges(); // Lưu thay đổi vào database
                return Json(new { success = true }); // Gửi thông tin thành công về client
            }

            return Json(new { success = false, message = "Computer not found" });
        }

        // GET: MaintenanceRequest/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceRequest = await _context.MaintenanceRequests
                .Include(m => m.Computer)
                .Include(m => m.Lab)
                .Include(m => m.RequestedByNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceRequest == null)
            {
                return NotFound();
            }

            return View(maintenanceRequest);
        }

        // GET: MaintenanceRequest/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["ComputerId"] = new SelectList(_context.Computers, "Id", "AssetTag");
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName");
            ViewData["RequestedBy"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }

        // POST: MaintenanceRequest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaintenanceRequestCreateDto createDto)
        {
            if (ModelState.IsValid)
            {
                MaintenanceRequest createMaintenanceRequest = new MaintenanceRequest()
                {
                    LabId = createDto.LabId,
                    ComputerId = createDto.ComputerId,
                    Description = createDto.Description,
                    RequestedBy = createDto.RequestedBy,
                    ResolvedAt = createDto.ResolvedAt,
                };
                _context.Add(createMaintenanceRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ComputerId"] = new SelectList(_context.Computers, "Id", "AssetTag", createDto.ComputerId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", createDto.LabId);
            ViewData["RequestedBy"] = new SelectList(_context.Users, "Id", "FullName", createDto.RequestedBy);
            return View(createDto);
        }

        // GET: MaintenanceRequest/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceRequest = await _context.MaintenanceRequests.FindAsync(id);
            if (maintenanceRequest == null)
            {
                return NotFound();
            }
            ViewData["ComputerId"] = new SelectList(_context.Computers, "Id", "AssetTag", maintenanceRequest.ComputerId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", maintenanceRequest.LabId);
            ViewData["RequestedBy"] = new SelectList(_context.Users, "Id", "FullName", maintenanceRequest.RequestedBy);
            return View(maintenanceRequest);
        }

        // POST: MaintenanceRequest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MaintenanceRequestEditDto editDto)
        {
            if (id != editDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var editMaintenanceRequest = await _context.MaintenanceRequests.FindAsync(id);
                    if (editMaintenanceRequest == null)
                    {
                        return NotFound();
                    }

                    // Update the properties of the existing entity
                    editMaintenanceRequest.LabId = editDto.LabId;
                    editMaintenanceRequest.ComputerId = editDto.ComputerId;
                    editMaintenanceRequest.Description = editDto.Description;
                    editMaintenanceRequest.RequestedBy = editDto.RequestedBy;
                    //editMaintenanceRequest.CreatedAt = editDto.CreatedAt;
                    //editMaintenanceRequest.ResolvedAt = editDto.ResolvedAt;

                    _context.Update(editMaintenanceRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaintenanceRequestExists(id))
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
            ViewData["ComputerId"] = new SelectList(_context.Computers, "Id", "AssetTag", editDto.ComputerId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabbName", editDto.LabId);
            ViewData["RequestedBy"] = new SelectList(_context.Users, "Id", "FullName", editDto.RequestedBy);
            return View(editDto);
        }

        // GET: MaintenanceRequest/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceRequest = await _context.MaintenanceRequests
                .Include(m => m.Computer)
                .Include(m => m.Lab)
                .Include(m => m.RequestedByNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceRequest == null)
            {
                return NotFound();
            }

            return View(maintenanceRequest);
        }

        // POST: MaintenanceRequest/Delete/5
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maintenanceRequest = await _context.MaintenanceRequests.FindAsync(id);
            if (maintenanceRequest != null)
            {
                _context.MaintenanceRequests.Remove(maintenanceRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaintenanceRequestExists(int id)
        {
            return _context.MaintenanceRequests.Any(e => e.Id == id);
        }
    }
}
