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
    [Route("admin/lab")]
    [Authorize(Policy = "AdminOnly")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class LabController : Controller
    {
        private readonly EAdministrationLabsContext _context;

        public LabController(EAdministrationLabsContext context)
        {
            _context = context;
        }

        // GET: Lab
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Labs.ToListAsync());
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var computer = _context.Labs.FirstOrDefault(c => c.Id == id);

            if (computer != null)
            {
                computer.Status = status;
                _context.SaveChanges(); // Lưu thay đổi vào database
                return Json(new { success = true }); // Gửi thông tin thành công về client
            }

            return Json(new { success = false, message = "Computer not found" });
        }


        // GET: Lab/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lab = await _context.Labs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lab == null)
            {
                return NotFound();
            }

            return View(lab);
        }

        // GET: Lab/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Lab/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LabCreateDto lapDto)
        {
            if (ModelState.IsValid)
            {
                Lab createLab = new Lab()
                {
                    LabName = lapDto.LabName,
                    Location = lapDto.Location,
                    Capacity = lapDto.Capacity
                };

                _context.Add(createLab);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(new Lab());
        }

        //LabName Location Capacity
        // GET: Lab/Edit/5
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
            return View(lab);
        }

        // POST: Lab/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LabEditDto labDto)
        {
            if (id != labDto.Id)
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


                    editLab.LabName = labDto.LabName;
                    editLab.Location = labDto.Location;
                    editLab.Capacity = labDto.Capacity;
                    editLab.UpdatedAt = labDto.UpdatedAt;
                    editLab.CreatedAt = labDto.CreatedAt;

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
            return View(labDto);
        }

        // GET: Lab/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lab = await _context.Labs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lab == null)
            {
                return NotFound();
            }

            return View(lab);
        }

        // POST: Lab/Delete/5
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
