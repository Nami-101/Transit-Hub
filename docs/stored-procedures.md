# Transit-Hub Stored Procedures Documentation

## Overview
This document provides comprehensive documentation for all stored procedures implemented in the Transit-Hub system. Each procedure includes proper error handling, transaction management, and audit logging.

## Error Handling Strategy
- **Custom Error Codes**: 50001-59999 range for business logic errors
- **Transaction Management**: All procedures use BEGIN/COMMIT/ROLLBACK
- **Audit Logging**: All operations are logged in SystemLogs table
- **Structured Responses**: Consistent return format with Success flag

## Procedure Categories

### 1. User Management Procedures

#### sp_RegisterUser
**Purpose**: Register a new user with email verification
**Parameters**:
- `@Name` (NVARCHAR(100)): User's full name
- `@Email` (NVARCHAR(255))`: User's email address
- `@PasswordHash` (NVARCHAR(500)): Hashed password
- `@Phone` (NVARCHAR(15))`: Phone number
- `@Age` (INT): User's age
- `@CreatedBy` (NVARCHAR(100)): Who created the record

**Business Rules**:
- Email must be unique
- Phone must be unique
- Age must be between 0-120
- Senior citizen status auto-calculated (age >= 60)
- Generates verification token valid for 24 hours

**Returns**: UserID, VerificationToken, Success status

#### sp_LoginUser
**Purpose**: Authenticate user login
**Parameters**:
- `@Email` (NVARCHAR(255))`: User's email
- `@PasswordHash` (NVARCHAR(500))`: Hashed password

**Business Rules**:
- User must be active and verified
- Password must match stored hash

**Returns**: User profile data with Success status

#### sp_VerifyEmail
**Purpose**: Verify user's email address
**Parameters**:
- `@Token` (NVARCHAR(500))`: Verification token

**Business Rules**:
- Token must be valid and not expired
- Token can only be used once
- Marks user as verified

**Returns**: Success status with message

### 2. Search Operations Procedures

#### sp_SearchTrains
**Purpose**: Search available trains with real-time availability
**Parameters**:
- `@SourceStationID/Code`: Source station
- `@DestinationStationID/Code`: Destination station
- `@TravelDate` (DATE): Journey date
- `@QuotaTypeID` (INT, Optional): Quota filter
- `@TrainClassID` (INT, Optional): Class filter
- `@PassengerCount` (INT): Number of passengers

**Business Rules**:
- Travel date cannot be in past
- Passenger count: 1-6
- Shows availability status (Available/Limited/Waitlist)
- Includes waitlist position if applicable

**Returns**: Train schedules with availability and pricing

#### sp_SearchFlights
**Purpose**: Search available flights
**Parameters**: Similar to train search but for flights
**Business Rules**:
- Flights don't support waitlist
- Only shows available flights

**Returns**: Flight schedules with availability

### 3. Booking Operations Procedures

#### sp_CreateTrainBooking
**Purpose**: Create train booking with waitlist support
**Parameters**:
- `@UserID` (INT): User making booking
- `@TrainScheduleID` (INT): Selected train schedule
- `@PassengerDetails` (NVARCHAR(MAX))`: JSON array of passengers
- `@CreatedBy` (NVARCHAR(100))`: Creator name

**Business Rules**:
- User must be verified
- Tatkal booking time restrictions (10 AM, 1 day before)
- Auto-waitlist if seats unavailable
- Senior citizen priority in waitlist
- Generates unique booking reference

**Returns**: BookingID, BookingReference, Status, Amount

#### sp_CreateFlightBooking
**Purpose**: Create flight booking (no waitlist)
**Parameters**: Similar to train booking
**Business Rules**:
- Must have available seats
- No waitlist support
- Immediate confirmation

**Returns**: Booking details with confirmation

### 4. Payment Operations Procedures

#### sp_ProcessPayment
**Purpose**: Process payment for booking
**Parameters**:
- `@BookingID` (INT): Booking to pay for
- `@PaymentModeID` (INT): Payment method
- `@Amount` (DECIMAL(10,2))`: Payment amount
- `@TransactionRef` (NVARCHAR(100), Optional): Transaction reference

**Business Rules**:
- Amount must match booking amount
- Booking must be in valid state
- Simulates 95% success rate
- Generates transaction reference if not provided

**Returns**: Payment status and transaction details

#### sp_ProcessRefund
**Purpose**: Process refund for cancelled booking
**Parameters**:
- `@BookingID` (INT): Booking to refund
- `@RefundAmount` (DECIMAL(10,2))`: Refund amount
- `@CancellationFee` (DECIMAL(10,2))`: Cancellation charges
- `@ProcessedBy` (NVARCHAR(100))`: Who processed refund

**Business Rules**:
- Must have successful payment
- Refund + fee must equal payment amount
- Updates booking status to cancelled

**Returns**: Refund confirmation details

### 5. Waitlist Operations Procedures

#### sp_PromoteWaitlist
**Purpose**: Auto-promote waitlisted bookings when seats available
**Parameters**:
- `@TrainScheduleID/FlightScheduleID`: Schedule with available seats
- `@AvailableSeats` (INT): Number of seats to allocate
- `@ProcessedBy` (NVARCHAR(100))`: Who triggered promotion

