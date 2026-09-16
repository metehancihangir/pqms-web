# Patient Queue Management System

PQMS is a full-stack application for patient registration, appointment booking, check-in, and clinic queue management. A React frontend provides patient-facing screens, a doctor dashboard, an administrator panel, and a public queue display.

## Features

- Patient registration and search.
- Appointment creation and patient appointment lookup.
- Check-in, queue numbers, calling the next patient, and completing visits.
- A waiting-room queue display.
- Staff registration and login with JWT authentication.
- Doctor/admin access to queue operations.
- Administrator management of staff roles, account status, visit reasons, and queue history.

## Technology stack

| Layer | Technologies |
| --- | --- |
| Backend | ASP.NET Core Web API, .NET 8, C# |
| Persistence | MySQL 8, Entity Framework Core 8, Pomelo |
| Authentication | JWT bearer tokens, BCrypt password hashing |
| Frontend | React 19, React Router 7, Axios |
| Tooling | Vite 8, Oxlint, npm |
| Deployment configuration | Firebase Hosting frontend; Railway-aware MySQL configuration |

## Run locally

### Requirements

- .NET **8 SDK**.
- Node.js **22.12 or later** and npm.
- MySQL **8**.

### 1. Start the API

From the repository root, configure a local database and your own credentials. The following PowerShell example uses environment variables so passwords do not need to be committed:

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Port=3306;Database=pqms;User Id=pqms_app;Password=<your-database-password>;"
$env:JWT_SECRET = "<your-random-signing-key-at-least-32-characters>"
$env:ADMIN_PASSWORD = "<your-local-admin-password>"

dotnet restore .\PQMS.API\PQMS.API.csproj
dotnet run --project .\PQMS.API\PQMS.API.csproj --launch-profile http
```

Replace the angle-bracketed values before running. Create the database/user and grant the application user the permissions needed to apply migrations.

The API listens at **http://localhost:5140**. Swagger is available at **http://localhost:5140/swagger** in Development. Startup applies EF Core migrations and creates `admin@hospital.com` when no administrator exists. `ADMIN_PASSWORD` controls that initial account's password; changing it later does not reset an existing account.

### 2. Start the frontend

In a second terminal:

```powershell
cd pqms-client
npm ci
$env:VITE_API_URL = "http://localhost:5140/api"
npm run dev
```

Open the address printed by Vite, normally **http://localhost:5173**. The frontend API URL includes the `/api` suffix.

### 3. Explore the workflow

- `/login`: staff authentication.
- `/dashboard`: doctor/admin queue dashboard.
- `/patient/welcome`: patient entry point.
- `/patient/register` and `/patient/search`: patient registration and lookup.
- `/queue-display`: public waiting-room display.
- `/admin`: administrative tools.

## Configuration

| Setting | Purpose |
| --- | --- |
| `ConnectionStrings__DefaultConnection` | Local MySQL connection string |
| `MYSQL_URL` | Deployment MySQL URL or connection string; takes precedence |
| `MYSQLHOST`, `MYSQLPORT`, `MYSQLDATABASE`, `MYSQLUSER`, `MYSQLPASSWORD` | Alternative deployment database settings; underscore variants are also supported |
| `JWT_SECRET` | JWT signing key |
| `ADMIN_PASSWORD` | Password used only when seeding the first administrator |
| `VITE_API_URL` | Browser-visible API base URL |

If Railway database variables are present in your terminal, they override the local connection string. Production CORS origins are configured in [Program.cs](PQMS.API/Program.cs); update them for your own frontend domain.

The repository includes development fallback credentials. Supply your own values before deployment. Patient and kiosk endpoints include anonymous access, so review access rules before using the application with real patient data.

## API areas

| Prefix | Responsibility |
| --- | --- |
| `/api/auth` | Staff login and registration |
| `/api/patients` | Registration and search |
| `/api/appointments` | Booking and patient appointment lookup |
| `/api/queue` | Check-in, today's queue, call-next, completion, and display |
| `/api/visit-reasons` | Visit reason lookup |
| `/api/admin` | Staff administration, visit reasons, and queue history |
| `/api/health` | Health endpoint |

## Project structure

```text
PQMS.API/
  Controllers/     HTTP endpoints
  DTOs/            Request and response models
  Models/          Database entities
  Data/            DbContext and entity configuration
  Migrations/      EF Core database migrations
  Services/        Application logic
pqms-client/
  src/pages/       Patient, staff, admin, and display screens
  src/services/    API clients
  src/context/     Authentication state
docs/              Requirements and development notes
```

## Build and checks

```powershell
dotnet build .\PQMS.API\PQMS.API.csproj
cd pqms-client
npm ci
npm run lint
npm run build
```

There is no dedicated automated test project in the current repository. Existing requirements and phase notes are under [docs/](docs/).
