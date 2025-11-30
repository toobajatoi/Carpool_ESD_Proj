# Quick script to create tables using SQL Server command line
# This will create all new tables in the existing database

$sqlScript = @"
USE my_sql_project;
GO

-- Vehicle Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Vehicle')
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
    PRINT 'Vehicle table created.';
END
GO

-- Notification Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notification')
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
    PRINT 'Notification table created.';
END
GO

-- Message Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Message')
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
    PRINT 'Message table created.';
END
GO

-- FavoriteRide Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FavoriteRide')
BEGIN
    CREATE TABLE FavoriteRide (
        favorite_id INT PRIMARY KEY IDENTITY(1,1),
        user_id INT NOT NULL,
        ride_id INT NOT NULL,
        created_date DATETIME NOT NULL,
        FOREIGN KEY (user_id) REFERENCES Users(user_id),
        FOREIGN KEY (ride_id) REFERENCES RideDetails(ride_id)
    );
    PRINT 'FavoriteRide table created.';
END
GO

-- RideHistory Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RideHistory')
BEGIN
    CREATE TABLE RideHistory (
        history_id INT PRIMARY KEY IDENTITY(1,1),
        ride_id INT NOT NULL,
        driver_id INT NOT NULL,
        passenger_id INT NOT NULL,
        ride_request_id INT,
        start_time DATETIME,
        end_time DATETIME,
        distance DECIMAL(10,2),
        status NVARCHAR(50) NOT NULL,
        completed_date DATETIME NOT NULL,
        FOREIGN KEY (ride_id) REFERENCES RideDetails(ride_id),
        FOREIGN KEY (driver_id) REFERENCES Users(user_id),
        FOREIGN KEY (passenger_id) REFERENCES Users(user_id),
        FOREIGN KEY (ride_request_id) REFERENCES RideRequest(request_id)
    );
    PRINT 'RideHistory table created.';
END
GO

-- Cancellation Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Cancellation')
BEGIN
    CREATE TABLE Cancellation (
        cancellation_id INT PRIMARY KEY IDENTITY(1,1),
        ride_request_id INT NOT NULL UNIQUE,
        cancelled_by INT NOT NULL,
        reason NVARCHAR(500) NOT NULL,
        cancellation_date DATETIME NOT NULL,
        refund_status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        refund_amount DECIMAL(10,2),
        FOREIGN KEY (ride_request_id) REFERENCES RideRequest(request_id),
        FOREIGN KEY (cancelled_by) REFERENCES Users(user_id)
    );
    PRINT 'Cancellation table created.';
END
GO

-- Complaint Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Complaint')
BEGIN
    CREATE TABLE Complaint (
        complaint_id INT PRIMARY KEY IDENTITY(1,1),
        reporter_id INT NOT NULL,
        reported_user_id INT NOT NULL,
        ride_id INT,
        complaint_type NVARCHAR(50) NOT NULL,
        description NVARCHAR(2000) NOT NULL,
        status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
        created_date DATETIME NOT NULL,
        resolved_date DATETIME,
        resolution_notes NVARCHAR(1000),
        FOREIGN KEY (reporter_id) REFERENCES Users(user_id),
        FOREIGN KEY (reported_user_id) REFERENCES Users(user_id),
        FOREIGN KEY (ride_id) REFERENCES RideDetails(ride_id)
    );
    PRINT 'Complaint table created.';
END
GO

-- Route Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Route')
BEGIN
    CREATE TABLE Route (
        route_id INT PRIMARY KEY IDENTITY(1,1),
        ride_id INT NOT NULL UNIQUE,
        start_location NVARCHAR(255) NOT NULL,
        end_location NVARCHAR(255) NOT NULL,
        waypoints NVARCHAR(2000),
        distance DECIMAL(10,2),
        estimated_duration INT,
        route_polyline NVARCHAR(5000),
        start_latitude DECIMAL(10,8),
        start_longitude DECIMAL(11,8),
        end_latitude DECIMAL(10,8),
        end_longitude DECIMAL(11,8),
        FOREIGN KEY (ride_id) REFERENCES RideDetails(ride_id)
    );
    PRINT 'Route table created.';
