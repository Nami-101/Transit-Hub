# Transit Hub - Train Booking System API

A comprehensive train and flight booking system built with ASP.NET Core 9.0, featuring modern architecture and enterprise-level functionality similar to IRCTC.

## 🚀 Project Overview

This project demonstrates advanced .NET development skills through the implementation of a complete booking system with authentication, search capabilities, and comprehensive data management.

## ✨ Key Features

### Core Functionality
- **Secure Authentication System** - JWT-based authentication with role management
- **Advanced Search Engine** - Multi-criteria search for trains and flights
- **Complete Booking Workflow** - End-to-end booking management system
- **Master Data Management** - Comprehensive station and route management
- **Payment Integration Ready** - Structured architecture for payment processing

### Technical Excellence
- **Clean Architecture** - Proper separation of concerns and SOLID principles
- **Entity Framework Core** - Code-first approach with complex relationships
- **Comprehensive API Documentation** - Interactive Swagger documentation
- **Role-Based Security** - Admin and user authorization levels
- **Audit Trail System** - Complete tracking of data changes

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: Entity Framework Core with SQL Server
- **Authentication**: JWT with ASP.NET Core Identity
- **Documentation**: Swagger/OpenAPI 3.0
- **Architecture**: Clean Architecture with Service Layer Pattern

## 📋 Prerequisites

- .NET 9.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

## 🚀 Quick Start

### 1. Setup Project
```bash
git clone <repository-url>
cd transit-hub/backend
```

### 2. Configure Database
Update connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TrainBookingDb_Dev;Trusted_Connection=true"
  }
}
```

### 3. Initialize Database
```bash
dotnet ef database update
```

### 4. Run Application
```bash
dotnet run
```

### 5. Access Documentation
Open browser: `http://localhost:5000/swagger`

## 📁 Project Architecture

```
backend/
├── Controllers/              # API Endpoints
│   ├── AuthController.cs        # Authentication & Authorization
│   ├── BookingController.cs     # Booking Management
│   ├── SearchController.cs      # Search Functionality
│   └── MasterDataController.cs  # Master Data Operations
├── Services/                 # Business Logic Layer
│   ├── Interfaces/             # Service Contracts
│   ├── AuthService.cs          # Authentication Logic
│   ├── BookingService.cs       # Booking Operations
│   ├── SearchService.cs        # Search Algorithms
│   └── JwtService.cs           # Token Management
├── Models/                   # Data Models
│   ├── DTOs/                   # Data Transfer Objects
│   ├── Entities/               # Database Entities
│   └── BaseEntity.cs           # Base Entity with Audit Fields
├── Data/                     # Data Access Layer
│   ├── ApplicationDbContext.cs # Entity Framework Context
│   └── DataSeeder.cs           # Database Initialization
└── Extensions/               # Configuration Extensions
    └── ServiceExtensions.cs    # Dependency Injection Setup
```

## 🔌 API Endpoints

### Authentication & User Management
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | User registration | No |
| POST | `/api/auth/login` | User authentication | No |
| GET | `/api/auth/me` | Get current user | Yes |

### Search & Discovery
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/search/trains` | Search available trains | No |
| GET | `/api/search/flights` | Search available flights | No |
| GET | `/api/search/stations` | Get all stations | No |
| GET | `/api/search/lookup` | Get lookup data | No |

### Booking Management
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/booking/train` | Create train booking | Yes |
| POST | `/api/booking/flight` | Create flight booking | Yes |
| GET | `/api/booking/user/{userId}` | Get user bookings | Yes |
| GET | `/api/booking/{bookingId}/user/{userId}` | Get booking details | Yes |
| POST | `/api/booking/{bookingId}/cancel` | Cancel booking | Yes |

### Master Data Management
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/masterdata/stations` | Get all stations | No |
| GET | `/api/masterdata/trainclasses` | Get train classes | No |
| POST | `/api/masterdata/seed` | Initialize master data | Admin |
| GET | `/api/masterdata/health` | System health check | No |

## 🔐 Authentication

The API uses JWT (JSON Web Tokens) for secure authentication. Include the token in requests:

```http
Authorization: Bearer <your-jwt-token>
```

### Default Admin Account
- **Email**: `admin@trainbooking.com`
- **Password**: `Admin@123`

## 🗄️ Database Design

### Core Entities
- **Users & Roles** - Identity management with ASP.NET Core Identity
- **Stations** - Train and flight stations with unique codes
- **Trains** - Train master data with route information
- **Flights** - Flight master data with airline details
- **TrainSchedules** - Scheduled train services with pricing
- **Bookings** - Comprehensive booking records
- **Passengers** - Detailed passenger information
- **Payments** - Payment transaction tracking

### Key Features
- **Audit Trail** - All entities track creation and modification
- **Soft Delete** - Data preservation with IsActive flags
- **Referential Integrity** - Proper foreign key relationships
- **Performance Optimization** - Strategic indexing and constraints

## 🚀 Development Highlights

### Technical Achievements
- **Complex Integration** - Successfully merged multiple service architectures
- **Database Optimization** - Resolved cascade delete conflicts and relationship issues
- **API Documentation** - Comprehensive Swagger integration with JWT support
- **Error Handling** - Consistent error responses across all endpoints
- **Code Quality** - Clean code principles and SOLID design patterns

### Problem Solving
- **Entity Framework Challenges** - Resolved complex relationship mappings
- **Authentication Integration** - Seamless JWT implementation with Identity
- **Swagger Configuration** - Fixed DateOnly type support and documentation generation
- **Service Architecture** - Implemented proper dependency injection patterns

## ⚙️ Configuration

### JWT Settings
```json
{
  "JwtSettings": {
    "SecretKey": "your-256-bit-secret-key-here",
    "Issuer": "TrainBookingAPI",
    "Audience": "TrainBookingUsers",
    "ExpiryInMinutes": 60
  }
}
```

### Database Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TrainBookingDb_Dev;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

## 📊 Development Status

| Feature | Status | Description |
|---------|--------|-------------|
| Authentication | ✅ Complete | JWT-based auth with roles |
| Database Design | ✅ Complete | Full schema with relationships |
| Master Data APIs | ✅ Complete | Station and class management |
| Search System | ✅ Complete | Multi-criteria search engine |
| Booking System | ✅ Complete | End-to-end booking workflow |
| API Documentation | ✅ Complete | Interactive Swagger UI |
| Error Handling | ✅ Complete | Consistent error responses |
| Data Seeding | ✅ Complete | Automated data initialization |

## 🎯 Future Enhancements

- Payment gateway integration
- Real-time seat availability
- Email notification system
- Advanced reporting features
- Mobile API optimizations

## 🤝 Development Notes

This project showcases:
- **Full-Stack API Development** with modern .NET practices
- **Complex Database Design** with proper normalization
- **Security Implementation** with JWT and role-based access
- **Service-Oriented Architecture** with clean separation
- **Professional Documentation** and API design
- **Integration Skills** combining multiple system components

---

**Developed with modern ASP.NET Core practices and enterprise architecture patterns**