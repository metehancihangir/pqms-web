using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PQMS.API.Data;
using PQMS.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== Database =====
var mysqlHost = Environment.GetEnvironmentVariable("MYSQLHOST");
string connectionString;

if (!string.IsNullOrEmpty(mysqlHost))
{
    // Railway environment: build connection string from individual env vars
    var mysqlPort = Environment.GetEnvironmentVariable("MYSQLPORT") ?? "3306";
    var mysqlDatabase = Environment.GetEnvironmentVariable("MYSQLDATABASE") ?? "railway";
    var mysqlUser = Environment.GetEnvironmentVariable("MYSQLUSER") ?? "root";
    var mysqlPassword = Environment.GetEnvironmentVariable("MYSQLPASSWORD") ?? "";
    connectionString = $"Server={mysqlHost};Port={mysqlPort};Database={mysqlDatabase};User Id={mysqlUser};Password={mysqlPassword};CharSet=utf8mb4;";
}
else
{
    // Local development: use appsettings.json
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
}

builder.Services.AddDbContext<PqmsDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 36))
    ));

// ===== JWT Authentication =====
var jwtSecret = builder.Configuration["Jwt:Secret"]!;
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
    options.AddPolicy("AllowReactApp", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()));

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

// ===== Apply Migrations & Seed Admin =====
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PqmsDbContext>();
    try
    {
        dbContext.Database.Migrate();

        if (!dbContext.Users.Any(u => u.Role == "Admin"))
        {
            dbContext.Users.Add(new PQMS.API.Models.User
            {
                FullName = "System Administrator",
                Email = "admin@hospital.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                IsActive = true
            });
            dbContext.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration error: {ex.Message}");
    }
}

// ===== Middleware Pipeline =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
