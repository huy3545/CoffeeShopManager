using CoffeeShopManager.DAL.Data;
using CoffeeShopManager.DAL.Repositories;
using CoffeeShopManager.BLL.Services;
using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Chỉ dùng console logging để ứng dụng chạy ổn định trên máy local và môi trường triển khai.
// Tránh Windows Event Log yêu cầu quyền quản trị khi ứng dụng ghi lỗi khởi động.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<CoffeeShopDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/access-denied";
    options.SlidingExpiration = true;
});
builder.Services.AddAuthorization();

// Lưu khóa Data Protection trong thư mục riêng của ứng dụng để cookie Identity
// hoạt động ổn định khi chạy local. Khi triển khai production nên thay bằng
// kho khóa dùng chung như Azure Key Vault hoặc Redis.
var dataProtectionKeyDirectory = Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys");
Directory.CreateDirectory(dataProtectionKeyDirectory);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeyDirectory))
    .SetApplicationName("CoffeeShopManager.Web");

// Add DbContext with dynamic path resolution
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("Data Source="))
{
    var dbPath = connectionString.Replace("Data Source=", "").Trim();
    
    // Always resolve to absolute path to ensure we use the correct database
    if (!Path.IsPathRooted(dbPath))
    {
        // Get the directory where the .sln file is located (solution root)
        var currentDir = Directory.GetCurrentDirectory();
        var solutionRoot = currentDir;
        
        // Navigate up to find solution root (where .sln file is located)
        // Start from current working directory (usually the project folder when running dotnet run)
        for (int i = 0; i < 8; i++)
        {
            var slnFiles = Directory.GetFiles(solutionRoot, "*.sln");
            if (slnFiles.Length > 0)
            {
                // Found .sln file, this is the solution root
                break;
            }
            
            var parent = Directory.GetParent(solutionRoot);
            if (parent == null)
            {
                // Can't go up further, try using AppContext.BaseDirectory instead
                var baseDir = AppContext.BaseDirectory;
                // Navigate up from bin/Debug/net9.0 to solution root
                solutionRoot = baseDir;
                for (int j = 0; j < 6; j++)
                {
                    var slnFiles2 = Directory.GetFiles(solutionRoot, "*.sln");
                    if (slnFiles2.Length > 0) break;
                    var parent2 = Directory.GetParent(solutionRoot);
                    if (parent2 == null) break;
                    solutionRoot = parent2.FullName;
                }
                break;
            }
            solutionRoot = parent.FullName;
        }
        
        // ALWAYS use the database from API folder - this is the source of truth
        var apiDbPath = Path.Combine(solutionRoot, "CoffeeShopManager.API", "CoffeeShopDB.db");
        
        // Verify the database file exists and has content
        if (File.Exists(apiDbPath))
        {
            var fileInfo = new FileInfo(apiDbPath);
            if (fileInfo.Length > 0)
            {
                dbPath = apiDbPath;
            }
            else
            {
                // Database file exists but is empty - this shouldn't happen
                Console.WriteLine($"WARNING: Database file exists but is empty: {apiDbPath}");
                dbPath = apiDbPath; // Still use it, but log warning
            }
        }
        else
        {
            // Database doesn't exist - this is a problem
            Console.WriteLine($"ERROR: Database file not found at: {apiDbPath}");
            Console.WriteLine("Please run: dotnet ef database update --project CoffeeShopManager.DAL --startup-project CoffeeShopManager.API");
            // Use API path anyway - will fail with clear error
            dbPath = apiDbPath;
        }
        
        // Normalize path separators
        dbPath = Path.GetFullPath(dbPath).Replace('\\', Path.DirectorySeparatorChar);
        
        // Log for debugging (only in development)
        if (builder.Environment.IsDevelopment())
        {
            Console.WriteLine("=== Database Path Resolution ===");
            Console.WriteLine($"Current Directory: {currentDir}");
            Console.WriteLine($"Solution Root: {solutionRoot}");
            Console.WriteLine($"Original DB Path: {connectionString.Replace("Data Source=", "").Trim()}");
            Console.WriteLine($"Final DB Path: {dbPath}");
            Console.WriteLine($"DB File Exists: {File.Exists(dbPath)}");
            if (File.Exists(dbPath))
            {
                var fileInfo = new FileInfo(dbPath);
                Console.WriteLine($"DB File Size: {fileInfo.Length} bytes");
                Console.WriteLine($"DB File Last Modified: {fileInfo.LastWriteTime}");
                
                // Verify database has tables
                try
                {
                    using var testConn = new SqliteConnection($"Data Source={dbPath}");
                    testConn.Open();
                    using var cmd = testConn.CreateCommand();
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Ban';";
                    var result = cmd.ExecuteScalar();
                    Console.WriteLine($"Table 'Ban' exists: {result != null}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking database: {ex.Message}");
                }
            }
            Console.WriteLine("================================");
        }
        
        connectionString = $"Data Source={dbPath}";
    }
}

builder.Services.AddDbContext<CoffeeShopDbContext>(options =>
{
    options.UseSqlite(connectionString);
    // Disable automatic database creation - we use migrations
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});

// Add Repositories
builder.Services.AddScoped<IBanRepository, BanRepository>();
builder.Services.AddScoped<IMonRepository, MonRepository>();
builder.Services.AddScoped<IHoaDonRepository, HoaDonRepository>();
builder.Services.AddScoped<IChiTietHDRepository, ChiTietHDRepository>();
builder.Services.AddScoped<INhanVienRepository, NhanVienRepository>();
builder.Services.AddScoped<IKhachHangRepository, KhachHangRepository>();
builder.Services.AddScoped<IKhuyenMaiRepository, KhuyenMaiRepository>();
builder.Services.AddScoped<IHoaDonKhuyenMaiRepository, HoaDonKhuyenMaiRepository>();
builder.Services.AddScoped<IDatBanRepository, DatBanRepository>();
builder.Services.AddScoped<INhatKyHoatDongRepository, NhatKyHoatDongRepository>();

