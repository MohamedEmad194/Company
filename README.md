# Company Demo MVC Application

## Overview
A sample ASP.NET Core MVC application for managing Departments and Employees, featuring:
- CRUD operations for Departments and Employees
- Authentication and role-based authorization (Admin/User)
- Responsive Bootstrap UI
- Server-side paging, sorting, and filtering
- Business validation and error handling
- In-memory caching for performance

## Setup Instructions

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or change connection string for your DB)
- Node.js (optional, for front-end tooling)

### Getting Started
1. **Clone the repository:**
   ```bash
   git clone <your-repo-url>
   cd Company/Company/Company
   ```
2. **Configure the database:**
   - Update `appsettings.json` with your SQL Server connection string.
   - Run EF Core migrations (if needed):
     ```bash
     dotnet ef database update --project Company.Demo.DAL
     ```
3. **Build and run the app:**
   ```bash
   dotnet build
   dotnet run --project Company.Demo.PL
   ```
4. **Access the app:**
   - Open your browser at `https://localhost:5001` (or the port shown in the console)

### Seeding Admin User (if not present)
- Register a new user and assign the "Admin" role in the database or via Identity management tools.

## Usage
- **Login:** All pages require authentication.
- **Roles:**
  - **Admin:** Can create, edit, and delete Departments and Employees.
  - **User:** Can view lists and details only.
- **CRUD:** Use the navigation bar to access Departments and Employees. Only Admins see Create/Edit/Delete buttons.
- **Paging/Sorting/Filtering:** Use the controls at the top of each list.
- **Responsive UI:** Works on desktop and mobile.

## Testing
- (Add instructions here if you have automated tests. Example:)
  ```bash
  dotnet test
  ```

## Contribution Guidelines
- Fork the repo and create a feature branch.
- Follow C# and .NET best practices.
- Submit a pull request with a clear description.
- For major changes, open an issue first to discuss.

## License
This project is licensed under the MIT License. See [LICENSE](LICENSE) for details. 