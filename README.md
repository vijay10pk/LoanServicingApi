# Loan Servicing API

An ASP.NET Core API for loan servicing operations. This API manages loans, payments, users, and generates reports. It includes authentication and regulatory compliance features.

## Features

- User authentication and authorization
- Loan management (create, modify, query)
- Payment processing and tracking
- Loan officer performance monitoring
- Report generation (internal, regulatory, performance)
- Regulatory compliance tracking

## Technologies

- ASP.NET Core
- Entity Framework Core
- MySQL Database
- JWT Authentication

## Getting Started

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run database migrations: `dotnet ef database update`
4. Start the application: `dotnet run`

## API Endpoints

- `/api/Authentication`: User login and registration
- `/api/Loan`: Loan management
- `/api/Payment`: Payment processing
- `/api/LoanOfficerManagement`: Loan officer operations
- `/api/Report`: Report generation and retrieval
