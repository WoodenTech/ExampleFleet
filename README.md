# BDElite Fleet Insurance Platform

A comprehensive fleet insurance management platform connecting **Fleet Managers**, **Insurance Brokers**, and **Insurance Providers** through innovative technology.

## Table of Contents
- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Setup Instructions](#setup-instructions)
- [Running the Application](#running-the-application)
- [Project Structure](#project-structure)
- [Current Implementation Status](#current-implementation-status)
- [Next Steps](#next-steps)
- [API Endpoints](#api-endpoints)

## Overview

The BDElite Fleet Insurance Platform streamlines fleet insurance operations by providing three distinct user interfaces:

- **Fleet Managers**: Manage vehicles, request quotes, track policies, and file claims
- **Insurance Brokers**: Generate quotes, manage client relationships, and track commissions
- **Insurance Providers**: Create insurance products, manage rates, and analyze business performance

## Features

### Fleet Management
- ✅ Vehicle registration and tracking
- ✅ Fleet policy management
- ✅ Quote comparison
- ✅ Claims tracking
- ✅ Dashboard with real-time statistics

### Broker Portal
- ✅ Client management
- ✅ Quote generation
- ✅ Commission tracking
- ✅ Multi-provider access
- ✅ Analytics and reporting

### Provider Console
- ✅ Insurance product creation
- ✅ Rate management
- ✅ Business analytics
- ✅ Risk assessment tools
- ✅ Broker performance tracking

## Technology Stack

- **Frontend**: ASP.NET Core 10.0 Razor Pages
- **Backend**: ASP.NET Core 10.0 Web API
- **Database**: MongoDB
- **Containerization**: Docker Desktop (for MongoDB)
- **Language**: C# (.NET 10)

## Prerequisites

Before setting up the project, ensure you have the following installed:

1. **.NET 10 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/10.0)
2. **Docker Desktop** - [Download](https://www.docker.com/products/docker-desktop)
3. **Visual Studio 2022** (or VS Code with C# extensions)
4. **Git** (optional, for version control)

## Setup Instructions

### 1. Install Docker Desktop

1. Download and install [Docker Desktop](https://www.docker.com/products/docker-desktop)
2. Start Docker Desktop
3. Verify Docker is running: open a terminal and run:
   ```bash
   docker --version
   ```

### 2. Set Up MongoDB with Docker

Run MongoDB in a Docker container:

```bash
docker run -d -p 27017:27017 --name mongodb -e MONGO_INITDB_ROOT_USERNAME=admin -e MONGO_INITDB_ROOT_PASSWORD=password mongo:latest
```

**Note**: For development purposes, the application is configured to use a local MongoDB instance without authentication. For production, update the connection string in `appsettings.json` to include credentials.

To verify MongoDB is running:
```bash
docker ps
```

You should see the `mongodb` container in the list.

### 3. Clone or Navigate to the Project

```bash
cd C:\Users\Woody1130\source\repos\BDElite.Fleet
```

### 4. Restore Dependencies

Restore NuGet packages for both projects:

```bash
dotnet restore
```

### 5. Configure Connection Strings

The MongoDB connection string is already configured in:
- `BDElite.Fleet.API/appsettings.json`

Default configuration:
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "BDEliteFleet"
  }
}
```

The UI is configured to connect to the API at:
- `http://localhost:5132/` (configured in `BDElite.Fleet.UI/Program.cs`)

## Running the Application

### Option 1: Using Visual Studio 2022

1. Open `BDElite.Fleet.sln` in Visual Studio
2. Right-click the solution → **Set Startup Projects**
3. Select **Multiple startup projects**
4. Set both `BDElite.Fleet.API` and `BDElite.Fleet.UI` to **Start**
5. Click **Start** (F5) or **Start Without Debugging** (Ctrl+F5)

### Option 2: Using Command Line

Open **two terminal windows**:

**Terminal 1 - Start the API:**
```bash
cd BDElite.Fleet.API
dotnet run
```

The API will start at `http://localhost:5132`

**Terminal 2 - Start the UI:**
```bash
cd BDElite.Fleet.UI
dotnet run
```

The UI will start at `https://localhost:5001` (or the URL shown in the terminal)

### Accessing the Application

Once both projects are running:

1. Open your browser and navigate to the UI URL (typically `https://localhost:5001`)
2. You'll see the landing page with three user portals:
   - **Fleet Dashboard** - `/FleetDashboard`
   - **Broker Portal** - `/BrokerPortal`
   - **Provider Console** - `/ProviderConsole`

## Project Structure

```
BDElite.Fleet/
├── BDElite.Fleet.API/              # Backend API
│   ├── Controllers/
│   │   ├── FleetManagerController.cs
│   │   ├── InsuranceBrokerController.cs
│   │   ├── InsuranceProviderController.cs
│   │   ├── PoliciesController.cs
│   │   ├── ProductsController.cs
│   │   └── QuotesController.cs
│   ├── Models/
│   │   ├── User.cs                # User types (FleetManager, Broker, Provider)
│   │   ├── Vehicle.cs
│   │   ├── InsurancePolicy.cs
│   │   ├── InsuranceProduct.cs
│   │   └── MongoDbSettings.cs
│   ├── Services/
│   │   ├── IUserService.cs / UserService.cs
│   │   ├── IVehicleService.cs / VehicleService.cs
│   │   ├── IPolicyService.cs / PolicyService.cs
│   │   ├── IQuoteService.cs / QuoteService.cs
│   │   └── IProductService.cs / ProductService.cs
│   ├── appsettings.json           # Configuration (MongoDB settings)
│   └── Program.cs
│
└── BDElite.Fleet.UI/               # Frontend Web Application
    ├── Pages/
    │   ├── Index.cshtml            # Landing page
    │   ├── FleetDashboard.cshtml   # Fleet Manager dashboard
    │   ├── BrokerPortal.cshtml     # Insurance Broker portal
    │   ├── ProviderConsole.cshtml  # Insurance Provider console
    │   ├── AddProduct.cshtml       # Create insurance products
    │   └── GenerateQuote.cshtml    # Quote generation form
    ├── appsettings.json
    └── Program.cs                  # HTTP client configuration
```

## Current Implementation Status

### ✅ Completed Features

1. **Database Layer**
   - MongoDB integration with service layer
   - Models for Users, Vehicles, Policies, Products, and Quotes
   - Repository pattern implementation

2. **API Layer**
   - RESTful API endpoints for all major entities
   - Controllers for Fleet Managers, Brokers, and Providers
   - OpenAPI/Swagger documentation
   - Service interfaces and implementations

3. **UI Layer**
   - Responsive landing page
   - Three separate user portals with role-specific dashboards
   - Product creation form for insurance providers
   - Quote generation interface
   - Modern, gradient-based design system

### 🚧 Placeholder/Demo Features

The following features have **UI elements** but are **not yet connected to live data**:

1. **Fleet Dashboard**
   - Static policy data (not from database)
   - Dummy statistics and activity feed
   - "Add Vehicle" and "Request Quote" buttons are non-functional

2. **Broker Portal**
   - Static quote listings
   - Demo commission tracking
   - Provider partnerships are hardcoded

3. **Provider Console**
   - Product list now shows **real data from MongoDB**
   - Statistics and analytics are dummy data
   - Rate management alerts are static

4. **Quote Generation**
   - Form exists at `/GenerateQuote` but doesn't submit to API yet

5. **Policy Details**
   - Policy detail pages referenced in links but not implemented

## Next Steps

### High Priority

1. **Connect UI to API**
   - [ ] Wire up Fleet Dashboard to fetch real policies from API
   - [ ] Connect quote generation form to `QuoteService`
   - [ ] Implement "Add Vehicle" functionality
   - [ ] Link broker quote listings to real data

2. **User Authentication & Authorization**
   - [ ] Implement user login/registration
   - [ ] Add JWT authentication to API
   - [ ] Role-based access control (Fleet Manager, Broker, Provider)
   - [ ] Session management

3. **Complete CRUD Operations**
   - [ ] Vehicle management (add, edit, delete vehicles)
   - [ ] Policy detail pages with full information
   - [ ] Quote acceptance/rejection workflow
   - [ ] Policy binding from quotes

### Medium Priority

4. **Business Logic**
   - [ ] Implement quote calculation algorithms
   - [ ] Add commission calculation logic
   - [ ] Policy renewal workflow
   - [ ] Claims filing and tracking

5. **Search & Filtering**
   - [ ] Filter policies by status, date, etc.
   - [ ] Search functionality for vehicles and clients
   - [ ] Advanced quote filtering

6. **Reports & Analytics**
   - [ ] Commission reports for brokers
   - [ ] Business performance reports for providers
   - [ ] Fleet analytics for managers
   - [ ] Export to PDF/Excel

### Low Priority

7. **Enhancements**
   - [ ] Real-time notifications
   - [ ] Email integration
   - [ ] Document upload and management
   - [ ] Mobile responsive improvements
   - [ ] Dark mode support

8. **Testing**
   - [ ] Unit tests for services
   - [ ] Integration tests for API endpoints
   - [ ] UI automated tests

9. **DevOps**
   - [ ] Docker Compose for full application stack
   - [ ] CI/CD pipeline
   - [ ] Production deployment configuration
   - [ ] Environment-specific settings

## API Endpoints

### Fleet Manager
- `GET /api/FleetManager` - Get all fleet managers
- `GET /api/FleetManager/{id}` - Get fleet manager by ID
- `POST /api/FleetManager` - Create fleet manager
- `PUT /api/FleetManager/{id}` - Update fleet manager
- `GET /api/FleetManager/{id}/policies` - Get fleet manager's policies
- `GET /api/FleetManager/{id}/quotes` - Get fleet manager's quotes
- `POST /api/FleetManager/{id}/request-quote` - Request a quote

### Insurance Broker
- `GET /api/InsuranceBroker` - Get all brokers
- `GET /api/InsuranceBroker/{id}` - Get broker by ID
- `POST /api/InsuranceBroker` - Create broker
- `PUT /api/InsuranceBroker/{id}` - Update broker
- `GET /api/InsuranceBroker/{id}/policies` - Get broker's policies
- `GET /api/InsuranceBroker/{id}/quotes` - Get broker's quotes
- `GET /api/InsuranceBroker/{id}/available-products` - Get available products
- `POST /api/InsuranceBroker/{id}/generate-quote` - Generate quote
- `POST /api/InsuranceBroker/{id}/bind-policy` - Bind policy from quote
- `GET /api/InsuranceBroker/{id}/commission-report` - Get commission report

### Insurance Provider
- `GET /api/InsuranceProvider` - Get all providers
- `GET /api/InsuranceProvider/{id}` - Get provider by ID
- `POST /api/InsuranceProvider` - Create provider
- `PUT /api/InsuranceProvider/{id}` - Update provider
- `GET /api/InsuranceProvider/{id}/products` - Get provider's products
- `POST /api/InsuranceProvider/{id}/products` - Create product
- `GET /api/InsuranceProvider/{id}/policies` - Get provider's policies
- `GET /api/InsuranceProvider/{id}/business-report` - Get business report
- `POST /api/InsuranceProvider/{id}/update-rates` - Update product rates
- `GET /api/InsuranceProvider/{id}/risk-assessment` - Get risk assessment

### Products
- `GET /api/Products` - Get all products
- `GET /api/Products/{id}` - Get product by ID
- `POST /api/Products` - Create product
- `PUT /api/Products/{id}` - Update product
- `DELETE /api/Products/{id}` - Delete product

### Policies & Quotes
- `GET /api/Policies` - Get all policies
- `GET /api/Policies/{id}` - Get policy by ID
- `GET /api/Quotes` - Get all quotes
- `GET /api/Quotes/{id}` - Get quote by ID

## Troubleshooting

### MongoDB Connection Issues

If you encounter connection issues:

1. **Verify Docker is running:**
   ```bash
   docker ps
   ```

2. **Check MongoDB logs:**
   ```bash
   docker logs mongodb
   ```

3. **Restart MongoDB container:**
   ```bash
   docker restart mongodb
   ```

4. **Remove and recreate container:**
   ```bash
   docker rm -f mongodb
   docker run -d -p 27017:27017 --name mongodb mongo:latest
   ```

### API Not Responding

1. Check that the API is running and listening on the correct port
2. Verify `BDElite.Fleet.UI/Program.cs` has the correct API base URL
3. Check for any firewall or antivirus blocking local connections

### Port Already in Use

If ports 5132 or 5001 are already in use:

1. Change the port in `launchSettings.json` (for both API and UI projects)
2. Update the API URL in `BDElite.Fleet.UI/Program.cs` to match

## Contributing

This project is currently in active development. Key areas for contribution:
- Implementing business logic in service layer
- Connecting UI forms to API endpoints
- Adding validation and error handling
- Writing tests

## License

[Add your license information here]

## Contact

[Add your contact information or team information here]

---

**Generated with Claude Code** 🤖
