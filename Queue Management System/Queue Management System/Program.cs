using Microsoft.AspNetCore.Authentication.Cookies;
using Npgsql;
using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;
using Queue_Management_System.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddAuthentication();
builder.Services.AddAuthentication(options=> {
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme= CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme= CookieAuthenticationDefaults.AuthenticationScheme;
    }
).AddCookie("ServiceUser");

// Configuration for PostgreSQL Connection String
var connectionString = builder.Configuration.GetConnectionString("MyPostgresConnection");

// Register NpgsqlConnectionFactory with the connection string
builder.Services.AddScoped(provider => new NpgsqlConnectionFactory(connectionString));

// Register DbOperationsRepository
builder.Services.AddScoped<DbOperationsRepository>();

// Register TicketService
builder.Services.AddScoped<ITicketService, TicketService>();

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
