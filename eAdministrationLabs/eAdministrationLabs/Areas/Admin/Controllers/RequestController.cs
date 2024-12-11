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
    [Route("admin/request")]
    ////[Authorize(Roles = "Admin, Manager, Technician, Staff")]
    [Authorize(Policy = "AdminOnly")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class RequestController : Controller
    {
        private readonly EAdministrationLabsContext _context;

        public RequestController(EAdministrationLabsContext context)
        {
            _context = context;
        }

        // GET: Admin/Request
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = _context.Requests.Include(r => r.Equipment).Include(r => r.Image).Include(r => r.Lab);
            return View(await eAdministrationLabsContext.ToListAsync());
        }

        // GET: Admin/Request/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .Include(r => r.Equipment)
                .Include(r => r.Image)
                .Include(r => r.Lab)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // GET: Admin/Request/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Id");
            ViewData["ImageId"] = new SelectList(_context.RequestImages, "Id", "Id");
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Id");
            return View();
        }

        // POST: Admin/Request/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LabId,EquipmentId,ImageId,CreatedAt")] Request request)
        {
            if (ModelState.IsValid)
            {
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Id", request.EquipmentId);
            ViewData["ImageId"] = new SelectList(_context.RequestImages, "Id", "Id", request.ImageId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Id", request.LabId);
            return View(request);
        }

        // GET: Admin/Request/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Id", request.EquipmentId);
            ViewData["ImageId"] = new SelectList(_context.RequestImages, "Id", "Id", request.ImageId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Id", request.LabId);
            return View(request);
        }

        // POST: Admin/Request/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LabId,EquipmentId,ImageId,CreatedAt")] Request request)
        {
            if (id != request.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(request.Id))
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
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Id", request.EquipmentId);
            ViewData["ImageId"] = new SelectList(_context.RequestImages, "Id", "Id", request.ImageId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Id", request.LabId);
            return View(request);
        }

        // GET: Admin/Request/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .Include(r => r.Equipment)
                .Include(r => r.Image)
                .Include(r => r.Lab)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // POST: Admin/Request/Delete/5
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request != null)
            {
                _context.Requests.Remove(request);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
