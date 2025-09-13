-- Transit-Hub Database Schema
-- Created: 2025-09-09
-- Description: Complete database schema for train and flight booking system

USE master;
GO

-- Create Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TransitHubDB')
BEGIN
    CREATE DATABASE TransitHubDB;
END
GO

USE TransitHubDB;
GO

-- 1. Users Table
CREATE TABLE Users (
    UserID int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    Email nvarchar(255) NOT NULL UNIQUE,
    PasswordHash nvarchar(500) NOT NULL,
    Phone nvarchar(15) NOT NULL,
    Age int NOT NULL CHECK (Age >= 0 AND Age <= 120),
    IsSeniorCitizen bit NOT NULL DEFAULT 0,
    IsVerified bit NOT NULL DEFAULT 0,
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1
);

-- 2. Admins Table
CREATE TABLE Admins (
    AdminID int IDENTITY(1,1) PRIMARY KEY,
    Name nvarchar(100) NOT NULL,
    Email nvarchar(255) NOT NULL UNIQUE,
    PasswordHash nvarchar(500) NOT NULL,
    Phone nvarchar(15) NOT NULL,
    Role nvarchar(50) NOT NULL CHECK (Role IN ('SuperAdmin', 'Admin')),
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1
);

-- 3. Stations Table
CREATE TABLE Stations (
    StationID int IDENTITY(1,1) PRIMARY KEY,
    StationName nvarchar(100) NOT NULL,
    City nvarchar(100) NOT NULL,
    State nvarchar(100) NOT NULL,
    StationCode nvarchar(10) NOT NULL UNIQUE,
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1
);

-- 4. Airports Table
CREATE TABLE Airports (
    AirportID int IDENTITY(1,1) PRIMARY KEY,
    AirportName nvarchar(150) NOT NULL,
    City nvarchar(100) NOT NULL,
    State nvarchar(100) NOT NULL,
    Code nvarchar(5) NOT NULL UNIQUE, -- IATA Code (DEL, BLR, etc.)
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1
);

-- 5. Trains Table
CREATE TABLE Trains (
    TrainID int IDENTITY(1,1) PRIMARY KEY,
    TrainName nvarchar(100) NOT NULL,
    TrainNumber nvarchar(10) NOT NULL UNIQUE,
    SourceStationID int NOT NULL,
    DestinationStationID int NOT NULL,
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1,
    FOREIGN KEY (SourceStationID) REFERENCES Stations(StationID),
    FOREIGN KEY (DestinationStationID) REFERENCES Stations(StationID),
    CHECK (SourceStationID != DestinationStationID)
);

-- 6. Flights Table
CREATE TABLE Flights (
    FlightID int IDENTITY(1,1) PRIMARY KEY,
    FlightNumber nvarchar(10) NOT NULL UNIQUE,
    Airline nvarchar(100) NOT NULL,
    SourceAirportID int NOT NULL,
    DestinationAirportID int NOT NULL,
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1,
    FOREIGN KEY (SourceAirportID) REFERENCES Airports(AirportID),
    FOREIGN KEY (DestinationAirportID) REFERENCES Airports(AirportID),
    CHECK (SourceAirportID != DestinationAirportID)
);

-- 7. TrainQuotaTypes Table
CREATE TABLE TrainQuotaTypes (
    QuotaTypeID int IDENTITY(1,1) PRIMARY KEY,
    QuotaName nvarchar(50) NOT NULL UNIQUE,
    Description nvarchar(200) NULL,
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1
);

-- 8. BookingStatusTypes Table
CREATE TABLE BookingStatusTypes (
    StatusID int IDENTITY(1,1) PRIMARY KEY,
    StatusName nvarchar(50) NOT NULL UNIQUE,
    Description nvarchar(200) NULL,
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1
);

-- 9. PaymentModes Table
CREATE TABLE PaymentModes (
    PaymentModeID int IDENTITY(1,1) PRIMARY KEY,
    ModeName nvarchar(50) NOT NULL UNIQUE,
    Description nvarchar(200) NULL,
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1
);

-- 10. TrainClasses Table
CREATE TABLE TrainClasses (
    TrainClassID int IDENTITY(1,1) PRIMARY KEY,
    ClassName nvarchar(50) NOT NULL UNIQUE,
    Description nvarchar(200) NULL,
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1
);

-- 11. FlightClasses Table
CREATE TABLE FlightClasses (
    FlightClassID int IDENTITY(1,1) PRIMARY KEY,
    ClassName nvarchar(50) NOT NULL UNIQUE,
    Description nvarchar(200) NULL,
    CreatedBy nvarchar(100) NOT NULL DEFAULT 'System',
    CreatedAt datetime2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedBy nvarchar(100) NULL,
    UpdatedAt datetime2 NULL,
    IsActive bit NOT NULL DEFAULT 1
);