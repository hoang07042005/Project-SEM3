using eAdministrationLabs;
using eAdministrationLabs.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
string ketnoi = builder.Configuration.GetConnectionString("DefaultConnection");


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EAdministrationLabsContext>(options =>
{
    options.UseSqlServer(ketnoi);
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
