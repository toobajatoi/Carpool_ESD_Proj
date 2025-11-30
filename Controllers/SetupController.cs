using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Carpool_DB_Proj.Controllers
{
    public class SetupController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SetupController> _logger;

        public SetupController(IConfiguration configuration, ILogger<SetupController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult CreateTables()
        {
            var connectionString = _configuration.GetConnectionString("dbcs");
            if (string.IsNullOrEmpty(connectionString))
            {
                return Json(new { success = false, message = "Connection string not found" });
            }

            var tableScripts = new[]
            {
                // Notification Table
                @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notification')
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
END",
                // Message Table
                @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Message')
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
END",
                // Vehicle Table
                @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Vehicle')
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
END",
                // FavoriteRide Table
                @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FavoriteRide')
BEGIN
    CREATE TABLE FavoriteRide (
        favorite_id INT PRIMARY KEY IDENTITY(1,1),
        user_id INT NOT NULL,
        ride_id INT NOT NULL,
        created_date DATETIME NOT NULL,
        FOREIGN KEY (user_id) REFERENCES Users(user_id),
        FOREIGN KEY (ride_id) REFERENCES RideDetails(ride_id)
    );
END",
                // EmergencyContact Table
                @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmergencyContact')
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
END"
            };

            var createdTables = new List<string>();
            var errors = new List<string>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    foreach (var script in tableScripts)
                    {
                        try
                        {
                            using (var cmd = new SqlCommand(script, connection))
                            {
                                cmd.ExecuteNonQuery();
                                // Extract table name from script
                                var tableMatch = System.Text.RegularExpressions.Regex.Match(script, @"name = '(\w+)'");
                                if (tableMatch.Success)
                                {
                                    createdTables.Add(tableMatch.Groups[1].Value);
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number != 2714) // Table already exists
                            {
                                errors.Add($"Error: {ex.Message}");
                                _logger.LogWarning(ex, "Error creating table");
                            }
                        }
                    }
                }

                if (errors.Any())
                {
                    return Json(new { 
                        success = true, 
                        message = $"Tables created with some warnings. Created: {string.Join(", ", createdTables)}",
                        createdTables = createdTables,
                        errors = errors
                    });
                }
                
                return Json(new { 
                    success = true, 
                    message = $"Successfully created {createdTables.Count} tables: {string.Join(", ", createdTables)}",
                    createdTables = createdTables
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tables");
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}


