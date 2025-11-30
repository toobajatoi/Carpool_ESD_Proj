# Database Update Instructions

## ✅ Update Existing Database (Without Deleting Data)

### Method 1: Using EF Core Migrations (Recommended)

1. **Create Migration:**
   ```powershell
   cd "f:\Tooba's Documents\University Study\Semester 7\Carpool_ESD_Proj\Carpool_ESD_Proj"
   dotnet ef migrations add AddNewEntities
   ```

2. **Apply Migration:**
   ```powershell
   dotnet ef database update
   ```

3. **Or let the application do it automatically:**
   - Just run the application: `dotnet run`
   - The application will automatically apply pending migrations on startup

### Method 2: Manual SQL Script (If Migrations Don't Work)

1. Open **SQL Server Management Studio**
2. Connect to: `localhost\SQLEXPRESS`
3. Open the file: `CREATE_TABLES.sql`
4. Execute the script
5. This will create all new tables without affecting existing data

### Method 3: Using PowerShell Script

Run the provided script:
```powershell
.\update-database.ps1
```

---

## 📋 New Tables That Will Be Created

1. **Vehicle** - Vehicle information
2. **Notification** - User notifications
3. **Message** - User messages
4. **FavoriteRide** - Favorite rides
5. **RideHistory** - Ride history records
6. **Cancellation** - Cancellation records
7. **Complaint** - User complaints
8. **Route** - Route details with coordinates
9. **EmergencyContact** - Emergency contacts
10. **Rating** - Detailed ratings
11. **Schedule** - Recurring ride schedules
12. **Waitlist** - Waitlist for full rides

---

## ✅ Verification

After updating, verify tables exist:
```sql
USE my_sql_project;
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

You should see all the new tables listed.

---

## 🔧 Troubleshooting

### If migrations fail:
- Use Method 2 (Manual SQL Script) - it's guaranteed to work
- Check SQL Server is running: `Get-Service 'MSSQL$SQLEXPRESS'`

### If tables already exist:
- The script uses `IF NOT EXISTS` so it's safe to run multiple times
- No data will be lost

---

## ✨ After Update

Once tables are created:
1. Restart your application
2. All new features will be available
3. Existing data remains intact
