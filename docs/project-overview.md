# Transit-Hub - Project Overview

## 🎯 Project Description
Transit-Hub is a mini IRCTC-style booking system built using ASP.NET Core Web API, Angular, and SQL Server. It provides train and flight booking functionalities, with a focus on real-world business rules like Tatkal quota, waitlist, and priority handling.

## 🔹 Core Features

### User Management
- Registration & Login with Gmail confirmation
- Role-based access (User/Admin)

### Booking System
- Train booking (Normal + Tatkal)
- Flight booking (Normal)
- Passenger details with senior citizen preference
- Waitlist handling + auto seat reallocation on cancellation

### Payments
- Mock payment flow (Card/UPI/NetBanking)
- Transaction status tracking

### Admin Module
- Manage trains, stations, flights, airports, schedules, quotas
- View reports (bookings, cancellations, revenues)

### Extras
- AI Chat assistant for FAQs
- Notifications (booking confirmations, cancellations, waitlist upgrades)
- System Logs & Booking Audit for error tracking and booking history

## 🔹 Tech Stack
- **Backend**: ASP.NET Core Web API with Swagger, Entity Framework Core, Identity
- **Frontend**: Angular (TypeScript, Bootstrap/Material)
- **Database**: SQL Server (20-table schema with audit fields)
- **Collaboration**: GitHub (branches, pull requests, daily commits)

## 🔹 Development Plan

### Phase 1
Focus on Train Booking module (Identity, Master tables, CRUD APIs, Angular CRUD screens)

### Phase 2
Extend to Flights, Payments, Tatkal logic, Waitlist allocation

### Phase 3
Add extras (AI Chat, Notifications, Gmail confirmation)

## ✅ Project Goals
Transit-Hub demonstrates end-to-end system design: database modeling, API development, UI integration, and real-world booking logic.