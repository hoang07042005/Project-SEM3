﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eAdministrationLabs.Models;
using eAdministrationLabs.Dtos.Create;
using eAdministrationLabs.Dtos.Edit;
using System.Security.Claims;
using eAdministrationLabs.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace eAdministrationLabs.Controllers
{
    
    public class LabUsageLogsController : Controller
    {
        private readonly EAdministrationLabsContext _context;

        public LabUsageLogsController(EAdministrationLabsContext context)
        {
            _context = context;
        }

        // GET: LabUsageLogs
        public async Task<IActionResult> Index()
        {
            var eAdministrationLabsContext = _context.LabUsageLogs.Include(l => l.Lab).Include(l => l.User);
            return View(await eAdministrationLabsContext.ToListAsync());
        }

        // GET: LabUsageLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
               .FirstOrDefaultAsync(n => n.LabUsageLogId == id && n.ReadStatus == "Unread");

            if (notification != null)
            {

                notification.ReadStatus = "Read";
                await _context.SaveChangesAsync();
            }

            var labUsageLog = await _context.LabUsageLogs
                .Include(l => l.Lab)
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (labUsageLog == null)
            {
                return NotFound();
            }

            return View(labUsageLog);
        }

        // GET: LabUsageLogs/Create
        [Authorize]
        public IActionResult Create()
        {
            // Lấy full name của người dùng hiện tại
            var loggedUserFullName = User.Identity.Name;

            // Truy vấn để lấy người dùng từ full name và tạo SelectList cho Users
            var loggedUser = _context.Users
                .Where(u => u.FullName == loggedUserFullName)
                .Select(u => new { u.Id, u.FullName })
                .FirstOrDefault();

            var labs = _context.Labs
                .Where(l => l.StatusLabId == 2)
                .Select(l => new { l.Id, l.LabName })
                .ToList();


            var viewModel = new CreateLabUsageLogViewModel
            {
                Labs = new SelectList(labs, "Id", "LabName"),
                Users = new SelectList(new[] { loggedUser }, "Id", "FullName", loggedUser?.Id)
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLabUsageLogDto createLabUsageLogDto)
        {
            if (ModelState.IsValid)
            {
                // Lấy ID của người dùng từ FullName của người dùng hiện tại
                var loggedUserFullName = User.Identity.Name;
                var loggedUser = _context.Users
                    .Where(u => u.FullName == loggedUserFullName)
                    .Select(u => new { u.Id, u.FullName })
                    .FirstOrDefault();

                if (loggedUser != null) // Kiểm tra xem loggedUser có tồn tại không
                {
                    LabUsageLog createLabUsageLog = new LabUsageLog()
                    {
                        LabId = createLabUsageLogDto.LabId,
                        UserId = loggedUser.Id, // Sử dụng trực tiếp ID từ loggedUser
                        Purpose = createLabUsageLogDto.Purpose,
                        StartTime = createLabUsageLogDto.StartTime,
                        EndTime = createLabUsageLogDto.EndTime
                    };

                    _context.Add(createLabUsageLog);
                    await _context.SaveChangesAsync();

                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == createLabUsageLogDto.UserId);
                    var userFullName = user?.FullName ?? "Unknown User"; // Nếu không tìm thấy người dùng, sử dụng "Unknown User"


                    // Tạo Notification sau khi tạo LabUsageLog thành công
                    var notification = new Notification
                    {
                        UserId = loggedUser.Id,
                        Message = $"Congratulations, {userFullName}! Your room booking has been confirmed. We look forward to hosting you.",
                        ReadStatus = "Unread", // Mặc định là chưa đọc
                        CreatedAt = DateTime.UtcNow,
                        LabUsageLogId = createLabUsageLog.Id
                    };

                    _context.Add(notification);
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
                                Message = $"A new lab has been created by {userFullName}.",
                                ReadStatus = "Unread",
                                CreatedAt = DateTime.Now,
                                LabUsageLogId = createLabUsageLog.Id
                            };
                            _context.Notifications.Add(notificationAdmin);
                        }
                    }

                    
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            // Nếu model không hợp lệ hoặc không tìm thấy user
            var viewModel = new CreateLabUsageLogViewModel
            {
                Labs = new SelectList(_context.Labs
                        .Where(l => l.StatusLabId == 2)
                        .Select(l => new { l.Id, l.LabName }), "Id", "LabName", createLabUsageLogDto.LabId),
                Users = new SelectList(_context.Users
                          .Where(u => u.FullName == User.Identity.Name)
                          .Select(u => new { u.Id, u.FullName }), "Id", "FullName", createLabUsageLogDto.UserId)
            };

            return View(viewModel);
        }


        // GET: LabUsageLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labUsageLog = await _context.LabUsageLogs.FindAsync(id);
            if (labUsageLog == null)
            {
                return NotFound();
            }
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", labUsageLog.LabId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", labUsageLog.UserId);
            return View(labUsageLog);
        }

        // POST: LabUsageLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //LabId,UserId,Purpose,StartTime,EndTime,CreatedAt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditLabUsageLogDto editLabUsageLogDto)
        {
            if (id != editLabUsageLogDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    var editLabUsageLog = await _context.LabUsageLogs.FindAsync(id);
                    if (editLabUsageLog == null)
                    {
                        return NotFound();
                    }

                    editLabUsageLog.UserId = editLabUsageLogDto.UserId;
                    editLabUsageLog.LabId = editLabUsageLogDto.LabId;
                    editLabUsageLog.Purpose = editLabUsageLogDto.Purpose;
                    editLabUsageLog.StartTime = editLabUsageLogDto.StartTime;
                    editLabUsageLog.EndTime = editLabUsageLogDto.EndTime;
                    editLabUsageLog.CreatedAt = editLabUsageLogDto.CreatedAt;


                    _context.Update(editLabUsageLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LabUsageLogExists(id))
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
            ViewData["LabId"] = new SelectList(_context.Labs, "Id", "LabName", editLabUsageLogDto.LabId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "FullName", editLabUsageLogDto.UserId);
            return View(editLabUsageLogDto);
        }

        // GET: LabUsageLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var labUsageLog = await _context.LabUsageLogs
                .Include(l => l.Lab)
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (labUsageLog == null)
            {
                return NotFound();
            }

            return View(labUsageLog);
        }

        // POST: LabUsageLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var labUsageLog = await _context.LabUsageLogs.FindAsync(id);
            if (labUsageLog != null)
            {
                _context.LabUsageLogs.Remove(labUsageLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LabUsageLogExists(int id)
        {
            return _context.LabUsageLogs.Any(e => e.Id == id);
        }
    }
}