using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Queue_Management_System.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(
  o=>o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))  
    );
builder.Services.AddRazorPages();
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
    name: "CheckIn",
    pattern: "CheckIn/{action}",
    defaults: new { controller = "CheckIn" });
app.MapControllerRoute(
    name: "WaitingPage",
    pattern: "WaitingPage/{action}",
    defaults: new { controller = "WaitingPage" });
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// Map the root URL to the CheckIn page
/*app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );
});*/

// Map the root URL to the InsertCustomer view
/*app.MapGet("/", async (HttpContext context) =>
{
    context.Response.Redirect("/Home/CheckIn");
});*/
//app.MapRazorPages();
app.Run();
