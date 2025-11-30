using Carpool_DB_Proj.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Data.SqlClient;

// Helper function to create missing tables
void CreateMissingTables(MySqlProjectContext db)
{
    var tableScripts = new[]
    {
        ("Notification", @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notification')
BEGIN
    CREATE TABLE Notification (
        notification_id INT PRIMARY KEY IDENTITY(1,1),
        user_id INT NOT NULL,
        title NVARCHAR(200) NOT NULL,
        message NVARCHAR(1000) NOT NULL,
        type NVARCHAR(50) NOT NULL,
        is_read BIT NOT NULL DEFAULT 0,
        created_date DATETIME NOT NULL,
        related_entity_type NVARCHAR(50),
        related_entity_id INT,
        FOREIGN KEY (user_id) REFERENCES Users(user_id)
    );
END"),
        ("Message", @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Message')
BEGIN
    CREATE TABLE Message (
        message_id INT PRIMARY KEY IDENTITY(1,1),
        sender_id INT NOT NULL,
        receiver_id INT NOT NULL,
        ride_request_id INT,
        content NVARCHAR(2000) NOT NULL,
        sent_date DATETIME NOT NULL,
        is_read BIT NOT NULL DEFAULT 0,
        FOREIGN KEY (sender_id) REFERENCES Users(user_id),
        FOREIGN KEY (receiver_id) REFERENCES Users(user_id),
        FOREIGN KEY (ride_request_id) REFERENCES RideRequest(request_id)
    );
END"),
        ("Vehicle", @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Vehicle')
BEGIN
    CREATE TABLE Vehicle (
        vehicle_id INT PRIMARY KEY IDENTITY(1,1),
        user_id INT NOT NULL,
        make NVARCHAR(50) NOT NULL,
        model NVARCHAR(50) NOT NULL,
        year INT NOT NULL,
        license_plate NVARCHAR(20) NOT NULL,
        color NVARCHAR(30),
        capacity INT NOT NULL,
        registration_number NVARCHAR(50),
        insurance_number NVARCHAR(50),
        vehicle_type NVARCHAR(30),
        created_date DATETIME NOT NULL,
        is_active BIT NOT NULL DEFAULT 1,
        FOREIGN KEY (user_id) REFERENCES Users(user_id)
    );
END"),
        ("FavoriteRide", @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FavoriteRide')
BEGIN
    CREATE TABLE FavoriteRide (
        favorite_id INT PRIMARY KEY IDENTITY(1,1),
        user_id INT NOT NULL,
        ride_id INT NOT NULL,
        created_date DATETIME NOT NULL,
        FOREIGN KEY (user_id) REFERENCES Users(user_id),
        FOREIGN KEY (ride_id) REFERENCES RideDetails(ride_id)
    );
END"),
        ("EmergencyContact", @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmergencyContact')
BEGIN
    CREATE TABLE EmergencyContact (
        contact_id INT PRIMARY KEY IDENTITY(1,1),
        user_id INT NOT NULL,
        contact_name NVARCHAR(100) NOT NULL,
        contact_phone NVARCHAR(20) NOT NULL,
        relationship NVARCHAR(50),
        is_primary BIT NOT NULL DEFAULT 0,
        created_date DATETIME NOT NULL,
        FOREIGN KEY (user_id) REFERENCES Users(user_id)
    );
END")
    };

    foreach (var (tableName, script) in tableScripts)
    {
        try
        {
            db.Database.ExecuteSqlRaw(script);
            Console.WriteLine($"✓ Table '{tableName}' created or already exists.");
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2714) // Table already exists
            {
                Console.WriteLine($"✓ Table '{tableName}' already exists.");
            }
            else
            {
                Console.WriteLine($"⚠ Warning creating table '{tableName}': {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠ Warning creating table '{tableName}': {ex.Message}");
        }
    }
}

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

        // Create missing tables if they don't exist
        Console.WriteLine("Checking for missing tables...");
        CreateMissingTables(db);
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