// Add Services
builder.Services.AddScoped<IBanService, BanService>();
builder.Services.AddScoped<IMonService, MonService>();
builder.Services.AddScoped<IHoaDonService, HoaDonService>();
builder.Services.AddScoped<INhanVienService, NhanVienService>();
builder.Services.AddScoped<IKhachHangService, KhachHangService>();
builder.Services.AddScoped<IThongKeService, ThongKeService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IKhuyenMaiService, KhuyenMaiService>();
builder.Services.AddScoped<IDatBanService, DatBanService>();
builder.Services.AddScoped<INhatKyHoatDongService, NhatKyHoatDongService>();

// Add HttpClient for API calls
builder.Services.AddHttpClient();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CoffeeShopDbContext>();
    await db.Database.MigrateAsync();
    if (app.Environment.IsDevelopment())
    {
        await SeedIdentityAsync(scope.ServiceProvider);
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/Account/Login", async (HttpRequest request,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    INhatKyHoatDongService auditService) =>
{
    var form = await request.ReadFormAsync();
    var username = form["username"].ToString().Trim();
    var password = form["password"].ToString();
    var returnUrl = form["returnUrl"].ToString();
    var target = IsLocalUrl(returnUrl) ? returnUrl : "/";

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        await TryWriteAuditAsync(auditService, "Đăng nhập", "Xác thực", null, $"Đăng nhập thất bại cho tài khoản {username}: thiếu thông tin.", "Thất bại");
        return LoginError("Vui lòng nhập đầy đủ tài khoản và mật khẩu.");
    }

    var user = await userManager.FindByNameAsync(username) ?? await userManager.FindByEmailAsync(username);
    if (user is null)
    {
        await TryWriteAuditAsync(auditService, "Đăng nhập", "Xác thực", null, $"Đăng nhập thất bại cho tài khoản {username}.", "Thất bại");
        return LoginError("Tài khoản hoặc mật khẩu không chính xác.");
    }

    var result = await signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
    if (result.Succeeded)
    {
        var roles = await userManager.GetRolesAsync(user);
        await TryWriteAuditAsync(auditService, "Đăng nhập", "Xác thực", user.Id, $"Đăng nhập thành công bằng tài khoản {user.UserName}.", "Thành công", user.UserName, roles.FirstOrDefault());
    }
    else
    {
        await TryWriteAuditAsync(auditService, "Đăng nhập", "Xác thực", user.Id, $"Đăng nhập thất bại cho tài khoản {user.UserName}.", "Thất bại", user.UserName);
    }
    return result.Succeeded
        ? Results.Redirect(target)
        : LoginError("Tài khoản hoặc mật khẩu không chính xác.");
});

app.MapGet("/Account/Logout", async (SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    INhatKyHoatDongService auditService) =>
{
    var user = await userManager.GetUserAsync(signInManager.Context.User);
    if (user is not null)
    {
        var roles = await userManager.GetRolesAsync(user);
        await TryWriteAuditAsync(auditService, "Đăng xuất", "Xác thực", user.Id, $"Đăng xuất tài khoản {user.UserName}.", "Thành công", user.UserName, roles.FirstOrDefault());
    }
    await signInManager.SignOutAsync();
    return Results.Redirect("/login");
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

static bool IsLocalUrl(string? url) =>
    !string.IsNullOrWhiteSpace(url) && url.StartsWith('/') && !url.StartsWith("//") && !url.StartsWith("/\\");

static IResult LoginError(string message) =>
    Results.Redirect($"/login?Error={Uri.EscapeDataString(message)}");

static async Task TryWriteAuditAsync(INhatKyHoatDongService auditService, string action, string objectType, string? objectId, string description, string result, string? userName = null, string? role = null)
{
    try
    {
        await auditService.GhiNhanAsync(new GhiNhatKyDto
        {
            MaNguoiDung = objectId,
            TenNguoiDung = userName,
            VaiTro = role,
            HanhDong = action,
            LoaiDoiTuong = objectType,
            MaDoiTuong = objectId,
            MoTa = description,
            KetQua = result
        });
    }
    catch
    {
        // Không để lỗi ghi nhật ký làm gián đoạn đăng nhập hoặc đăng xuất.
    }
}

static async Task SeedIdentityAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    foreach (var role in new[] { "Admin", "Manager", "Staff" })
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
            if (!roleResult.Succeeded)
                throw new InvalidOperationException(string.Join("; ", roleResult.Errors.Select(e => e.Description)));
        }
    }

    var accounts = new[]
    {
        new { UserName = "admin", Email = "admin@coffee.com", Role = "Admin", MaNV = (int?)1 },
        new { UserName = "manager", Email = "manager@coffee.com", Role = "Manager", MaNV = (int?)2 },
        new { UserName = "staff", Email = "staff@coffee.com", Role = "Staff", MaNV = (int?)3 }
    };

    foreach (var account in accounts)
    {
        if (await userManager.FindByNameAsync(account.UserName) is not null)
            continue;

        var user = new ApplicationUser
        {
            UserName = account.UserName,
            Email = account.Email,
            EmailConfirmed = true,
            MaNV = account.MaNV
        };

        var createResult = await userManager.CreateAsync(user, "CoffeeShop123!");
        if (!createResult.Succeeded)
            throw new InvalidOperationException(string.Join("; ", createResult.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, account.Role);
    }
}