END
GO

-- EmergencyContact Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmergencyContact')
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
    PRINT 'EmergencyContact table created.';
END
GO

-- Rating Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Rating')
BEGIN
    CREATE TABLE Rating (
        rating_id INT PRIMARY KEY IDENTITY(1,1),
        ride_id INT NOT NULL,
        rated_by INT NOT NULL,
        rated_user INT NOT NULL,
        rating_value INT NOT NULL,
        category NVARCHAR(50) NOT NULL,
        created_date DATETIME NOT NULL,
        FOREIGN KEY (ride_id) REFERENCES RideDetails(ride_id),
        FOREIGN KEY (rated_by) REFERENCES Users(user_id),
        FOREIGN KEY (rated_user) REFERENCES Users(user_id)
    );
    PRINT 'Rating table created.';
END
GO

-- Schedule Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Schedule')
BEGIN
    CREATE TABLE Schedule (
        schedule_id INT PRIMARY KEY IDENTITY(1,1),
        user_id INT NOT NULL,
        day_of_week NVARCHAR(20) NOT NULL,
        time TIME NOT NULL,
        route NVARCHAR(255) NOT NULL,
        is_active BIT NOT NULL DEFAULT 1,
        start_date DATETIME,
        end_date DATETIME,
        to_fast NVARCHAR(10),
        fare DECIMAL(10,2),
        available_seats INT,
        FOREIGN KEY (user_id) REFERENCES Users(user_id)
    );
    PRINT 'Schedule table created.';
END
GO

-- Waitlist Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Waitlist')
BEGIN
    CREATE TABLE Waitlist (
        waitlist_id INT PRIMARY KEY IDENTITY(1,1),
        ride_id INT NOT NULL,
        user_id INT NOT NULL,
        position INT NOT NULL,
        status NVARCHAR(50) NOT NULL DEFAULT 'Waiting',
        created_date DATETIME NOT NULL,
        notified_date DATETIME,
        FOREIGN KEY (ride_id) REFERENCES RideDetails(ride_id),
        FOREIGN KEY (user_id) REFERENCES Users(user_id)
    );
    PRINT 'Waitlist table created.';
END
GO

PRINT 'All new tables created successfully!';
GO
"@

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Creating Database Tables" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Try to use sqlcmd if available
$sqlcmdPath = Get-Command sqlcmd -ErrorAction SilentlyContinue

if ($sqlcmdPath) {
    Write-Host "Using sqlcmd to create tables..." -ForegroundColor Yellow
    $sqlScript | sqlcmd -S "localhost\SQLEXPRESS" -d "my_sql_project" -E
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Tables created successfully!" -ForegroundColor Green
    } else {
        Write-Host "✗ Error creating tables with sqlcmd" -ForegroundColor Red
        Write-Host ""
        Write-Host "Please run CREATE_TABLES.sql manually in SQL Server Management Studio:" -ForegroundColor Yellow
        Write-Host "1. Open SQL Server Management Studio" -ForegroundColor White
        Write-Host "2. Connect to: localhost\SQLEXPRESS" -ForegroundColor White
        Write-Host "3. Open CREATE_TABLES.sql file" -ForegroundColor White
        Write-Host "4. Execute the script" -ForegroundColor White
    }
} else {
    Write-Host "sqlcmd not found. Please run CREATE_TABLES.sql manually:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "1. Open SQL Server Management Studio" -ForegroundColor White
    Write-Host "2. Connect to: localhost\SQLEXPRESS" -ForegroundColor White
    Write-Host "3. Open CREATE_TABLES.sql file" -ForegroundColor White
    Write-Host "4. Execute the script" -ForegroundColor White
    Write-Host ""
    Write-Host "Or install SQL Server Command Line Utilities" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
