-- Transit-Hub Database - Seed Data
-- Initial data for lookup tables and test data

USE TransitHubDB;
GO

-- Seed TrainQuotaTypes
INSERT INTO TrainQuotaTypes (QuotaName, Description) VALUES
('Normal', 'Regular booking quota'),
('Tatkal', 'Emergency booking quota (premium charges)'),
('Premium Tatkal', 'Premium emergency booking quota'),
('Senior Citizen', 'Senior citizen quota'),
('Ladies', 'Ladies quota'),
('Physically Handicapped', 'Quota for physically handicapped passengers');

-- Seed BookingStatusTypes
INSERT INTO BookingStatusTypes (StatusName, Description) VALUES
('Confirmed', 'Booking confirmed with seat allocation'),
('Waitlisted', 'Booking in waitlist queue'),
('Cancelled', 'Booking cancelled by user or system'),
('RAC', 'Reservation Against Cancellation'),
('Chart Prepared', 'Final chart prepared, no more bookings');

-- Seed PaymentModes
INSERT INTO PaymentModes (ModeName, Description) VALUES
('Credit Card', 'Payment via credit card'),
('Debit Card', 'Payment via debit card'),
('UPI', 'Unified Payments Interface'),
('Net Banking', 'Internet banking payment'),
('Wallet', 'Digital wallet payment');

-- Seed TrainClasses
INSERT INTO TrainClasses (ClassName, Description) VALUES
('SL', 'Sleeper Class'),
('3A', 'Third AC'),
('2A', 'Second AC'),
('1A', 'First AC'),
('CC', 'Chair Car'),
('2S', 'Second Sitting'),
('FC', 'First Class');

-- Seed FlightClasses
INSERT INTO FlightClasses (ClassName, Description) VALUES
('Economy', 'Economy class seating'),
('Premium Economy', 'Premium economy class'),
('Business', 'Business class seating'),
('First', 'First class seating');

-- Seed States (Major Indian States)
INSERT INTO Stations (StationName, City, State, StationCode) VALUES
('New Delhi Railway Station', 'New Delhi', 'Delhi', 'NDLS'),
('Mumbai Central', 'Mumbai', 'Maharashtra', 'BCT'),
('Howrah Junction', 'Kolkata', 'West Bengal', 'HWH'),
('Chennai Central', 'Chennai', 'Tamil Nadu', 'MAS'),
('Bangalore City Junction', 'Bangalore', 'Karnataka', 'SBC'),
('Hyderabad Deccan', 'Hyderabad', 'Telangana', 'HYB'),
('Pune Junction', 'Pune', 'Maharashtra', 'PUNE'),
('Ahmedabad Junction', 'Ahmedabad', 'Gujarat', 'ADI'),
('Jaipur Junction', 'Jaipur', 'Rajasthan', 'JP'),
('Lucknow Junction', 'Lucknow', 'Uttar Pradesh', 'LJN');

-- Seed Airports
INSERT INTO Airports (AirportName, City, State, Code) VALUES
('Indira Gandhi International Airport', 'New Delhi', 'Delhi', 'DEL'),
('Chhatrapati Shivaji International Airport', 'Mumbai', 'Maharashtra', 'BOM'),
('Netaji Subhas Chandra Bose International Airport', 'Kolkata', 'West Bengal', 'CCU'),
('Chennai International Airport', 'Chennai', 'Tamil Nadu', 'MAA'),
('Kempegowda International Airport', 'Bangalore', 'Karnataka', 'BLR'),
('Rajiv Gandhi International Airport', 'Hyderabad', 'Telangana', 'HYD'),
('Pune Airport', 'Pune', 'Maharashtra', 'PNQ'),
('Sardar Vallabhbhai Patel International Airport', 'Ahmedabad', 'Gujarat', 'AMD'),
('Jaipur International Airport', 'Jaipur', 'Rajasthan', 'JAI'),
('Chaudhary Charan Singh International Airport', 'Lucknow', 'Uttar Pradesh', 'LKO');

-- Seed Sample Trains
INSERT INTO Trains (TrainName, TrainNumber, SourceStationID, DestinationStationID) VALUES
('Rajdhani Express', '12301', 1, 4), -- Delhi to Chennai
('Shatabdi Express', '12002', 1, 2), -- Delhi to Mumbai
('Duronto Express', '12259', 3, 1), -- Kolkata to Delhi
('Garib Rath', '12204', 5, 2), -- Bangalore to Mumbai
('Jan Shatabdi', '12023', 6, 5), -- Hyderabad to Bangalore
('Intercity Express', '12345', 7, 2), -- Pune to Mumbai
('Superfast Express', '12678', 8, 1), -- Ahmedabad to Delhi
('Mail Express', '11234', 9, 1), -- Jaipur to Delhi
('Passenger Train', '56789', 10, 3), -- Lucknow to Kolkata
('Express Train', '13579', 1, 5); -- Delhi to Bangalore

-- Seed Sample Flights
INSERT INTO Flights (FlightNumber, Airline, SourceAirportID, DestinationAirportID) VALUES
('AI101', 'Air India', 1, 4), -- Delhi to Chennai
('6E202', 'IndiGo', 1, 2), -- Delhi to Mumbai
('SG303', 'SpiceJet', 3, 1), -- Kolkata to Delhi
('UK404', 'Vistara', 5, 2), -- Bangalore to Mumbai
('AI505', 'Air India', 6, 5), -- Hyderabad to Bangalore
('6E606', 'IndiGo', 7, 2), -- Pune to Mumbai
('SG707', 'SpiceJet', 8, 1), -- Ahmedabad to Delhi
('UK808', 'Vistara', 9, 1), -- Jaipur to Delhi
('AI909', 'Air India', 10, 3), -- Lucknow to Kolkata
('6E010', 'IndiGo', 1, 5); -- Delhi to Bangalore

-- Create Sample Admin User
INSERT INTO Admins (Name, Email, PasswordHash, Phone, Role) VALUES
('System Administrator', 'admin@transithub.com', 'hashed_password_here', '+919876543210', 'SuperAdmin');

PRINT 'Seed data inserted successfully!';
GO