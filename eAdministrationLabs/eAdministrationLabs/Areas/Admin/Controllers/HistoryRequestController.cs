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
using Microsoft.Build.Framework;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using System.Numerics;
using Microsoft.CodeAnalysis.Scripting;


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




        [Route("")]
        [Route("index")]
        public async Task<IActionResult> Index(string changedByFilter, int? statusFilter)
        {
            IQueryable<HistoryRequest> eAdministrationLabsContext = _context.HistoryRequests
                .Include(h => h.Request)
                .Include(h => h.StatusRequest)
                .Include(h => h.User);

            // Nếu không có bộ lọc trạng thái nào được chọn, mặc định chỉ hiển thị trạng thái "Pending"
            if (!statusFilter.HasValue)
            {
                statusFilter = 1; // 1 là ID của trạng thái Pending
            }

            // Lọc theo trạng thái
            eAdministrationLabsContext = eAdministrationLabsContext.Where(h => h.StatusRequestId == statusFilter);

            // Lọc theo "Changed By"
            if (!string.IsNullOrEmpty(changedByFilter))
            {
                eAdministrationLabsContext = eAdministrationLabsContext.Where(h => h.ChangedBy.Contains(changedByFilter.Trim()));
            }

            eAdministrationLabsContext = eAdministrationLabsContext
                .OrderBy(h => h.StatusRequestId != 1) // Ưu tiên trạng thái Pending nếu có
                .ThenByDescending(h => h.ChangedAt);

            var statusOptions = await _context.StatusRequests.ToListAsync();
            var technicalStaffUsers = await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "administrator" || ur.Role.RoleName == "technicalstaff"))
                .ToListAsync();

            // Pass filter values to the ViewBag
            ViewBag.ChangedByFilter = changedByFilter;
            ViewBag.StatusFilter = statusFilter; // Gửi trạng thái đang được lọc để hiển thị lại trên giao diện
            ViewBag.TechnicalStaffUsers = technicalStaffUsers;
            ViewBag.StatusOptions = statusOptions;

            return View(await eAdministrationLabsContext.ToListAsync());
        }


        [HttpPost]
        public async Task<IActionResult> UpdateChangedBy(int id, int userId)
        {
            try
            {
                // Kiểm tra HistoryRequest
                var historyRequest = await _context.HistoryRequests
                    .Include(h => h.Request)
                    .ThenInclude(r => r.Equipment)
                    .Include(h => h.Request)
                    .ThenInclude(r => r.Lab)
                    .Include(h => h.Request)
                    .ThenInclude(r => r.Image)
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
                        await SendStatusEmail(newUser, "Assigned to Request Update", historyRequest.RequestId, historyRequest.Request.Lab.LabName, historyRequest.Request.Equipment.NameEquipment, historyRequest.Notes, historyRequest.Request.Image.Image);

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


        private async Task SendStatusEmail(User user, string statusName, int requestId, string labName, string nameEquipment, string notes, byte[] imageBytes)
        {
            string subject = string.Empty;
            string body = string.Empty;

            // Hàm tạo phần header chung cho email (có thể mở rộng nếu cần)
            string GetEmailBodyHeader(string status)
            {
                return $@"
        <html>
        <head>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    margin: 0;
                    padding: 0;
                    background-color: #f4f4f4;
                }}
                .container {{
                    width: 100%;
                    max-width: 600px;
                    margin: 0 auto;
                    padding: 20px;
                    background-color: #ffffff;
                    border-radius: 8px;
                    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                }}
                .header {{
                    font-size: 18px;
                    font-weight: bold;
                    color: #333333;
                }}
                .content {{
                    font-size: 16px;
                    color: #555555;
                }}
                .footer {{
                    font-size: 14px;
                    color: #999999;
                    text-align: center;
                    margin-top: 20px;
                }}
                .strong {{
                    font-weight: bold;
                }}
                a {{
                    color: #0066cc;
                    text-decoration: none;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <p>Dear {user.FullName},</p>
                </div>
                <div class='content'>
        ";
            }

            // Hàm kết thúc email và phần footer chung
            string GetEmailBodyFooter()
            {
                return $@"
                </div>
                <div class='footer'>
                    <p>If you have any questions, feel free to contact our support team.</p>
                    <p>Thank you for using our service.</p>
                </div>
            </div>
        </body>
        </html>";
            }
            string imageBase64 = imageBytes != null ? Convert.ToBase64String(imageBytes) : null;
            // Dựa trên statusName, tạo nội dung email và subject
            if (statusName == "Assigned to Request Update")
            {
                subject = "You have been assigned to a request update";
                body = $"{GetEmailBodyHeader(statusName)}<p>You have been assigned to handle the update for request (ID: {requestId}).</p><p>Lab Name: {labName}</p><p>Equipment Name: {nameEquipment}</p><p>Notes: {notes}</p>";
                if (!string.IsNullOrEmpty(imageBase64))
                {
                    body += $"<img src='data:image/png;base64,{imageBase64}' style='max-width:100px; max-height:100px;' alt='Request Image' />";
                }
                body += $"<p> Please check your tasks and ensure the request is processed promptly.</p>{GetEmailBodyFooter()}";
            }
       
            else if (statusName == "Approved" || statusName == "In Progress" || statusName == "Complete")
            {
                subject = $"Your request has been {statusName}";
                body = $"{GetEmailBodyHeader(statusName)}<p>Your request (ID: {requestId}) has been updated to <span class='strong'>{statusName}</span>.</p>< p > Thank you for using our service.</ p >{ GetEmailBodyFooter()}";
            }
            else if (statusName == "Reject")
            {
                subject = "Your request has been rejected";
                body = $"{GetEmailBodyHeader(statusName)}<p>We regret to inform you that your request (ID: {requestId}) has been <span class='strong'>rejected</span>.</p>< p > If you have any questions, please contact our support team.</ p >{ GetEmailBodyFooter()}";
            }

            // Gửi email nếu subject và body đã được xác định
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
