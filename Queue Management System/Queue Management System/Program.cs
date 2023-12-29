using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;
using Queue_Management_System.Services;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Configuration for PostgreSQL Connection String
var connectionString = builder.Configuration.GetConnectionString("MyPostgresConnection");

// Register NpgsqlConnectionFactory with the connection string
builder.Services.AddScoped(provider => new NpgsqlConnectionFactory(connectionString));

// Register DbOperationsRepository and other services
builder.Services.AddScoped<DbOperationsRepository>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IServicePointOperations, ServicePointOperations>();

// JWT Authentication setup
var issuer = UserUtility.issuer;
var secretKey = UserUtility.secretKey;

builder.Services.AddSingleton<JwtAuthenticationService>(_ =>
    new JwtAuthenticationService(builder.Configuration, issuer));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "ServiceUser";
})
.AddCookie("ServiceProvider")
.AddCookie("ServiceUser")
.AddJwtBearer("JwtBearerScheme", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ServiceProvider", policy =>
    {
        policy.AuthenticationSchemes.Add("ServiceProvider"); // Specify the authentication scheme
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context => UserUtility.IsAuthorized);
        policy.RequireClaim(ClaimTypes.Role, "ServiceProvider");
        policy.RequireClaim(ClaimTypes.Name); // Adding additional claim requirement
    });

    options.AddPolicy("Admin", policy =>
    {
        policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme); // Use CookieAuthentication scheme
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context => UserUtility.IsAdmin);
        policy.RequireClaim(ClaimTypes.Role, "Admin");
    });
});




var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});

app.Run();
