-- Transit-Hub Database - Schedule Seed Data
-- Adding comprehensive train and flight schedules for better search results

USE TransitHubDB;
GO

-- Clear existing schedules first (if any)
DELETE FROM TrainSchedules;
DELETE FROM FlightSchedules;

-- Generate train schedules for the next 30 days
DECLARE @StartDate DATE = CAST(GETDATE() AS DATE);
DECLARE @EndDate DATE = DATEADD(DAY, 30, @StartDate);
DECLARE @CurrentDate DATE = @StartDate;

-- Seed Train Schedules
WHILE @CurrentDate <= @EndDate
BEGIN
    -- Rajdhani Express (Delhi to Chennai) - Daily
    INSERT INTO TrainSchedules (TrainID, TravelDate, DepartureTime, ArrivalTime, QuotaTypeID, TrainClassID, TotalSeats, AvailableSeats, Fare)
    VALUES 
    (1, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('16:00:00' AS TIME), DATEADD(HOUR, 28, CAST(@CurrentDate AS DATETIME) + CAST('16:00:00' AS TIME)), 1, 1, 72, 45, 2500.00), -- SL
    (1, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('16:00:00' AS TIME), DATEADD(HOUR, 28, CAST(@CurrentDate AS DATETIME) + CAST('16:00:00' AS TIME)), 1, 2, 64, 32, 3500.00), -- 3A
    (1, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('16:00:00' AS TIME), DATEADD(HOUR, 28, CAST(@CurrentDate AS DATETIME) + CAST('16:00:00' AS TIME)), 1, 3, 48, 25, 4500.00), -- 2A
    (1, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('16:00:00' AS TIME), DATEADD(HOUR, 28, CAST(@CurrentDate AS DATETIME) + CAST('16:00:00' AS TIME)), 1, 4, 24, 12, 6000.00); -- 1A

    -- Shatabdi Express (Delhi to Mumbai) - Daily except Sunday
    IF DATEPART(WEEKDAY, @CurrentDate) != 1 -- Not Sunday
    BEGIN
        INSERT INTO TrainSchedules (TrainID, TravelDate, DepartureTime, ArrivalTime, QuotaTypeID, TrainClassID, TotalSeats, AvailableSeats, Fare)
        VALUES 
        (2, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('06:00:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('22:30:00' AS TIME), 1, 5, 80, 55, 1800.00), -- CC
        (2, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('06:00:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('22:30:00' AS TIME), 1, 2, 60, 40, 2200.00); -- 3A
    END

    -- Duronto Express (Kolkata to Delhi) - 3 times a week
    IF DATEPART(WEEKDAY, @CurrentDate) IN (2, 4, 6) -- Monday, Wednesday, Friday
    BEGIN
        INSERT INTO TrainSchedules (TrainID, TravelDate, DepartureTime, ArrivalTime, QuotaTypeID, TrainClassID, TotalSeats, AvailableSeats, Fare)
        VALUES 
        (3, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('20:15:00' AS TIME), DATEADD(HOUR, 18, CAST(@CurrentDate AS DATETIME) + CAST('20:15:00' AS TIME)), 1, 1, 72, 50, 1800.00), -- SL
        (3, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('20:15:00' AS TIME), DATEADD(HOUR, 18, CAST(@CurrentDate AS DATETIME) + CAST('20:15:00' AS TIME)), 1, 2, 64, 35, 2500.00), -- 3A
        (3, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('20:15:00' AS TIME), DATEADD(HOUR, 18, CAST(@CurrentDate AS DATETIME) + CAST('20:15:00' AS TIME)), 1, 3, 48, 20, 3200.00); -- 2A
    END

    -- Garib Rath (Bangalore to Mumbai) - Daily
    INSERT INTO TrainSchedules (TrainID, TravelDate, DepartureTime, ArrivalTime, QuotaTypeID, TrainClassID, TotalSeats, AvailableSeats, Fare)
    VALUES 
    (4, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('22:30:00' AS TIME), DATEADD(HOUR, 14, CAST(@CurrentDate AS DATETIME) + CAST('22:30:00' AS TIME)), 1, 2, 80, 60, 1500.00), -- 3A
    (4, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('22:30:00' AS TIME), DATEADD(HOUR, 14, CAST(@CurrentDate AS DATETIME) + CAST('22:30:00' AS TIME)), 1, 5, 100, 75, 800.00); -- CC

    -- Jan Shatabdi (Hyderabad to Bangalore) - Daily except Sunday
    IF DATEPART(WEEKDAY, @CurrentDate) != 1 -- Not Sunday
    BEGIN
        INSERT INTO TrainSchedules (TrainID, TravelDate, DepartureTime, ArrivalTime, QuotaTypeID, TrainClassID, TotalSeats, AvailableSeats, Fare)
        VALUES 
        (5, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('07:15:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('17:45:00' AS TIME), 1, 5, 120, 90, 600.00), -- CC
        (5, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('07:15:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('17:45:00' AS TIME), 1, 6, 80, 65, 350.00); -- 2S
    END

    -- Intercity Express (Pune to Mumbai) - Daily
    INSERT INTO TrainSchedules (TrainID, TravelDate, DepartureTime, ArrivalTime, QuotaTypeID, TrainClassID, TotalSeats, AvailableSeats, Fare)
    VALUES 
    (6, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('05:45:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('09:15:00' AS TIME), 1, 6, 150, 120, 180.00), -- 2S
    (6, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('05:45:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('09:15:00' AS TIME), 1, 5, 80, 60, 250.00); -- CC

    -- Superfast Express (Ahmedabad to Delhi) - Daily
    INSERT INTO TrainSchedules (TrainID, TravelDate, DepartureTime, ArrivalTime, QuotaTypeID, TrainClassID, TotalSeats, AvailableSeats, Fare)
    VALUES 
    (7, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('19:30:00' AS TIME), DATEADD(HOUR, 12, CAST(@CurrentDate AS DATETIME) + CAST('19:30:00' AS TIME)), 1, 1, 72, 45, 1200.00), -- SL
    (7, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('19:30:00' AS TIME), DATEADD(HOUR, 12, CAST(@CurrentDate AS DATETIME) + CAST('19:30:00' AS TIME)), 1, 2, 64, 30, 1800.00), -- 3A
    (7, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('19:30:00' AS TIME), DATEADD(HOUR, 12, CAST(@CurrentDate AS DATETIME) + CAST('19:30:00' AS TIME)), 1, 5, 80, 55, 900.00); -- CC

    -- Mail Express (Jaipur to Delhi) - Daily
    INSERT INTO TrainSchedules (TrainID, TravelDate, DepartureTime, ArrivalTime, QuotaTypeID, TrainClassID, TotalSeats, AvailableSeats, Fare)
    VALUES 
    (8, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('21:45:00' AS TIME), DATEADD(HOUR, 6, CAST(@CurrentDate AS DATETIME) + CAST('21:45:00' AS TIME)), 1, 1, 72, 55, 450.00), -- SL
    (8, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('21:45:00' AS TIME), DATEADD(HOUR, 6, CAST(@CurrentDate AS DATETIME) + CAST('21:45:00' AS TIME)), 1, 6, 100, 80, 200.00); -- 2S

    -- Passenger Train (Lucknow to Kolkata) - 4 times a week
    IF DATEPART(WEEKDAY, @CurrentDate) IN (1, 3, 5, 7) -- Sunday, Tuesday, Thursday, Saturday
    BEGIN
        INSERT INTO TrainSchedules (TrainID, TravelDate, DepartureTime, ArrivalTime, QuotaTypeID, TrainClassID, TotalSeats, AvailableSeats, Fare)
        VALUES 
        (9, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('14:30:00' AS TIME), DATEADD(HOUR, 16, CAST(@CurrentDate AS DATETIME) + CAST('14:30:00' AS TIME)), 1, 1, 72, 60, 800.00), -- SL
        (9, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('14:30:00' AS TIME), DATEADD(HOUR, 16, CAST(@CurrentDate AS DATETIME) + CAST('14:30:00' AS TIME)), 1, 6, 120, 100, 350.00); -- 2S
    END

    -- Express Train (Delhi to Bangalore) - 5 times a week
    IF DATEPART(WEEKDAY, @CurrentDate) IN (1, 2, 4, 5, 6) -- Sunday, Monday, Wednesday, Thursday, Friday
    BEGIN
        INSERT INTO TrainSchedules (TrainID, TravelDate, DepartureTime, ArrivalTime, QuotaTypeID, TrainClassID, TotalSeats, AvailableSeats, Fare)
        VALUES 
        (10, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('12:15:00' AS TIME), DATEADD(HOUR, 32, CAST(@CurrentDate AS DATETIME) + CAST('12:15:00' AS TIME)), 1, 1, 72, 40, 2800.00), -- SL
        (10, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('12:15:00' AS TIME), DATEADD(HOUR, 32, CAST(@CurrentDate AS DATETIME) + CAST('12:15:00' AS TIME)), 1, 2, 64, 25, 3800.00), -- 3A
        (10, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('12:15:00' AS TIME), DATEADD(HOUR, 32, CAST(@CurrentDate AS DATETIME) + CAST('12:15:00' AS TIME)), 1, 3, 48, 18, 4800.00); -- 2A
    END

    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
END

-- Reset date for flight schedules
SET @CurrentDate = @StartDate;

-- Seed Flight Schedules
WHILE @CurrentDate <= @EndDate
BEGIN
    -- AI101 (Delhi to Chennai) - Daily
    INSERT INTO FlightSchedules (FlightID, TravelDate, DepartureTime, ArrivalTime, FlightClassID, TotalSeats, AvailableSeats, Fare)
    VALUES 
    (1, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('08:30:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('11:15:00' AS TIME), 1, 180, 120, 8500.00), -- Economy
    (1, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('08:30:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('11:15:00' AS TIME), 2, 24, 15, 15000.00), -- Premium Economy
    (1, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('08:30:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('11:15:00' AS TIME), 3, 16, 8, 25000.00); -- Business

    -- 6E202 (Delhi to Mumbai) - Daily multiple flights
    INSERT INTO FlightSchedules (FlightID, TravelDate, DepartureTime, ArrivalTime, FlightClassID, TotalSeats, AvailableSeats, Fare)
    VALUES 
    (2, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('06:15:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('08:30:00' AS TIME), 1, 186, 140, 4500.00), -- Economy Morning
    (2, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('14:20:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('16:35:00' AS TIME), 1, 186, 110, 5200.00), -- Economy Afternoon
    (2, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('20:45:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('23:00:00' AS TIME), 1, 186, 95, 6800.00); -- Economy Evening

    -- SG303 (Kolkata to Delhi) - Daily
    INSERT INTO FlightSchedules (FlightID, TravelDate, DepartureTime, ArrivalTime, FlightClassID, TotalSeats, AvailableSeats, Fare)
    VALUES 
    (3, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('16:40:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('19:10:00' AS TIME), 1, 180, 130, 7200.00), -- Economy
    (3, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('16:40:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('19:10:00' AS TIME), 3, 12, 6, 18000.00); -- Business

    -- UK404 (Bangalore to Mumbai) - Daily
    INSERT INTO FlightSchedules (FlightID, TravelDate, DepartureTime, ArrivalTime, FlightClassID, TotalSeats, AvailableSeats, Fare)
    VALUES 
    (4, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('11:25:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('12:55:00' AS TIME), 1, 158, 100, 3800.00), -- Economy
    (4, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('11:25:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('12:55:00' AS TIME), 3, 8, 4, 9500.00); -- Business

    -- AI505 (Hyderabad to Bangalore) - Daily
    INSERT INTO FlightSchedules (FlightID, TravelDate, DepartureTime, ArrivalTime, FlightClassID, TotalSeats, AvailableSeats, Fare)
    VALUES 
    (5, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('13:15:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('14:30:00' AS TIME), 1, 138, 90, 2500.00), -- Economy
    (5, @CurrentDate, CAST(@CurrentDate AS DATETIME) + CAST('13:15:00' AS TIME), CAST(@CurrentDate AS DATETIME) + CAST('14:30:00' AS TIME), 3, 8, 5, 6500.00); -- Business

    SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
END

PRINT 'Schedule seed data inserted successfully!';
PRINT CAST(@@ROWCOUNT AS VARCHAR) + ' total schedules created for the next 30 days.';
GO