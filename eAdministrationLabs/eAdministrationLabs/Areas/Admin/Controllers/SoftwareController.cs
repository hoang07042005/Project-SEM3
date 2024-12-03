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

namespace eAdministrationLabs.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/software")]
    public class SoftwareController : Controller
    {
        private readonly EAdministrationLabsContext _context;

        public SoftwareController(EAdministrationLabsContext context)
        {
            _context = context;
        }

        // GET: Software
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = _context.Softwares.Include(s => s.Lab);
            return View(await eAdministrationLabsContext.ToListAsync());
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string license)
        {
            var software = _context.Softwares.FirstOrDefault(c => c.Id == id);

            if (software != null)
            {
                software.License = license;
                _context.SaveChanges(); // Lưu thay đổi vào database
                return Json(new { success = true }); // Gửi thông tin thành công về client
            }

            return Json(new { success = false, message = "Notification not found" });
        }


        // GET: Software/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var software = await _context.Softwares
                .Include(s => s.Lab)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (software == null)
            {
                return NotFound();
            }

            return View(software);
        }

        // GET: Software/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName");
            return View();
        }

        // POST: Software/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SoftwareCreateDto createDto)
        {
            if (ModelState.IsValid)
            {
                Software createSoftwares = new Software() 
                {
                    Name = createDto.Name,
                    Version = createDto.Version,
                    LabId = createDto.LabId
                };

                _context.Add(createSoftwares);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", createDto.LabId);
            return View(new Software());
        }

        // GET: Software/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var software = await _context.Softwares.FindAsync(id);
            if (software == null)
            {
                return NotFound();
            }
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", software.LabId);
            return View(software);
        }

        // POST: Software/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SoftwareEditDto editDto)
        {
            if (id != editDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var editSoftware = await _context.Softwares.FindAsync(id);
                    if (editSoftware == null)
                    {
                        return NotFound();
                    }

                    // Update the properties of the existing entity
                    editSoftware.Name = editDto.Name;
                    editSoftware.Version = editDto.Version;
                    editSoftware.LabId = editDto.LabId;

                    _context.Update(editSoftware);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SoftwareExists(id))
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
            return View(editDto);
        }

        // GET: Software/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var software = await _context.Softwares
                .Include(s => s.Lab)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (software == null)
            {
                return NotFound();
            }

            return View(software);
        }

        // POST: Software/Delete/5
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var software = await _context.Softwares.FindAsync(id);
            if (software != null)
            {
                _context.Softwares.Remove(software);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SoftwareExists(int id)
        {
            return _context.Softwares.Any(e => e.Id == id);
        }
    }
}
