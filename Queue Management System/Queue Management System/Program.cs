using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Queue_Management_System.Repository;
using Queue_Management_System.ServiceInterface;
using Queue_Management_System.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();


builder.Services.AddSingleton<JwtAuthenticationService>(_ =>
    new JwtAuthenticationService(builder.Configuration, "Admin"));

// Add authentication services
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie("ServiceUser")
  .AddJwtBearer(options =>
  {
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = false,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = "Admin",
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("NVRBpevqncT8sw8gZMysnaRJ3Ww/ha6c9evS3TzcQBU="))
      };
  });


// Configuration for PostgreSQL Connection String
var connectionString = builder.Configuration.GetConnectionString("MyPostgresConnection");

// Register NpgsqlConnectionFactory with the connection string
builder.Services.AddScoped(provider => new NpgsqlConnectionFactory(connectionString));

// Register DbOperationsRepository
builder.Services.AddScoped<DbOperationsRepository>();

// Register other services
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IServiceProviderRepository, ServiceProviderRepository>();
builder.Services.AddScoped<IServicePointOperations, ServicePointOperations>();

// Register JwtAuthenticationService
builder.Services.AddSingleton<JwtAuthenticationService>(_ =>
    new JwtAuthenticationService(builder.Configuration, "Admin"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
