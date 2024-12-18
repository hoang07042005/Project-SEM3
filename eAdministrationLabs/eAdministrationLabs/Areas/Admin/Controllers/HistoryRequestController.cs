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
        private readonly ILogger<RequestController> _logger;

        public HistoryRequestController(EAdministrationLabsContext context, EmailService emailService, ILogger<RequestController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        // GET: Admin/HistoryRequest
        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = _context.HistoryRequests.Include(h => h.Request).Include(h => h.StatusRequest).Include(h => h.User);
            //ViewBag.StatusOptions = _context.StatusRequests.ToList();
            var technicalStaffUsers = await _context.Users
                 .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "technical staff"))
                 .ToListAsync();

            // Pass the list to the view
            ViewBag.TechnicalStaffUsers = technicalStaffUsers;
            return View(await eAdministrationLabsContext.ToListAsync());
        }



        //[HttpPost]
        //public async Task<IActionResult> UpdateChangedBy(int id, int userId)
        //{
        //    try
        //    {
        //        // Kiểm tra HistoryRequest
        //        var historyRequest = await _context.HistoryRequests
        //            .Include(h => h.Request)
        //            .Include(h => h.User)
        //            .FirstOrDefaultAsync(h => h.Id == id);

        //        if (historyRequest == null)
        //            return Json(new { success = false, message = "History request not found" });

        //        // Kiểm tra User mới
        //        var newUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        //        if (newUser == null)
        //            return Json(new { success = false, message = "User not found" });

        //        // Cập nhật ChangedBy
        //        historyRequest.ChangedBy = newUser.FullName;

        //        // Bắt đầu transaction
        //        using (var transaction = await _context.Database.BeginTransactionAsync())
        //        {
        //            try
        //            {
        //                await _context.SaveChangesAsync();

        //                // Tạo thông báo
        //                var notification = new Notification
        //                {
        //                    UserId = historyRequest.UserId,
        //                    Message = $"Your request (ID: {historyRequest.RequestId}) has been updated by.",
        //                    ReadStatus = "Unread",
        //                    CreatedAt = DateTime.UtcNow,
        //                    RequestId = historyRequest.RequestId
        //                };

        //                await _context.Notifications.AddAsync(notification);
        //                await _context.SaveChangesAsync();

        //                await transaction.CommitAsync();

        //                // Trả kết quả thành công
        //                return Json(new { success = true, updatedChangedBy = newUser.FullName });
        //            }
        //            catch (Exception ex)
        //            {
        //                await transaction.RollbackAsync();
        //                // Log lỗi (nếu cần thiết)
        //                return Json(new { success = false, message = "An error occurred while updating ChangedBy.", error = ex.Message });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log lỗi (tùy framework logging)
        //        return Json(new { success = false, message = "An unexpected error occurred.", error = ex.Message });
        //    }
        //}


        [HttpPost]
        public async Task<IActionResult> UpdateChangedBy(int id, int userId)
        {
            try
            {
                // Kiểm tra HistoryRequest
                var historyRequest = await _context.HistoryRequests
                    .Include(h => h.Request)
                    .Include(h => h.User)
                    .FirstOrDefaultAsync(h => h.Id == id);

                if (historyRequest == null)
                    return Json(new { success = false, message = "History request not found" });

                // Kiểm tra User mới
                var newUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (newUser == null)
                    return Json(new { success = false, message = "User not found" });

                // Cập nhật ChangedBy
                historyRequest.ChangedBy = newUser.FullName;

                // Bắt đầu transaction
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await _context.SaveChangesAsync();

                        // Tạo thông báo cho User được cập nhật
                        var notification = new Notification
                        {
                            UserId = userId,
                            Message = $"You have been assigned to update request (ID: {historyRequest.RequestId}).",
                            ReadStatus = "Unread",
                            CreatedAt = DateTime.UtcNow,
                            RequestId = historyRequest.RequestId
                        };

                        await _context.Notifications.AddAsync(notification);
                        await _context.SaveChangesAsync();

                        // Gửi email thông báo
                        await SendStatusEmail(newUser, "Assigned to Request Update", historyRequest.RequestId);

                        await transaction.CommitAsync();

                        // Trả kết quả thành công
                        return Json(new { success = true, updatedChangedBy = newUser.FullName });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        // Log lỗi (nếu cần thiết)
                        return Json(new { success = false, message = "An error occurred while updating ChangedBy.", error = ex.Message });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi (tùy framework logging)
                return Json(new { success = false, message = "An unexpected error occurred.", error = ex.Message });
            }
        }


        private async Task SendStatusEmail(User user, string statusName, int requestId)
        {
            string subject = string.Empty;
            string body = string.Empty;

            if (statusName == "Assigned to Request Update")
            {
                subject = "You have been assigned to a request update";
                body = $@"
            <p>Dear {user.FullName},</p>
            <p>You have been assigned to handle the update for request (ID: {requestId}).</p>
            <p>Please check your tasks and ensure the request is processed promptly.</p>
            <p>Thank you!</p>";
            }
            else if (statusName == "Approved" || statusName == "Complete")
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
