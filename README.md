# AuthFlowPro 🔐

A comprehensive authentication and authorization system built with .NET 9 and Angular 20, featuring role-based access control, multi-tenant organizations, real-time notifications, and subscription management.

## ✨ Features

- **🔐 Authentication & Authorization**
  - JWT-based authentication with refresh tokens
  - Role-based access control (RBAC)
  - Permission-based authorization
  - Secure password policies

- **🏢 Multi-Tenant Organizations**
  - Organization management
  - Member invitations and role assignments
  - Organization-specific permissions

- **📊 Admin Dashboard**
  - User management (CRUD operations)
  - Role and permission management
  - Organization oversight
  - Real-time audit logging

- **🔔 Real-Time Notifications**
  - SignalR integration
  - In-app notifications
  - User activity tracking

- **💳 Subscription Management**
  - Multiple subscription plans
  - Billing and payment tracking
  - Usage monitoring

- **📈 Audit & Compliance**
  - Comprehensive audit logging
  - User activity tracking
  - Security event monitoring

## 🛠️ Tech Stack

### Backend
- **.NET 9** - Web API
- **Entity Framework Core** - ORM
- **PostgreSQL** - Database
- **JWT** - Authentication
- **SignalR** - Real-time communication
- **AutoMapper** - Object mapping

### Frontend
- **Angular 20** - Frontend framework
- **Angular Material** - UI components
- **TypeScript** - Type safety
- **RxJS** - Reactive programming
- **SignalR Client** - Real-time updates

