using Carpool_DB_Proj.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var provider = builder.Services.BuildServiceProvider();
var config = provider.GetRequiredService<IConfiguration>();
builder.Services.AddDbContext<MySqlProjectContext>(item => item.UseSqlServer(config.GetConnectionString("dbcs")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MySqlProjectContext>();
    try
    {
        // Try to apply migrations first (for updating existing database)
        try
        {
            var pendingMigrations = db.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                Console.WriteLine($"Applying {pendingMigrations.Count} pending migration(s) to update database...");
                db.Database.Migrate();
                Console.WriteLine("✓ Database migrations applied successfully.");
            }
            else
            {
                Console.WriteLine("✓ Database is up to date (no pending migrations).");
            }
        }
        catch (Exception migrationEx)
        {
            // If migrations fail, try to create missing tables manually
            Console.WriteLine($"Migration attempt: {migrationEx.Message}");
            Console.WriteLine("Note: If new tables are missing, run CREATE_TABLES.sql manually in SQL Server.");
            
            // Ensure database can connect
            var canConnect = db.Database.CanConnect();
            if (!canConnect)
            {
                db.Database.EnsureCreated();
                Console.WriteLine("✓ Database created with all tables.");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection error: {ex.Message}");
        Console.WriteLine("Please ensure SQL Server Express (SQLEXPRESS) is installed and running.");
        Console.WriteLine("To add new tables manually, run CREATE_TABLES.sql in SQL Server Management Studio.");
        // Don't throw - allow app to continue
    }

    if (!db.Users.Any())
    {
        var demoUser = new User
        {
            NuId = "NU0001",
            Name = "Demo Driver",
            PhoneNumber = "0000000000",
            Email = "demo@carpool.local",
            Password = "demo123"
        };

        db.Users.Add(demoUser);
        db.SaveChanges();

        var demoRide = new RideDetail
        {
            UserId = demoUser.UserId,
            CarId = 1001,
            ToFast = "Yes",
            Routes = "City Center > Campus",
            Fare = 500,
            AvailableSeats = 3,
            ClassTime = new TimeOnly(9, 0),
            LeavingTime = new TimeOnly(8, 15)
        };

        db.RideDetails.Add(demoRide);
        db.SaveChanges();

        db.DriverProfiles.Add(new DriverProfile
        {
            DriverId = demoUser.UserId,
            RideId = demoRide.RideId
        });

        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
