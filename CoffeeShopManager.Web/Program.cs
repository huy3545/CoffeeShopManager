using CoffeeShopManager.DAL.Data;
using CoffeeShopManager.DAL.Repositories;
using CoffeeShopManager.BLL.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

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

// Add Services
builder.Services.AddScoped<IBanService, BanService>();
builder.Services.AddScoped<IMonService, MonService>();
builder.Services.AddScoped<IHoaDonService, HoaDonService>();
builder.Services.AddScoped<INhanVienService, NhanVienService>();
builder.Services.AddScoped<IKhachHangService, KhachHangService>();
builder.Services.AddScoped<IThongKeService, ThongKeService>();
builder.Services.AddScoped<IExportService, ExportService>();

// Add HttpClient for API calls
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

