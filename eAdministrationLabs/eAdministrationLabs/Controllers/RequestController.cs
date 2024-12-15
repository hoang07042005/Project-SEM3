using Microsoft.AspNetCore.Mvc;
using eAdministrationLabs.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eAdministrationLabs.Models.ViewModels;
using X.PagedList;


namespace eAdministrationLabs.Controllers
{
    public class RequestController : Controller
    {
        private readonly EAdministrationLabsContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<RequestController> _logger;

        public RequestController(EAdministrationLabsContext context, IWebHostEnvironment hostingEnvironment, ILogger<RequestController> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }


        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 2;
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

        public async Task<IActionResult> MyRequest(string statusFilter = "All")
        {
            // Lấy thông tin tài khoản đang đăng nhập
            var currentFullName = User.Identity?.Name;

            // Lấy thông tin User ID
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.FullName == currentFullName);

            if (currentUser == null)
            {
                return RedirectToAction("Index", "Home"); // Nếu không tìm thấy tài khoản, chuyển hướng về trang chính
            }

            // Lấy danh sách tất cả requests của người dùng hiện tại
            var myRequestsQuery = _context.Requests
                .Where(r => r.HistoryRequests
                    .OrderByDescending(h => h.ChangedAt)
                    .FirstOrDefault().UserId == currentUser.Id);

            // Áp dụng bộ lọc theo trạng thái
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                myRequestsQuery = myRequestsQuery.Where(r => r.HistoryRequests
                    .OrderByDescending(h => h.ChangedAt)
                    .FirstOrDefault().StatusRequest.StatusName == statusFilter);
            }

            // Truy vấn và chuyển đổi sang ViewModel
            var myRequests = await myRequestsQuery
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

            // Truyền dữ liệu bộ lọc trạng thái đến View
            ViewBag.StatusFilter = statusFilter;
            ViewBag.AvailableStatuses = new List<string> { "All", "Complete", "Pending", "Approved", "Reject" };

            return View(myRequests);
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
                        StatusRequestId = model.StatusRequestId,
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




        public async Task<IActionResult> Notifications()
        {
            
                var currentFullName = User.Identity?.Name;
                if (string.IsNullOrEmpty(currentFullName))
                {
                    return RedirectToAction("Index", "Home");
                }

                var currentUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.FullName == currentFullName);

                if (currentUser == null)
                {
                    return RedirectToAction("Index", "Home");
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

                return View(notifications);
            
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

        

    }
}
