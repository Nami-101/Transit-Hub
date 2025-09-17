# Transit-Hub API Documentation

## Overview
This document provides comprehensive API documentation for the Transit-Hub booking system. The API follows RESTful principles and uses the Generic Repository + Unit of Work pattern with stored procedures for optimal performance.

## Base URL
```
Development: https://localhost:7xxx/api
Production: https://api.transithub.com/api
```

## Authentication
Most endpoints require JWT authentication. Include the token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## API Endpoints

### 1. User Management (`/api/user`)

#### Register User
```http
POST /api/user/register
Content-Type: application/json

{
    "name": "John Doe",
    "email": "john@example.com",
    "password": "SecurePass123",
    "phone": "+919876543210",
    "age": 30
}
```

**Response:**
```json
{
    "userID": 1,
    "verificationToken": "abc123...",
    "message": "User registered successfully",
    "success": true
}
```

#### User Login
```http
POST /api/user/login
Content-Type: application/json

{
    "email": "john@example.com",
    "password": "SecurePass123"
}
```

**Response:**
```json
{
    "userID": 1,
    "name": "John Doe",
    "email": "john@example.com",
    "phone": "+919876543210",
    "age": 30,
    "isSeniorCitizen": false,
    "message": "Login successful",
    "success": true,
    "token": "jwt-token-here"
}
```

#### Verify Email
```http
POST /api/user/verify-email?token=abc123...
```

#### Get User Profile
```http
GET /api/user/profile/{userId}
Authorization: Bearer <token>
```

#### Update User Profile
```http
PUT /api/user/profile/{userId}
Authorization: Bearer <token>
Content-Type: application/json

{
    "name": "John Doe Updated",
    "phone": "+919876543211",
    "age": 31
}
```

### 2. Search Operations (`/api/search`)

#### Search Trains
```http
POST /api/search/trains
Content-Type: application/json

{
    "sourceStationCode": "NDLS",
    "destinationStationCode": "MAS",
    "travelDate": "2025-09-15",
    "quotaTypeID": 1,
    "trainClassID": 2,
    "passengerCount": 2
}
```

**Response:**
```json
[
    {
        "scheduleID": 1,
        "trainID": 1,
        "trainName": "Rajdhani Express",
        "trainNumber": "12301",
        "sourceStation": "New Delhi Railway Station",
        "sourceStationCode": "NDLS",
        "destinationStation": "Chennai Central",
        "destinationStationCode": "MAS",
        "travelDate": "2025-09-15",
        "departureTime": "2025-09-15T16:00:00",
        "arrivalTime": "2025-09-16T08:30:00",
        "journeyTimeMinutes": 990,
        "quotaName": "Normal",
        "trainClass": "3A",
        "totalSeats": 72,
        "availableSeats": 15,
        "fare": 2500.00,
        "availabilityStatus": "Available",
        "availableOrWaitlistPosition": 15
    }
]
```

#### Search Flights
```http
POST /api/search/flights
Content-Type: application/json

{
    "sourceAirportCode": "DEL",
    "destinationAirportCode": "MAA",
    "travelDate": "2025-09-15",
    "flightClassID": 1,
    "passengerCount": 2
}
```

#### Get Stations
```http
GET /api/search/stations
```

#### Get Airports
```http
GET /api/search/airports
```

#### Get Lookup Data
```http
GET /api/search/lookup-data
```

### 3. Booking Operations (`/api/booking`)

#### Create Train Booking
```http
POST /api/booking/train
Authorization: Bearer <token>
Content-Type: application/json

{
    "userID": 1,
    "trainScheduleID": 1,
    "passengers": [
        {
            "name": "John Doe",
            "age": 30,
            "gender": "Male"
        },
        {
            "name": "Jane Doe",
            "age": 28,
            "gender": "Female"
        }
    ]
}
```

**Response:**
```json
{
    "bookingID": 1,
    "bookingReference": "TH20250915123456",
    "status": "Confirmed",
    "totalAmount": 5000.00,
    "message": "Booking created successfully",
    "success": true
}
```

#### Create Flight Booking
```http
POST /api/booking/flight
Authorization: Bearer <token>
Content-Type: application/json

{
    "userID": 1,
    "flightScheduleID": 1,
    "passengers": [
        {
            "name": "John Doe",
            "age": 30,
            "gender": "Male"
        }
    ]
}
```

#### Get User Bookings
```http
GET /api/booking/user/{userId}?bookingType=Train&status=Confirmed
Authorization: Bearer <token>
```

#### Get Booking Details
```http
GET /api/booking/{bookingId}/user/{userId}
Authorization: Bearer <token>
```

#### Cancel Booking
```http
POST /api/booking/{bookingId}/cancel
Authorization: Bearer <token>
Content-Type: application/json

{
    "userId": 1,
    "reason": "Change of plans"
}
```

