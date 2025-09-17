# Transit-Hub Database Schema

## Overview
The Transit-Hub database consists of 20+ tables designed to handle train and flight booking operations with comprehensive audit trails and real-world business logic.

## Core Entities

### User Management
- **Users**: Customer accounts with verification status
- **Admins**: Administrative users with role-based access
- **GmailVerificationTokens**: Email verification tokens

### Master Data
- **Stations**: Train stations with location details
- **Airports**: Flight airports with IATA codes
- **Trains**: Train information with source/destination
- **Flights**: Flight information with airline details

### Classification Tables
- **TrainQuotaTypes**: Normal, Tatkal quota types
- **BookingStatusTypes**: Confirmed, Waitlisted, Cancelled
- **PaymentModes**: Card, UPI, NetBanking
- **TrainClasses**: Sleeper, 3AC, 2AC, 1AC
- **FlightClasses**: Economy, Business

### Operational Tables
- **TrainSchedules**: Train schedules with pricing
- **FlightSchedules**: Flight schedules with pricing
- **Bookings**: Main booking records
- **BookingPassengers**: Passenger details per booking
- **Payments**: Payment transactions
- **SeatAllocations**: Individual seat assignments
- **WaitlistQueue**: Waitlist management

### Audit & Logging
- **BookingAudit**: Booking action history
- **Notifications**: User notifications
- **SystemLogs**: Application logs
- **Cancellations**: Cancellation tracking

## Key Features
- Comprehensive audit fields on all tables
- Support for both train and flight bookings
- Waitlist and seat reallocation logic
- Senior citizen preference handling
- Payment tracking with transaction references
- Email verification workflow
- Role-based admin access