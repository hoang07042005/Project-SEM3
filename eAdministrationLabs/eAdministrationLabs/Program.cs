using eAdministrationLabs.Models;
using eAdministrationLabs.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// C?u h�nh d?ch v?
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EAdministrationLabsContext>(options =>
    options.UseSqlServer(connectionString));

// Th�m b? nh? ??m v� c?u h�nh session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ??ng k� EmailService
builder.Services.AddTransient<EmailService>();

// C?u h�nh t�y ch?n cho x? l� form
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});

// C?u h�nh x�c th?c v� ph�n quy?n
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

// C?u h�nh pipeline x? l� HTTP request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

// Middleware x�c th?c v� ph�n quy?n
app.UseAuthentication(); // ??m b?o middleware authentication ch?y tr??c
app.UseAuthorization();  // ??m b?o middleware authorization ch?y sau

// ??nh tuy?n c�c controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=HomeAdmin}/{action=Index}/{id?}")
    .RequireAuthorization("AdminOnly"); // Y�u c?u ph�n quy?n cho admin

// Ch?y ?ng d?ng
app.Run();
