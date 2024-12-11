using eAdministrationLabs.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using eAdministrationLabs.Models.ViewModels;
using eAdministrationLabs.Services;

namespace eAdministrationLabs.Controllers;

public class AccountController : Controller
{
    private readonly EAdministrationLabsContext _context;
    private readonly EmailService _emailService;

    public AccountController(EAdministrationLabsContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.Roles = _context.Roles.ToList();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userExists = _context.Users.Any(u => u.Email == model.Email);
            if (userExists)
            {
                ModelState.AddModelError("", "Email already registered.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            foreach (var roleId in model.SelectedRoleIds)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId
                };
                _context.UserRoles.Add(userRole);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        ViewBag.Roles = _context.Roles.ToList();
        return View(model);
    }

    [HttpGet]
    public IActionResult Login() => View();



    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Kiểm tra người dùng có trong cơ sở dữ liệu không
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                // Tạo Claims cho người dùng
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                    };

                // Lấy danh sách các role của người dùng
                var roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();

                // Thêm Claims cho các role
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // Duy trì session đăng nhập
                };

                // Đăng nhập người dùng và lưu vào cookie
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetInt32("UserID", user.Id);
                HttpContext.Session.SetString("Roles", string.Join(",", roles));

                if (roles.Contains("administrator") || roles.Contains("HOD") || roles.Contains("instructors") || roles.Contains("technicalstaff"))
                {
                    return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
                }
                else if (roles.Contains("students"))
                {
                    return RedirectToAction("Index", "Home");
                }

            }

            // Nếu đăng nhập thất bại
            ModelState.AddModelError("", "Invalid login attempt.");
        }

        return View(model);
    }

    
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View(model);
            }

            // Cập nhật mật khẩu mới
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            ViewBag.Message = "Password changed successfully.";
            return View();
        }

        return View(model);
    }






    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user != null)
            {
                var token = Guid.NewGuid().ToString();
                user.PasswordResetToken = token;
                user.TokenExpirationTime = DateTime.UtcNow.AddHours(1);
                await _context.SaveChangesAsync();

                var resetLink = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);
                var emailBody = $"Click vào đây để đặt lại mật khẩu: <a href='{resetLink}'>Đặt lại mật khẩu</a>";
                await _emailService.SendEmailAsync(user.Email, "Đặt lại mật khẩu", emailBody);

                ViewBag.Message = "Email đặt lại mật khẩu đã được gửi.";
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            ModelState.AddModelError("", "Không tìm thấy email.");
        }

        return View(model);
    }


    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }


    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login");
        }

        var model = new ResetPasswordViewModel { Token = token, Email = email };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user != null && user.PasswordResetToken == model.Token && user.TokenExpirationTime > DateTime.UtcNow)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                user.PasswordResetToken = null;
                user.TokenExpirationTime = null;
                await _context.SaveChangesAsync();

                ViewBag.Message = "Mật khẩu đã được đặt lại thành công.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Token không hợp lệ hoặc đã hết hạn.");
        }

        return View(model);
    }

}