## 🚀 Quick Start

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/)
- [PostgreSQL 13+](https://www.postgresql.org/)
- [Angular CLI](https://angular.io/cli)

### Backend Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/AuthFlowPro.git
   cd AuthFlowPro
   ```

2. **Configure the database**
   - Create a PostgreSQL database named `authflowdb`
   - Update the connection string in `src/AuthFlowPro.API/appsettings.json`

3. **Set up environment variables**
   ```bash
   cp .env.example .env
   # Edit .env with your configuration
   ```

4. **Install dependencies and run migrations**
   ```bash
   cd src/AuthFlowPro.API
   dotnet restore
   dotnet ef database update
   ```

5. **Start the API**
   ```bash
   dotnet run
   ```
   The API will be available at `https://localhost:7084` or `http://localhost:5063`

### Frontend Setup

1. **Navigate to the Angular project**
   ```bash
   cd src/AuthFlowPro.Client/authflow-admin
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Start the development server**
   ```bash
   npm start
   ```
   The application will be available at `http://localhost:4200`

## 🔧 Configuration

### Environment Variables

Create a `.env` file in the root directory with the following variables:

```env
# Database Configuration
DB_HOST=localhost
DB_PORT=5432
DB_NAME=authflowdb
DB_USER=postgres
DB_PASSWORD=your_password

# JWT Configuration
JWT_SECRET_KEY=your-super-secret-jwt-key-here
JWT_ISSUER=AuthFlowPro.API
JWT_AUDIENCE=AuthFlowPro.Client
JWT_EXPIRATION_MINUTES=60

# Admin User Configuration
ADMIN_EMAIL=admin@authflowpro.com
ADMIN_PASSWORD=Admin123$

# CORS Configuration
CORS_ORIGINS=http://localhost:4200,https://yourdomain.com

# SignalR Configuration
SIGNALR_HUB_URL=/notificationHub

# Logging Configuration
LOG_LEVEL=Information
```

### Database Configuration

Update `src/AuthFlowPro.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=authflowdb;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "Key": "your-super-secret-jwt-key-here",
    "Issuer": "AuthFlowPro.API",
    "Audience": "AuthFlowPro.Client",
    "DurationInMinutes": "60"
  },
  "AdminUser": {
    "Email": "admin@authflowpro.com",
    "Password": "Admin123$",
    "Roles": ["Admin"]
  }
}
```

## 👥 Roles & Permissions

### Default Roles

| Role | Description | Permissions |
|------|-------------|-------------|
| **Admin** | Full system access | All permissions |
| **Manager** | Organization management | User.View, Product.View, Product.Create |
| **Basic** | Limited access | Product.View |

### Permission Structure

```csharp
// User Permissions
Permissions.User.View     // View users
Permissions.User.Create   // Create users
Permissions.User.Edit     // Edit users
Permissions.User.Delete   // Delete users

// Role Permissions
Permissions.Role.View     // View roles
Permissions.Role.Create   // Create roles
Permissions.Role.Edit     // Edit roles
Permissions.Role.Delete   // Delete roles

// Product Permissions
Permissions.Product.View     // View products
Permissions.Product.Create   // Create products
Permissions.Product.Edit     // Edit products
Permissions.Product.Delete   // Delete products
```

### Custom Role Example

```csharp
// Creating a custom role with specific permissions
var customRole = new CreateRoleRequest
{
    RoleName = "ContentManager",
    Permissions = new List<string>
    {
        Permissions.Product.View,
        Permissions.Product.Create,
        Permissions.Product.Edit,
        Permissions.User.View
    }
};
```

## 🔐 Demo Credentials

### Admin Account
- **Email:** `admin@authflowpro.com`
- **Password:** `Admin123$`
- **Role:** Admin (Full Access)

### Test Users
You can create additional test users through the admin panel or use the registration endpoint.

## 📱 API Endpoints

### Authentication
```
POST /api/auth/register     # User registration
POST /api/auth/login        # User login
POST /api/auth/refresh-token # Refresh JWT token
```

### User Management
```
GET    /api/user/users           # Get all users
POST   /api/user/create-user     # Create user
PUT    /api/user/edit-user       # Update user
DELETE /api/user/delete-user/{id} # Delete user
POST   /api/user/assign-roles    # Assign roles to user
```

### Role Management
```
GET    /api/role                    # Get all roles
POST   /api/role                    # Create role
PUT    /api/role                    # Update role
DELETE /api/role/{roleName}         # Delete role
GET    /api/role/permissions        # Get all permissions
POST   /api/role/assign-permissions # Assign permissions to role
```

### Organizations
```
GET    /api/organization/my-organizations     # Get user's organizations
POST   /api/organization                      # Create organization
GET    /api/organization/{id}                 # Get organization details
PUT    /api/organization/{id}                 # Update organization
DELETE /api/organization/{id}                 # Delete organization
```

## 🖼️ Screenshots

### Login Page
![Login Page](docs/screenshots/login.png)

### Admin Dashboard
![Admin Dashboard](docs/screenshots/dashboard.png)

### User Management
![User Management](docs/screenshots/users.png)

### Role Management
![Role Management](docs/screenshots/roles.png)

### Organization Management
![Organization Management](docs/screenshots/organizations.png)

## 🧪 Testing

### Backend Tests
```bash
cd src/AuthFlowPro.API
dotnet test
```

### Frontend Tests
```bash
cd src/AuthFlowPro.Client/authflow-admin
npm test
```

## 📦 Deployment

### Docker Deployment

1. **Build and run with Docker Compose**
   ```bash
   docker-compose up -d
   ```

### Manual Deployment

1. **Build the backend**
   ```bash
   cd src/AuthFlowPro.API
   dotnet publish -c Release -o ./publish
   ```

2. **Build the frontend**
   ```bash
   cd src/AuthFlowPro.Client/authflow-admin
   npm run build
   ```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

If you encounter any issues or have questions:

1. Check the [Issues](https://github.com/yourusername/AuthFlowPro/issues) page
2. Create a new issue with detailed information
3. Contact support at support@authflowpro.com

## 🙏 Acknowledgments

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [Angular](https://angular.io/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Angular Material](https://material.angular.io/)
- [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/)

---

**Built with ❤️ by the AuthFlowPro Team**