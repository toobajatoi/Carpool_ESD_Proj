# Script to update existing database with new entity tables
# This uses EF Core Migrations to add new tables without deleting existing data

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Database Update Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

cd "f:\Tooba's Documents\University Study\Semester 7\Carpool_ESD_Proj\Carpool_ESD_Proj"

Write-Host "Step 1: Creating migration for new entities..." -ForegroundColor Yellow
$migrationResult = dotnet ef migrations add AddNewEntities 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Migration created successfully" -ForegroundColor Green
    Write-Host ""
    Write-Host "Step 2: Applying migration to database..." -ForegroundColor Yellow
    $updateResult = dotnet ef database update 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Database updated successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "All new entity tables have been added to your existing database." -ForegroundColor Green
    } else {
        Write-Host "✗ Database update failed" -ForegroundColor Red
        Write-Host $updateResult
    }
} else {
    Write-Host "✗ Migration creation failed" -ForegroundColor Red
    Write-Host $migrationResult
    Write-Host ""
    Write-Host "Trying alternative: Manual SQL script..." -ForegroundColor Yellow
    Write-Host "See CREATE_TABLES.sql for manual table creation" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
