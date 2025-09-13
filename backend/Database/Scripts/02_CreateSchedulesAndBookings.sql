-- Transit-Hub Database Schema - Part 2
-- Schedules, Bookings, and Operational Tables

USE TransitHubDB;
GO

-- 12. TrainSchedules Table
CREATE TABLE TrainSchedules (
    ScheduleID int IDENTITY(1,1) PRIMARY KEY,
    TrainID int NOT NULL,
    TravelDate date NOT NULL,
    DepartureTime datetime2 NOT NULL,
    ArrivalTime datetime2 NOT NULL,
    QuotaTypeID int NOT NULL,
    TrainClassID int NOT NULL,
    TotalSeats int NOT NULL CHECK (TotalSeats > 0),
    AvailableSeats int NOT NULL CHECK (AvailableSeats >= 0),
    Fare decimal(10,2) NOT NULL CHECK (Fare >= 0),
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1,
    FOREIGN KEY (TrainID) REFERENCES Trains(TrainID),
    FOREIGN KEY (QuotaTypeID) REFERENCES TrainQuotaTypes(QuotaTypeID),
    FOREIGN KEY (TrainClassID) REFERENCES TrainClasses(TrainClassID),
    CHECK (ArrivalTime > DepartureTime),
    CHECK (AvailableSeats <= TotalSeats)
);

-- 13. FlightSchedules Table
CREATE TABLE FlightSchedules (
    ScheduleID int IDENTITY(1,1) PRIMARY KEY,
    FlightID int NOT NULL,
    TravelDate date NOT NULL,
    DepartureTime datetime2 NOT NULL,
    ArrivalTime datetime2 NOT NULL,
    FlightClassID int NOT NULL,
    TotalSeats int NOT NULL CHECK (TotalSeats > 0),
    AvailableSeats int NOT NULL CHECK (AvailableSeats >= 0),
    Fare decimal(10,2) NOT NULL CHECK (Fare >= 0),
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1,
    FOREIGN KEY (FlightID) REFERENCES Flights(FlightID),
    FOREIGN KEY (FlightClassID) REFERENCES FlightClasses(FlightClassID),
    CHECK (ArrivalTime > DepartureTime),
    CHECK (AvailableSeats <= TotalSeats)
);

-- 14. Bookings Table
CREATE TABLE Bookings (
    BookingID int IDENTITY(1,1) PRIMARY KEY,
    BookingReference nvarchar(20) NOT NULL UNIQUE,
    UserID int NOT NULL,
    BookingType nvarchar(10) NOT NULL CHECK (BookingType IN ('Train', 'Flight')),
    TrainScheduleID int NULL,
    FlightScheduleID int NULL,
    StatusID int NOT NULL,
    TotalPassengers int NOT NULL CHECK (TotalPassengers > 0 AND TotalPassengers <= 6),
    TotalAmount decimal(10,2) NOT NULL CHECK (TotalAmount >= 0),
    BookingDate datetime2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (TrainScheduleID) REFERENCES TrainSchedules(ScheduleID),
    FOREIGN KEY (FlightScheduleID) REFERENCES FlightSchedules(ScheduleID),
    FOREIGN KEY (StatusID) REFERENCES BookingStatusTypes(StatusID),
    CHECK (
        (BookingType = 'Train' AND TrainScheduleID IS NOT NULL AND FlightScheduleID IS NULL) OR
        (BookingType = 'Flight' AND FlightScheduleID IS NOT NULL AND TrainScheduleID IS NULL)
    )
);

-- 15. BookingPassengers Table
CREATE TABLE BookingPassengers (
    PassengerID int IDENTITY(1,1) PRIMARY KEY,
    BookingID int NOT NULL,
    Name nvarchar(100) NOT NULL,
    Age int NOT NULL CHECK (Age >= 0 AND Age <= 120),
    Gender nvarchar(10) NOT NULL CHECK (Gender IN ('Male', 'Female', 'Other')),
    IsSeniorCitizen bit NOT NULL DEFAULT 0,
    SeatNumber nvarchar(10) NULL,
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1,
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID)
);

-- 16. Payments Table
CREATE TABLE Payments (
    PaymentID int IDENTITY(1,1) PRIMARY KEY,
    BookingID int NOT NULL,
    PaymentModeID int NOT NULL,
    Amount decimal(10,2) NOT NULL CHECK (Amount > 0),
    Status nvarchar(20) NOT NULL CHECK (Status IN ('Pending', 'Success', 'Failed', 'Refunded')),
    TransactionRef nvarchar(100) NULL,
    PaymentDate datetime2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1,
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID),
    FOREIGN KEY (PaymentModeID) REFERENCES PaymentModes(PaymentModeID)
);

-- 17. WaitlistQueue Table
CREATE TABLE WaitlistQueue (
    WaitlistID int IDENTITY(1,1) PRIMARY KEY,
    BookingID int NOT NULL,
    ScheduleType nvarchar(10) NOT NULL CHECK (ScheduleType IN ('Train', 'Flight')),
    TrainScheduleID int NULL,
    FlightScheduleID int NULL,
    QueuePosition int NOT NULL CHECK (QueuePosition > 0),
    Priority int NOT NULL DEFAULT 1, -- Higher number = higher priority
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1,
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID),
    FOREIGN KEY (TrainScheduleID) REFERENCES TrainSchedules(ScheduleID),
    FOREIGN KEY (FlightScheduleID) REFERENCES FlightSchedules(ScheduleID),
    CHECK (
        (ScheduleType = 'Train' AND TrainScheduleID IS NOT NULL AND FlightScheduleID IS NULL) OR
        (ScheduleType = 'Flight' AND FlightScheduleID IS NOT NULL AND TrainScheduleID IS NULL)
    )
);

-- 18. Cancellations Table
CREATE TABLE Cancellations (
    CancellationID int IDENTITY(1,1) PRIMARY KEY,
    BookingID int NOT NULL,
    CancellationReason nvarchar(500) NULL,
    RefundAmount decimal(10,2) NOT NULL DEFAULT 0,
    CancellationFee decimal(10,2) NOT NULL DEFAULT 0,
    CancelledBy nvarchar(100) NOT NULL,
    CancelledAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID)
);