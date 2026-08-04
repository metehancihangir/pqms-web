# 🏥 PQMS — Patient Queue Management System
## Kapsamlı Geliştirme Yol Haritası (Phases)

> **Teknoloji Yığını:** React (Vite) · .NET 8 Web API (EF Core) · MySQL (utf8mb4) · JWT Authentication
>
> Her faz bağımsız olarak çalışabilecek şekilde tasarlanmıştır. Bir fazı tamamlamadan diğerine geçmemeniz önerilir.

---

## Ön Hazırlık — Faz 0: Proje İskeleti ve Altyapı Kurulumu

> Bu faz, tüm diğer fazların temelini oluşturur. Hiçbir ekrana dokunmadan önce tamamlanmalıdır.

### 🎨 UI/UX Tasarımı
- Bu fazda ekran yoktur; yalnızca proje iskeleti kurulur.
- **Klasör Yapısı (Frontend):**
  ```
  pqms-client/
  ├── src/
  │   ├── components/       # Paylaşılan UI bileşenleri (Button, Input, Card, Modal)
  │   ├── pages/            # Sayfa bileşenleri (LoginPage, DashboardPage, vb.)
  │   ├── hooks/            # Özel React Hook'ları (useAuth, useQueue, vb.)
  │   ├── services/         # API çağrıları (authService, patientService, vb.)
  │   ├── context/          # React Context (AuthContext)
  │   ├── utils/            # Yardımcı fonksiyonlar (formatDate, validators, vb.)
  │   ├── styles/           # Global CSS dosyaları
  │   ├── App.jsx
  │   └── main.jsx
  └── package.json
  ```
- **Klasör Yapısı (Backend):**
  ```
  PQMS.API/
  ├── Controllers/          # API Controller'ları
  ├── Models/               # Entity sınıfları
  ├── DTOs/                 # Data Transfer Object'leri
  ├── Data/                 # DbContext ve konfigürasyonlar
  ├── Services/             # İş mantığı katmanı
  ├── Middleware/            # JWT, Exception Handling, vb.
  ├── Migrations/            # EF Core Migration dosyaları
  ├── Program.cs
  └── appsettings.json
  ```

### 🗄️ Veri Yapısı (Mimari & Veritabanı)

#### MySQL Veritabanı Oluşturma
```sql
CREATE DATABASE pqms_db
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;
```

#### EF Core DbContext Konfigürasyonu
```csharp
// Data/PqmsDbContext.cs
public class PqmsDbContext : DbContext
{
    public PqmsDbContext(DbContextOptions<PqmsDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<QueueEntry> QueueEntries { get; set; }
    public DbSet<VisitReason> VisitReasons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // utf8mb4 karakter seti her tablo için
        modelBuilder.HasCharSet("utf8mb4");

        // Fluent API konfigürasyonları burada tanımlanacak
        base.OnModelCreating(modelBuilder);
    }
}
```

#### Connection String (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=pqms_db;User=root;Password=YOUR_PASSWORD;CharSet=utf8mb4;"
  }
}
```

#### Program.cs Temel Servislerin Kaydı
```csharp
// MySQL bağlantısı (Pomelo.EntityFrameworkCore.MySql)
builder.Services.AddDbContext<PqmsDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 36))
    ));

// CORS
builder.Services.AddCors(options =>
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()));
```

### ⚙️ Kodlama Süreci
1. `dotnet new webapi -n PQMS.API` ile backend projesi oluştur
2. `npm create vite@latest pqms-client -- --template react` ile frontend projesi oluştur
3. Gerekli NuGet paketlerini yükle:
   - `Pomelo.EntityFrameworkCore.MySql`
   - `Microsoft.AspNetCore.Authentication.JwtBearer`
   - `BCrypt.Net-Next`
   - `Swashbuckle.AspNetCore` (Swagger)
4. Gerekli npm paketlerini yükle:
   - `axios` (API istekleri)
   - `react-router-dom` (Yönlendirme)
5. Frontend'de `axios` instance oluştur (baseURL, interceptor):
   ```javascript
   // services/api.js
   import axios from 'axios';

   const api = axios.create({
     baseURL: 'https://localhost:5001/api',
   });

   // Her istekte JWT token'ı header'a ekle
   api.interceptors.request.use((config) => {
     const token = localStorage.getItem('token');
     if (token) {
       config.headers.Authorization = `Bearer ${token}`;
     }
     return config;
   });

   export default api;
   ```
6. İlk migration'ı oluştur ve veritabanını güncelle:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

### ✅ Test
| Test Türü        | Senaryo                                                   |
|------------------|-----------------------------------------------------------|
| Manuel           | Backend projesini çalıştır → Swagger UI açılmalı          |
| Manuel           | Frontend projesini çalıştır → Vite welcome sayfası görülmeli |
| Manuel           | MySQL bağlantısını doğrula → `dotnet ef database update` hatasız tamamlanmalı |
| Entegrasyon      | `/api/health` endpoint'i oluştur ve 200 OK döndüğünü doğrula |

---

## Faz 1: Kimlik Doğrulama — Login & Register Ekranı

> **Ekran Referansı:** Login/Register sekmeli form (Ekran Görüntüsü 1)

### 🎨 UI/UX Tasarımı

#### Ekran Bileşenleri
| Bileşen               | Açıklama                                              |
|------------------------|-------------------------------------------------------|
| **Tab Bar**            | `Login` ve `Register` sekmeleri — aktif sekmenin altı mavi çizgi |
| **Email Input**        | `Email Address` etiketli text input                   |
| **Password Input**     | `Password` etiketli password input                    |
| **Remember Me**        | Checkbox — oturum hatırlama                           |
| **Forgot Password?**   | Sağ tarafta mavi link                                 |
| **Login Button**       | Tam genişlikte mavi buton, ikon + "Login" yazısı      |
| **Ayırıcı**           | `— or —` yazısı ile görsel ayrım                     |
| **Create New Account** | Outlined (çerçeveli) buton — Register sekmesine yönlendirir |

#### Register Sekmesi (Login sekmesinin yanı)
| Bileşen               | Açıklama                                              |
|------------------------|-------------------------------------------------------|
| **Full Name Input**    | Ad-soyad alanı                                        |
| **Email Input**        | E-posta adresi                                        |
| **Password Input**     | Şifre (min. 6 karakter)                               |
| **Confirm Password**   | Şifre tekrarı                                         |
| **Role Selector**      | Dropdown: `Patient` / `Doctor` / `Admin`              |
| **Register Button**    | Tam genişlikte mavi buton                             |

#### Tasarım Notları
- Kart yapısı: beyaz arkaplan, hafif gölge (`box-shadow`), yuvarlatılmış köşeler
- Sayfa arkplanı: açık gri (`#f5f5f5`)
- Buton rengi: `#4361ee` (Royal Blue)
- Font: System font stack veya Google Fonts'tan "Inter"

### 🗄️ Veri Yapısı (Mimari & Veritabanı)

#### User Entity
```csharp
// Models/User.cs
public class User
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required, MaxLength(20)]
    public string Role { get; set; } = "Patient"; // Patient, Doctor, Admin

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
```

#### Fluent API Konfigürasyonu
```csharp
// Data/Configurations/UserConfiguration.cs
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.FullName)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(u => u.Email)
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(u => u.Role)
               .HasMaxLength(20)
               .HasDefaultValue("Patient");
    }
}
```

#### DTO'lar
```csharp
// DTOs/Auth/LoginRequestDto.cs
public record LoginRequestDto(string Email, string Password);

// DTOs/Auth/RegisterRequestDto.cs
public record RegisterRequestDto(string FullName, string Email, string Password, string Role);

// DTOs/Auth/AuthResponseDto.cs
public record AuthResponseDto(string Token, string FullName, string Role);
```

### ⚙️ Kodlama Süreci

#### Backend — API Endpoint Akışı

