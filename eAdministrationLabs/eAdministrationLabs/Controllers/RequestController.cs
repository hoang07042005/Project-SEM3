﻿using Microsoft.AspNetCore.Mvc;
using eAdministrationLabs.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

using eAdministrationLabs.Models.ViewModels;
using X.PagedList;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using eAdministrationLabs.Services;
using Microsoft.EntityFrameworkCore;


namespace eAdministrationLabs.Controllers
{
    public class RequestController : Controller
    {
        private readonly EAdministrationLabsContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<RequestController> _logger;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;

        public RequestController(EAdministrationLabsContext context, IWebHostEnvironment hostingEnvironment, ILogger<RequestController> logger, EmailService emailService, IWebHostEnvironment env)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _emailService = emailService;
            _env = env;
        }



        public async Task<IActionResult> Index(int? page)
        {
            
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var requests = await _context.Requests
                .Select(r => new RequestViewModel
                {
                    Id = r.Id,
                    LabName = r.Lab.LabName,
                    EquipmentName = r.Equipment != null ? r.Equipment.NameEquipment : "N/A",
                    UserName = r.HistoryRequests.OrderByDescending(h => h.ChangedAt).FirstOrDefault().User.FullName,
                    StatusName = r.HistoryRequests.OrderByDescending(h => h.ChangedAt).FirstOrDefault().StatusRequest.StatusName,
                    ChangedBy = r.HistoryRequests.OrderByDescending(h => h.ChangedAt).FirstOrDefault().ChangedBy,
                    Notes = r.HistoryRequests.OrderByDescending(h => h.ChangedAt).FirstOrDefault().Notes,
                    CreatedAt = r.CreatedAt ?? DateTime.MinValue,
                    ImageBase64 = Convert.ToBase64String(r.Image.Image)
                })
                .ToListAsync();
            PagedList<RequestViewModel> requestViewModel = new PagedList<RequestViewModel>(requests, pageNumber, pageSize);

            return View(requestViewModel);
        }



