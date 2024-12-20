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
        //// GET: Admin/Request
        //// GET: Admin/Request
        //[Route("")]
        //[Route("index")]
        //public async Task<IActionResult> Index()
        //{
        //    var pendingRequests = await _context.Requests
        //        .Include(r => r.Equipment)
        //        .Include(r => r.Image)
        //        .Include(r => r.Lab)
        //        .Where(r => r.HistoryRequests
        //            .OrderByDescending(h => h.ChangedAt)
        //            .FirstOrDefault().StatusRequest.StatusName == "Pending")
        //        .OrderByDescending(r => r.CreatedAt)  // Order by CreatedAt descending
        //        .ToListAsync();

        //    return View(pendingRequests);
        //}

        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = await _context.Requests.Include(r => r.Equipment).Include(r => r.Image).Include(r => r.Lab).Where(r => r.HistoryRequests
                  .OrderByDescending(h => h.ChangedAt)
                .FirstOrDefault().StatusRequest.StatusName == "Pending")
                .OrderByDescending(r => r.CreatedAt).ToListAsync();
            return View(eAdministrationLabsContext);
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
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "NameEquipment");
            ViewData["ImageId"] = new SelectList(_context.RequestImages, "Id", "Image");
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName");
            return View();
        }

        // POST: Admin/Request/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRequestDto createRequestDto)
        {
            if (ModelState.IsValid)
            {
                Request createRequest = new Request()
                {
                    EquipmentId = createRequestDto.EquipmentId,
                    ImageId = createRequestDto.ImageId,
                    LabId = createRequestDto.LabId
                };

                _context.Add(createRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "NameEquipment", createRequestDto.EquipmentId);
            ViewData["ImageId"] = new SelectList(_context.RequestImages, "Id", "Image", createRequestDto.ImageId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", createRequestDto.LabId);
            return View(new Request());
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
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "NameEquipment", request.EquipmentId);
            ViewData["ImageId"] = new SelectList(_context.RequestImages, "Id", "Image", request.ImageId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", request.LabId);
            return View(request);
        }

        // POST: Admin/Request/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditRequestDto editRequestDto)
        {
            if (id != editRequestDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var editRequest = await _context.Requests.FindAsync(id);
                    if (editRequest == null)
                    {
                        return NotFound();
                    }

                    editRequest.EquipmentId = editRequestDto.EquipmentId;
                    editRequest.ImageId = editRequestDto.ImageId;
                    editRequest.LabId = editRequestDto.LabId;
                    editRequest.CreatedAt = editRequestDto.CreatedAt;
                   

                    _context.Update(editRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(id))
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
            ViewData["EquipmentId"] = new SelectList(_context.Equipments, "Id", "NameEquipment", editRequestDto.EquipmentId);
            ViewData["ImageId"] = new SelectList(_context.RequestImages, "Id", "Image", editRequestDto.ImageId);
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", editRequestDto.LabId);
            return View(editRequestDto);
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
