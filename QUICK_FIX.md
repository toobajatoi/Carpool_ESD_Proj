# 🚨 QUICK FIX - Create Missing Tables

## The Problem
The application is crashing because the `Notification` table (and other new tables) don't exist in your database yet.

## ✅ Solution - Run This SQL Script

**Option 1: Using SQL Server Management Studio (Easiest)**
1. Open **SQL Server Management Studio** (SSMS)
2. Connect to: `localhost\SQLEXPRESS`
3. Open the file: `CREATE_TABLES.sql` (in your project folder)
4. Click **Execute** (F5)
5. Wait for "All new tables created successfully!" message
6. Restart your application

**Option 2: Using Command Line**
```powershell
cd "f:\Tooba's Documents\University Study\Semester 7\Carpool_ESD_Proj\Carpool_ESD_Proj"
sqlcmd -S "localhost\SQLEXPRESS" -d "my_sql_project" -E -i "CREATE_TABLES.sql"
```

**Option 3: Copy-Paste This SQL**
Open SQL Server Management Studio, connect to `localhost\SQLEXPRESS`, select database `my_sql_project`, and run:

```sql
-- Notification Table (MOST IMPORTANT - fixes the login error)
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
END
GO
```

Then run the full `CREATE_TABLES.sql` script for all other tables.

## ✅ After Creating Tables

1. **Restart your application**: `dotnet run`
2. **Try logging in again** - it should work now!

## 🔍 Verify Tables Were Created

Run this in SQL Server Management Studio:
```sql
USE my_sql_project;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME IN 
('Notification', 'Vehicle', 'Message', 'FavoriteRide', 'EmergencyContact')
ORDER BY TABLE_NAME;
```

You should see all 5 tables listed.

---

**Note**: I've also added error handling to the code so it won't crash if tables are missing, but you still need to create the tables for the features to work.