| Endpoint           | Method | Açıklama                 | Request Body         | Response            |
|--------------------|--------|--------------------------|----------------------|---------------------|
| `/api/auth/login`  | POST   | Kullanıcı girişi         | `LoginRequestDto`    | `AuthResponseDto`   |
| `/api/auth/register` | POST | Yeni hesap oluşturma     | `RegisterRequestDto` | `AuthResponseDto`   |

**Login Akışı:**
```
İstek geldi → Email ile kullanıcıyı bul → BCrypt ile şifre doğrula
  → Başarılı: JWT token üret → AuthResponseDto döndür (200 OK)
  → Başarısız: 401 Unauthorized döndür
```

**JWT Token Üretimi:**
```csharp
// Services/AuthService.cs
public class AuthService : IAuthService
{
    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

#### Frontend — React Durum Yönetimi

```jsx
// pages/AuthPage.jsx
function AuthPage() {
  // Aktif sekmeyi takip eden state
  const [activeTab, setActiveTab] = useState('login'); // 'login' | 'register'

  // Login formu state'leri
  const [loginEmail, setLoginEmail] = useState('');
  const [loginPassword, setLoginPassword] = useState('');
  const [rememberMe, setRememberMe] = useState(false);

  // Register formu state'leri
  const [registerName, setRegisterName] = useState('');
  const [registerEmail, setRegisterEmail] = useState('');
  const [registerPassword, setRegisterPassword] = useState('');
  const [registerConfirm, setRegisterConfirm] = useState('');
  const [registerRole, setRegisterRole] = useState('Patient');

  // Ortak state'ler
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleLogin = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    try {
      const response = await authService.login(loginEmail, loginPassword);
      localStorage.setItem('token', response.data.token);
      // AuthContext üzerinden kullanıcı bilgisini set et
      // React Router ile dashboard'a yönlendir
    } catch (err) {
      setError('Geçersiz e-posta veya şifre.');
    } finally {
      setLoading(false);
    }
  };

  // ... handleRegister benzer mantık
}
```

**AuthContext ile Global Durum Yönetimi:**
```jsx
// context/AuthContext.jsx
const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);       // { fullName, role, token }
  const [isLoading, setIsLoading] = useState(true); // Sayfa yüklenirken token kontrolü

  useEffect(() => {
    // Sayfa yüklendiğinde localStorage'dan token kontrol et
    const token = localStorage.getItem('token');
    if (token) {
      // Token'ı decode et ve user bilgisini set et
      const decoded = parseJwt(token);
      setUser({ fullName: decoded.name, role: decoded.role, token });
    }
    setIsLoading(false);
  }, []);

  const login = (userData) => {
    setUser(userData);
    localStorage.setItem('token', userData.token);
  };

  const logout = () => {
    setUser(null);
    localStorage.removeItem('token');
  };

  return (
    <AuthContext.Provider value={{ user, isLoading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}
```

**Protected Route Bileşeni:**
```jsx
// components/ProtectedRoute.jsx
function ProtectedRoute({ children, allowedRoles }) {
  const { user, isLoading } = useAuth();

  if (isLoading) return <LoadingSpinner />;
  if (!user) return <Navigate to="/login" />;
  if (allowedRoles && !allowedRoles.includes(user.role)) {
    return <Navigate to="/unauthorized" />;
  }
  return children;
}
```

### ✅ Test

| Test Türü        | Senaryo                                                           | Beklenen Sonuç                              |
|------------------|-------------------------------------------------------------------|---------------------------------------------|
| Unit             | `AuthService.GenerateJwtToken()` — geçerli user ile çağır        | Geçerli JWT string döner                    |
| Unit             | `AuthService.Register()` — aynı email ile iki kez kayıt          | İkincisinde hata fırlatır                   |
| Unit             | `AuthService.Login()` — yanlış şifre gönder                      | Null/Exception döner                        |
| Entegrasyon      | `POST /api/auth/register` — geçerli veri gönder                  | 200 OK + token döner                        |
| Entegrasyon      | `POST /api/auth/login` — doğru bilgiler ile                      | 200 OK + token döner                        |
| Entegrasyon      | `POST /api/auth/login` — yanlış şifre ile                        | 401 Unauthorized                            |
| Frontend         | Login tab'a tıkla → form alanlarını doldur → Login butonuna bas  | Başarılı girişte Dashboard'a yönlendirilme   |
| Frontend         | Register tab'a geç → alanları doldur → farklı şifre gir          | "Şifreler eşleşmiyor" hata mesajı           |
| Frontend         | Boş email ile Login'e bas                                        | Validation hatası gösterilir                |

---

## Faz 2: Yönetici Dashboard — Kuyruk Yönetim Ekranı

> **Ekran Referansı:** "Today's Queue" ekranı — 3 kolonlu Kanban görünümü (Ekran Görüntüsü 2)

### 🎨 UI/UX Tasarımı

#### Ekran Bileşenleri

**Navbar (Üst Navigasyon Çubuğu)**
| Bileşen            | Açıklama                                                      |
|--------------------|---------------------------------------------------------------|
| **PQMS Logo**      | Sol üstte kalın beyaz yazı                                   |
| **Nav Links**      | `Dashboard`, `Queue Display`, `Admin` — beyaz linkler        |
| **User Info**      | Sağ üstte `Dr. Mvuma` yazısı                                |
| **Logout Button**  | Beyaz çerçeveli outlined buton                               |
| **Arkaplan**       | Mavi gradient (`#4361ee`)                                    |

**Sayfa İçeriği**
| Bileşen                 | Açıklama                                                     |
|-------------------------|--------------------------------------------------------------|
| **Başlık**              | `Today's Queue` — büyük bold başlık                          |
| **Tarih**               | Başlığın altında günün tarihi (örn: "Thursday, September 4, 2025") |
| **Call Next Patient**   | Sağ üstte yeşil buton — sıradaki hastayı çağırır            |

**3 Kolonlu Kuyruk Tahtası (Kanban)**
| Kolon            | Renk Kodu  | İçerik                                                        |
|------------------|------------|----------------------------------------------------------------|
| **Waiting Room** | Açık Mavi (`#4fc3f7`) | Bekleyen hastalar listesi                         |
| **In Progress**  | Sarı/Turuncu (`#ffca28`) | Muayenesi devam eden hastalar                  |
| **Completed**    | Yeşil (`#43a047`) | Muayenesi tamamlanan hastalar                       |

**Hasta Kartı (Her kolonda tekrarlanan bileşen)**
| Bileşen           | Açıklama                                                       |
|--------------------|---------------------------------------------------------------|
| **Hasta Adı**      | Bold yazı (örn: "Chikondi Gama")                             |
| **Saat**           | Kayıt saati (örn: "12:32 PM")                                |
| **Ziyaret Sebebi** | Alt satırda (örn: "Follow-up Visit")                         |
| **Kuyruk Numarası**| Sağ tarafta renkli badge (örn: `WALK-6`, `APPT-7`)          |
|                    | Mavi badge = Walk-in, Yeşil badge = Appointment              |

**Kolon Alt Bilgisi**
| Bileşen            | Açıklama                                                     |
|--------------------|--------------------------------------------------------------|
| **Toplam Sayı**    | `Total waiting: 2 patients` gibi sayaç                       |

### 🗄️ Veri Yapısı (Mimari & Veritabanı)

#### QueueEntry Entity
```csharp
// Models/QueueEntry.cs
public class QueueEntry
{
    public int Id { get; set; }

    [Required]
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    [Required, MaxLength(15)]
    public string QueueNumber { get; set; } = string.Empty; // "WALK-6", "APPT-7"

    [Required, MaxLength(20)]
    public string Status { get; set; } = "Waiting"; // Waiting, InProgress, Completed

    [Required, MaxLength(20)]
    public string CheckInType { get; set; } = "WalkIn"; // WalkIn, Appointment

    [MaxLength(100)]
    public string? VisitReason { get; set; }

    [MaxLength(500)]
    public string? AdditionalInfo { get; set; }

    public DateTime CheckInTime { get; set; } = DateTime.UtcNow;
    public DateTime? CalledAt { get; set; }      // InProgress'e geçiş zamanı
    public DateTime? CompletedAt { get; set; }   // Completed'a geçiş zamanı

    public DateTime QueueDate { get; set; } = DateTime.UtcNow.Date; // Hangi güne ait
}
```

#### Patient Entity
```csharp
// Models/Patient.cs
public class Patient
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<QueueEntry> QueueEntries { get; set; } = new List<QueueEntry>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
```

#### Fluent API — QueueEntry Konfigürasyonu
```csharp
public class QueueEntryConfiguration : IEntityTypeConfiguration<QueueEntry>
{
    public void Configure(EntityTypeBuilder<QueueEntry> builder)
    {
        builder.ToTable("QueueEntries");

        builder.HasOne(q => q.Patient)
               .WithMany(p => p.QueueEntries)
               .HasForeignKey(q => q.PatientId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(q => q.QueueDate);
        builder.HasIndex(q => q.Status);

        builder.Property(q => q.QueueNumber).HasMaxLength(15);
        builder.Property(q => q.Status).HasMaxLength(20).HasDefaultValue("Waiting");
        builder.Property(q => q.CheckInType).HasMaxLength(20);
    }
}
```

### ⚙️ Kodlama Süreci

#### Backend — API Endpoint'leri

| Endpoint                              | Method | Açıklama                                 | Yetki   |
|---------------------------------------|--------|------------------------------------------|---------|
| `/api/queue/today`                    | GET    | Bugünkü tüm kuyruk kayıtlarını getir    | Doctor/Admin |
| `/api/queue/call-next`                | POST   | Sıradaki hastayı çağır (Waiting → InProgress) | Doctor/Admin |
| `/api/queue/{id}/complete`            | PUT    | Hastayı tamamla (InProgress → Completed) | Doctor/Admin |
| `/api/queue/{id}/status`              | PUT    | Durum güncelle                           | Doctor/Admin |

**"Call Next Patient" İş Mantığı:**
```
1. QueueDate = bugün ve Status = "Waiting" olan kayıtları bul
2. CheckInTime'a göre sırala (ilk gelen önce)
3. İlk kaydın Status'unu "InProgress" yap, CalledAt = DateTime.UtcNow
4. Eğer halihazırda InProgress olan biri varsa, onu "Completed" yap
5. Güncellenen kaydı döndür
```

**Kuyruk Numarası Üretme Mantığı:**
```csharp
// Services/QueueService.cs
public async Task<string> GenerateQueueNumber(string checkInType, DateTime date)
{
    string prefix = checkInType == "Appointment" ? "APPT" : "WALK";

    int todayCount = await _context.QueueEntries
        .Where(q => q.QueueDate == date.Date && q.CheckInType == checkInType)
        .CountAsync();

    return $"{prefix}-{todayCount + 1}";
}
```

#### Frontend — React Durum Yönetimi

```jsx
// pages/DashboardPage.jsx
function DashboardPage() {
  // Kuyruk verilerini tutan state
  const [queueEntries, setQueueEntries] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // Duruma göre filtreleme (türetilmiş state — hesaplanmış değer)
  const waitingPatients = queueEntries.filter(q => q.status === 'Waiting');
  const inProgressPatients = queueEntries.filter(q => q.status === 'InProgress');
  const completedPatients = queueEntries.filter(q => q.status === 'Completed');

  // Sayfa yüklendiğinde bugünkü kuyruğu çek
  useEffect(() => {
    fetchTodayQueue();
  }, []);

  const fetchTodayQueue = async () => {
    setLoading(true);
    try {
      const response = await queueService.getTodayQueue();
      setQueueEntries(response.data);
    } catch (err) {
      setError('Kuyruk verileri yüklenemedi.');
    } finally {
      setLoading(false);
    }
  };

  const handleCallNext = async () => {
    try {
      await queueService.callNextPatient();
      await fetchTodayQueue(); // Listeyi yenile
    } catch (err) {
      setError('Sıradaki hasta çağrılamadı.');
    }
  };

  return (
    <>
      <Navbar />
      <h1>Today's Queue</h1>
      <p>{formatDate(new Date())}</p>
      <button onClick={handleCallNext}>Call Next Patient</button>

      <div className="queue-board">
        <QueueColumn title="Waiting Room" color="blue" patients={waitingPatients} />
        <QueueColumn title="In Progress" color="yellow" patients={inProgressPatients} />
        <QueueColumn title="Completed" color="green" patients={completedPatients} />
      </div>
    </>
  );
}
```

**QueueColumn Bileşeni:**
```jsx
// components/QueueColumn.jsx
function QueueColumn({ title, color, patients }) {
  return (
    <div className={`queue-column queue-column--${color}`}>
      <h2 className="queue-column__header">{title}</h2>
      {patients.map(patient => (
        <PatientCard key={patient.id} patient={patient} />
      ))}
      <p className="queue-column__footer">
        Total {title.toLowerCase()}: {patients.length} patients
      </p>
    </div>
  );
}
```

**PatientCard Bileşeni:**
```jsx
// components/PatientCard.jsx
function PatientCard({ patient }) {
  const badgeClass = patient.checkInType === 'WalkIn' ? 'badge--blue' : 'badge--green';

  return (
    <div className="patient-card">
      <div className="patient-card__info">
        <h3>{patient.patientName}</h3>
        <span>{formatTime(patient.checkInTime)}</span>
        {patient.visitReason && <span> - {patient.visitReason}</span>}
      </div>
      <span className={`badge ${badgeClass}`}>
        {patient.queueNumber}
      </span>
    </div>
  );
}
```

### ✅ Test

| Test Türü        | Senaryo                                                             | Beklenen Sonuç                               |
|------------------|---------------------------------------------------------------------|----------------------------------------------|
| Unit             | `QueueService.GenerateQueueNumber("WalkIn", bugün)` — ilk kayıt    | `"WALK-1"` döner                             |
| Unit             | `QueueService.GenerateQueueNumber("Appointment", bugün)` — 3. kayıt| `"APPT-3"` döner                             |
| Unit             | `QueueService.CallNext()` — Waiting kaydı yokken çağır             | Uygun hata mesajı döner                      |
| Entegrasyon      | `GET /api/queue/today` — Authorization header ile                   | 200 OK + kuyruk listesi                      |
| Entegrasyon      | `GET /api/queue/today` — Token olmadan                              | 401 Unauthorized                             |
| Entegrasyon      | `POST /api/queue/call-next` — Waiting kaydı varken                  | 200 OK + durum InProgress olarak güncellenir |
| Frontend         | Dashboard yüklendiğinde 3 kolon görünür mü?                        | Evet — Waiting, In Progress, Completed       |
| Frontend         | "Call Next Patient" butonuna bas                                    | Waiting'den ilk hasta InProgress'e geçer     |
| Frontend         | Tamamlanan hasta Completed kolonunda görünür mü?                   | Evet — saat bilgisi ile gösterilir           |

---

## Faz 3: Hasta Karşılama — Welcome Ekranı

> **Ekran Referansı:** "Have you been here before?" sorusu ile iki seçenekli karşılama (Ekran Görüntüsü 3)

### 🎨 UI/UX Tasarımı

#### Ekran Bileşenleri
| Bileşen                         | Açıklama                                                       |
|----------------------------------|---------------------------------------------------------------|
| **Navbar (Minimal)**             | Solda `PQMS` logosu, sağda `Patient Welcome` yazısı          |
| **Soru İkonu**                   | Mavi daire içinde `?` ikonu — kartın üst orta kısmında       |
| **Başlık**                       | `Have you been here before, or is this your first visit?`     |
| **Alt Başlık**                   | `Please select one of the options below` — gri renk          |
| **Evet Butonu**                  | Tam genişlikte mavi buton: `✓ Yes, I've been here before`    |
| **Hayır Butonu**                 | Tam genişlikte outlined buton: `👤+ No, this is my first visit` |
| **Kart Yapısı**                  | Beyaz kart, hafif gölge, sayfa ortasında dikey hizalı        |

#### Tasarım Notları
- Kart ortalanmalı (`max-width: 600px`, `margin: auto`)
- Butonlar arasında yeterli boşluk (`gap: 16px`)
- Mobil uyumlu: tek kolon düzeni

### 🗄️ Veri Yapısı
- Bu ekranda yeni veri modeli eklenmez
- Yalnızca yönlendirme mantığı vardır:
  - **"Yes"** → Faz 4 (Hasta Arama ekranı)
  - **"No"** → Faz 5 (Yeni Hasta Kayıt formu, bu faz şimdilik placeholder olabilir)

### ⚙️ Kodlama Süreci

#### Backend
- Bu ekran için backend gereksinimi **yoktur**. Tamamen frontend routing ile çalışır.

#### Frontend — React Durum Yönetimi

```jsx
// pages/PatientWelcomePage.jsx
import { useNavigate } from 'react-router-dom';

function PatientWelcomePage() {
  const navigate = useNavigate();

  // Bu ekranda karmaşık state yoktur — yalnızca navigasyon
  const handleExistingPatient = () => {
    navigate('/patient/search');   // Hasta Arama ekranına git
  };

  const handleNewPatient = () => {
    navigate('/patient/register'); // Yeni Hasta Kayıt ekranına git
  };

  return (
    <div className="welcome-container">
      <div className="welcome-card">
        <div className="welcome-card__icon">?</div>
        <h1>Have you been here before, or is this your first visit?</h1>
        <p>Please select one of the options below</p>

        <button className="btn btn--primary btn--full" onClick={handleExistingPatient}>
          ✓ Yes, I've been here before
        </button>
        <button className="btn btn--outlined btn--full" onClick={handleNewPatient}>
          👤+ No, this is my first visit
        </button>
      </div>
    </div>
  );
}
```

**React Router Konfigürasyonu (bu noktada):**
```jsx
// App.jsx — Bu fazda route yapısını güncelle
<Routes>
  <Route path="/login" element={<AuthPage />} />
  <Route path="/dashboard" element={
    <ProtectedRoute allowedRoles={['Doctor', 'Admin']}>
      <DashboardPage />
    </ProtectedRoute>
  } />
  <Route path="/patient/welcome" element={<PatientWelcomePage />} />
  <Route path="/patient/search" element={<PatientSearchPage />} />
  <Route path="/patient/register" element={<PatientRegisterPage />} />
  <Route path="/patient/checkin/:patientId" element={<PatientCheckInPage />} />
</Routes>
```

### ✅ Test

| Test Türü        | Senaryo                                                       | Beklenen Sonuç                                 |
|------------------|---------------------------------------------------------------|------------------------------------------------|
| Frontend         | Welcome sayfası yüklendiğinde iki buton görünür mü?           | Evet — `Yes` (mavi) ve `No` (outlined)         |
| Frontend         | "Yes" butonuna tıkla                                          | `/patient/search` sayfasına yönlendirilme      |
| Frontend         | "No" butonuna tıkla                                           | `/patient/register` sayfasına yönlendirilme    |
| Frontend         | Sayfanın mobilde doğru görünüp görünmediğini kontrol et       | Kart dar ekranlarda da düzgün ortalanır        |

---

## Faz 4: Hasta Arama Ekranı

> **Ekran Referansı:** "Find Patient" formu + sonuç kartları (Ekran Görüntüsü 4 & 5)

### 🎨 UI/UX Tasarımı

#### Ekran Bileşenleri — Arama Formu
| Bileşen                   | Açıklama                                                        |
|----------------------------|-----------------------------------------------------------------|
| **Navbar**                 | Sağ üstte `Patient Search` yazısı                              |
| **Kart Başlığı**           | `Find Patient` — büyük bold başlık                             |
| **Alt Başlık**             | `Search by name or phone number` — gri renk                   |
| **Search Term Input**      | Text input — hasta adı veya telefon numarası                   |
| **Date of Birth Input**    | Date picker — doğum tarihi filtresi                            |
| **Search Patient Button** | Tam genişlikte mavi buton                                      |

#### Ekran Bileşenleri — Sonuç Durumları

**Sonuç Bulunamadı (Ekran Görüntüsü 4):**
| Bileşen                     | Açıklama                                                     |
|------------------------------|--------------------------------------------------------------|
| **Uyarı Kutusu**             | Sarı arkaplan: `No patients found.`                         |
| **Register Linki**           | Mavi link: `Register new patient` — kayıt formuna yönlendir |

**Sonuç Bulundu (Ekran Görüntüsü 5):**
| Bileşen                     | Açıklama                                                     |
|------------------------------|--------------------------------------------------------------|
| **Hasta Kartı**              | Beyaz kart, hafif gölge                                     |
| **Hasta Adı**                | Bold — `James Kamanga`                                       |
| **Telefon**                  | Telefon ikonu + numara — `📞 0888876600`                    |
| **Doğum Tarihi**             | Sağ üstte — `June 14, 2000`                                |
| **Book Appointment Button** | Mavi buton — randevu oluşturma akışına yönlendirir          |
| **Proceed to Check-In**     | Yeşil buton — direkt check-in ekranına yönlendirir          |

### 🗄️ Veri Yapısı (Mimari & Veritabanı)

#### DTO'lar
```csharp
// DTOs/Patient/PatientSearchRequestDto.cs
public record PatientSearchRequestDto(string? SearchTerm, DateTime? DateOfBirth);

// DTOs/Patient/PatientSearchResultDto.cs
public record PatientSearchResultDto(
    int Id,
    string FullName,
    string? PhoneNumber,
    DateTime? DateOfBirth
);

// DTOs/Patient/PatientCreateDto.cs (Faz 5'te kullanılacak)
public record PatientCreateDto(
    string FullName,
    string? PhoneNumber,
    DateTime? DateOfBirth
);
```

#### Arama için Index (Performans)
```csharp
// Patient Konfigürasyonuna ekle
builder.HasIndex(p => p.FullName);
builder.HasIndex(p => p.PhoneNumber);
builder.HasIndex(p => p.DateOfBirth);
```

### ⚙️ Kodlama Süreci

#### Backend — API Endpoint'leri

| Endpoint                     | Method | Açıklama                              | Request               | Response                  |
|------------------------------|--------|---------------------------------------|------------------------|---------------------------|
| `/api/patients/search`       | GET    | Hasta ara (isim/telefon + doğum tarihi)| Query params          | `List<PatientSearchResultDto>` |
| `/api/patients`              | POST   | Yeni hasta oluştur                    | `PatientCreateDto`     | `PatientSearchResultDto`  |
| `/api/patients/{id}`         | GET    | Hasta detayı                          | —                      | `PatientSearchResultDto`  |

**Arama Mantığı:**
```csharp
// Services/PatientService.cs
public async Task<List<PatientSearchResultDto>> SearchPatients(string? searchTerm, DateTime? dateOfBirth)
{
    var query = _context.Patients.AsQueryable();

    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        string term = searchTerm.Trim().ToLower();
        query = query.Where(p =>
            p.FullName.ToLower().Contains(term) ||
            (p.PhoneNumber != null && p.PhoneNumber.Contains(term))
        );
    }

    if (dateOfBirth.HasValue)
    {
        query = query.Where(p => p.DateOfBirth == dateOfBirth.Value.Date);
    }

    return await query
        .Select(p => new PatientSearchResultDto(p.Id, p.FullName, p.PhoneNumber, p.DateOfBirth))
        .Take(20) // Performans için limit
        .ToListAsync();
}
```

#### Frontend — React Durum Yönetimi

```jsx
// pages/PatientSearchPage.jsx
function PatientSearchPage() {
  const navigate = useNavigate();

  // Form inputları
  const [searchTerm, setSearchTerm] = useState('');
  const [dateOfBirth, setDateOfBirth] = useState('');

  // Sonuç state'leri
  const [results, setResults] = useState([]);        // Arama sonuçları dizisi
  const [hasSearched, setHasSearched] = useState(false); // Arama yapıldı mı?
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSearch = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    setHasSearched(true);

    try {
      const response = await patientService.search(searchTerm, dateOfBirth);
      setResults(response.data);
    } catch (err) {
      setError('Arama sırasında bir hata oluştu.');
    } finally {
      setLoading(false);
    }
  };

  const handleBookAppointment = (patientId) => {
    navigate(`/patient/appointment/${patientId}`);
  };

  const handleCheckIn = (patientId) => {
    navigate(`/patient/checkin/${patientId}`);
  };

  return (
    <div className="search-container">
      <div className="search-card">
        <h1>Find Patient</h1>
        <p>Search by name or phone number</p>

        <form onSubmit={handleSearch}>
          <div className="search-form__row">
            <input
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="Search Term"
            />
            <input
              type="date"
              value={dateOfBirth}
              onChange={(e) => setDateOfBirth(e.target.value)}
            />
          </div>
          <button type="submit" className="btn btn--primary btn--full">
            🔍 Search Patient
          </button>
        </form>

        {hasSearched && (
          <div className="search-results">
            <h2>Search Results</h2>
            {results.length === 0 ? (
              <div className="alert alert--warning">
                No patients found. <a href="/patient/register">Register new patient</a>
              </div>
            ) : (
              results.map(patient => (
                <PatientResultCard
                  key={patient.id}
                  patient={patient}
                  onBookAppointment={() => handleBookAppointment(patient.id)}
                  onCheckIn={() => handleCheckIn(patient.id)}
                />
              ))
            )}
          </div>
        )}
      </div>
    </div>
  );
}
```

**PatientResultCard Bileşeni:**
```jsx
// components/PatientResultCard.jsx
function PatientResultCard({ patient, onBookAppointment, onCheckIn }) {
  return (
    <div className="patient-result-card">
      <div className="patient-result-card__info">
        <h3>{patient.fullName}</h3>
        <span>📞 {patient.phoneNumber}</span>
      </div>
      <span className="patient-result-card__dob">
        {formatDate(patient.dateOfBirth)}
      </span>
      <div className="patient-result-card__actions">
        <button className="btn btn--primary" onClick={onBookAppointment}>
          📅 Book Appointment
        </button>
        <button className="btn btn--success" onClick={onCheckIn}>
          ✓ Proceed to Check-In
        </button>
      </div>
    </div>
  );
}
```

### ✅ Test

| Test Türü        | Senaryo                                                          | Beklenen Sonuç                                 |
|------------------|------------------------------------------------------------------|------------------------------------------------|
| Unit             | `PatientService.Search("James", null)` — eşleşen kayıt var      | İlgili hasta listesi döner                     |
| Unit             | `PatientService.Search("XYZ", null)` — eşleşen kayıt yok        | Boş liste döner                                |
| Unit             | `PatientService.Search(null, "2000-06-14")` — sadece doğum tarihi| Eşleşen hastalar döner                         |
| Entegrasyon      | `GET /api/patients/search?searchTerm=James&dateOfBirth=2000-06-14` | 200 OK + sonuç listesi                       |
| Entegrasyon      | `GET /api/patients/search?searchTerm=ZZZ`                         | 200 OK + boş liste                            |
| Frontend         | Arama formunu doldur ve "Search Patient" butonuna bas            | Sonuçlar listelenir veya "bulunamadı" mesajı   |
| Frontend         | "Book Appointment" butonuna tıkla                                | Randevu ekranına yönlendirilme                 |
| Frontend         | "Proceed to Check-In" butonuna tıkla                             | Check-in ekranına yönlendirilme                |
| Frontend         | Boş arama yap                                                   | Validation uyarısı veya tüm hastalar listelenir|

---

## Faz 5: Hasta Check-In Ekranı

> **Ekran Referansı:** "Welcome to Our Clinic" — Appointment/Walk-In sekmeli check-in formu (Ekran Görüntüsü 6)

### 🎨 UI/UX Tasarımı

#### Ekran Bileşenleri
| Bileşen                        | Açıklama                                                        |
|---------------------------------|-----------------------------------------------------------------|
| **Navbar**                      | Sağ üstte `Patient Check-In` yazısı                           |
| **Kart Başlığı**                | `Welcome to Our Clinic` — büyük bold, ortalı                  |
| **Alt Başlık**                  | `Please complete your check-in process` — gri, ortalı         |
| **Tab Bar**                     | İki sekme: `Appointment` (mavi yazı) / `Walk-In`              |
| **Reason for Visit Dropdown**   | Select/dropdown — ziyaret sebebi seçimi                       |
| **Additional Info Textarea**    | Çok satırlı metin alanı — ek bilgiler                         |
| **Terms Checkbox**              | `I agree to the clinic's terms and conditions` + kırmızı `*`  |
| **Check Ins Button**            | Mavi buton — formun ortasında                                  |

#### Ziyaret Sebepleri (Dropdown Seçenekleri)
- General Checkup
- Follow-up Visit
- Urgent Care
- Consultation
- Lab Results
- Vaccination
- Other

### 🗄️ Veri Yapısı (Mimari & Veritabanı)

#### VisitReason Lookup Tablosu (Opsiyonel ama önerilir)
```csharp
// Models/VisitReason.cs
public class VisitReason
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
```

#### Seed Data (Başlangıç Verileri)
```csharp
// Data/Configurations/VisitReasonConfiguration.cs
public class VisitReasonConfiguration : IEntityTypeConfiguration<VisitReason>
{
    public void Configure(EntityTypeBuilder<VisitReason> builder)
    {
        builder.ToTable("VisitReasons");

        builder.HasData(
            new VisitReason { Id = 1, Name = "General Checkup" },
            new VisitReason { Id = 2, Name = "Follow-up Visit" },
            new VisitReason { Id = 3, Name = "Urgent Care" },
            new VisitReason { Id = 4, Name = "Consultation" },
            new VisitReason { Id = 5, Name = "Lab Results" },
            new VisitReason { Id = 6, Name = "Vaccination" },
            new VisitReason { Id = 7, Name = "Other" }
        );
    }
}
```

#### Check-In DTO
```csharp
// DTOs/Queue/CheckInRequestDto.cs
public record CheckInRequestDto(
    int PatientId,
    string CheckInType,    // "Appointment" veya "WalkIn"
    string VisitReason,
    string? AdditionalInfo,
    bool TermsAccepted
);

// DTOs/Queue/CheckInResponseDto.cs
public record CheckInResponseDto(
    int QueueEntryId,
    string QueueNumber,    // Örn: "WALK-6"
    string PatientName,
    string Status,
    DateTime CheckInTime
);
```

### ⚙️ Kodlama Süreci

#### Backend — API Endpoint'leri

| Endpoint                        | Method | Açıklama                              | Request             | Response              |
|---------------------------------|--------|---------------------------------------|----------------------|-----------------------|
| `/api/queue/checkin`            | POST   | Hasta check-in yap → kuyruk numarası al | `CheckInRequestDto` | `CheckInResponseDto` |
| `/api/visit-reasons`            | GET    | Ziyaret sebeplerini listele           | —                    | `List<VisitReason>`  |

**Check-In İş Mantığı:**
```
1. PatientId geçerliliğini kontrol et (hasta mevcut mu?)
2. Aynı hasta bugün zaten check-in yapmış mı? (çift kayıt engeli)
3. TermsAccepted == true olmalı
4. Kuyruk numarası üret (GenerateQueueNumber)
5. QueueEntry kaydını oluştur ve kaydet
6. CheckInResponseDto döndür
```

```csharp
// Services/QueueService.cs
public async Task<CheckInResponseDto> CheckIn(CheckInRequestDto request)
{
    // 1. Hasta kontrolü
    var patient = await _context.Patients.FindAsync(request.PatientId)
        ?? throw new NotFoundException("Patient not found.");

    // 2. Çift kayıt kontrolü
    bool alreadyCheckedIn = await _context.QueueEntries
        .AnyAsync(q => q.PatientId == request.PatientId
                    && q.QueueDate == DateTime.UtcNow.Date
                    && q.Status != "Completed");

    if (alreadyCheckedIn)
        throw new BusinessException("Patient is already in the queue today.");

    // 3. Terms kontrolü
    if (!request.TermsAccepted)
        throw new ValidationException("You must accept the terms and conditions.");

    // 4-5. Kuyruk numarası üret ve kaydet
    string queueNumber = await GenerateQueueNumber(request.CheckInType, DateTime.UtcNow);

    var entry = new QueueEntry
    {
        PatientId = request.PatientId,
        QueueNumber = queueNumber,
        CheckInType = request.CheckInType,
        VisitReason = request.VisitReason,
        AdditionalInfo = request.AdditionalInfo,
        Status = "Waiting",
        CheckInTime = DateTime.UtcNow,
        QueueDate = DateTime.UtcNow.Date
    };

    _context.QueueEntries.Add(entry);
    await _context.SaveChangesAsync();

    return new CheckInResponseDto(entry.Id, queueNumber, patient.FullName, "Waiting", entry.CheckInTime);
}
```

#### Frontend — React Durum Yönetimi

```jsx
// pages/PatientCheckInPage.jsx
function PatientCheckInPage() {
  const { patientId } = useParams();
  const navigate = useNavigate();

  // Form state'leri
  const [checkInType, setCheckInType] = useState('Appointment'); // 'Appointment' | 'WalkIn'
  const [visitReason, setVisitReason] = useState('');
  const [additionalInfo, setAdditionalInfo] = useState('');
  const [termsAccepted, setTermsAccepted] = useState(false);

  // Dropdown seçenekleri (API'den gelecek)
  const [visitReasons, setVisitReasons] = useState([]);

  // Sonuç ve hata state'leri
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [checkInResult, setCheckInResult] = useState(null); // Başarılı check-in sonucu

  // Ziyaret sebeplerini yükle
  useEffect(() => {
    const loadVisitReasons = async () => {
      try {
        const response = await queueService.getVisitReasons();
        setVisitReasons(response.data);
      } catch {
        setError('Ziyaret sebepleri yüklenemedi.');
      }
    };
    loadVisitReasons();
  }, []);

  const handleCheckIn = async (e) => {
    e.preventDefault();

    // Client-side validation
    if (!visitReason) {
      setError('Lütfen bir ziyaret sebebi seçin.');
      return;
    }
    if (!termsAccepted) {
      setError('Koşulları kabul etmelisiniz.');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const response = await queueService.checkIn({
        patientId: parseInt(patientId),
        checkInType,
        visitReason,
        additionalInfo,
        termsAccepted,
      });
      setCheckInResult(response.data);
      // Başarı sonrası kuyruk numarasını göster veya yönlendir
    } catch (err) {
      setError(err.response?.data?.message || 'Check-in başarısız.');
    } finally {
      setLoading(false);
    }
  };

  // Eğer check-in başarılıysa, sonuç ekranı göster
  if (checkInResult) {
    return (
      <CheckInSuccess
        queueNumber={checkInResult.queueNumber}
        patientName={checkInResult.patientName}
      />
    );
  }

  return (
    <div className="checkin-container">
      <div className="checkin-card">
        <h1>Welcome to Our Clinic</h1>
        <p>Please complete your check-in process</p>

        {/* Tab Bar */}
        <div className="tab-bar">
          <button
            className={`tab ${checkInType === 'Appointment' ? 'tab--active' : ''}`}
            onClick={() => setCheckInType('Appointment')}
          >
            Appointment
          </button>
          <button
            className={`tab ${checkInType === 'WalkIn' ? 'tab--active' : ''}`}
            onClick={() => setCheckInType('WalkIn')}
          >
            Walk-In
          </button>
        </div>

        <form onSubmit={handleCheckIn}>
          <label>Reason for Visit</label>
          <select value={visitReason} onChange={(e) => setVisitReason(e.target.value)}>
            <option value="">Select reason</option>
            {visitReasons.map(r => (
              <option key={r.id} value={r.name}>{r.name}</option>
            ))}
          </select>

          <label>Additional Info</label>
          <textarea
            value={additionalInfo}
            onChange={(e) => setAdditionalInfo(e.target.value)}
            rows={4}
          />

          <label className="checkbox-label">
            <input
              type="checkbox"
              checked={termsAccepted}
              onChange={(e) => setTermsAccepted(e.target.checked)}
            />
            I agree to the clinic's <a href="#">terms and conditions</a> *
          </label>

          {error && <div className="alert alert--error">{error}</div>}

          <button type="submit" className="btn btn--primary" disabled={loading}>
            {loading ? 'Processing...' : 'Check Ins'}
          </button>
        </form>
      </div>
    </div>
  );
}
```

### ✅ Test

| Test Türü        | Senaryo                                                         | Beklenen Sonuç                                 |
|------------------|-----------------------------------------------------------------|------------------------------------------------|
| Unit             | `QueueService.CheckIn()` — geçerli veri ile                    | QueueEntry oluşturulur, kuyruk numarası döner   |
| Unit             | `QueueService.CheckIn()` — zaten bugün check-in yapmış hasta   | `BusinessException` fırlatılır                 |
| Unit             | `QueueService.CheckIn()` — termsAccepted = false               | `ValidationException` fırlatılır               |
| Unit             | `QueueService.CheckIn()` — olmayan PatientId                   | `NotFoundException` fırlatılır                 |
| Entegrasyon      | `POST /api/queue/checkin` — geçerli veri                        | 200 OK + kuyruk numarası                       |
| Entegrasyon      | `POST /api/queue/checkin` — aynı hasta ikinci kez              | 400 Bad Request                                |
| Entegrasyon      | `GET /api/visit-reasons`                                        | 200 OK + sebep listesi                         |
| Frontend         | Appointment/Walk-In tab'larına tıkla                            | checkInType state değişir                      |
| Frontend         | Reason seçmeden "Check Ins" butonuna bas                        | Hata mesajı gösterilir                         |
| Frontend         | Terms checkbox'ı işaretlemeden submit et                        | Hata mesajı gösterilir                         |
| Frontend         | Başarılı check-in sonrası                                       | Kuyruk numarası ekranda gösterilir             |

---

## Faz 6: Yeni Hasta Kayıt Ekranı

> **Ekran Referansı:** Doğrudan ekran görüntüsünde yok, ama "No, this is my first visit" ve "Register new patient" linklerinden erişilen form

### 🎨 UI/UX Tasarımı

#### Ekran Bileşenleri
| Bileşen                  | Açıklama                                                        |
|---------------------------|-----------------------------------------------------------------|
| **Navbar**                | Sağ üstte `Patient Registration` yazısı                       |
| **Kart Başlığı**          | `Register New Patient` — büyük bold başlık                    |
| **Full Name Input**       | Text input — zorunlu alan                                      |
| **Phone Number Input**    | Tel input — telefon numarası                                   |
| **Date of Birth Input**   | Date picker — doğum tarihi                                     |
| **Register Button**       | Tam genişlikte mavi buton                                      |
| **Back Link**             | Alt kısımda `← Back to search` linki                          |

### 🗄️ Veri Yapısı
- Faz 4'te tanımlanan `Patient` entity ve `PatientCreateDto` kullanılır
- Ek model gerekmez

### ⚙️ Kodlama Süreci

#### Backend — API Endpoint
| Endpoint              | Method | Açıklama               | Request            | Response                 |
|-----------------------|--------|-------------------------|---------------------|--------------------------|
| `/api/patients`       | POST   | Yeni hasta kaydı oluştur | `PatientCreateDto`  | `PatientSearchResultDto` |

**Kayıt Mantığı:**
```csharp
public async Task<PatientSearchResultDto> CreatePatient(PatientCreateDto dto)
{
    // Aynı isim + doğum tarihi ile kayıt kontrolü (opsiyonel duplicate check)
    var existing = await _context.Patients
        .AnyAsync(p => p.FullName == dto.FullName && p.DateOfBirth == dto.DateOfBirth);

    if (existing)
        throw new BusinessException("A patient with this name and date of birth already exists.");

    var patient = new Patient
    {
        FullName = dto.FullName,
        PhoneNumber = dto.PhoneNumber,
        DateOfBirth = dto.DateOfBirth,
    };

    _context.Patients.Add(patient);
    await _context.SaveChangesAsync();

    return new PatientSearchResultDto(patient.Id, patient.FullName, patient.PhoneNumber, patient.DateOfBirth);
}
```

#### Frontend — React Durum Yönetimi

```jsx
// pages/PatientRegisterPage.jsx
function PatientRegisterPage() {
  const navigate = useNavigate();

  const [fullName, setFullName] = useState('');
  const [phoneNumber, setPhoneNumber] = useState('');
  const [dateOfBirth, setDateOfBirth] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleRegister = async (e) => {
    e.preventDefault();

    if (!fullName.trim()) {
      setError('Ad-soyad zorunludur.');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const response = await patientService.create({ fullName, phoneNumber, dateOfBirth });
      // Kayıt başarılı → check-in ekranına yönlendir
      navigate(`/patient/checkin/${response.data.id}`);
    } catch (err) {
      setError(err.response?.data?.message || 'Kayıt başarısız.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="register-container">
      <div className="register-card">
        <h1>Register New Patient</h1>
        <form onSubmit={handleRegister}>
          <label>Full Name *</label>
          <input value={fullName} onChange={(e) => setFullName(e.target.value)} required />

          <label>Phone Number</label>
          <input type="tel" value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} />

          <label>Date of Birth</label>
          <input type="date" value={dateOfBirth} onChange={(e) => setDateOfBirth(e.target.value)} />

          {error && <div className="alert alert--error">{error}</div>}

          <button type="submit" className="btn btn--primary btn--full" disabled={loading}>
            {loading ? 'Registering...' : 'Register Patient'}
          </button>
        </form>
        <a href="/patient/search" className="back-link">← Back to search</a>
      </div>
    </div>
  );
}
```

### ✅ Test

| Test Türü        | Senaryo                                                     | Beklenen Sonuç                                 |
|------------------|-------------------------------------------------------------|------------------------------------------------|
| Unit             | `PatientService.CreatePatient()` — geçerli veri             | Yeni hasta kaydı oluşturulur                   |
| Unit             | `PatientService.CreatePatient()` — aynı isim+doğum tarihi  | `BusinessException` fırlatılır                 |
| Entegrasyon      | `POST /api/patients` — geçerli veri                         | 201 Created + hasta bilgisi                    |
| Entegrasyon      | `POST /api/patients` — boş fullName                         | 400 Bad Request                                |
| Frontend         | İsim girmeden "Register" butonuna bas                       | Validation hatası gösterilir                   |
| Frontend         | Geçerli bilgilerle kayıt yap                                | Check-in ekranına yönlendirilme                |

---

## Faz 7: Admin Paneli — Kullanıcı ve Sistem Yönetimi

> **Ekran Referansı:** Navbar'daki `Admin` linki — ekran görüntüsü mevcut değil ama sistemin gerektirdiği yönetim paneli

### 🎨 UI/UX Tasarımı

#### Ekran Bileşenleri
| Bileşen                  | Açıklama                                                        |
|---------------------------|-----------------------------------------------------------------|
| **Sidebar / Tab Menu**    | `Users`, `Visit Reasons`, `Queue History` sekmeleri           |
| **Users Listesi**         | Tablo: ID, Ad, Email, Rol, Durum, İşlemler                   |
| **User Edit Modal**       | Rol değiştirme, hesap aktif/pasif yapma                       |
| **Visit Reasons Listesi** | CRUD — ziyaret sebeplerini yönet                              |
| **Queue History**         | Tarih filtreli geçmiş kuyruk kayıtları tablosu                |

### 🗄️ Veri Yapısı
- Mevcut `User`, `VisitReason`, `QueueEntry` entity'leri kullanılır
- Ek DTO'lar:

```csharp
// DTOs/Admin/UserListDto.cs
public record UserListDto(int Id, string FullName, string Email, string Role, bool IsActive, DateTime CreatedAt);

// DTOs/Admin/UpdateUserRoleDto.cs
public record UpdateUserRoleDto(string Role, bool IsActive);

// DTOs/Queue/QueueHistoryFilterDto.cs
public record QueueHistoryFilterDto(DateTime? StartDate, DateTime? EndDate, string? Status);
```

### ⚙️ Kodlama Süreci

#### Backend — API Endpoint'leri

| Endpoint                          | Method | Açıklama                              | Yetki  |
|-----------------------------------|--------|---------------------------------------|--------|
| `/api/admin/users`                | GET    | Tüm kullanıcıları listele            | Admin  |
| `/api/admin/users/{id}/role`      | PUT    | Kullanıcı rolünü güncelle            | Admin  |
| `/api/admin/users/{id}/status`    | PUT    | Kullanıcı aktif/pasif yap            | Admin  |
| `/api/admin/visit-reasons`        | GET    | Ziyaret sebeplerini listele          | Admin  |
| `/api/admin/visit-reasons`        | POST   | Yeni ziyaret sebebi ekle             | Admin  |
| `/api/admin/visit-reasons/{id}`   | PUT    | Ziyaret sebebini güncelle            | Admin  |
| `/api/admin/visit-reasons/{id}`   | DELETE | Ziyaret sebebini sil (soft delete)   | Admin  |
| `/api/admin/queue-history`        | GET    | Geçmiş kuyruk kayıtlarını listele   | Admin  |

**Rol Tabanlı Yetkilendirme:**
```csharp
// Controllers/AdminController.cs
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")] // Tüm endpoint'ler sadece Admin rolü ile erişilebilir
public class AdminController : ControllerBase
{
    // ...
}
```

#### Frontend — React Durum Yönetimi

```jsx
// pages/AdminPage.jsx
function AdminPage() {
  const [activeSection, setActiveSection] = useState('users'); // 'users' | 'visitReasons' | 'history'
  const [users, setUsers] = useState([]);
  const [visitReasons, setVisitReasons] = useState([]);
  const [queueHistory, setQueueHistory] = useState([]);

  // Filtre state'leri (Queue History için)
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [statusFilter, setStatusFilter] = useState('');

  // Modal state
  const [editingUser, setEditingUser] = useState(null);   // Düzenlenen kullanıcı
  const [showModal, setShowModal] = useState(false);

  const [loading, setLoading] = useState(false);

  useEffect(() => {
    // Aktif sekmeye göre veri çek
    if (activeSection === 'users') loadUsers();
    else if (activeSection === 'visitReasons') loadVisitReasons();
    else if (activeSection === 'history') loadQueueHistory();
  }, [activeSection]);

  // ... her section için load ve CRUD fonksiyonları
}
```

### ✅ Test

| Test Türü        | Senaryo                                                      | Beklenen Sonuç                                 |
|------------------|--------------------------------------------------------------|------------------------------------------------|
| Entegrasyon      | `GET /api/admin/users` — Admin token ile                     | 200 OK + kullanıcı listesi                     |
| Entegrasyon      | `GET /api/admin/users` — Doctor token ile                    | 403 Forbidden                                  |
| Entegrasyon      | `PUT /api/admin/users/1/role` — rol güncelle                 | 200 OK + güncellenmiş kullanıcı                |
| Entegrasyon      | `POST /api/admin/visit-reasons` — yeni sebep ekle            | 201 Created                                    |
| Frontend         | Admin panelinde Users sekmesine tıkla                        | Kullanıcı tablosu görünür                      |
| Frontend         | Bir kullanıcının rolünü değiştir                             | Tablo güncellenir, eski rol değişir            |

---

## Faz 8: Kuyruk Gösterim Ekranı (Queue Display)

> **Ekran Referansı:** Navbar'daki `Queue Display` linki — bekleme salonunda büyük ekranda gösterilecek basit görünüm

### 🎨 UI/UX Tasarımı

#### Ekran Bileşenleri
| Bileşen                   | Açıklama                                                        |
|----------------------------|-----------------------------------------------------------------|
| **Büyük Kuyruk Numarası**  | Şu anda çağrılan hasta: `WALK-6` — çok büyük font             |
| **Hasta Adı**              | Çağrılan hastanın adı                                          |
| **Bekleme Listesi**        | Sırada bekleyen sonraki hastalar                               |
| **Otomatik Yenileme**      | Her 10-15 saniyede bir veriyi tazele (polling veya SignalR)     |

### 🗄️ Veri Yapısı
- Mevcut `QueueEntry` entity kullanılır
- Ek endpoint gerekli

### ⚙️ Kodlama Süreci

#### Backend — API Endpoint

| Endpoint                     | Method | Açıklama                                       | Yetki       |
|------------------------------|--------|-------------------------------------------------|-------------|
| `/api/queue/display`         | GET    | Aktif kuyruk durumu (current + waiting list)    | Public/Auth |

```csharp
// DTOs/Queue/QueueDisplayDto.cs
public record QueueDisplayDto(
    QueueDisplayItemDto? CurrentPatient,     // Şu an çağrılan hasta
    List<QueueDisplayItemDto> WaitingList    // Bekleyenler
);

public record QueueDisplayItemDto(
    string QueueNumber,
    string PatientName,
    DateTime CheckInTime
);
```

#### Frontend — React Durum Yönetimi (Otomatik Yenileme)

```jsx
// pages/QueueDisplayPage.jsx
function QueueDisplayPage() {
  const [currentPatient, setCurrentPatient] = useState(null);
  const [waitingList, setWaitingList] = useState([]);
  const [loading, setLoading] = useState(true);

  // Otomatik yenileme — her 10 saniyede bir
  useEffect(() => {
    fetchQueueDisplay(); // İlk yükleme

    const interval = setInterval(() => {
      fetchQueueDisplay();
    }, 10000); // 10 saniye

    return () => clearInterval(interval); // Cleanup
  }, []);

  const fetchQueueDisplay = async () => {
    try {
      const response = await queueService.getQueueDisplay();
      setCurrentPatient(response.data.currentPatient);
      setWaitingList(response.data.waitingList);
    } catch (err) {
      console.error('Queue display fetch failed:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="queue-display">
      <div className="queue-display__current">
        <h2>Now Serving</h2>
        {currentPatient ? (
          <>
            <span className="queue-display__number">{currentPatient.queueNumber}</span>
            <span className="queue-display__name">{currentPatient.patientName}</span>
          </>
        ) : (
          <span>No patient currently being served</span>
        )}
      </div>

      <div className="queue-display__waiting">
        <h2>Up Next</h2>
        {waitingList.map((patient, index) => (
          <div key={patient.queueNumber} className="queue-display__item">
            <span>{index + 1}.</span>
            <span>{patient.queueNumber}</span>
            <span>{patient.patientName}</span>
          </div>
        ))}
      </div>
    </div>
  );
}
```

### ✅ Test

| Test Türü        | Senaryo                                                        | Beklenen Sonuç                                 |
|------------------|----------------------------------------------------------------|------------------------------------------------|
| Entegrasyon      | `GET /api/queue/display` — InProgress hasta varken             | currentPatient dolu döner                      |
| Entegrasyon      | `GET /api/queue/display` — InProgress hasta yokken             | currentPatient null, waitingList dolu/boş       |
| Frontend         | Sayfa yüklendiğinde kuyruk durumu görünür mü?                  | Evet — "Now Serving" ve "Up Next" bölümleri    |
| Frontend         | 10 saniye sonra otomatik güncelleme çalışıyor mu?              | Evet — veri tazelenir                          |
| Frontend         | Doktor dashboard'dan "Call Next" yapıldığında display güncelleniyor mu? | Evet — bir sonraki polling'de güncellenir |

---

## 📊 Genel Veritabanı Şeması (ER Diyagramı Özeti)

```
┌──────────────┐       ┌──────────────────┐       ┌──────────────┐
│    Users     │       │   QueueEntries   │       │   Patients   │
├──────────────┤       ├──────────────────┤       ├──────────────┤
│ Id (PK)      │       │ Id (PK)          │       │ Id (PK)      │
│ FullName     │       │ PatientId (FK)───┼──────►│ FullName     │
│ Email (UQ)   │       │ QueueNumber      │       │ PhoneNumber  │
│ PasswordHash │       │ Status           │       │ DateOfBirth  │
│ Role         │       │ CheckInType      │       │ CreatedAt    │
│ CreatedAt    │       │ VisitReason      │       └──────────────┘
│ IsActive     │       │ AdditionalInfo   │              │
└──────────────┘       │ CheckInTime      │              │
                       │ CalledAt         │       ┌──────┴───────┐
┌──────────────┐       │ CompletedAt      │       │ Appointments │
│ VisitReasons │       │ QueueDate        │       ├──────────────┤
├──────────────┤       └──────────────────┘       │ Id (PK)      │
│ Id (PK)      │                                  │ PatientId(FK)│
│ Name         │                                  │ AppointDate  │
│ IsActive     │                                  │ Reason       │
└──────────────┘                                  │ Status       │
                                                  │ Notes        │
                                                  │ CreatedAt    │
                                                  └──────────────┘
```

---

## 🗺️ Önerilen İlerleme Sırası

| Sıra | Faz   | Açıklama                        | Tahmini Süre  | Bağımlılık         |
|------|-------|----------------------------------|---------------|---------------------|
| 1    | Faz 0 | Proje İskeleti & Altyapı        | 1-2 gün       | —                   |
| 2    | Faz 1 | Login & Register                 | 2-3 gün       | Faz 0               |
| 3    | Faz 6 | Yeni Hasta Kayıt                 | 1-2 gün       | Faz 1               |
| 4    | Faz 3 | Hasta Karşılama (Welcome)        | 0.5-1 gün     | Faz 1               |
| 5    | Faz 4 | Hasta Arama                      | 2-3 gün       | Faz 6               |
| 6    | Faz 5 | Check-In                         | 2-3 gün       | Faz 4               |
| 7    | Faz 2 | Dashboard (Kanban Kuyruk)        | 3-4 gün       | Faz 5               |
| 8    | Faz 8 | Queue Display                    | 1-2 gün       | Faz 2               |
| 9    | Faz 7 | Admin Panel                      | 2-3 gün       | Faz 2               |
|      |       | **Toplam**                       | **~15-23 gün**|                     |

---

## 💡 Ekstra İpuçları

### Geliştirme Sırası Prensibi
> **Backend-first yaklaşımı** uygula: Önce entity → sonra DTO → sonra service → sonra controller → en son React tarafı. Bu şekilde Swagger üzerinden API'yi test edebilir, frontend'e geçmeden emin olabilirsin.

### Her Faz Sonunda Yapılacaklar
1. ✅ Swagger üzerinden tüm endpoint'leri test et
2. ✅ Frontend'de tüm akışları elle dene
3. ✅ Git commit at: `feat(faz-X): kısa açıklama`
4. ✅ Bir sonraki faza geçmeden önce README.md'ye ilerleme notu ekle

### Öğrenme Odak Noktaları
| Faz   | Öğreneceğin Ana Konu                                  |
|-------|--------------------------------------------------------|
| Faz 0 | Proje yapılandırma, EF Core, MySQL bağlantısı         |
| Faz 1 | JWT Authentication, BCrypt, React Context              |
| Faz 2 | Kanban UI, filtreleme, durum makinesi (state machine)  |
| Faz 3 | React Router, navigasyon, basit UI                     |
| Faz 4 | API sorguları, arama mantığı, koşullu render           |
| Faz 5 | Form yönetimi, validation, iş mantığı kuralları        |
| Faz 6 | CRUD işlemleri, form state'leri                        |
| Faz 7 | Rol tabanlı yetkilendirme, CRUD, tablo bileşenleri    |
| Faz 8 | Polling/auto-refresh, büyük ekran UI                   |

---

> **🚀 Haydi başlayalım!** Faz 0 ile proje iskeletini kurup, Faz 1'de ilk ekranını kodlamaya başlayabilirsin.
> Herhangi bir fazda takıldığın yerde bana sorabilirsin — adım adım birlikte ilerleriz.
