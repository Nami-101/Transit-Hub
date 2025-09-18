# Transit-Hub 🚌✈️

A comprehensive transit booking system built with .NET 8 and Angular 20, featuring JWT authentication and modern full-stack architecture.

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js (18+)
- SQL Server (LocalDB or full version)
- Angular CLI: `npm install -g @angular/cli`

### Clone and Setup
```bash
# Clone the repository
git clone https://github.com/YOUR_USERNAME/Transit-Hub.git
cd Transit-Hub

# Backend setup
cd backend
dotnet restore
dotnet ef database update
dotnet run

# Frontend setup (new terminal)
cd frontend/transit-hub-frontend
npm install
ng serve
```

### Default URLs
- **Backend API**: http://localhost:5000
- **Frontend**: http://localhost:4200
- **Swagger**: http://localhost:5000/swagger

## 🔐 Authentication

The system includes complete JWT authentication:
- **Register**: Create new user accounts
- **Login**: JWT token-based authentication  
- **Role-based access**: Admin, User, Moderator roles
- **Auto-refresh**: Seamless token renewal

### Test Accounts
After running the backend, you can register new accounts or use seeded data.

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

## 📁 Project Structure

```
Transit-Hub/
├── backend/
│   ├── Controllers/         # API endpoints
│   ├── Services/           # Business logic
│   ├── Models/DTOs/        # Data transfer objects
│   ├── Data/              # Database context
│   └── Program.cs         # App configuration
├── frontend/transit-hub-frontend/
│   ├── src/app/
│   │   ├── features/      # Feature modules
│   │   ├── services/      # Angular services
│   │   ├── guards/        # Route guards
│   │   └── interceptors/  # HTTP interceptors
│   └── src/styles.css     # Global styles
└── README.md
```

## 🛠️ Development

### Running Both Services
```bash
# Terminal 1 - Backend
cd backend
dotnet run

# Terminal 2 - Frontend  
cd frontend/transit-hub-frontend
ng serve
```

### Database
The backend automatically:
- Creates the database if it doesn't exist
- Runs migrations
- Seeds initial data and roles
- Creates stored procedures

### API Testing
Visit http://localhost:5000/swagger for interactive API documentation.

## 🚀 Features

- ✅ **User Authentication** (Register, Login, JWT)
- ✅ **Role Management** (Admin, User, Moderator)
- ✅ **Responsive UI** (Mobile-friendly design)
- ✅ **API Documentation** (Swagger/OpenAPI)
- ✅ **Database Seeding** (Test data included)
- 🔄 **Train/Flight Search** (In development)
- 🔄 **Booking System** (In development)
- 🔄 **Payment Integration** (Planned)

## 🤝 Contributing

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Make changes and commit: `git commit -m "Add your feature"`
3. Push to branch: `git push origin feature/your-feature`
4. Create Pull Request

## 📞 Support

For questions or issues, please create an issue in the repository.

---
*Built with ❤️ using .NET 8 and Angular 20*
