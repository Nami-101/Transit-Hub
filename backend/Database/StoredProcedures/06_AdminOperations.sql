-- Transit-Hub Stored Procedures - Admin Operations
-- Created: 2025-09-09

USE TransitHubDB;
GO

-- =============================================
-- Admin Login
-- =============================================
CREATE OR ALTER PROCEDURE sp_AdminLogin
    @Email NVARCHAR(255),
    @PasswordHash NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Check if admin exists and is active
        IF NOT EXISTS (SELECT 1 FROM Admins WHERE Email = @Email AND IsActive = 1)
        BEGIN
            THROW 50035, 'Invalid email or password', 1;
        END
        
        -- Validate password
        DECLARE @StoredPasswordHash NVARCHAR(500);
        DECLARE @AdminID INT;
        DECLARE @Role NVARCHAR(50);
        
        SELECT @StoredPasswordHash = PasswordHash, @AdminID = AdminID, @Role = Role
        FROM Admins 
        WHERE Email = @Email AND IsActive = 1;
        
        IF @StoredPasswordHash != @PasswordHash
        BEGIN
            THROW 50035, 'Invalid email or password', 1;
        END
        
        -- Return admin details
        SELECT 
            AdminID,
            Name,
            Email,
            Phone,
            Role,
            'Admin login successful' as Message,
            1 as Success
        FROM Admins 
        WHERE AdminID = @AdminID;
        
    END TRY
    BEGIN CATCH
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace)
        VALUES ('Warning', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)));
        
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Get Dashboard Statistics
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetDashboardStats
    @AdminID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate admin
        IF NOT EXISTS (SELECT 1 FROM Admins WHERE AdminID = @AdminID AND IsActive = 1)
        BEGIN
            THROW 50036, 'Invalid admin ID', 1;
        END
        
        -- Get current date for calculations
        DECLARE @Today DATE = CAST(GETDATE() AS DATE);
        DECLARE @ThisMonth DATE = DATEFROMPARTS(YEAR(@Today), MONTH(@Today), 1);
        DECLARE @LastMonth DATE = DATEADD(MONTH, -1, @ThisMonth);
        
        -- Overall Statistics
        SELECT 
            -- User Statistics
            (SELECT COUNT(*) FROM Users WHERE IsActive = 1) as TotalUsers,
            (SELECT COUNT(*) FROM Users WHERE IsActive = 1 AND IsVerified = 1) as VerifiedUsers,
            (SELECT COUNT(*) FROM Users WHERE IsActive = 1 AND CAST(CreatedAt AS DATE) = @Today) as NewUsersToday,
            
            -- Booking Statistics
            (SELECT COUNT(*) FROM Bookings WHERE IsActive = 1) as TotalBookings,
            (SELECT COUNT(*) FROM Bookings b INNER JOIN BookingStatusTypes bs ON b.StatusID = bs.StatusID 
             WHERE b.IsActive = 1 AND bs.StatusName = 'Confirmed') as ConfirmedBookings,
            (SELECT COUNT(*) FROM Bookings b INNER JOIN BookingStatusTypes bs ON b.StatusID = bs.StatusID 
             WHERE b.IsActive = 1 AND bs.StatusName = 'Waitlisted') as WaitlistedBookings,
            (SELECT COUNT(*) FROM Bookings b INNER JOIN BookingStatusTypes bs ON b.StatusID = bs.StatusID 
             WHERE b.IsActive = 1 AND bs.StatusName = 'Cancelled') as CancelledBookings,
            
            -- Revenue Statistics
            (SELECT ISNULL(SUM(Amount), 0) FROM Payments WHERE Status = 'Success') as TotalRevenue,
            (SELECT ISNULL(SUM(Amount), 0) FROM Payments WHERE Status = 'Success' AND CAST(PaymentDate AS DATE) >= @ThisMonth) as MonthlyRevenue,
            (SELECT ISNULL(SUM(Amount), 0) FROM Payments WHERE Status = 'Success' AND CAST(PaymentDate AS DATE) = @Today) as DailyRevenue,
            
            -- Transport Statistics
            (SELECT COUNT(*) FROM Trains WHERE IsActive = 1) as TotalTrains,
            (SELECT COUNT(*) FROM Flights WHERE IsActive = 1) as TotalFlights,
            (SELECT COUNT(*) FROM Stations WHERE IsActive = 1) as TotalStations,
            (SELECT COUNT(*) FROM Airports WHERE IsActive = 1) as TotalAirports;
        
        -- Monthly Booking Trends (Last 6 months)
        SELECT 
            FORMAT(b.BookingDate, 'yyyy-MM') as Month,
            COUNT(*) as BookingCount,
            SUM(b.TotalAmount) as Revenue,
            COUNT(CASE WHEN b.BookingType = 'Train' THEN 1 END) as TrainBookings,
            COUNT(CASE WHEN b.BookingType = 'Flight' THEN 1 END) as FlightBookings
        FROM Bookings b
        WHERE b.IsActive = 1 AND b.BookingDate >= DATEADD(MONTH, -6, @Today)
        GROUP BY FORMAT(b.BookingDate, 'yyyy-MM')
        ORDER BY Month;
        
        -- Top Routes (Train)
        SELECT TOP 10
            CONCAT(ss.StationName, ' → ', ds.StationName) as Route,
            COUNT(*) as BookingCount,
            SUM(b.TotalAmount) as Revenue
        FROM Bookings b
        INNER JOIN TrainSchedules ts ON b.TrainScheduleID = ts.ScheduleID
        INNER JOIN Trains t ON ts.TrainID = t.TrainID
        INNER JOIN Stations ss ON t.SourceStationID = ss.StationID
        INNER JOIN Stations ds ON t.DestinationStationID = ds.StationID
        WHERE b.IsActive = 1 AND b.BookingType = 'Train'
        GROUP BY ss.StationName, ds.StationName
        ORDER BY BookingCount DESC;
        
        -- Top Routes (Flight)
        SELECT TOP 10
            CONCAT(sa.Code, ' → ', da.Code) as Route,
            COUNT(*) as BookingCount,
            SUM(b.TotalAmount) as Revenue
        FROM Bookings b
        INNER JOIN FlightSchedules fs ON b.FlightScheduleID = fs.ScheduleID
        INNER JOIN Flights f ON fs.FlightID = f.FlightID
        INNER JOIN Airports sa ON f.SourceAirportID = sa.AirportID
        INNER JOIN Airports da ON f.DestinationAirportID = da.AirportID
        WHERE b.IsActive = 1 AND b.BookingType = 'Flight'
        GROUP BY sa.Code, da.Code
        ORDER BY BookingCount DESC;
        
    END TRY
    BEGIN CATCH
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace)
        VALUES ('Error', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)));
        
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Manage Train Schedule
-- =============================================
CREATE OR ALTER PROCEDURE sp_ManageTrainSchedule
    @Action NVARCHAR(10), -- 'INSERT', 'UPDATE', 'DELETE'
    @ScheduleID INT = NULL,
    @TrainID INT = NULL,
    @TravelDate DATE = NULL,
    @DepartureTime DATETIME2 = NULL,
    @ArrivalTime DATETIME2 = NULL,
    @QuotaTypeID INT = NULL,
    @TrainClassID INT = NULL,
    @TotalSeats INT = NULL,
    @Fare DECIMAL(10,2) = NULL,
    @AdminID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate admin
        IF NOT EXISTS (SELECT 1 FROM Admins WHERE AdminID = @AdminID AND IsActive = 1)
        BEGIN
            THROW 50036, 'Invalid admin ID', 1;
        END
        
        DECLARE @AdminName NVARCHAR(100);
        SELECT @AdminName = Name FROM Admins WHERE AdminID = @AdminID;
        
        IF @Action = 'INSERT'
        BEGIN
            -- Validate required fields
            IF @TrainID IS NULL OR @TravelDate IS NULL OR @DepartureTime IS NULL OR @ArrivalTime IS NULL 
               OR @QuotaTypeID IS NULL OR @TrainClassID IS NULL OR @TotalSeats IS NULL OR @Fare IS NULL
            BEGIN
                THROW 50037, 'All fields are required for insert operation', 1;
            END
            
            -- Check if schedule already exists
            IF EXISTS (SELECT 1 FROM TrainSchedules WHERE TrainID = @TrainID AND TravelDate = @TravelDate 
                      AND QuotaTypeID = @QuotaTypeID AND TrainClassID = @TrainClassID AND IsActive = 1)
            BEGIN
                THROW 50038, 'Schedule already exists for this train, date, quota and class combination', 1;
            END
            
            -- Insert new schedule
            INSERT INTO TrainSchedules (TrainID, TravelDate, DepartureTime, ArrivalTime, QuotaTypeID, 
                                      TrainClassID, TotalSeats, AvailableSeats, Fare, CreatedBy)
            VALUES (@TrainID, @TravelDate, @DepartureTime, @ArrivalTime, @QuotaTypeID, 
                   @TrainClassID, @TotalSeats, @TotalSeats, @Fare, @AdminName);
            
            SET @ScheduleID = SCOPE_IDENTITY();
        END
        ELSE IF @Action = 'UPDATE'
        BEGIN
            -- Validate schedule exists
            IF NOT EXISTS (SELECT 1 FROM TrainSchedules WHERE ScheduleID = @ScheduleID AND IsActive = 1)
            BEGIN
                THROW 50039, 'Schedule not found', 1;
            END
            
            -- Update schedule
            UPDATE TrainSchedules 
            SET TrainID = ISNULL(@TrainID, TrainID),
                TravelDate = ISNULL(@TravelDate, TravelDate),
                DepartureTime = ISNULL(@DepartureTime, DepartureTime),
                ArrivalTime = ISNULL(@ArrivalTime, ArrivalTime),
                QuotaTypeID = ISNULL(@QuotaTypeID, QuotaTypeID),
                TrainClassID = ISNULL(@TrainClassID, TrainClassID),
                TotalSeats = ISNULL(@TotalSeats, TotalSeats),
                Fare = ISNULL(@Fare, Fare),
                UpdatedBy = @AdminName,
                UpdatedAt = GETUTCDATE()
            WHERE ScheduleID = @ScheduleID;
        END
        ELSE IF @Action = 'DELETE'
        BEGIN
            -- Check if there are active bookings
            IF EXISTS (SELECT 1 FROM Bookings WHERE TrainScheduleID = @ScheduleID AND IsActive = 1)
            BEGIN
                THROW 50040, 'Cannot delete schedule with active bookings', 1;
            END
            
            -- Soft delete
            UPDATE TrainSchedules 
            SET IsActive = 0, UpdatedBy = @AdminName, UpdatedAt = GETUTCDATE()
            WHERE ScheduleID = @ScheduleID;
        END
        ELSE
        BEGIN
            THROW 50041, 'Invalid action. Use INSERT, UPDATE, or DELETE', 1;
        END
        
        COMMIT TRANSACTION;
        
        SELECT 
            @ScheduleID as ScheduleID,
            @Action + ' operation completed successfully' as Message,
            1 as Success;
            
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace)
        VALUES ('Error', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)));
        
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Get Booking Reports
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetBookingReports
    @ReportType NVARCHAR(20), -- 'DAILY', 'WEEKLY', 'MONTHLY', 'CUSTOM'
    @StartDate DATE = NULL,
    @EndDate DATE = NULL,
    @BookingType NVARCHAR(10) = NULL, -- 'Train', 'Flight'
    @AdminID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate admin
        IF NOT EXISTS (SELECT 1 FROM Admins WHERE AdminID = @AdminID AND IsActive = 1)
        BEGIN
            THROW 50036, 'Invalid admin ID', 1;
        END
        
        -- Set date ranges based on report type
        DECLARE @ReportStartDate DATE, @ReportEndDate DATE;
        
        IF @ReportType = 'DAILY'
        BEGIN
            SET @ReportStartDate = CAST(GETDATE() AS DATE);
            SET @ReportEndDate = @ReportStartDate;
        END
        ELSE IF @ReportType = 'WEEKLY'
        BEGIN
            SET @ReportStartDate = DATEADD(DAY, -DATEPART(WEEKDAY, GETDATE()) + 1, CAST(GETDATE() AS DATE));
            SET @ReportEndDate = DATEADD(DAY, 6, @ReportStartDate);
        END
        ELSE IF @ReportType = 'MONTHLY'
        BEGIN
            SET @ReportStartDate = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
            SET @ReportEndDate = EOMONTH(@ReportStartDate);
        END
        ELSE IF @ReportType = 'CUSTOM'
        BEGIN
            IF @StartDate IS NULL OR @EndDate IS NULL
            BEGIN
                THROW 50042, 'Start date and end date are required for custom reports', 1;
            END
            SET @ReportStartDate = @StartDate;
            SET @ReportEndDate = @EndDate;
        END
        ELSE
        BEGIN
            THROW 50043, 'Invalid report type. Use DAILY, WEEKLY, MONTHLY, or CUSTOM', 1;
        END
        
        -- Generate report
        SELECT 
            CAST(b.BookingDate AS DATE) as BookingDate,
            b.BookingType,
            COUNT(*) as TotalBookings,
            COUNT(CASE WHEN bs.StatusName = 'Confirmed' THEN 1 END) as ConfirmedBookings,
            COUNT(CASE WHEN bs.StatusName = 'Waitlisted' THEN 1 END) as WaitlistedBookings,
            COUNT(CASE WHEN bs.StatusName = 'Cancelled' THEN 1 END) as CancelledBookings,
            SUM(b.TotalAmount) as TotalRevenue,
            SUM(CASE WHEN p.Status = 'Success' THEN p.Amount ELSE 0 END) as CollectedRevenue,
            AVG(b.TotalAmount) as AverageBookingValue,
            SUM(b.TotalPassengers) as TotalPassengers
        FROM Bookings b
        INNER JOIN BookingStatusTypes bs ON b.StatusID = bs.StatusID
        LEFT JOIN Payments p ON b.BookingID = p.BookingID AND p.IsActive = 1
        WHERE 
            b.IsActive = 1
            AND CAST(b.BookingDate AS DATE) BETWEEN @ReportStartDate AND @ReportEndDate
            AND (@BookingType IS NULL OR b.BookingType = @BookingType)
        GROUP BY CAST(b.BookingDate AS DATE), b.BookingType
        ORDER BY BookingDate DESC, BookingType;
        
        -- Summary statistics
        SELECT 
            @ReportType as ReportType,
            @ReportStartDate as StartDate,
            @ReportEndDate as EndDate,
            COUNT(*) as TotalBookings,
            SUM(b.TotalAmount) as TotalRevenue,
            SUM(CASE WHEN p.Status = 'Success' THEN p.Amount ELSE 0 END) as CollectedRevenue,
            COUNT(DISTINCT b.UserID) as UniqueCustomers,
            AVG(b.TotalAmount) as AverageBookingValue
        FROM Bookings b
        LEFT JOIN Payments p ON b.BookingID = p.BookingID AND p.IsActive = 1
        WHERE 
            b.IsActive = 1
            AND CAST(b.BookingDate AS DATE) BETWEEN @ReportStartDate AND @ReportEndDate
            AND (@BookingType IS NULL OR b.BookingType = @BookingType);
        
    END TRY
    BEGIN CATCH
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace)
        VALUES ('Error', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)));
        
        THROW;
    END CATCH
END;
GO