# Transit-Hub 🚌✈️

A comprehensive transit booking system built with .NET 8 and Angular 20, featuring JWT authentication and modern full-stack architecture.

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js (18+)
- SQL Server (LocalDB or full version)
- Angular CLI: `npm install -g @angular/cli`


### Default URLs
- **Backend API**: http://localhost:5000
- **Frontend**: http://localhost:4200
- **Swagger**: http://localhost:5000/swagger

## 🔐 Authentication

The system includes complete JWT authentication:
- **Register**: Create new user accounts
- **Login**: JWT token-based authentication  
- **Role-based access**: Admin, User
- **Auto-refresh**: Seamless token renewal


## 🏗️ Architecture

### Backend (.NET 8)
- **Repository Pattern** with Unit of Work
- **JWT Authentication** with role-based authorization
- **Entity Framework Core** with SQL Server
- **Swagger Documentation** for API testing
- **Comprehensive DTOs** with validation

### Frontend (Angular 20)
- **Standalone Components** with reactive forms
- **Material UI** design system
- **Tailwind CSS** for styling
- **HTTP Interceptors** for JWT handling
- **Route Guards** for authentication
- **Feature-based** modular structure



### Database
The backend automatically:
- Creates the database if it doesn't exist
- Runs migrations
- Seeds initial data and roles
- Creates stored procedures

### API Testing
Visit http://localhost:5000/swagger for interactive API documentation.




*Built with ❤️ using .NET 8 and Angular 20*
