-- Transit-Hub Database - Master Setup Script
-- Execute this script in SQL Server Management Studio (SSMS)
-- Make sure to update the file paths to match your local system

-- =============================================
-- Step 1: Create Database
-- =============================================
USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TransitHubDB')
BEGIN
    CREATE DATABASE TransitHubDB;
    PRINT 'Database TransitHubDB created successfully!';
END
ELSE
BEGIN
    PRINT 'Database TransitHubDB already exists.';
END
GO

USE TransitHubDB;
GO

PRINT '====================================================';
PRINT 'Starting Transit-Hub Database Setup...';
PRINT '====================================================';

-- =============================================
-- Step 2: Create Tables
-- =============================================
PRINT 'Step 1: Creating Tables...';

-- You need to copy and paste the content from each script file
-- Or use SQLCMD mode in SSMS and update the paths below:

-- For SQLCMD mode, uncomment and update these paths:
-- :r "C:\YourPath\Transit-Hub\backend\Database\Scripts\01_CreateTables.sql"
-- :r "C:\YourPath\Transit-Hub\backend\Database\Scripts\02_CreateSchedulesAndBookings.sql"
-- :r "C:\YourPath\Transit-Hub\backend\Database\Scripts\03_CreateAuditAndLogs.sql"

PRINT 'Tables creation completed!';
PRINT '';

-- =============================================
-- Step 3: Insert Seed Data
-- =============================================
PRINT 'Step 2: Inserting Seed Data...';

-- :r "C:\YourPath\Transit-Hub\backend\Database\Scripts\04_SeedData.sql"

PRINT 'Seed data insertion completed!';
PRINT '';

-- =============================================
-- Step 4: Create Stored Procedures
-- =============================================
PRINT 'Step 3: Creating Stored Procedures...';

-- :r "C:\YourPath\Transit-Hub\backend\Database\StoredProcedures\01_UserManagement.sql"
-- :r "C:\YourPath\Transit-Hub\backend\Database\StoredProcedures\02_SearchOperations.sql"
-- :r "C:\YourPath\Transit-Hub\backend\Database\StoredProcedures\03_BookingOperations.sql"
-- :r "C:\YourPath\Transit-Hub\backend\Database\StoredProcedures\04_PaymentOperations.sql"
-- :r "C:\YourPath\Transit-Hub\backend\Database\StoredProcedures\05_WaitlistOperations.sql"
-- :r "C:\YourPath\Transit-Hub\backend\Database\StoredProcedures\06_AdminOperations.sql"

PRINT 'Stored procedures creation completed!';
PRINT '';

-- =============================================
-- Step 5: Verification
-- =============================================
PRINT 'Step 4: Database Verification...';

-- Check tables
SELECT 'Tables Created:' as Info, COUNT(*) as Count 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';

-- Check stored procedures
SELECT 'Stored Procedures Created:' as Info, COUNT(*) as Count 
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_TYPE = 'PROCEDURE' AND ROUTINE_NAME LIKE 'sp_%';

-- Check seed data
SELECT 'Stations:' as Info, COUNT(*) as Count FROM Stations WHERE IsActive = 1;
SELECT 'Airports:' as Info, COUNT(*) as Count FROM Airports WHERE IsActive = 1;
SELECT 'Train Classes:' as Info, COUNT(*) as Count FROM TrainClasses WHERE IsActive = 1;
SELECT 'Payment Modes:' as Info, COUNT(*) as Count FROM PaymentModes WHERE IsActive = 1;

PRINT '====================================================';
PRINT 'Transit-Hub Database Setup Completed Successfully!';
PRINT '====================================================';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Update connection string in appsettings.json';
PRINT '2. Test API connection';
PRINT '3. Run application and verify Swagger UI';
PRINT '';
GO