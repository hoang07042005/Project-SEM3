
using eAdministrationLabs.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// C?u hình d?ch v?
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EAdministrationLabsContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("administrator", "HOD", "technicalstaff", "instructors"));
});


var app = builder.Build();

// C?u hình pipeline cho HTTP request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

// Middleware cho xác th?c và phân quy?n
app.UseAuthentication(); // ??m b?o middleware authentication ch?y tr??c
app.UseAuthorization();  // ??m b?o middleware authorization ch?y sau

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "admin/{controller=HomeAdmin}/{action=Index}/{id?}")
    .RequireAuthorization();

app.Run();
