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
    [Route("admin/equilab")]
    ////[Authorize(Roles = "Admin, Manager, Technician, Staff")]
    [Authorize(Policy = "AdminOnly")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class EquiLabController : Controller
    {
        private readonly EAdministrationLabsContext _context;

        public EquiLabController(EAdministrationLabsContext context)
        {
            _context = context;
        }


        // GET: Admin/EquiLab
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = _context.EquiLabs.Include(e => e.Equipment).Include(e => e.Lab).Include(e => e.User);
            return View(await eAdministrationLabsContext.ToListAsync());
        }

        // GET: Admin/EquiLab/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equiLab = await _context.EquiLabs
                .Include(e => e.Equipment)
                .Include(e => e.Lab)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equiLab == null)
            {
                return NotFound();
            }

            return View(equiLab);
        }

        // GET: Admin/EquiLab/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Id");
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Admin/EquiLab/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EquipmentId,LabId,UserId")] EquiLab equiLab)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equiLab);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Id", equiLab.EquipmentId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Id", equiLab.LabId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", equiLab.UserId);
            return View(equiLab);
        }

        // GET: Admin/EquiLab/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equiLab = await _context.EquiLabs.FindAsync(id);
            if (equiLab == null)
            {
                return NotFound();
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Id", equiLab.EquipmentId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Id", equiLab.LabId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", equiLab.UserId);
            return View(equiLab);
        }

        // POST: Admin/EquiLab/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EquipmentId,LabId,UserId")] EquiLab equiLab)
        {
            if (id != equiLab.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equiLab);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquiLabExists(equiLab.Id))
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
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "Id", equiLab.EquipmentId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "Id", equiLab.LabId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", equiLab.UserId);
            return View(equiLab);
        }

        // GET: Admin/EquiLab/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equiLab = await _context.EquiLabs
                .Include(e => e.Equipment)
                .Include(e => e.Lab)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (equiLab == null)
            {
                return NotFound();
            }

            return View(equiLab);
        }

        // POST: Admin/EquiLab/Delete/5
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equiLab = await _context.EquiLabs.FindAsync(id);
            if (equiLab != null)
            {
                _context.EquiLabs.Remove(equiLab);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquiLabExists(int id)
        {
            return _context.EquiLabs.Any(e => e.Id == id);
        }
    }
}
