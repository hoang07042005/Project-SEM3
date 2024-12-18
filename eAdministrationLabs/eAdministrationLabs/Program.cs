using eAdministrationLabs.Models;
using eAdministrationLabs.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// C?u hình d?ch v?
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EAdministrationLabsContext>(options =>
    options.UseSqlServer(connectionString));

// Thêm b? nh? ??m và c?u hình session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ??ng ký EmailService
builder.Services.AddTransient<EmailService>();

// C?u hình tùy ch?n cho x? lý form
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});

// C?u hình xác th?c và phân quy?n
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("administrator"));
});

var app = builder.Build();

// C?u hình pipeline x? lý HTTP request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

// Middleware xác th?c và phân quy?n
app.UseAuthentication(); // ??m b?o middleware authentication ch?y tr??c
app.UseAuthorization();  // ??m b?o middleware authorization ch?y sau

// ??nh tuy?n các controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=HomeAdmin}/{action=Index}/{id?}")
    .RequireAuthorization("AdminOnly"); // Yêu c?u phân quy?n cho admin

// Ch?y ?ng d?ng
app.Run();
