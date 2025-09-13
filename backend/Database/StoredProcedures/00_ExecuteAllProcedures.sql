-- Transit-Hub Stored Procedures - Master Execution Script
-- Created: 2025-09-09
-- Description: Execute all stored procedures in correct order

USE TransitHubDB;
GO

PRINT 'Starting Transit-Hub Stored Procedures Creation...';
PRINT '====================================================';

-- 1. User Management Procedures
PRINT 'Creating User Management Procedures...';
:r "01_UserManagement.sql"
PRINT 'User Management Procedures Created Successfully!';
PRINT '';

-- 2. Search Operations Procedures
PRINT 'Creating Search Operations Procedures...';
:r "02_SearchOperations.sql"
PRINT 'Search Operations Procedures Created Successfully!';
PRINT '';

-- 3. Booking Operations Procedures
PRINT 'Creating Booking Operations Procedures...';
:r "03_BookingOperations.sql"
PRINT 'Booking Operations Procedures Created Successfully!';
PRINT '';

-- 4. Payment Operations Procedures
PRINT 'Creating Payment Operations Procedures...';
:r "04_PaymentOperations.sql"
PRINT 'Payment Operations Procedures Created Successfully!';
PRINT '';

-- 5. Waitlist Operations Procedures
PRINT 'Creating Waitlist Operations Procedures...';
:r "05_WaitlistOperations.sql"
PRINT 'Waitlist Operations Procedures Created Successfully!';
PRINT '';

-- 6. Admin Operations Procedures
PRINT 'Creating Admin Operations Procedures...';
:r "06_AdminOperations.sql"
PRINT 'Admin Operations Procedures Created Successfully!';
PRINT '';

PRINT '====================================================';
PRINT 'All Transit-Hub Stored Procedures Created Successfully!';
PRINT '';
PRINT 'Available Procedures:';
PRINT '- User Management: sp_RegisterUser, sp_LoginUser, sp_VerifyEmail, sp_GetUserProfile, sp_UpdateUserProfile';
PRINT '- Search Operations: sp_SearchTrains, sp_SearchFlights, sp_GetAllStations, sp_GetAllAirports, sp_GetLookupData';
PRINT '- Booking Operations: sp_CreateTrainBooking, sp_CreateFlightBooking, sp_GetUserBookings';
PRINT '- Payment Operations: sp_ProcessPayment, sp_GetPaymentHistory, sp_ProcessRefund';
PRINT '- Waitlist Operations: sp_PromoteWaitlist, sp_UpdateWaitlistPositions, sp_GetWaitlistStatus, sp_CancelBookingAndPromoteWaitlist';
PRINT '- Admin Operations: sp_AdminLogin, sp_GetDashboardStats, sp_ManageTrainSchedule, sp_GetBookingReports';
PRINT '';
PRINT 'Ready for API Integration!';
GO