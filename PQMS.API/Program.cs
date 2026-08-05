using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using PQMS.API.Data;
using PQMS.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== Database =====
string connectionString;
var mysqlUrl = Environment.GetEnvironmentVariable("MYSQL_URL");
var mysqlHost = Environment.GetEnvironmentVariable("MYSQLHOST") ?? Environment.GetEnvironmentVariable("MYSQL_HOST");

if (!string.IsNullOrEmpty(mysqlUrl))
{
    // Railway MYSQL_URL format: mysql://user:password@host:port/database
    bool isMySqlUri = Uri.TryCreate(mysqlUrl, UriKind.Absolute, out Uri uri);
    if (isMySqlUri)
    {
        var userInfo = uri.UserInfo.Split(':');
        var user = userInfo.Length > 0 ? userInfo[0] : "root";
        var pass = userInfo.Length > 1 ? userInfo[1] : "";
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 3306;
        var db = uri.LocalPath.TrimStart('/');
        connectionString = $"Server={host};Port={port};Database={db};User Id={user};Password={pass};SslMode=None;AllowPublicKeyRetrieval=True;CharSet=utf8mb4;";
        Console.WriteLine($"[PQMS] Using Railway MYSQL_URL: {host}:{port}/{db}");
    }
    else
    {
        connectionString = mysqlUrl;
        Console.WriteLine($"[PQMS] Using Railway MYSQL_URL directly (unparsed)");
    }
}
else if (!string.IsNullOrEmpty(mysqlHost))
{
    // Railway environment: build connection string from individual env vars
    var mysqlPort = Environment.GetEnvironmentVariable("MYSQLPORT") ?? Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
    var mysqlDatabase = Environment.GetEnvironmentVariable("MYSQLDATABASE") ?? Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "railway";
    var mysqlUser = Environment.GetEnvironmentVariable("MYSQLUSER") ?? Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
    var mysqlPassword = Environment.GetEnvironmentVariable("MYSQLPASSWORD") ?? Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "";
    connectionString = $"Server={mysqlHost};Port={mysqlPort};Database={mysqlDatabase};User Id={mysqlUser};Password={mysqlPassword};SslMode=None;AllowPublicKeyRetrieval=True;CharSet=utf8mb4;";
    Console.WriteLine($"[PQMS] Using Railway MySQL individual vars: {mysqlHost}:{mysqlPort}/{mysqlDatabase}");
}
else
{
    // Local development: use appsettings.json
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    Console.WriteLine("[PQMS] Using local DefaultConnection from appsettings.json");
}

builder.Services.AddDbContext<PqmsDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 36)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)
    ));

// ===== JWT Authentication =====
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? builder.Configuration["Jwt:Secret"]!;
if (jwtSecret == "YOUR_JWT_SECRET_MUST_BE_PROVIDED_IN_ENV_VARS")
{
    throw new InvalidOperationException("JWT Secret is missing. Please set JWT_SECRET in environment variables.");
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ===== CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        }
        else
        {
            policy.WithOrigins("https://pqms-web-production.up.railway.app")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

// ===== Rate Limiting =====
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = global::System.Threading.RateLimiting.PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        global::System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

// ===== Services =====
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IQueueService, QueueService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

// ===== Controllers & Swagger =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===== Apply Migrations & Seed Admin (with retry) =====
for (int attempt = 1; attempt <= 5; attempt++)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PqmsDbContext>();

        Console.WriteLine($"[PQMS] Migration attempt {attempt}/5...");
        dbContext.Database.Migrate();
        Console.WriteLine("[PQMS] Migration completed successfully.");

        if (!dbContext.Users.Any(u => u.Role == "Admin"))
        {
            dbContext.Users.Add(new PQMS.API.Models.User
            {
                FullName = "System Administrator",
                Email = "admin@hospital.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "admin123"),
                Role = "Admin",
                IsActive = true
            });
            dbContext.SaveChanges();
            Console.WriteLine("[PQMS] Admin user seeded successfully.");
        }
        else
        {
            Console.WriteLine("[PQMS] Admin user already exists.");
        }

        break; // success, exit retry loop
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[PQMS] Migration attempt {attempt} failed: {ex.Message}");
        if (attempt == 5)
            Console.WriteLine("[PQMS] All migration attempts failed. App will start without DB setup.");
        else
            Thread.Sleep(3000); // wait 3 seconds before retry
    }
}

// ===== Middleware Pipeline =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
