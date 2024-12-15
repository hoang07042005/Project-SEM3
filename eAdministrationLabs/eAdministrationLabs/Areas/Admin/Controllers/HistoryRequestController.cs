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
using eAdministrationLabs.Services;


namespace eAdministrationLabs.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("admin/historyrequest")]
    ////[Authorize(Roles = "Admin, Manager, Technician, Staff")]
    [Authorize(Policy = "AdminOnly")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class HistoryRequestController : Controller
    {
        private readonly EAdministrationLabsContext _context;
        private readonly EmailService _emailService;

        public HistoryRequestController(EAdministrationLabsContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Admin/HistoryRequest
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = _context.HistoryRequests.Include(h => h.Request).Include(h => h.StatusRequest).Include(h => h.User);
            ViewBag.StatusOptions = _context.StatusRequests.ToList();
            return View(await eAdministrationLabsContext.ToListAsync());
        }



        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int statusId)
        {
            try
            {
                var historyRequest = await _context.HistoryRequests
                    .Include(h => h.Request)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (historyRequest == null)
                    return Json(new { success = false, message = "History request not found" });

                var status = await _context.StatusRequests.FirstOrDefaultAsync(s => s.Id == statusId);
                if (status == null)
                    return Json(new { success = false, message = "Invalid status" });

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Cập nhật trạng thái
                        historyRequest.StatusRequestId = statusId;
                        await _context.SaveChangesAsync();

                        // Tạo thông báo
                        var notification = new Notification
                        {
                            UserId = historyRequest.UserId,
                            Message = $"Your request has been updated to: {status.StatusName}",
                            ReadStatus = "Unread",
                            CreatedAt = DateTime.UtcNow,
                            RequestId = historyRequest.RequestId
                        };

                        await _context.Notifications.AddAsync(notification);
                        await _context.SaveChangesAsync();

                        // Lấy thông tin người dùng
                        var user = historyRequest.User;
                        if (user != null && !string.IsNullOrEmpty(user.Email))
                        {
                            // Gửi email dựa trên trạng thái
                            string subject = string.Empty;
                            string body = string.Empty;

                            if (status.StatusName == "Approved" || status.StatusName == "Complete")
                            {
                                subject = $"Your request has been {status.StatusName}";
                                body = $@"
                            <p>Dear {user.FullName},</p>
                            <p>Your request (ID: {historyRequest.RequestId}) has been updated to <strong>{status.StatusName}</strong>.</p>
                            <p>Thank you for using our service.</p>";
                            }
                            else if (status.StatusName == "Reject")
                            {
                                subject = "Your request has been rejected";
                                body = $@"
                            <p>Dear {user.FullName},</p>
                            <p>We regret to inform you that your request (ID: {historyRequest.RequestId}) has been <strong>rejected</strong>.</p>
                            <p>If you have any questions, please contact our support team.</p>";
                            }

                            // Gửi email nếu có tiêu đề và nội dung
                            if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(body))
                            {
                                await _emailService.SendEmailAsync(user.Email, subject, body);
                            }
                        }

                        await transaction.CommitAsync();

                        return Json(new { success = true, updatedStatus = status.StatusName });
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        return Json(new { success = false, message = "An error occurred while updating the status." });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi (tùy framework logging của bạn)
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }



        private async Task SendStatusEmail(User user, string statusName, int requestId)
        {
            string subject = string.Empty;
            string body = string.Empty;

            if (statusName == "Approved" || statusName == "Complete")
            {
                subject = $"Your request has been {statusName}";
                body = $@"
            <p>Dear {user.FullName},</p>
            <p>Your request (ID: {requestId}) has been updated to <strong>{statusName}</strong>.</p>
            <p>Thank you for using our service.</p>";
            }
            else if (statusName == "Reject")
            {
                subject = "Your request has been rejected";
                body = $@"
            <p>Dear {user.FullName},</p>
            <p>We regret to inform you that your request (ID: {requestId}) has been <strong>rejected</strong>.</p>
            <p>If you have any questions, please contact our support team.</p>";
            }

            if (!string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(body))
            {
                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
        }





        // GET: Admin/HistoryRequest/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historyRequest = await _context.HistoryRequests
                .Include(h => h.Request)
                .Include(h => h.StatusRequest)
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historyRequest == null)
            {
                return NotFound();
            }

            return View(historyRequest);
        }

        // GET: Admin/HistoryRequest/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Id");
            ViewData["StatusRequestId"] = new SelectList(_context.StatusRequests, "Id", "StatusName");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName");
            return View();
        }

        // POST: Admin/HistoryRequest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateHistoryRequestDto createHistoryRequestDto)
        {
            if (ModelState.IsValid)
            {
                HistoryRequest createHistoryRequest = new HistoryRequest()
                {
                    UserId = createHistoryRequestDto.UserId,
                    RequestId = createHistoryRequestDto.RequestId,
                    StatusRequestId  = createHistoryRequestDto.StatusRequestId,
                    ChangedBy = createHistoryRequestDto.ChangedBy,
                    Notes = createHistoryRequestDto.Notes
                };

                _context.Add(createHistoryRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Id", createHistoryRequestDto.RequestId);
            ViewData["StatusRequestId"] = new SelectList(_context.StatusRequests, "Id", "StatusName", createHistoryRequestDto.StatusRequestId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", createHistoryRequestDto.UserId);
            return View(new HistoryRequest());
        }

        // GET: Admin/HistoryRequest/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historyRequest = await _context.HistoryRequests.FindAsync(id);
            if (historyRequest == null)
            {
                return NotFound();
            }
            ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Id", historyRequest.RequestId);
            ViewData["StatusRequestId"] = new SelectList(_context.StatusRequests, "Id", "StatusName", historyRequest.StatusRequestId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", historyRequest.UserId);
            return View(historyRequest);
        }

        // POST: Admin/HistoryRequest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditHistoryRequestDto editHistoryRequestDto)
        {
            if (id != editHistoryRequestDto.Id)
            {
                return NotFound();
            }
            //[Bind("Id,UserId,RequestId,StatusRequestId,ChangedBy,ChangedAt,Notes")]
            if (ModelState.IsValid)
            {
                try
                {

                    var editHistoryRequest = await _context.HistoryRequests.FindAsync(id);
                    if (editHistoryRequest == null)
                    {
                        return NotFound();
                    }

                    editHistoryRequest.UserId = editHistoryRequestDto.UserId;
                    editHistoryRequest.RequestId = editHistoryRequestDto.RequestId;
                    editHistoryRequest.StatusRequestId = editHistoryRequestDto.StatusRequestId;
                    editHistoryRequest.ChangedBy = editHistoryRequestDto.ChangedBy;
                    editHistoryRequest.ChangedAt = editHistoryRequestDto.ChangedAt;
                    editHistoryRequest.Notes = editHistoryRequestDto.Notes;


                    _context.Update(editHistoryRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoryRequestExists(id))
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
            ViewData["RequestId"] = new SelectList(_context.Requests, "Id", "Id", editHistoryRequestDto.RequestId);
            ViewData["StatusRequestId"] = new SelectList(_context.StatusRequests, "Id", "StatusName", editHistoryRequestDto.StatusRequestId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", editHistoryRequestDto.UserId);
            return View(editHistoryRequestDto);
        }

        // GET: Admin/HistoryRequest/Delete/5
        [Route("Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historyRequest = await _context.HistoryRequests
                .Include(h => h.Request)
                .Include(h => h.StatusRequest)
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historyRequest == null)
            {
                return NotFound();
            }

            return View(historyRequest);
        }

        // POST: Admin/HistoryRequest/Delete/5
        [Route("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var historyRequest = await _context.HistoryRequests.FindAsync(id);
            if (historyRequest != null)
            {
                _context.HistoryRequests.Remove(historyRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoryRequestExists(int id)
        {
            return _context.HistoryRequests.Any(e => e.Id == id);
        }
    }
}