**Business Rules**:
- Processes by priority then queue position
- Senior citizens get higher priority
- Updates seat availability
- Sends notifications to promoted users

**Returns**: List of promoted bookings

#### sp_CancelBookingAndPromoteWaitlist
**Purpose**: Cancel booking and auto-promote waitlist
**Parameters**:
- `@BookingID` (INT): Booking to cancel
- `@CancellationReason` (NVARCHAR(500), Optional): Reason
- `@CancelledBy` (NVARCHAR(100))`: Who cancelled

**Business Rules**:
- Releases seats for confirmed bookings
- Removes from waitlist if waitlisted
- Auto-promotes next in queue
- Updates queue positions

**Returns**: Cancellation confirmation

### 6. Admin Operations Procedures

#### sp_GetDashboardStats
**Purpose**: Get comprehensive dashboard statistics
**Parameters**:
- `@AdminID` (INT): Admin requesting stats

**Returns**: Multiple result sets:
- Overall statistics (users, bookings, revenue)
- Monthly trends (last 6 months)
- Top train routes
- Top flight routes

#### sp_ManageTrainSchedule
**Purpose**: CRUD operations for train schedules
**Parameters**:
- `@Action` (NVARCHAR(10))`: INSERT/UPDATE/DELETE
- Schedule details (conditional based on action)
- `@AdminID` (INT): Admin performing action

**Business Rules**:
- Prevents duplicate schedules
- Cannot delete schedules with active bookings
- Validates all required fields for insert

**Returns**: Operation success confirmation

#### sp_GetBookingReports
**Purpose**: Generate booking reports
**Parameters**:
- `@ReportType` (NVARCHAR(20))`: DAILY/WEEKLY/MONTHLY/CUSTOM
- `@StartDate/@EndDate` (DATE, Optional): For custom reports
- `@BookingType` (NVARCHAR(10), Optional): Filter by type
- `@AdminID` (INT): Admin requesting report

**Returns**: Detailed booking analytics and summary

## Error Codes Reference

| Code | Description |
|------|-------------|
| 50001 | Email already registered |
| 50002 | Phone number already registered |
| 50003 | Invalid age range |
| 50004 | Invalid email or password |
| 50005 | Email not verified |
| 50006-50008 | Verification token issues |
| 50009 | User not found |
| 50010 | Travel date in past |
| 50011 | Invalid passenger count |
| 50012-50015 | Station/Airport validation errors |
| 50016 | Invalid or unverified user |
| 50017-50022 | Booking validation errors |
| 50023-50031 | Payment processing errors |
| 50032-50034 | Waitlist operation errors |
| 50035-50043 | Admin operation errors |

## Usage Examples

### Register User
```sql
EXEC sp_RegisterUser 
    @Name = 'John Doe',
    @Email = 'john@example.com',
    @PasswordHash = 'hashed_password',
    @Phone = '+919876543210',
    @Age = 30;
```

### Search Trains
```sql
EXEC sp_SearchTrains 
    @SourceStationCode = 'NDLS',
    @DestinationStationCode = 'MAS',
    @TravelDate = '2025-09-15',
    @PassengerCount = 2;
```

### Create Booking
```sql
EXEC sp_CreateTrainBooking 
    @UserID = 1,
    @TrainScheduleID = 5,
    @PassengerDetails = '[{"Name":"John","Age":30,"Gender":"Male"},{"Name":"Jane","Age":28,"Gender":"Female"}]',
    @CreatedBy = 'User';
```

## Integration Notes

1. **Entity Framework Integration**: Use `FromSqlRaw()` or `ExecuteSqlRaw()` methods
2. **Parameter Mapping**: Map C# models to procedure parameters
3. **Error Handling**: Catch `SqlException` and parse error codes
4. **Transaction Scope**: Procedures handle their own transactions
5. **Async Support**: Use async versions of EF methods

## Performance Considerations

1. **Indexing**: All procedures leverage existing indexes
2. **Query Plans**: Procedures use parameterized queries for plan reuse
3. **Batch Operations**: Waitlist promotion handles multiple bookings efficiently
4. **Minimal Locking**: Short transaction scopes to reduce blocking
5. **Result Set Optimization**: Only return necessary columns

## Security Features

1. **SQL Injection Protection**: All inputs are parameterized
2. **Business Rule Enforcement**: Server-side validation
3. **Audit Trail**: All operations logged
4. **Role-based Access**: Admin procedures validate admin permissions
5. **Data Integrity**: Foreign key and check constraints enforced