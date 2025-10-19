# Blazor Furniture App

A modern, full-stack web application for furniture management built with Blazor, featuring secure authentication, role-based authorization, and a clean architecture approach.

## 🏗️ Architecture

This application follows **Clean Architecture** principles with a clear separation of concerns:

- **Core Layer**
  - `Domain`: Business entities and enums
  - `Application`: Business logic and use cases
  - `Shared`: Common shared types and configurations
  - `Templates`: Reusable templates

- **Infrastructure Layer**
  - `Infrastructure`: External services and integrations
  - `Persistence`: Data access and database context

- **Presentation Layer**
  - `BlazorFurniture`: Main Blazor Server + WebAssembly host application
  - `BlazorFurniture.Client`: Blazor WebAssembly client project
  - `BlazorFurniture.Shared`: Shared UI components and resources

- **Orchestration**
  - `.NET Aspire`: Application orchestration with AppHost and ServiceDefaults

## 🚀 Tech Stack

- **.NET 10.0** - Latest .NET framework
- **Blazor** - Hybrid Server + WebAssembly rendering modes
- **MudBlazor** - Material Design component library
- **FastEndpoints** - High-performance API endpoints
- **Keycloak** - Authentication and authorization (OIDC/OAuth2)
- **PostgreSQL** - Database
- **.NET Aspire** - Cloud-native orchestration
- **Serilog** - Structured logging
- **Swagger/Scalar** - API documentation

## 🔐 Authentication & Authorization

This application uses **Keycloak** as the identity provider, proxying authentication and authorization functionalities:

- **OpenID Connect (OIDC)** authentication flow
- **OAuth2 Authorization Code Flow** with PKCE
- **Role-based access control** with hierarchical roles:
  - **Platform Roles**: Platform Admin, Admin, User
  - **Group Roles**: Group Admin, Group Member
- **Group-based permissions** for multi-tenant scenarios
- **JWT Bearer token** authentication for API endpoints

### Authorization Architecture

The application implements a sophisticated authorization model using Keycloak Authorization Services:

- **Controller-scoped resources** (e.g., `group-management`, `user-management`)
- **Operation-based scopes** (view, list, create, update, delete)
- **Dynamic group-based policies** via custom Java Policy SPI
- **Policy Decision Point (PDP)** integration for runtime authorization decisions

For more details, see the [authorization planning documentation](BlazorFurniture/docs/plans/authorizationPlanning.md).

## 📋 Features

- ✅ Secure authentication with Keycloak
- ✅ Role-based and group-based authorization
- ✅ Interactive UI with Blazor Server and WebAssembly
- ✅ RESTful API with FastEndpoints
- ✅ API documentation with Swagger/Scalar
- ✅ Clean Architecture design
- ✅ Comprehensive logging with Serilog
- ✅ Unit and integration tests
- ✅ Container support with Docker
- ✅ Cloud-native orchestration with .NET Aspire

## 🛠️ Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [Docker](https://www.docker.com/) (for Keycloak and PostgreSQL)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# extension
- [PostgreSQL](https://www.postgresql.org/) (or use Docker)

## 🏃 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/BrumaSebastian/blazor-furniture-app.git
cd blazor-furniture-app
```

### 2. Set up infrastructure

The application uses .NET Aspire to orchestrate services including Keycloak and PostgreSQL.

### 3. Configure application settings

Update the `appsettings.json` or use User Secrets to configure:

- Keycloak connection settings
- Database connection strings
- CORS policies
- Other environment-specific settings

### 4. Run with .NET Aspire

```bash
cd BlazorFurniture/Aspire/BlazorFurniture.AppHost
dotnet run
```

This will start:
- The main Blazor application
- Keycloak identity server
- Maildev (for email testing)
- PostgreSQL database

### 5. Access the application

- **Application**: `https://localhost:7xxx` (check Aspire dashboard for exact port)
- **Aspire Dashboard**: `http://localhost:15xxx` (shown in console output)
- **Keycloak**: `http://localhost:8080`
- **API Documentation**: `https://localhost:7xxx/swagger` or `https://localhost:7xxx/scalar`

## 🧪 Testing

### Run Unit Tests

```bash
cd BlazorFurniture
dotnet test tests/BlazorFurniture.UnitTests
```

### Run Integration Tests

```bash
dotnet test tests/BlazorFurniture.IntegrationTests
```

### Run All Tests

```bash
dotnet test
```

## 📁 Project Structure

```
blazor-furniture-app/
├── BlazorFurniture/
│   ├── Aspire/                          # .NET Aspire orchestration
│   │   ├── BlazorFurniture.AppHost/     # Application host
│   │   └── BlazorFurniture.ServiceDefaults/  # Service defaults
│   ├── src/
│   │   ├── Core/                        # Core business logic
│   │   │   ├── BlazorFurniture.Domain/
│   │   │   ├── BlazorFurniture.Application/
│   │   │   ├── BlazorFurniture.Shared/
│   │   │   └── BlazorFurniture.Templates/
│   │   ├── Infrastructure/              # Data and external services
│   │   │   ├── BlazorFurniture.Infrastructure/
│   │   │   └── BlazorFurniture.Persistence/
│   │   └── Presentation/                # UI and API
│   │       ├── BlazorFurniture/         # Main app (Server + API)
│   │       ├── BlazorFurniture.Client/  # WASM client
│   │       └── BlazorFurniture.Shared/  # Shared UI components
│   ├── tests/
│   │   ├── BlazorFurniture.UnitTests/
│   │   └── BlazorFurniture.IntegrationTests/
│   ├── docs/                            # Documentation
│   └── BlazorFurniture.sln
├── .github/                             # GitHub workflows
└── README.md
```

## 📚 Documentation

Additional documentation is available in the `docs` folder:

- [Authorization Planning](BlazorFurniture/docs/plans/authorizationPlanning.md) - Detailed authorization architecture with Keycloak
- [User Management Planning](BlazorFurniture/docs/plans/userManagement.md) - User management features and flows
- [Role Mapping](BlazorFurniture/docs/plans/roleMapping.md) - Role hierarchy and permissions

## 🔧 Configuration

### Keycloak Setup

The application proxies Keycloak functionalities for authentication and authorization. Key configuration:

1. **Realm**: Configure a realm for the application
2. **Clients**: 
   - API client (confidential) with Authorization Services enabled
   - Public client for Blazor WASM (with PKCE)
3. **Roles**: Platform Admin, Admin, User
4. **Groups**: Hierarchical group structure for multi-tenancy
5. **Authorization Resources**: Controller-scoped resources with operation scopes

### Application Configuration

Key configuration sections in `appsettings.json`:

```json
{
  "OpenIdConnect": {
    "Authority": "http://localhost:8080/realms/your-realm",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=blazor-furniture;Username=postgres;Password=..."
  }
}
```

## 🐳 Docker Support

The application includes Docker support with a Dockerfile in the main project.

```bash
docker build -t blazor-furniture-app .
docker run -p 8080:8080 blazor-furniture-app
```

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is open source and available under the [MIT License](LICENSE).

## 🙏 Acknowledgments

- [MudBlazor](https://mudblazor.com/) - Material Design component library
- [FastEndpoints](https://fast-endpoints.com/) - High-performance endpoint framework
- [Keycloak](https://www.keycloak.org/) - Identity and access management
- [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/) - Cloud-native orchestration
- [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) - Interactive web UI framework

## 📧 Contact

For questions or support, please open an issue in the GitHub repository.

---

**Note**: This is a full-stack application currently designed to proxy Keycloak functionalities for authentication and authorization. Future plans may include additional features and architectural refinements.
