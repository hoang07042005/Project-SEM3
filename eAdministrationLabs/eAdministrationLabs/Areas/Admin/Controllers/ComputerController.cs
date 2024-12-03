using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    [Route("admin/Computer")]
    [Authorize(Policy = "AdminOnly")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class ComputerController : Controller
    {
        private readonly EAdministrationLabsContext _context;

        public ComputerController(EAdministrationLabsContext context)
        {
            _context = context;
        }

        // GET: Computer
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = _context.Computers.Include(c => c.Lab);
            return View(await eAdministrationLabsContext.ToListAsync());
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var computer = _context.Computers.FirstOrDefault(c => c.Id == id);

            if (computer != null)
            {
                computer.Status = status;
                _context.SaveChanges(); // Lưu thay đổi vào database
                return Json(new { success = true }); // Gửi thông tin thành công về client
            }

            return Json(new { success = false, message = "Computer not found" });
        }


        // GET: Computer/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var computer = await _context.Computers
                .Include(c => c.Lab)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (computer == null)
            {
                return NotFound();
            }

            return View(computer);
        }

        // GET: Computer/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName");
            return View();
        }

        // POST: Computer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComputerCreateDto computerDto)
        {
            if (ModelState.IsValid)
            {
                Computer createComputer = new Computer()
                {
                    LabId = computerDto.LabId,
                    AssetTag = computerDto.AssetTag,
                    Status = computerDto.Status,
                    Specifications = computerDto.Specifications,
                };


                _context.Add(createComputer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", computerDto.LabId);
            return View(computerDto);
        }

        // GET: Computer/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var computer = await _context.Computers.FindAsync(id);
            if (computer == null)
            {
                return NotFound();
            }
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", computer.LabId);
            return View(computer);
        }

        // POST: Computer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ComputerEditDto computerDto)
        {
            if (id != computerDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var editComputer = await _context.Computers.FindAsync(id);
                    if (editComputer == null)
                    {
                        return NotFound();
                    }


                    editComputer.LabId = computerDto.LabId;
                    editComputer.AssetTag = computerDto.AssetTag;
                    editComputer.Specifications = computerDto.Specifications;
                    editComputer.UpdatedAt = computerDto.UpdatedAt;
                    editComputer.CreatedAt = computerDto.CreatedAt;
                     

                    _context.Update(editComputer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComputerExists(id))
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
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", computerDto.LabId);
            return View(computerDto);
        }

        // GET: Computer/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var computer = await _context.Computers
                .Include(c => c.Lab)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (computer == null)
            {
                return NotFound();
            }

            return View(computer);
        }

        // POST: Computer/Delete/5
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var computer = await _context.Computers.FindAsync(id);
            if (computer != null)
            {
                _context.Computers.Remove(computer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComputerExists(int id)
        {
            return _context.Computers.Any(e => e.Id == id);
        }
    }
}
