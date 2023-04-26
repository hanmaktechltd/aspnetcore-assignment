using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Queue_Management_System.Hubs;
using Queue_Management_System.Repositories;
using Queue_Management_System.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IAdminRepository, AdminRepository>();
builder.Services.AddSingleton<IServicePointRepository, ServicePointRepository>();
builder.Services.AddSingleton<ICheckInRepository, CheckInRepository>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie("ServicePointAuthentication", options =>
{
    options.LoginPath = "/ServicePoint/Login";
    options.LogoutPath = "/ServicePoint/Logout";
})
.AddCookie("AdminAuthentication", options =>
{
    options.LoginPath = "/Admin/Login";
    options.LogoutPath = "/Admin/Logout";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("admin");
    });
    options.AddPolicy("ServiceProvider", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("serviceProvider");
    });
});


// Add session middleware
builder.Services.AddSession(options =>
{
    // Set session timeout to 20 minutes
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// telling our application it has to use the Db Context
// builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
//     builder.Configuration.GetConnectionString("DefaultConnection")  // configures our sql server
//     ));
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

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

// Use session middleware
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "admin",
        pattern: "/Admin/Authenticated",
        defaults: new { controller = "Admin", action = "Authenticated" })
        .RequireAuthorization("AdminPolicy");

    endpoints.MapControllerRoute(
        name: "servicePoint",
        pattern: "/ServicePoint/SelectService",
        defaults: new { controller = "ServicePoint", action = "SelectService" })
        .RequireAuthorization("ServiceProvider");

    endpoints.MapHub<QueueHub>("/queueHub");
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=CheckIn}/{action=Index}/{id?}");

app.Run();
