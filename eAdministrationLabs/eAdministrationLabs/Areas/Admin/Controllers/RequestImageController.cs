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
    [Route("admin/requestimage")]
    ////[Authorize(Roles = "Admin, Manager, Technician, Staff")]
    [Authorize(Policy = "AdminOnly")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class RequestImageController : Controller
    {
        private readonly EAdministrationLabsContext _context;

        public RequestImageController(EAdministrationLabsContext context)
        {
            _context = context;
        }

        // GET: Admin/RequestImage
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.RequestImages.ToListAsync());
        }

        // GET: Admin/RequestImage/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestImage = await _context.RequestImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestImage == null)
            {
                return NotFound();
            }

            return View(requestImage);
        }

        // GET: Admin/RequestImage/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/RequestImage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRequestImageDto createRequestImageDto)
        {
            if (!ModelState.IsValid)
            {
                return View(createRequestImageDto);
            }

            if (createRequestImageDto.Image == null || createRequestImageDto.Image.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Please upload a valid image file.");
                return View(createRequestImageDto);
            }

            var requestImage = new RequestImage
            {
                CreatedAt = createRequestImageDto.CreatedAt ?? DateTime.Now
            };

            using (var memoryStream = new MemoryStream())
            {
                await createRequestImageDto.Image.CopyToAsync(memoryStream);
                requestImage.Image = memoryStream.ToArray();
            }

            try
            {
                _context.Add(requestImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to database: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the data.");
                return View(createRequestImageDto);
            }
        }


        // GET: Admin/RequestImage/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestImage = await _context.RequestImages.FindAsync(id);
            if (requestImage == null)
            {
                return NotFound();
            }
            return View(requestImage);
        }

        // POST: Admin/RequestImage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditRequestImageDto editRequestImageDto)
        {
            if (id != editRequestImageDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tìm kiếm hình ảnh hiện tại trong cơ sở dữ liệu
                    var existingImage = await _context.RequestImages.FindAsync(id);
                    if (existingImage == null)
                    {
                        return NotFound();
                    }

                    // Nếu có hình ảnh mới được tải lên, cập nhật hình ảnh
                    if (editRequestImageDto.Image != null && editRequestImageDto.Image.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await editRequestImageDto.Image.CopyToAsync(memoryStream);
                            existingImage.Image = memoryStream.ToArray(); // Cập nhật hình ảnh mới
                        }
                    }

                    // Cập nhật thời gian tạo nếu cần
                    existingImage.CreatedAt = editRequestImageDto.CreatedAt ?? existingImage.CreatedAt;

                    // Cập nhật các trường khác của existingImage nếu cần
                    _context.Update(existingImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.RequestImages.Any(e => e.Id == id))
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
            return View(editRequestImageDto);
        }

        // GET: Admin/RequestImage/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestImage = await _context.RequestImages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestImage == null)
            {
                return NotFound();
            }

            return View(requestImage);
        }

        // POST: Admin/RequestImage/Delete/5
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var requestImage = await _context.RequestImages.FindAsync(id);
            if (requestImage != null)
            {
                _context.RequestImages.Remove(requestImage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestImageExists(int id)
        {
            return _context.RequestImages.Any(e => e.Id == id);
        }
    }
}
