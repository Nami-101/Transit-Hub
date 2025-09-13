-- Transit-Hub Stored Procedures - Booking Operations
-- Created: 2025-09-09

USE TransitHubDB;
GO

-- =============================================
-- Create Train Booking
-- =============================================
CREATE OR ALTER PROCEDURE sp_CreateTrainBooking
    @UserID INT,
    @TrainScheduleID INT,
    @PassengerDetails NVARCHAR(MAX), -- JSON format: [{"Name":"John","Age":30,"Gender":"Male"}]
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate user
        IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = @UserID AND IsActive = 1 AND IsVerified = 1)
        BEGIN
            THROW 50016, 'Invalid or unverified user', 1;
        END
        
        -- Validate train schedule
        DECLARE @TrainID INT, @TravelDate DATE, @AvailableSeats INT, @Fare DECIMAL(10,2), @QuotaTypeID INT;
        
        SELECT @TrainID = TrainID, @TravelDate = TravelDate, @AvailableSeats = AvailableSeats, 
               @Fare = Fare, @QuotaTypeID = QuotaTypeID
        FROM TrainSchedules 
        WHERE ScheduleID = @TrainScheduleID AND IsActive = 1;
        
        IF @TrainID IS NULL
        BEGIN
            THROW 50017, 'Invalid train schedule', 1;
        END
        
        -- Check if travel date is valid
        IF @TravelDate < CAST(GETDATE() AS DATE)
        BEGIN
            THROW 50018, 'Cannot book for past dates', 1;
        END
        
        -- Parse passenger details
        DECLARE @PassengerCount INT;
        SELECT @PassengerCount = (SELECT COUNT(*) FROM OPENJSON(@PassengerDetails));
        
        IF @PassengerCount <= 0 OR @PassengerCount > 6
        BEGIN
            THROW 50019, 'Invalid passenger count. Must be between 1 and 6', 1;
        END
        
        -- Check Tatkal booking time restrictions
        IF @QuotaTypeID IN (SELECT QuotaTypeID FROM TrainQuotaTypes WHERE QuotaName LIKE '%Tatkal%')
        BEGIN
            DECLARE @BookingTime TIME = CAST(GETDATE() AS TIME);
            DECLARE @DaysUntilTravel INT = DATEDIFF(DAY, GETDATE(), @TravelDate);
            
            -- Tatkal booking opens at 10:00 AM, 1 day before travel
            IF @DaysUntilTravel != 1 OR @BookingTime < '10:00:00'
            BEGIN
                THROW 50020, 'Tatkal booking is only allowed 1 day before travel after 10:00 AM', 1;
            END
        END
        
        -- Generate booking reference
        DECLARE @BookingReference NVARCHAR(20) = 'TH' + FORMAT(GETDATE(), 'yyyyMMdd') + RIGHT('000000' + CAST(ABS(CHECKSUM(NEWID())) % 1000000 AS NVARCHAR(6)), 6);
        
        -- Calculate total amount
        DECLARE @TotalAmount DECIMAL(10,2) = @Fare * @PassengerCount;
        
        -- Determine booking status
        DECLARE @StatusID INT;
        IF @AvailableSeats >= @PassengerCount
        BEGIN
            SELECT @StatusID = StatusID FROM BookingStatusTypes WHERE StatusName = 'Confirmed';
        END
        ELSE
        BEGIN
            SELECT @StatusID = StatusID FROM BookingStatusTypes WHERE StatusName = 'Waitlisted';
        END
        
        -- Create booking
        DECLARE @BookingID INT;
        INSERT INTO Bookings (BookingReference, UserID, BookingType, TrainScheduleID, StatusID, TotalPassengers, TotalAmount, CreatedBy)
        VALUES (@BookingReference, @UserID, 'Train', @TrainScheduleID, @StatusID, @PassengerCount, @TotalAmount, @CreatedBy);
        
        SET @BookingID = SCOPE_IDENTITY();
        
        -- Add passengers
        DECLARE @PassengerIndex INT = 0;
        WHILE @PassengerIndex < @PassengerCount
        BEGIN
            DECLARE @PassengerName NVARCHAR(100) = JSON_VALUE(@PassengerDetails, '$[' + CAST(@PassengerIndex AS NVARCHAR(2)) + '].Name');
            DECLARE @PassengerAge INT = CAST(JSON_VALUE(@PassengerDetails, '$[' + CAST(@PassengerIndex AS NVARCHAR(2)) + '].Age') AS INT);
            DECLARE @PassengerGender NVARCHAR(10) = JSON_VALUE(@PassengerDetails, '$[' + CAST(@PassengerIndex AS NVARCHAR(2)) + '].Gender');
            DECLARE @IsSeniorCitizen BIT = CASE WHEN @PassengerAge >= 60 THEN 1 ELSE 0 END;
            
            INSERT INTO BookingPassengers (BookingID, Name, Age, Gender, IsSeniorCitizen, CreatedBy)
            VALUES (@BookingID, @PassengerName, @PassengerAge, @PassengerGender, @IsSeniorCitizen, @CreatedBy);
            
            SET @PassengerIndex = @PassengerIndex + 1;
        END
        
        -- Update seat availability or add to waitlist
        IF @AvailableSeats >= @PassengerCount
        BEGIN
            -- Confirm booking and reduce available seats
            UPDATE TrainSchedules 
            SET AvailableSeats = AvailableSeats - @PassengerCount,
                UpdatedBy = @CreatedBy,
                UpdatedAt = GETUTCDATE()
            WHERE ScheduleID = @TrainScheduleID;
        END
        ELSE
        BEGIN
            -- Add to waitlist
            DECLARE @QueuePosition INT = (SELECT ISNULL(MAX(QueuePosition), 0) + 1 FROM WaitlistQueue WHERE TrainScheduleID = @TrainScheduleID AND IsActive = 1);
            DECLARE @Priority INT = CASE WHEN EXISTS(SELECT 1 FROM BookingPassengers WHERE BookingID = @BookingID AND IsSeniorCitizen = 1) THEN 2 ELSE 1 END;
            
            INSERT INTO WaitlistQueue (BookingID, ScheduleType, TrainScheduleID, QueuePosition, Priority, CreatedBy)
            VALUES (@BookingID, 'Train', @TrainScheduleID, @QueuePosition, @Priority, @CreatedBy);
        END
        
        -- Create audit log
        INSERT INTO BookingAudit (BookingID, Action, ActionBy, Details, CreatedBy)
        VALUES (@BookingID, 'Created', @CreatedBy, 'Booking created with ' + CAST(@PassengerCount AS NVARCHAR(2)) + ' passengers', @CreatedBy);
        
        -- Create notification
        DECLARE @NotificationMessage NVARCHAR(1000) = 'Your booking ' + @BookingReference + ' has been ' + 
            CASE WHEN @AvailableSeats >= @PassengerCount THEN 'confirmed' ELSE 'added to waitlist' END;
            
        INSERT INTO Notifications (UserID, Title, Message, Type, RelatedBookingID)
        VALUES (@UserID, 'Booking ' + CASE WHEN @AvailableSeats >= @PassengerCount THEN 'Confirmed' ELSE 'Waitlisted' END, 
                @NotificationMessage, 'Success', @BookingID);
        
        COMMIT TRANSACTION;
        
        -- Return booking details
        SELECT 
            @BookingID as BookingID,
            @BookingReference as BookingReference,
            CASE WHEN @AvailableSeats >= @PassengerCount THEN 'Confirmed' ELSE 'Waitlisted' END as Status,
            @TotalAmount as TotalAmount,
            'Booking created successfully' as Message,
            1 as Success;
            
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace, UserID)
        VALUES ('Error', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)), @UserID);
        
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Create Flight Booking
-- =============================================
CREATE OR ALTER PROCEDURE sp_CreateFlightBooking
    @UserID INT,
    @FlightScheduleID INT,
    @PassengerDetails NVARCHAR(MAX), -- JSON format
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Validate user
        IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = @UserID AND IsActive = 1 AND IsVerified = 1)
        BEGIN
            THROW 50016, 'Invalid or unverified user', 1;
        END
        
        -- Validate flight schedule
        DECLARE @FlightID INT, @TravelDate DATE, @AvailableSeats INT, @Fare DECIMAL(10,2);
        
        SELECT @FlightID = FlightID, @TravelDate = TravelDate, @AvailableSeats = AvailableSeats, @Fare = Fare
        FROM FlightSchedules 
        WHERE ScheduleID = @FlightScheduleID AND IsActive = 1;
        
        IF @FlightID IS NULL
        BEGIN
            THROW 50021, 'Invalid flight schedule', 1;
        END
        
        -- Check if travel date is valid
        IF @TravelDate < CAST(GETDATE() AS DATE)
        BEGIN
            THROW 50018, 'Cannot book for past dates', 1;
        END
        
        -- Parse passenger details
        DECLARE @PassengerCount INT;
        SELECT @PassengerCount = (SELECT COUNT(*) FROM OPENJSON(@PassengerDetails));
        
        IF @PassengerCount <= 0 OR @PassengerCount > 6
        BEGIN
            THROW 50019, 'Invalid passenger count. Must be between 1 and 6', 1;
        END
        
        -- Check seat availability (flights don't have waitlist)
        IF @AvailableSeats < @PassengerCount
        BEGIN
            THROW 50022, 'Insufficient seats available', 1;
        END
        
        -- Generate booking reference
        DECLARE @BookingReference NVARCHAR(20) = 'FH' + FORMAT(GETDATE(), 'yyyyMMdd') + RIGHT('000000' + CAST(ABS(CHECKSUM(NEWID())) % 1000000 AS NVARCHAR(6)), 6);
        
        -- Calculate total amount
        DECLARE @TotalAmount DECIMAL(10,2) = @Fare * @PassengerCount;
        
        -- Get confirmed status
        DECLARE @StatusID INT;
        SELECT @StatusID = StatusID FROM BookingStatusTypes WHERE StatusName = 'Confirmed';
        
        -- Create booking
        DECLARE @BookingID INT;
        INSERT INTO Bookings (BookingReference, UserID, BookingType, FlightScheduleID, StatusID, TotalPassengers, TotalAmount, CreatedBy)
        VALUES (@BookingReference, @UserID, 'Flight', @FlightScheduleID, @StatusID, @PassengerCount, @TotalAmount, @CreatedBy);
        
        SET @BookingID = SCOPE_IDENTITY();
        
        -- Add passengers
        DECLARE @PassengerIndex INT = 0;
        WHILE @PassengerIndex < @PassengerCount
        BEGIN
            DECLARE @PassengerName NVARCHAR(100) = JSON_VALUE(@PassengerDetails, '$[' + CAST(@PassengerIndex AS NVARCHAR(2)) + '].Name');
            DECLARE @PassengerAge INT = CAST(JSON_VALUE(@PassengerDetails, '$[' + CAST(@PassengerIndex AS NVARCHAR(2)) + '].Age') AS INT);
            DECLARE @PassengerGender NVARCHAR(10) = JSON_VALUE(@PassengerDetails, '$[' + CAST(@PassengerIndex AS NVARCHAR(2)) + '].Gender');
            DECLARE @IsSeniorCitizen BIT = CASE WHEN @PassengerAge >= 60 THEN 1 ELSE 0 END;
            
            INSERT INTO BookingPassengers (BookingID, Name, Age, Gender, IsSeniorCitizen, CreatedBy)
            VALUES (@BookingID, @PassengerName, @PassengerAge, @PassengerGender, @IsSeniorCitizen, @CreatedBy);
            
            SET @PassengerIndex = @PassengerIndex + 1;
        END
        
        -- Update seat availability
        UPDATE FlightSchedules 
        SET AvailableSeats = AvailableSeats - @PassengerCount,
            UpdatedBy = @CreatedBy,
            UpdatedAt = GETUTCDATE()
        WHERE ScheduleID = @FlightScheduleID;
        
        -- Create audit log
        INSERT INTO BookingAudit (BookingID, Action, ActionBy, Details, CreatedBy)
        VALUES (@BookingID, 'Created', @CreatedBy, 'Flight booking created with ' + CAST(@PassengerCount AS NVARCHAR(2)) + ' passengers', @CreatedBy);
        
        -- Create notification
        INSERT INTO Notifications (UserID, Title, Message, Type, RelatedBookingID)
        VALUES (@UserID, 'Flight Booking Confirmed', 'Your flight booking ' + @BookingReference + ' has been confirmed', 'Success', @BookingID);
        
        COMMIT TRANSACTION;
        
        -- Return booking details
        SELECT 
            @BookingID as BookingID,
            @BookingReference as BookingReference,
            'Confirmed' as Status,
            @TotalAmount as TotalAmount,
            'Flight booking created successfully' as Message,
            1 as Success;
            
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
            
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace, UserID)
        VALUES ('Error', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)), @UserID);
        
        THROW;
    END CATCH