### 4. Payment Operations (`/api/payment`)

#### Process Payment
```http
POST /api/payment/process
Authorization: Bearer <token>
Content-Type: application/json

{
    "bookingID": 1,
    "paymentModeID": 1,
    "amount": 5000.00,
    "transactionRef": "TXN123456789"
}
```

**Response:**
```json
{
    "paymentID": 1,
    "status": "Success",
    "transactionReference": "TXN123456789",
    "amount": 5000.00,
    "message": "Payment processed successfully",
    "success": true
}
```

#### Get Payment History
```http
GET /api/payment/history?userId=1&status=Success
Authorization: Bearer <token>
```

#### Process Refund (Admin Only)
```http
POST /api/payment/refund
Authorization: Bearer <admin-token>
Content-Type: application/json

{
    "bookingId": 1,
    "refundAmount": 4500.00,
    "cancellationFee": 500.00,
    "reason": "User cancellation",
    "processedBy": "Admin"
}
```

## Error Handling

### HTTP Status Codes
- `200 OK` - Request successful
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Access denied
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

### Error Response Format
```json
{
    "message": "Error description",
    "success": false,
    "errors": {
        "field": ["Validation error message"]
    }
}
```

### Business Logic Error Codes
| Code | Description |
|------|-------------|
| 50001 | Email already registered |
| 50002 | Phone number already registered |
| 50003 | Invalid age range |
| 50004 | Invalid email or password |
| 50005 | Email not verified |
| 50010 | Travel date in past |
| 50011 | Invalid passenger count |
| 50016 | Invalid or unverified user |
| 50020 | Tatkal booking time restriction |
| 50022 | Insufficient seats available |

## Rate Limiting
- Search endpoints: 100 requests per minute
- Booking endpoints: 10 requests per minute
- Payment endpoints: 5 requests per minute

## Data Models

### Passenger Object
```json
{
    "name": "string (required, max 100 chars)",
    "age": "integer (required, 0-120)",
    "gender": "string (required, Male/Female/Other)"
}
```

### Booking Status Values
- `Confirmed` - Booking confirmed with seat allocation
- `Waitlisted` - Booking in waitlist queue
- `Cancelled` - Booking cancelled
- `RAC` - Reservation Against Cancellation

### Payment Status Values
- `Pending` - Payment initiated but not completed
- `Success` - Payment completed successfully
- `Failed` - Payment failed
- `Refunded` - Payment refunded

## SDK Examples

### JavaScript/TypeScript
```typescript
// User registration
const registerUser = async (userData) => {
    const response = await fetch('/api/user/register', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(userData)
    });
    return response.json();
};

// Search trains
const searchTrains = async (searchData) => {
    const response = await fetch('/api/search/trains', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(searchData)
    });
    return response.json();
};

// Create booking
const createBooking = async (bookingData, token) => {
    const response = await fetch('/api/booking/train', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(bookingData)
    });
    return response.json();
};
```

### C# Client
```csharp
public class TransitHubClient
{
    private readonly HttpClient _httpClient;
    
    public TransitHubClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<UserRegistrationResponseDto> RegisterUserAsync(UserRegistrationDto userData)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/user/register", userData);
        return await response.Content.ReadFromJsonAsync<UserRegistrationResponseDto>();
    }
    
    public async Task<IEnumerable<TrainSearchResultDto>> SearchTrainsAsync(TrainSearchDto searchData)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/search/trains", searchData);
        return await response.Content.ReadFromJsonAsync<IEnumerable<TrainSearchResultDto>>();
    }
}
```

## Testing

### Unit Tests
Run unit tests for services and repositories:
```bash
dotnet test
```

### Integration Tests
Test API endpoints:
```bash
dotnet test --filter Category=Integration
```

### Load Testing
Use tools like Apache Bench or k6 for load testing:
```bash
ab -n 1000 -c 10 http://localhost:5000/api/search/trains
```

## Deployment

### Environment Variables
```bash
ConnectionStrings__DefaultConnection=Server=...;Database=TransitHubDB;...
JwtSettings__SecretKey=your-secret-key
EmailSettings__SmtpServer=smtp.gmail.com
```

### Docker Deployment
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY . .
EXPOSE 80
ENTRYPOINT ["dotnet", "TransitHub.dll"]
```

## Monitoring and Logging

### Application Insights
Configure Application Insights for monitoring:
```json
{
    "ApplicationInsights": {
        "InstrumentationKey": "your-key"
    }
}
```

### Structured Logging
All operations are logged with structured data for easy querying and monitoring.

## Support

For API support and questions:
- Email: api-support@transithub.com
- Documentation: https://docs.transithub.com
- Status Page: https://status.transithub.com