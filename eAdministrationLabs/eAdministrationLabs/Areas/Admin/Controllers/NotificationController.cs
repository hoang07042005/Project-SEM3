using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eAdministrationLabs.Dtos.Create;
using eAdministrationLabs.Dtos.Edit;
using eAdministrationLabs.Models;

namespace eAdministrationLabs.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/notification")]
    public class NotificationController : Controller
    {
        private readonly EAdministrationLabsContext _context;

        public NotificationController(EAdministrationLabsContext context)
        {
            _context = context;
        }

        // GET: Notifications
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = _context.Notifications.Include(n => n.User);
            return View(await eAdministrationLabsContext.ToListAsync());
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string readstatus)
        {
            var notification = _context.Notifications.FirstOrDefault(c => c.Id == id);

            if (notification != null)
            {
                notification.ReadStatus = readstatus;
                _context.SaveChanges(); // Lưu thay đổi vào database
                return Json(new { success = true }); // Gửi thông tin thành công về client
            }

            return Json(new { success = false, message = "Notification not found" });
        }


        // GET: Notifications/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // GET: Notifications/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NotificationCreateDto createDto)
        {
            if (ModelState.IsValid)
            {
                Notification createNotification = new Notification()
                {
                    UserId = createDto.UserId,
                    Message = createDto.Message
                };

                _context.Add(createNotification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", createDto.UserId);
            return View(new Notification());
        }

        // GET: Notifications/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", notification.UserId);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NotificationEditDto editDto)
        {
            if (id != editDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var editNotification = await _context.Notifications.FindAsync(id);
                    if (editNotification == null)
                    {
                        return NotFound();
                    }

                    // Update the properties of the existing entity
                    editNotification.UserId = editDto.UserId;
                    editNotification.Message = editDto.Message;
                    editNotification.CreatedAt = editDto.CreatedAt;


                    _context.Update(editNotification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", editDto.UserId);
            return View(editDto);
        }

        // GET: Notifications/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
