-- Transit-Hub Database Schema - Part 3
-- Audit, Logs, and Verification Tables

USE TransitHubDB;
GO

-- 19. GmailVerificationTokens Table
CREATE TABLE GmailVerificationTokens (
    TokenID int IDENTITY(1,1) PRIMARY KEY,
    UserID int NOT NULL,
    Token nvarchar(500) NOT NULL UNIQUE,
    ExpiryDate datetime2 NOT NULL,
    IsUsed bit NOT NULL DEFAULT 0,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- 20. BookingAudit Table
CREATE TABLE BookingAudit (
    AuditID int IDENTITY(1,1) PRIMARY KEY,
    BookingID int NOT NULL,
    Action nvarchar(100) NOT NULL,
    ActionBy nvarchar(100) NOT NULL,
    ActionTime datetime2 NOT NULL DEFAULT GETUTCDATE(),
    Details nvarchar(1000) NULL,
    OldValues nvarchar(max) NULL, -- JSON format for old values
    NewValues nvarchar(max) NULL, -- JSON format for new values
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID)
);

-- 21. Notifications Table
CREATE TABLE Notifications (
    NotificationID int IDENTITY(1,1) PRIMARY KEY,
    UserID int NOT NULL,
    Title nvarchar(200) NOT NULL,
    Message nvarchar(1000) NOT NULL,
    Type nvarchar(50) NOT NULL DEFAULT 'Info', -- Info, Success, Warning, Error
    Status nvarchar(20) NOT NULL DEFAULT 'Unread' CHECK (Status IN ('Unread', 'Read')),
    RelatedBookingID int NULL,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    ReadAt datetime2 NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (RelatedBookingID) REFERENCES Bookings(BookingID)
);

-- 22. SystemLogs Table
CREATE TABLE SystemLogs (
    LogID int IDENTITY(1,1) PRIMARY KEY,
    LogLevel nvarchar(20) NOT NULL CHECK (LogLevel IN ('Error', 'Warning', 'Info', 'Debug')),
    Message nvarchar(max) NOT NULL,
    StackTrace nvarchar(max) NULL,
    UserID int NULL,
    RequestPath nvarchar(500) NULL,
    UserAgent nvarchar(500) NULL,
    IPAddress nvarchar(50) NULL,
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Create Indexes for Performance
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_IsActive ON Users(IsActive);
CREATE INDEX IX_Bookings_UserID ON Bookings(UserID);
CREATE INDEX IX_Bookings_BookingReference ON Bookings(BookingReference);
CREATE INDEX IX_Bookings_BookingDate ON Bookings(BookingDate);
CREATE INDEX IX_TrainSchedules_TravelDate ON TrainSchedules(TravelDate);
CREATE INDEX IX_FlightSchedules_TravelDate ON FlightSchedules(TravelDate);
CREATE INDEX IX_Payments_BookingID ON Payments(BookingID);
CREATE INDEX IX_Notifications_UserID_Status ON Notifications(UserID, Status);
CREATE INDEX IX_SystemLogs_CreatedAt ON SystemLogs(CreatedAt);
CREATE INDEX IX_WaitlistQueue_QueuePosition ON WaitlistQueue(QueuePosition);

-- Create Unique Constraints
ALTER TABLE TrainSchedules ADD CONSTRAINT UQ_TrainSchedules_Unique 
    UNIQUE (TrainID, TravelDate, QuotaTypeID, TrainClassID);

ALTER TABLE FlightSchedules ADD CONSTRAINT UQ_FlightSchedules_Unique 
    UNIQUE (FlightID, TravelDate, FlightClassID);

-- Add Check Constraints for Business Rules
ALTER TABLE Users ADD CONSTRAINT CK_Users_SeniorCitizen 
    CHECK ((Age >= 60 AND IsSeniorCitizen = 1) OR (Age < 60 AND IsSeniorCitizen = 0));

ALTER TABLE BookingPassengers ADD CONSTRAINT CK_BookingPassengers_SeniorCitizen 
    CHECK ((Age >= 60 AND IsSeniorCitizen = 1) OR (Age < 60 AND IsSeniorCitizen = 0));

-- Add triggers for automatic updates
GO
CREATE TRIGGER TR_Users_UpdateTimestamp
ON Users
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Users 
    SET UpdatedAt = GETUTCDATE()
    FROM Users u
    INNER JOIN inserted i ON u.UserID = i.UserID;
END;
GO

CREATE TRIGGER TR_Bookings_UpdateTimestamp
ON Bookings
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Bookings 
    SET UpdatedAt = GETUTCDATE()
    FROM Bookings b
    INNER JOIN inserted i ON b.BookingID = i.BookingID;
END;
GO