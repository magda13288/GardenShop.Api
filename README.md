File: README.md
````````markdown
# GardenShop.Api

A RESTful API for managing a garden shop e-commerce platform, built with ASP.NET Core and following Clean Architecture principles.

## 🌿 Overview

GardenShop.Api is a backend service that provides endpoints for managing products, categories, and orders for a garden shop. The application is built using modern .NET practices with a focus on maintainability, testability, and scalability.

## 🏗️ Architecture

The solution follows Clean Architecture principles with clear separation of concerns:
        
- **Domain Layer**: Core business logic and business rules. Contains entities, value objects, and services.
- **Application Layer**: Use cases and orchestration of business logic. Contains controllers and services for handling HTTP requests and responses.
- **Infrastructure Layer**: Integration with external systems and data storage. Contains repositories, services, and configurations.

## 🌱 Features

- **Inventory Management**: Track products in inventory and manage restocking.
- **Order Processing**: Process orders efficiently and track their status.
- **Category Management**: Organize products into categories and subcategories.
- **Dashboard**: Monitor performance and key metrics.

## 🚧 Technologies

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core (Database access)
- xUnit (Testing framework)
- Moq (Mocking library)
- FluentAssertions (Assertion library)

## 📁 Project Structure

### Core Entities

- **Categories**: Product categorization
- **Products**: Garden shop items
- **Orders**: Customer order management

### Key Components

- **DTOs**: Data Transfer Objects for API communication
- **Services**: Business logic implementation
- **Controllers**: RESTful API endpoints
- **Migrations**: Database schema management
- **Seed Data**: Initial database population

## ⚙️ Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server (or your preferred database)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/username/GardenShop.Api
   ```
2. **Restore the NuGet packages**
   ```bash
   dotnet restore
   ```
3. **Apply database migrations**
   ```bash
   dotnet migrate
   ```
4. **Run the application**
   ```bash
   dotnet run
   ```

## 🚨 Important Notes

- The API is currently in development and may not be fully functional.
- The database schema is subject to changes based on requirements.

## 📞 Support

For support or issues, please contact the developers directly.

---

**License**: MIT
**Authors**: Magdalena Pietryszak, [GitHub: @magda13288](https://github.com/magda13288)
**Project**:
**Last Updated**:
**Version**:

---

The API will be available at `https://localhost:5001` (or the port specified in `launchSettings.json`)

## 🧪 Running Tests

Execute all unit tests:
```bash
dotnet test
```

Run tests with detailed output:
```bash
dotnet test --verbose
```

## 📚 API Endpoints

### Categories
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category by ID
- `POST /api/categories` - Create new category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

### Products
- Endpoints for product management (similar CRUD operations)

### Orders
- Endpoints for order management

## 🔧 Configuration

Configuration files:
- `appsettings.json` - Production settings
- `appsettings.Development.json` - Development-specific settings
- `launchSettings.json` - Local development profiles

## 🧩 Dependency Injection

Services are registered in:
- `GardenShop.Api.Application/DependencyInjection.cs` - Application services
- `GardenShop.Api.Infrastructure/DependencyInjection.cs` - Infrastructure services
- `GardenShop.Api/Program.cs` - Service registration

## 📝 License

MIT License

## 📧 Contact

GitHub: [@magda13288](https://github.com/magda13288)

Repository: [GardenShop.Api](https://github.com/magda13288/GardenShop.Api)
