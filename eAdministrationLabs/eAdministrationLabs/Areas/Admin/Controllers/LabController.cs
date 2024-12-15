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

namespace eAdministrationLabs.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/lab")]
    ////[Authorize(Roles = "Admin, Manager, Technician, Staff")]
    [Authorize(Policy = "AdminOnly")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class LabController : Controller
    {
        private readonly EAdministrationLabsContext _context;

        public LabController(EAdministrationLabsContext context)
        {
            _context = context;
        }

        // GET: Admin/Lab
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = _context.Labs.Include(l => l.StatusLab);
            ViewBag.StatusOptions = _context.StatusLabs.ToList();
            return View(await eAdministrationLabsContext.ToListAsync());
        }



        // GET: Admin/Lab/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lab = await _context.Labs
                .Include(l => l.StatusLab)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lab == null)
            {
                return NotFound();
            }

            return View(lab);
        }

        // GET: Admin/Lab/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["StatusLabId"] = new SelectList(_context.StatusLabs, "Id", "StatusName");
            return View();
        }

        // POST: Admin/Lab/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLabDto createLabDto)
        {
            if (ModelState.IsValid)
            {

                Lab createLab = new Lab()
                {
                    LabName = createLabDto.LabName,
                    Location = createLabDto.Location,
                    Capacity = createLabDto.Capacity,
                    StatusLabId = createLabDto.StatusLabId
                };

                _context.Add(createLab);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StatusLabId"] = new SelectList(_context.StatusLabs, "Id", "StatusName", createLabDto.StatusLabId);
            return View(new Lab());
        }

        // GET: Admin/Lab/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lab = await _context.Labs.FindAsync(id);
            if (lab == null)
            {
                return NotFound();
            }
            ViewData["StatusLabId"] = new SelectList(_context.StatusLabs, "Id", "StatusName", lab.StatusLabId);
            return View(lab);
        }

        // POST: Admin/Lab/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditLabDto editLabDto)
        {
            if (id != editLabDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var editLab = await _context.Labs.FindAsync(id);
                    if (editLab == null)
                    {
                        return NotFound();
                    }

                    editLab.LabName = editLabDto.LabName;
                    editLab.Location = editLabDto.Location;
                    editLab.Capacity = editLabDto.Capacity;
                    editLab.StatusLabId = editLabDto.StatusLabId;
                    editLab.CreatedAt = editLabDto.CreatedAt;
                    editLab.UpdatedAt = editLabDto.UpdatedAt;



                    _context.Update(editLab);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LabExists(id))
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
            ViewData["StatusLabId"] = new SelectList(_context.StatusLabs, "Id", "StatusName", editLabDto.StatusLabId);
            return View(editLabDto);
        }

        // GET: Admin/Lab/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lab = await _context.Labs
                .Include(l => l.StatusLab)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lab == null)
            {
                return NotFound();
            }

            return View(lab);
        }

        // POST: Admin/Lab/Delete/5
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lab = await _context.Labs.FindAsync(id);
            if (lab != null)
            {
                _context.Labs.Remove(lab);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LabExists(int id)
        {
            return _context.Labs.Any(e => e.Id == id);
        }
    }
}
