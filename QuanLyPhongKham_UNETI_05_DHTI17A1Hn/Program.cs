using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Data;

using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Common.Guards;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Account.Services;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Auth.Services;
using QuanLyPhongKham_UNETI_05_DHTI17A1Hn.Modules.Specialty.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ServiceExceptionFilter>();
});

// Cau hinh DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Cau hinh Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();

builder.Services.AddScoped<ServiceExceptionFilter>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name = ".QuanLyPhongKham.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAdminAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Chua can HTTPS: tam tat HSTS de chay HTTP thuan.
    // Khi can HTTPS thi mo lai: app.UseHsts();
}

// Chua can HTTPS: tam tat redirect de chay HTTP thuan.
// Khi can HTTPS thi mo lai: app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

// Phai co UseSession truoc Controller
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
