using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eAdministrationLabs.Models;
using Microsoft.AspNetCore.Authorization;
using eAdministrationLabs.Dtos.Create;
using eAdministrationLabs.Dtos.Edit;
using X.PagedList;

namespace eAdministrationLabs.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/equilab")]
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
        public async Task<IActionResult> Index(int? page)
        {
            try
            {
                int pageSize = 6;
                int pageNumber = page == null || page < 0 ? 1 : page.Value;

                var eAdministrationLabsContext = await _context.EquiLabs
                    .Include(e => e.Equipment)
                    .Include(e => e.Lab)
                    .ToListAsync();

                if (eAdministrationLabsContext == null || !eAdministrationLabsContext.Any())
                {
                    return NotFound(); // Handle case where no data is found
                }

                PagedList<EquiLab> equiLabs = new PagedList<EquiLab>(eAdministrationLabsContext, pageNumber, pageSize);
                return View(equiLabs);
            }
            catch (Exception ex)
            {
                // Log the error details for troubleshooting
                Console.Error.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
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
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "NameEquipment");
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName");
            return View();
        }

        // POST: Admin/EquiLab/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEquiLabDto createEquiLabDto)
        {
            if (ModelState.IsValid)
            {
                EquiLab createEquiLab = new EquiLab()
                {
                    EquipmentId = createEquiLabDto.EquipmentId,
                    LabId = createEquiLabDto.LabId
                };

                _context.Add(createEquiLab);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "NameEquipment", createEquiLabDto.EquipmentId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", createEquiLabDto.LabId);
            return View(new EquiLab());
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
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "NameEquipment", equiLab.EquipmentId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", equiLab.LabId);
            return View(equiLab);
        }

        // POST: Admin/EquiLab/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditEquiLabDto editEquiLabDto)
        {
            if (id != editEquiLabDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var editEquiLab = await _context.EquiLabs.FindAsync(id);
                    if (editEquiLab == null)
                    {
                        return NotFound();
                    }

                    editEquiLab.EquipmentId = editEquiLabDto.EquipmentId;
                    editEquiLab.LabId = editEquiLabDto.LabId;

                    _context.Update(editEquiLab);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquiLabExists(id))
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
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "NameEquipment", editEquiLabDto.EquipmentId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", editEquiLabDto.LabId);

            return View(editEquiLabDto);
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