END;
GO

-- =============================================
-- Get User Bookings
-- =============================================
CREATE OR ALTER PROCEDURE sp_GetUserBookings
    @UserID INT,
    @BookingType NVARCHAR(10) = NULL, -- 'Train' or 'Flight'
    @Status NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate user
        IF NOT EXISTS (SELECT 1 FROM Users WHERE UserID = @UserID AND IsActive = 1)
        BEGIN
            THROW 50009, 'User not found', 1;
        END
        
        SELECT 
            b.BookingID,
            b.BookingReference,
            b.BookingType,
            b.TotalPassengers,
            b.TotalAmount,
            b.BookingDate,
            bs.StatusName as Status,
            
            -- Train details (if applicable)
            CASE WHEN b.BookingType = 'Train' THEN t.TrainName END as TrainName,
            CASE WHEN b.BookingType = 'Train' THEN t.TrainNumber END as TrainNumber,
            CASE WHEN b.BookingType = 'Train' THEN ss.StationName END as SourceStation,
            CASE WHEN b.BookingType = 'Train' THEN ds.StationName END as DestinationStation,
            CASE WHEN b.BookingType = 'Train' THEN ts.TravelDate END as TravelDate,
            CASE WHEN b.BookingType = 'Train' THEN ts.DepartureTime END as DepartureTime,
            CASE WHEN b.BookingType = 'Train' THEN ts.ArrivalTime END as ArrivalTime,
            CASE WHEN b.BookingType = 'Train' THEN tc.ClassName END as TrainClass,
            
            -- Flight details (if applicable)
            CASE WHEN b.BookingType = 'Flight' THEN f.FlightNumber END as FlightNumber,
            CASE WHEN b.BookingType = 'Flight' THEN f.Airline END as Airline,
            CASE WHEN b.BookingType = 'Flight' THEN sa.AirportName END as SourceAirport,
            CASE WHEN b.BookingType = 'Flight' THEN da.AirportName END as DestinationAirport,
            CASE WHEN b.BookingType = 'Flight' THEN fs.TravelDate END as FlightDate,
            CASE WHEN b.BookingType = 'Flight' THEN fs.DepartureTime END as FlightDepartureTime,
            CASE WHEN b.BookingType = 'Flight' THEN fs.ArrivalTime END as FlightArrivalTime,
            CASE WHEN b.BookingType = 'Flight' THEN fc.ClassName END as FlightClass,
            
            -- Waitlist position (if applicable)
            wq.QueuePosition as WaitlistPosition
            
        FROM Bookings b
        INNER JOIN BookingStatusTypes bs ON b.StatusID = bs.StatusID
        
        -- Train joins
        LEFT JOIN TrainSchedules ts ON b.TrainScheduleID = ts.ScheduleID
        LEFT JOIN Trains t ON ts.TrainID = t.TrainID
        LEFT JOIN Stations ss ON t.SourceStationID = ss.StationID
        LEFT JOIN Stations ds ON t.DestinationStationID = ds.StationID
        LEFT JOIN TrainClasses tc ON ts.TrainClassID = tc.TrainClassID
        
        -- Flight joins
        LEFT JOIN FlightSchedules fs ON b.FlightScheduleID = fs.ScheduleID
        LEFT JOIN Flights f ON fs.FlightID = f.FlightID
        LEFT JOIN Airports sa ON f.SourceAirportID = sa.AirportID
        LEFT JOIN Airports da ON f.DestinationAirportID = da.AirportID
        LEFT JOIN FlightClasses fc ON fs.FlightClassID = fc.FlightClassID
        
        -- Waitlist join
        LEFT JOIN WaitlistQueue wq ON b.BookingID = wq.BookingID AND wq.IsActive = 1
        
        WHERE 
            b.UserID = @UserID
            AND b.IsActive = 1
            AND (@BookingType IS NULL OR b.BookingType = @BookingType)
            AND (@Status IS NULL OR bs.StatusName = @Status)
        ORDER BY b.BookingDate DESC;
        
    END TRY
    BEGIN CATCH
        -- Log error
        INSERT INTO SystemLogs (LogLevel, Message, StackTrace, UserID)
        VALUES ('Error', ERROR_MESSAGE(), ERROR_PROCEDURE() + ' - Line: ' + CAST(ERROR_LINE() AS NVARCHAR(10)), @UserID);
        
        THROW;
    END CATCH
END;
GO