        public async Task<IActionResult> MyRequest(int? page, string statusFilter = "All")
        {
            int pageSize = 3;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            // Get the currently logged-in user's full name
            var currentFullName = User.Identity?.Name;

            // Get current user's information
            var currentUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.FullName == currentFullName);

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user not found
            }

            // Get the list of all requests from the current user
            var myRequestsQuery = _context.Requests
                .AsNoTracking()
                .Where(r => r.HistoryRequests
                    .OrderByDescending(h => h.ChangedAt)
                    .FirstOrDefault().UserId == currentUser.Id);

            // Apply status filter
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                myRequestsQuery = myRequestsQuery.Where(r => r.HistoryRequests
                    .OrderByDescending(h => h.ChangedAt)
                    .FirstOrDefault().StatusRequest.StatusName == statusFilter);
            }

            // Count total requests and rejected requests
            int totalRequests = await myRequestsQuery.CountAsync();
            int rejectedRequests = await myRequestsQuery
                .CountAsync(r => r.HistoryRequests
                    .OrderByDescending(h => h.ChangedAt)
                    .FirstOrDefault().StatusRequest.StatusName == "Reject");

            // Apply pagination and project to ViewModel
            var myRequests = await myRequestsQuery
                .OrderBy(r => r.Id) // Ensure consistent ordering
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RequestViewModel
                {
                    Id = r.Id,
                    LabName = r.Lab.LabName,
                    EquipmentName = r.Equipment != null ? r.Equipment.NameEquipment : "N/A",
                    StatusName = r.HistoryRequests.OrderByDescending(h => h.ChangedAt).FirstOrDefault().StatusRequest.StatusName,
                    Notes = r.HistoryRequests.OrderByDescending(h => h.ChangedAt).FirstOrDefault().Notes,
                    ImageBase64 = Convert.ToBase64String(r.Image.Image),
                    CreatedAt = r.CreatedAt ?? DateTime.MinValue
                })
                .ToListAsync();

            // Pass the status filter and available statuses to the view
            ViewBag.StatusFilter = statusFilter;
            ViewBag.AvailableStatuses = new List<string> { "All", "Pending", "Approved", "In Progress", "Complete", "Reject" };
            ViewBag.TotalRequests = totalRequests;
            ViewBag.RejectedRequests = rejectedRequests;
            PagedList<RequestViewModel> requestViewModels = new PagedList<RequestViewModel>(myRequests, pageNumber, pageSize);

            return View(requestViewModels);
        }



        public async Task<IActionResult> GetEquipmentsByLab(int labId)
        {
            var equipments = await _context.EquiLabs
                .Where(el => el.LabId == labId)
                .Select(el => new SelectListItem
                {
                    Value = el.EquipmentId.ToString(),
                    Text = el.Equipment.NameEquipment
                })
                .ToListAsync();

            return Json(equipments);  // Return JSON list of equipment for the selected lab
        }



        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Labs = await _context.Labs.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.LabName
            }).ToListAsync();

            ViewBag.Equipments = new List<SelectListItem>();

            // Lấy thông tin tài khoản đang đăng nhập
            var currentUserName = User.Identity?.Name;

            var currentUser = await _context.Users
                .Where(u => u.FullName == currentUserName)
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.FullName
                })
                .FirstOrDefaultAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account"); // Nếu không tìm thấy tài khoản, chuyển hướng về đăng nhập
            }

            ViewBag.CurrentUser = currentUser;

            ViewBag.StatusRequests = await _context.StatusRequests.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.StatusName
            }).ToListAsync();

            return View();
        }




        public async Task<IActionResult> Create(RequestCreateModel model, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Xử lý tệp hình ảnh
                    byte[] imageData = null;
                    if (Image != null && Image.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await Image.CopyToAsync(memoryStream);
                            imageData = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Image", "Hình ảnh không được để trống.");
                        return View(model);
                    }

                    // Lưu hình ảnh
                    var requestImage = new RequestImage
                    {
                        Image = imageData,
                        CreatedAt = DateTime.Now
                    };
                    _context.RequestImages.Add(requestImage);
                    await _context.SaveChangesAsync();

                    // Kiểm tra LabId và các giá trị liên quan
                    if (!_context.Labs.Any(l => l.Id == model.LabId))
                    {
                        ModelState.AddModelError("LabId", "Lab không tồn tại.");
                        return View(model);
                    }

                    // Lưu request
                    var request = new Request
                    {
                        LabId = model.LabId,
                        EquipmentId = model.EquipmentId,
                        ImageId = requestImage.Id,
                        CreatedAt = DateTime.Now
                    };
                    _context.Requests.Add(request);
                    await _context.SaveChangesAsync();

                    // Lưu lịch sử
                    var historyRequest = new HistoryRequest
                    {
                        RequestId = request.Id,
                        UserId = model.UserId,
                        StatusRequestId = 1,
                        ChangedBy = "administrator",
                        ChangedAt = DateTime.Now,
                        Notes = model.Notes
                    };
                    _context.HistoryRequests.Add(historyRequest);
                    await _context.SaveChangesAsync();



                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
                    var userFullName = user?.FullName ?? "Unknown User"; // Nếu không tìm thấy người dùng, sử dụng "Unknown User"


                    // Gửi thông báo cho người tạo request
                    var notificationUser = new Notification
                    {
                        UserId = model.UserId,
                        Message = "Your request has been successfully created.",
                        ReadStatus = "Unread",
                        CreatedAt = DateTime.Now,
                        RequestId = request.Id
                    };
                    _context.Notifications.Add(notificationUser);

                    // Gửi thông báo cho tài khoản có Role "administrator"
                    var adminRoleId = _context.Roles.FirstOrDefault(r => r.RoleName == "administrator")?.Id;
                    if (adminRoleId != null)
                    {
                        // Tìm người dùng có Role "administrator"
                        var adminUserRoles = _context.UserRoles
                            .Where(ur => ur.RoleId == adminRoleId)
                            .ToList();

                        foreach (var userRole in adminUserRoles)
                        {
                            var notificationAdmin = new Notification
                            {
                                UserId = userRole.UserId,
                                Message = $"A new request has been created by {userFullName}.",
                                ReadStatus = "Unread",
                                CreatedAt = DateTime.Now,
                                RequestId = request.Id
                            };
                            _context.Notifications.Add(notificationAdmin);
                        }
                    }

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return RedirectToAction("MyRequest", "Request");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error occurred while creating request: {Message}", ex.Message);
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo yêu cầu. Vui lòng thử lại.");
                }
            }

            return View(model);
        }




        public async Task<IActionResult> Notifications(int? page)
        {
            
            int pageSize = 4;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
           

            var currentFullName = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentFullName))
            {
                return RedirectToAction("login", "Account");
            }

            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.FullName == currentFullName);

            if (currentUser == null)
            {
                return RedirectToAction("login", "Account");
            }

            var notifications = await _context.Notifications
                .Where(n => n.UserId == currentUser.Id)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            var unreadCount = await _context.Notifications
                .Where(n => n.UserId == currentUser.Id && n.ReadStatus == "Unread")
                .CountAsync();

            if (notifications == null)
            {
                notifications = new List<Notification>(); // Initialize an empty list if null
            }

            ViewData["UnreadCount"] = unreadCount; // Pass the unread count to the view
            PagedList<Notification> notification = new PagedList<Notification>(notifications, pageNumber, pageSize);
            return View(notification);
            
        }




        public async Task<IActionResult> Details(int id)
        {
            var request = await _context.Requests
                .Include(r => r.Image)
                .Include(r => r.Lab)
                .Include(r => r.Equipment)
                .Include(r => r.HistoryRequests)
                .ThenInclude(hr => hr.StatusRequest)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.RequestId == id && n.ReadStatus == "Unread");

            if (notification != null)
            {

                notification.ReadStatus = "Read";
                await _context.SaveChangesAsync();
            }


            string imageBase64 = string.Empty;
            if (request.Image != null && request.Image.Image != null)
            {
                try
                {
                    imageBase64 = Convert.ToBase64String(request.Image.Image);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error converting image to Base64: {ex.Message}");
                }
            }


            var latestHistory = request.HistoryRequests.OrderByDescending(h => h.ChangedAt).FirstOrDefault();

            var requestViewModel = new RequestViewModel
            {
                Id = request.Id,
                LabName = request.Lab?.LabName ?? "N/A",
                EquipmentName = request.Equipment?.NameEquipment ?? "N/A",
                StatusName = latestHistory?.StatusRequest?.StatusName ?? "N/A",
                ChangedBy = latestHistory?.ChangedBy ?? "N/A",
                Notes = latestHistory?.Notes ?? "N/A",
                CreatedAt = request.CreatedAt ?? DateTime.MinValue,
                ImageBase64 = imageBase64
            };



            return View(requestViewModel);
        }


        [Authorize]
        public async Task<IActionResult> GetRequestsByChangedBy()
        {
            // Pass the status options to the view
            ViewBag.StatusOptions = _context.StatusRequests.ToList();

            // Get the full name of the currently logged-in user
            var fullName = User.Identity?.Name;

            // Check if the full name is null or empty
            if (string.IsNullOrEmpty(fullName))
            {
                ModelState.AddModelError("", "Không thể xác định tên đầy đủ của tài khoản đang đăng nhập.");
                return View(new List<RequestViewModel>());
            }

            try
            {
                // Fetch the requests and their latest history, filter by ChangedBy, and map to RequestViewModel
                var requests = await _context.Requests
                    .Select(r => new
                    {
                        Request = r,
                        LatestHistory = r.HistoryRequests.OrderByDescending(h => h.ChangedAt).FirstOrDefault()
                    })
                    .Where(x => x.LatestHistory.ChangedBy == fullName)
                    .Select(x => new RequestViewModel
                    {
                        Id = x.Request.Id,
                        LabName = x.Request.Lab.LabName,
                        EquipmentName = x.Request.Equipment != null ? x.Request.Equipment.NameEquipment : "N/A",
                        StatusName = x.LatestHistory.StatusRequest.StatusName,
                        Notes = x.LatestHistory.Notes,
                        ChangedBy = x.LatestHistory.ChangedBy,
                        CreatedAt = x.Request.CreatedAt ?? DateTime.Now,
                        ImageBase64 = x.Request.Image != null ? Convert.ToBase64String(x.Request.Image.Image) : null
                    })
                    .ToListAsync();

                // Order the requests to prioritize "Pending" status
                var orderedRequests = requests
                    .OrderByDescending(r => r.StatusName == "Pending")
                    .ThenBy(r => r.CreatedAt)
                    .ToList();

                // Pass the full name of the user who changed the requests to the view
                ViewData["ChangedBy"] = fullName;
                return View(orderedRequests);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during data fetching
                ModelState.AddModelError("", "Đã xảy ra lỗi khi tải dữ liệu.");
                return View(new List<RequestViewModel>());
            }
        }



        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, int statusId)
        {
            try
            {
                var historyRequest = await _context.HistoryRequests
                    .Include(h => h.Request)
                    .ThenInclude(r => r.Equipment)
                    .Include(h => h.Request)
                    .ThenInclude(r => r.Lab)
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

                            if (status.StatusName == "Approved" || status.StatusName == "In Progress")
                            {
                                subject = $"Your request has been {status.StatusName}";
                                body = $@"
                                        <html>
                                            <head>
                                                <style>
                                                    body {{
                                                        font-family: Arial, sans-serif;
                                                        line-height: 1.6;
                                                    }}
                                                    .email-container {{
                                                        max-width: 600px;
                                                        margin: 0 auto;
                                                        padding: 20px;
                                                        background-color: #f4f4f4;
                                                    }}
                                                    .email-header {{
                                                        text-align: center;
                                                        padding-bottom: 10px;
                                                    }}
                                                    .email-body {{
                                                        background-color: #fff;
                                                        padding: 20px;
                                                        border-radius: 5px;
                                                        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                                                    }}
                                                    .email-footer {{
                                                        text-align: center;
                                                        padding-top: 10px;
                                                        font-size: 12px;
                                                        color: #777;
                                                    }}
                                                </style>
                                            </head>
                                            <body>
                                                <div class='email-container'>
                                                    <div class='email-header'>
                                                        <h2>Request Status Update</h2>
                                                    </div>
                                                    <div class='email-body'>
                                                        <p>Dear {user.FullName},</p>
                                                        <p>We are writing to inform you that your request (ID: {historyRequest.RequestId}) has been updated to <strong>{status.StatusName}</strong>.</p>
                                                        <p>Thank you for using our service. If you have any questions or need further assistance, please do not hesitate to reach out to us.</p>
                                                    </div>
                                                    <div class='email-footer'>
                                                        <p>Best regards,<br/>The Support Team</p>
                                                    </div>
                                                </div>
                                            </body>
                                        </html>";
                            }
                            if (status.StatusName == "Complete")
                            {
                                subject = $"Your request has been {status.StatusName}";
                                body = $@"
                                        <html>
                                            <head>
                                                <style>
                                                    body {{
                                                        font-family: Arial, sans-serif;
                                                        line-height: 1.6;
                                                    }}
                                                    .email-container {{
                                                        max-width: 600px;
                                                        margin: 0 auto;
                                                        padding: 20px;
                                                        background-color: #f4f4f4;
                                                    }}
                                                    .email-header {{
                                                        text-align: center;
                                                        padding-bottom: 10px;
                                                    }}
                                                    .email-body {{
                                                        background-color: #fff;
                                                        padding: 20px;
                                                        border-radius: 5px;
                                                        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                                                    }}
                                                    .email-footer {{
                                                        text-align: center;
                                                        padding-top: 10px;
                                                        font-size: 12px;
                                                        color: #777;
                                                    }}
                                                    .feedback-form {{
                                                        margin-top: 20px;
                                                    }}
                                                    .feedback-form input, .feedback-form select, .feedback-form textarea {{
                                                        width: 100%;
                                                        padding: 10px;
                                                        margin: 10px 0;
                                                        border: 1px solid #ddd;
                                                        border-radius: 4px;
                                                    }}
                                                    .feedback-form button {{
                                                        padding: 10px 20px;
                                                        background-color: #007BFF;
                                                        color: white;
                                                        border: none;
                                                        border-radius: 4px;
                                                        cursor: pointer;
                                                    }}
                                                </style>
                                            </head>
                                            <body>
                                                <div class='email-container'>
                                                    <div class='email-header'>
                                                        <h2>Request Status Update</h2>
                                                    </div>
                                                    <div class='email-body'>
                                                        <p>Dear {user.FullName},</p>
                                                        <p>We are writing to inform you that your request (ID: {historyRequest.RequestId})</strong>.</p>
                                                        <p><strong>Lab Name:</strong> {historyRequest.Request.Lab.LabName}</p>
                                                        <p><strong>Equipment Name:</strong> {historyRequest.Request.Equipment.NameEquipment}</p>
                                                        <p><strong>has been updated to <strong>{status.StatusName}</p>
                                                       


                                                        <p>Please take a moment to provide your feedback:</p>
                                                      <p><a href='https://localhost:7085/Feedback/Index?RequestId={historyRequest.RequestId}'>Click here to provide your feedback</a></p>
                                                        <p>Thank you for using our service. If you have any questions or need further assistance, please do not hesitate to reach out to us.</p>
                                                    </div>
                                                    <div class='email-footer'>
                                                        <p>Best regards,<br/>The Support Team</p>
                                                    </div>
                                                </div>
                                            </body>
                                        </html>";
                            }
                            else if (status.StatusName == "Reject")
                            {
                                subject = "Your request has been rejected";
                                body = $@"
                                <html>
                                    <head>
                                        <style>
                                            body {{
                                                font-family: Arial, sans-serif;
                                                line-height: 1.6;
                                            }}
                                            .email-container {{
                                                max-width: 600px;
                                                margin: 0 auto;
                                                padding: 20px;
                                                background-color: #f4f4f4;
                                            }}
                                            .email-header {{
                                                text-align: center;
                                                padding-bottom: 10px;
                                            }}
                                            .email-body {{
                                                background-color: #fff;
                                                padding: 20px;
                                                border-radius: 5px;
                                                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                                            }}
                                            .email-footer {{
                                                text-align: center;
                                                padding-top: 10px;
                                                font-size: 12px;
                                                color: #777;
                                            }}
                                        </style>
                                    </head>
                                    <body>
                                        <div class='email-container'>
                                            <div class='email-header'>
                                                <h2>Request Status Update</h2>
                                            </div>
                                            <div class='email-body'>
                                                <p>Dear {user.FullName},</p>
                                                <p>We regret to inform you that your request (ID: {historyRequest.RequestId}) has been <strong>rejected</strong>.</p>
                                                <p>If you have any questions or would like to discuss the details of the rejection, please do not hesitate to contact our support team.</p>
                                            </div>
                                            <div class='email-footer'>
                                                <p>Best regards,<br/>The Support Team</p>
                                            </div>
                                        </div>
                                    </body>
                                </html>";
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


        [HttpGet]
        public IActionResult IndexCompletion()
        {
            var requestCompletions = _context.RequestCompletions
                .Select(rc => new RequestCompletion
                {
                    Id = rc.Id,
                    HistoryRequestId = rc.HistoryRequestId,
                    CompletedBy = rc.CompletedBy,
                    CompletionTime = rc.CompletionTime,
                    ImageBase64 = rc.ImageBase64
                })
                .ToList();

            return View(requestCompletions);
        }


        [HttpGet]
        [Route("Request/CreateCompletion/{historyRequestId}")]
        public IActionResult CreateCompletion(int historyRequestId)
        {
            var historyRequest = _context.HistoryRequests
                .Include(h => h.Request)
                .FirstOrDefault(h => h.Id == historyRequestId);

            if (historyRequest == null || historyRequest.Request == null)
            {
                return NotFound();
            }

            var request = historyRequest.Request;
            var lab = _context.Labs.FirstOrDefault(l => l.Id == request.LabId);
            var equipment = _context.Equipments.FirstOrDefault(e => e.Id == request.EquipmentId);


            var viewModel = new CompletionViewModel
            {
                HistoryRequestId = historyRequestId,
                LabName = lab?.LabName,
                NameEquipment = equipment?.NameEquipment,
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCompletion(CompletionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Kiểm tra lỗi trong ModelState và trả về thông báo
                var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ModelState.AddModelError("", "Model validation failed: " + errors);
                return View(model); // Trả về view với lỗi
            }

            var historyRequest = _context.HistoryRequests
                .Include(h => h.Request)
                .FirstOrDefault(h => h.Id == model.HistoryRequestId);

            if (historyRequest == null || historyRequest.Request == null)
            {
                ModelState.AddModelError("", "Không tìm thấy HistoryRequest.");
                return View(model); // Kiểm tra xem có bị null không.
            }

            // Tiến hành lưu file (nếu có)
            string filePath = null;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                try
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ImageFile", "Lỗi tải lên tệp: " + ex.Message);
                    return View(model); // Trả về view nếu có lỗi tải file.
                }
            }

            var requestCompletion = new RequestCompletion
            {
                HistoryRequestId = model.HistoryRequestId,
                CompletedBy = User.Identity.Name,
                CompletionTime = DateTime.Now, // Hoặc có thể lấy từ thời gian người dùng đã chọn
                ImageBase64 = filePath != null ? Convert.ToBase64String(System.IO.File.ReadAllBytes(filePath)) : null, // Lưu hình ảnh dưới dạng base64
            };

            try
            {
                _context.RequestCompletions.Add(requestCompletion);
                await _context.SaveChangesAsync();
                return RedirectToAction("GetRequestsByChangedBy"); // Thành công, chuyển hướng.
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi lưu dữ liệu: " + ex.Message);
                return View(model); // Xử lý lỗi.
            }
        }


    }
}
