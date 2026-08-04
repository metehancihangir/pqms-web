# âœ… Faz 1 â€” Kimlik DoÄŸrulama (Login & Register): GÃ¶rev Listesi

> **Referans:** [phases.md â€” Faz 1](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L151-L406)
>
> **Ã–n KoÅŸul:** [Faz 0 gÃ¶revleri](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/tasks-faz0.md) tamamlanmÄ±ÅŸ olmalÄ±dÄ±r.
>
> **Ekran ReferansÄ±:** Login/Register sekmeli form

---

## 1. User Entity Modeli OluÅŸturma

> ğŸ“ Referans: [phases.md â€” User Entity](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L187-L209)

- [x] **1.1.** `Models/User.cs` dosyasÄ±nÄ± oluÅŸtur
- [x] **1.2.** AÅŸaÄŸÄ±daki property'leri tanÄ±mla:
  - [x] **1.2.1.** `int Id` â€” Primary Key
  - [x] **1.2.2.** `string FullName` â€” `[Required, MaxLength(100)]`
  - [x] **1.2.3.** `string Email` â€” `[Required, MaxLength(150)]`
  - [x] **1.2.4.** `string PasswordHash` â€” `[Required]`
  - [x] **1.2.5.** `string Role` â€” `[Required, MaxLength(20)]` â€” varsayÄ±lan deÄŸer: `"Patient"`
  - [x] **1.2.6.** `DateTime CreatedAt` â€” varsayÄ±lan deÄŸer: `DateTime.UtcNow`
  - [x] **1.2.7.** `bool IsActive` â€” varsayÄ±lan deÄŸer: `true`

---

## 2. User Fluent API KonfigÃ¼rasyonu

> ğŸ“ Referans: [phases.md â€” Fluent API KonfigÃ¼rasyonu](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L211-L234)

- [x] **2.1.** `Data/Configurations/UserConfiguration.cs` dosyasÄ±nÄ± oluÅŸtur
- [x] **2.2.** `IEntityTypeConfiguration<User>` interface'ini implemente et
- [x] **2.3.** Tablo adÄ±nÄ± `"Users"` olarak ayarla: `builder.ToTable("Users")`
- [x] **2.4.** `Email` alanÄ±na unique index ekle: `builder.HasIndex(u => u.Email).IsUnique()`
- [x] **2.5.** `FullName` alanÄ± iÃ§in `HasMaxLength(100)` ve `IsRequired()` konfigÃ¼rasyonu yap
- [x] **2.6.** `Email` alanÄ± iÃ§in `HasMaxLength(150)` ve `IsRequired()` konfigÃ¼rasyonu yap
- [x] **2.7.** `Role` alanÄ± iÃ§in `HasMaxLength(20)` ve `HasDefaultValue("Patient")` konfigÃ¼rasyonu yap
- [x] **2.8.** `PqmsDbContext.OnModelCreating` iÃ§inde bu konfigÃ¼rasyonun uygulandÄ±ÄŸÄ±ndan emin ol

---

## 3. Auth DTO'larÄ± OluÅŸturma

> ğŸ“ Referans: [phases.md â€” DTO'lar](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L237-L247)

- [x] **3.1.** `DTOs/Auth/` klasÃ¶rÃ¼nÃ¼ oluÅŸtur
- [x] **3.2.** `LoginRequestDto.cs` dosyasÄ±nÄ± oluÅŸtur â€” `record LoginRequestDto(string Email, string Password)`
- [x] **3.3.** `RegisterRequestDto.cs` dosyasÄ±nÄ± oluÅŸtur â€” `record RegisterRequestDto(string FullName, string Email, string Password, string Role)`
- [x] **3.4.** `AuthResponseDto.cs` dosyasÄ±nÄ± oluÅŸtur â€” `record AuthResponseDto(string Token, string FullName, string Role)`

---

## 4. JWT YapÄ±landÄ±rmasÄ±

> ğŸ“ Referans: [phases.md â€” JWT Token Ãœretimi](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L265-L293)

- [x] **4.1.** `appsettings.json` dosyasÄ±na JWT ayarlarÄ±nÄ± ekle:
  ```json
  "Jwt": {
    "Secret": "YOUR_SUPER_SECRET_KEY_AT_LEAST_32_CHARS_LONG!",
    "Issuer": "PQMS",
    "Audience": "PQMS-Client",
    "ExpirationInHours": 24
  }
  ```
- [x] **4.2.** `Program.cs` dosyasÄ±nda JWT Authentication servisini kaydet:
  - [x] **4.2.1.** `AddAuthentication(JwtBearerDefaults.AuthenticationScheme)` ekle
  - [x] **4.2.2.** `.AddJwtBearer()` iÃ§inde `TokenValidationParameters` konfigÃ¼re et:
    - [x] **4.2.2.1.** `ValidateIssuer = true` ve `ValidIssuer` ayarla
    - [x] **4.2.2.2.** `ValidateAudience = true` ve `ValidAudience` ayarla
    - [x] **4.2.2.3.** `ValidateIssuerSigningKey = true` ve `IssuerSigningKey` ayarla
    - [x] **4.2.2.4.** `ValidateLifetime = true` ayarla
- [x] **4.3.** `Program.cs` pipeline'Ä±na `app.UseAuthentication()` ve `app.UseAuthorization()` middleware'lerini doÄŸru sÄ±rada ekle

---

## 5. AuthService â€” Ä°ÅŸ MantÄ±ÄŸÄ± KatmanÄ±

> ğŸ“ Referans: [phases.md â€” JWT Token Ãœretimi & Login AkÄ±ÅŸÄ±](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L258-L293)

- [x] **5.1.** `Services/IAuthService.cs` interface dosyasÄ±nÄ± oluÅŸtur â€” metot imzalarÄ±:
  - [x] **5.1.1.** `Task<AuthResponseDto> Login(LoginRequestDto request)`
  - [x] **5.1.2.** `Task<AuthResponseDto> Register(RegisterRequestDto request)`
  - [x] **5.1.3.** `string GenerateJwtToken(User user)`
- [x] **5.2.** `Services/AuthService.cs` sÄ±nÄ±fÄ±nÄ± oluÅŸtur ve `IAuthService` interface'ini implemente et
- [x] **5.3.** Constructor'da `PqmsDbContext` ve `IConfiguration` dependency injection ile al
- [x] **5.4.** `GenerateJwtToken(User user)` metodunu implement et:
  - [x] **5.4.1.** Claim'leri tanÄ±mla: `NameIdentifier`, `Email`, `Name`, `Role`
  - [x] **5.4.2.** `SymmetricSecurityKey` oluÅŸtur (appsettings'den `Jwt:Secret` ile)
  - [x] **5.4.3.** `SigningCredentials` oluÅŸtur (`HmacSha256` algoritmasÄ±)
  - [x] **5.4.4.** `JwtSecurityToken` oluÅŸtur (issuer, audience, claims, expiration, credentials)
  - [x] **5.4.5.** `JwtSecurityTokenHandler().WriteToken()` ile token string'e Ã§evir ve dÃ¶ndÃ¼r
- [x] **5.5.** `Register(RegisterRequestDto request)` metodunu implement et:
  - [x] **5.5.1.** AynÄ± email ile kayÄ±tlÄ± kullanÄ±cÄ± var mÄ± kontrol et â€” varsa hata fÄ±rlat
  - [x] **5.5.2.** Åifreyi `BCrypt.HashPassword()` ile hashle
  - [x] **5.5.3.** Yeni `User` entity'si oluÅŸtur ve veritabanÄ±na kaydet
  - [x] **5.5.4.** JWT token Ã¼ret ve `AuthResponseDto` olarak dÃ¶ndÃ¼r
- [x] **5.6.** `Login(LoginRequestDto request)` metodunu implement et:
  - [x] **5.6.1.** Email ile kullanÄ±cÄ±yÄ± veritabanÄ±ndan bul â€” bulunamazsa hata dÃ¶ndÃ¼r
  - [x] **5.6.2.** `BCrypt.Verify()` ile ÅŸifreyi doÄŸrula â€” eÅŸleÅŸmezse 401 hatasÄ±
  - [x] **5.6.3.** `IsActive` kontrolÃ¼ yap â€” pasif hesap giriÅŸi engelle
  - [x] **5.6.4.** JWT token Ã¼ret ve `AuthResponseDto` olarak dÃ¶ndÃ¼r
- [x] **5.7.** `Program.cs` dosyasÄ±nda `IAuthService` â†’ `AuthService` DI kaydÄ±nÄ± ekle:
  ```csharp
  builder.Services.AddScoped<IAuthService, AuthService>();
  ```

---

## 6. AuthController â€” API Endpoint'leri

> ğŸ“ Referans: [phases.md â€” Backend API Endpoint AkÄ±ÅŸÄ±](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L251-L263)

- [x] **6.1.** `Controllers/AuthController.cs` dosyasÄ±nÄ± oluÅŸtur
- [x] **6.2.** `[ApiController]` ve `[Route("api/auth")]` attribute'lerini ekle
- [x] **6.3.** Constructor'da `IAuthService` dependency injection ile al
- [x] **6.4.** `POST /api/auth/login` endpoint'ini oluÅŸtur:
  - [x] **6.4.1.** `[HttpPost("login")]` attribute'u ekle
  - [x] **6.4.2.** `LoginRequestDto` parametresini `[FromBody]` ile al
  - [x] **6.4.3.** `AuthService.Login()` Ã§aÄŸÄ±r
  - [x] **6.4.4.** BaÅŸarÄ±lÄ±: `Ok(AuthResponseDto)` dÃ¶ndÃ¼r
  - [x] **6.4.5.** BaÅŸarÄ±sÄ±z: `Unauthorized()` dÃ¶ndÃ¼r (try-catch ile)
- [x] **6.5.** `POST /api/auth/register` endpoint'ini oluÅŸtur:
  - [x] **6.5.1.** `[HttpPost("register")]` attribute'u ekle
  - [x] **6.5.2.** `RegisterRequestDto` parametresini `[FromBody]` ile al
  - [x] **6.5.3.** `AuthService.Register()` Ã§aÄŸÄ±r
  - [x] **6.5.4.** BaÅŸarÄ±lÄ±: `Ok(AuthResponseDto)` dÃ¶ndÃ¼r
  - [x] **6.5.5.** Email zaten kayÄ±tlÄ±: `Conflict()` veya `BadRequest()` dÃ¶ndÃ¼r

---

## 7. Migration GÃ¼ncelleme (User Tablosu)

- [x] **7.1.** `dotnet ef migrations add AddUserEntity` komutu ile yeni migration oluÅŸtur
- [x] **7.2.** `dotnet ef database update` komutu ile veritabanÄ±nÄ± gÃ¼ncelle
- [x] **7.3.** MySQL'de `Users` tablosunun oluÅŸturulduÄŸunu doÄŸrula
- [x] **7.4.** `Email` alanÄ±nÄ±n unique index'e sahip olduÄŸunu doÄŸrula

---

## 8. Backend API Testleri (Swagger Ãœzerinden)

> ğŸ“ Referans: [phases.md â€” Test tablosu, Entegrasyon satÄ±rlarÄ±](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L400-L402)

- [x] **8.1.** Backend'i Ã§alÄ±ÅŸtÄ±r ve Swagger UI'Ä± aÃ§
- [x] **8.2.** âœ… **Register testi:** `POST /api/auth/register` â€” geÃ§erli veri gÃ¶nder â†’ `200 OK` + token dÃ¶ndÃ¼ÄŸÃ¼nÃ¼ doÄŸrula
- [x] **8.3.** âœ… **Duplicate Register testi:** AynÄ± email ile ikinci kez register â†’ hata dÃ¶ndÃ¼ÄŸÃ¼nÃ¼ doÄŸrula
- [x] **8.4.** âœ… **Login testi:** `POST /api/auth/login` â€” doÄŸru bilgilerle â†’ `200 OK` + token dÃ¶ndÃ¼ÄŸÃ¼nÃ¼ doÄŸrula
- [x] **8.5.** âœ… **Login yanlÄ±ÅŸ ÅŸifre testi:** YanlÄ±ÅŸ ÅŸifre ile â†’ `401 Unauthorized` dÃ¶ndÃ¼ÄŸÃ¼nÃ¼ doÄŸrula
- [x] **8.6.** âœ… **JWT token doÄŸrulama:** DÃ¶nen token'Ä± [jwt.io](https://jwt.io) adresinde decode et â†’ claim'lerin doÄŸru olduÄŸunu kontrol et

---

## 9. Frontend â€” AuthPage Sayfa Ä°skeleti ve TasarÄ±mÄ±

> ğŸ“ Referans: [phases.md â€” UI/UX TasarÄ±mÄ±, Ekran BileÅŸenleri](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L155-L183)

- [x] **9.1.** `src/pages/AuthPage.jsx` dosyasÄ±nÄ± oluÅŸtur
- [x] **9.2.** `src/styles/AuthPage.css` dosyasÄ±nÄ± oluÅŸtur
- [x] **9.3.** Sayfa genel dÃ¼zenini oluÅŸtur:
  - [x] **9.3.1.** Sayfa arkaplanÄ±: aÃ§Ä±k gri (`#f5f5f5`)
  - [x] **9.3.2.** OrtalanmÄ±ÅŸ beyaz kart (`max-width`, `box-shadow`, `border-radius`)
- [x] **9.4.** **Tab Bar bileÅŸenini** oluÅŸtur:
  - [x] **9.4.1.** `Login` ve `Register` sekmeleri
  - [x] **9.4.2.** Aktif sekmenin altÄ±nda mavi Ã§izgi efekti
  - [x] **9.4.3.** Sekmeye tÄ±klandÄ±ÄŸÄ±nda `activeTab` state'ini gÃ¼ncelle
- [x] **9.5.** **Login Formu** bileÅŸenlerini oluÅŸtur:
  - [x] **9.5.1.** `Email Address` etiketli text input
  - [x] **9.5.2.** `Password` etiketli password input
  - [x] **9.5.3.** `Remember me` checkbox'Ä±
  - [x] **9.5.4.** `Forgot password?` linki (saÄŸ hizalÄ±, mavi renk)
  - [x] **9.5.5.** Tam geniÅŸlikte mavi `Login` butonu (ikon + yazÄ±)
  - [x] **9.5.6.** `â€” or â€”` ayÄ±rÄ±cÄ± Ã§izgi
  - [x] **9.5.7.** `Create new account` outlined butonu â€” tÄ±klanÄ±nca Register sekmesine geÃ§
- [x] **9.6.** **Register Formu** bileÅŸenlerini oluÅŸtur:
  - [x] **9.6.1.** `Full Name` text input
  - [x] **9.6.2.** `Email Address` text input
  - [x] **9.6.3.** `Password` password input (min. 6 karakter)
  - [x] **9.6.4.** `Confirm Password` password input
  - [x] **9.6.5.** `Role` dropdown â€” `Patient` / `Doctor` / `Admin` seÃ§enekleri
  - [x] **9.6.6.** Tam geniÅŸlikte mavi `Register` butonu
- [x] **9.7.** **TasarÄ±m detaylarÄ±:**
  - [x] **9.7.1.** Buton rengi: `#4361ee` (Royal Blue)
  - [x] **9.7.2.** Font: Google Fonts'tan "Inter" veya system font stack
  - [x] **9.7.3.** Hata mesajlarÄ± iÃ§in kÄ±rmÄ±zÄ± alert kutusu stili
  - [x] **9.7.4.** Loading state'inde buton disabled ve "Processing..." yazÄ±sÄ±

> ğŸ“ Referans: [phases.md â€” TasarÄ±m NotlarÄ±](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L179-L183)

---

## 10. Frontend â€” React State YÃ¶netimi (useState)

> ğŸ“ Referans: [phases.md â€” Frontend React Durum YÃ¶netimi](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L296-L337)

- [x] **10.1.** `AuthPage` bileÅŸeninde aÅŸaÄŸÄ±daki state'leri tanÄ±mla:
  - [x] **10.1.1.** `const [activeTab, setActiveTab] = useState('login')` â€” aktif sekme
  - [x] **10.1.2.** `const [loginEmail, setLoginEmail] = useState('')` â€” login email
  - [x] **10.1.3.** `const [loginPassword, setLoginPassword] = useState('')` â€” login ÅŸifre
  - [x] **10.1.4.** `const [rememberMe, setRememberMe] = useState(false)` â€” beni hatÄ±rla
  - [x] **10.1.5.** `const [registerName, setRegisterName] = useState('')` â€” register ad
  - [x] **10.1.6.** `const [registerEmail, setRegisterEmail] = useState('')` â€” register email
  - [x] **10.1.7.** `const [registerPassword, setRegisterPassword] = useState('')` â€” register ÅŸifre
  - [x] **10.1.8.** `const [registerConfirm, setRegisterConfirm] = useState('')` â€” ÅŸifre tekrarÄ±
  - [x] **10.1.9.** `const [registerRole, setRegisterRole] = useState('Patient')` â€” register rol
  - [x] **10.1.10.** `const [loading, setLoading] = useState(false)` â€” yÃ¼kleniyor durumu
  - [x] **10.1.11.** `const [error, setError] = useState('')` â€” hata mesajÄ±
- [x] **10.2.** Her input'u ilgili `onChange` handler'Ä± ile state'e baÄŸla (`controlled components`)
- [x] **10.3.** Sekme deÄŸiÅŸtiÄŸinde hata mesajÄ±nÄ± temizle

---

## 11. Frontend â€” Auth API Servis DosyasÄ±

> ğŸ“ Referans: [phases.md â€” Kodlama SÃ¼reci (Frontend)](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L296-L337)

- [x] **11.1.** `src/services/authService.js` dosyasÄ±nÄ± oluÅŸtur
- [x] **11.2.** `login(email, password)` fonksiyonunu yaz â€” `POST /api/auth/login` Ã§aÄŸrÄ±sÄ±
- [x] **11.3.** `register(fullName, email, password, role)` fonksiyonunu yaz â€” `POST /api/auth/register` Ã§aÄŸrÄ±sÄ±
- [x] **11.4.** Faz 0'da oluÅŸturulan `api.js` instance'Ä±nÄ± import et ve kullan

---

## 12. Frontend â€” handleLogin ve handleRegister FonksiyonlarÄ±

> ğŸ“ Referans: [phases.md â€” handleLogin akÄ±ÅŸÄ±](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L320-L337)

- [x] **12.1.** `handleLogin` async fonksiyonunu implement et:
  - [x] **12.1.1.** `e.preventDefault()` ile form submit'ini engelle
  - [x] **12.1.2.** `setLoading(true)` ve `setError('')` ile state'leri sÄ±fÄ±rla
  - [x] **12.1.3.** `authService.login()` Ã§aÄŸÄ±r
  - [x] **12.1.4.** BaÅŸarÄ±lÄ±: `localStorage.setItem('token', ...)` ile token'Ä± kaydet
  - [x] **12.1.5.** BaÅŸarÄ±lÄ±: AuthContext'in `login()` metodunu Ã§aÄŸÄ±r
  - [x] **12.1.6.** BaÅŸarÄ±lÄ±: `navigate('/dashboard')` ile yÃ¶nlendir
  - [x] **12.1.7.** Hata: `setError('GeÃ§ersiz e-posta veya ÅŸifre.')` ile hata mesajÄ± gÃ¶ster
  - [x] **12.1.8.** `finally` bloÄŸunda `setLoading(false)` Ã§aÄŸÄ±r
- [x] **12.2.** `handleRegister` async fonksiyonunu implement et:
  - [x] **12.2.1.** `e.preventDefault()` ile form submit'ini engelle
  - [x] **12.2.2.** Client-side validation: ÅŸifre ve onay eÅŸleÅŸiyor mu kontrol et
  - [x] **12.2.3.** Client-side validation: ÅŸifre min. 6 karakter mi kontrol et
  - [x] **12.2.4.** `authService.register()` Ã§aÄŸÄ±r
  - [x] **12.2.5.** BaÅŸarÄ±lÄ±: token'Ä± kaydet ve AuthContext'i gÃ¼ncelle
  - [x] **12.2.6.** BaÅŸarÄ±lÄ±: Dashboard'a yÃ¶nlendir
  - [x] **12.2.7.** Hata: uygun hata mesajÄ±nÄ± gÃ¶ster (email zaten kayÄ±tlÄ± vs.)

---

## 13. Frontend â€” AuthContext (Global Durum YÃ¶netimi)

> ğŸ“ Referans: [phases.md â€” AuthContext ile Global Durum YÃ¶netimi](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L340-L375)

- [x] **13.1.** `src/context/AuthContext.jsx` dosyasÄ±nÄ± oluÅŸtur
- [x] **13.2.** `createContext()` ile `AuthContext` oluÅŸtur
- [x] **13.3.** `AuthProvider` bileÅŸenini oluÅŸtur â€” aÅŸaÄŸÄ±daki state'ler:
  - [x] **13.3.1.** `const [user, setUser] = useState(null)` â€” `{ fullName, role, token }`
  - [x] **13.3.2.** `const [isLoading, setIsLoading] = useState(true)` â€” ilk yÃ¼kleme kontrolÃ¼
- [x] **13.4.** `useEffect` iÃ§inde sayfa yÃ¼klendiÄŸinde `localStorage`'dan token kontrol et:
  - [x] **13.4.1.** Token varsa: decode et (`parseJwt` helper fonksiyonu ile)
  - [x] **13.4.2.** Decode edilen bilgilerle `setUser()` Ã§aÄŸÄ±r
  - [x] **13.4.3.** `setIsLoading(false)` Ã§aÄŸÄ±r
- [x] **13.5.** `login(userData)` fonksiyonunu tanÄ±mla â€” `setUser` + `localStorage.setItem`
- [x] **13.6.** `logout()` fonksiyonunu tanÄ±mla â€” `setUser(null)` + `localStorage.removeItem`
- [x] **13.7.** `AuthContext.Provider` ile `{ user, isLoading, login, logout }` deÄŸerlerini children'a saÄŸla
- [x] **13.8.** `src/hooks/useAuth.js` dosyasÄ±nÄ± oluÅŸtur â€” `useContext(AuthContext)` wrapper custom hook'u
- [x] **13.9.** `src/utils/parseJwt.js` dosyasÄ±nÄ± oluÅŸtur â€” JWT token'Ä± decode eden yardÄ±mcÄ± fonksiyon

---

## 14. Frontend â€” ProtectedRoute BileÅŸeni

> ğŸ“ Referans: [phases.md â€” Protected Route BileÅŸeni](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L378-L391)

- [x] **14.1.** `src/components/ProtectedRoute.jsx` dosyasÄ±nÄ± oluÅŸtur
- [x] **14.2.** `useAuth()` hook'u ile `user` ve `isLoading` deÄŸerlerini al
- [x] **14.3.** `isLoading` true iken `LoadingSpinner` bileÅŸenini gÃ¶ster
- [x] **14.4.** `user` null ise `<Navigate to="/login" />` ile login sayfasÄ±na yÃ¶nlendir
- [x] **14.5.** `allowedRoles` prop'u varsa ve kullanÄ±cÄ±nÄ±n rolÃ¼ listede yoksa `<Navigate to="/unauthorized" />` yÃ¶nlendir
- [x] **14.6.** TÃ¼m kontroller geÃ§erse `{children}` render et

---

## 15. Frontend â€” React Router KonfigÃ¼rasyonu

- [x] **15.1.** `src/App.jsx` dosyasÄ±nÄ± gÃ¼ncelle â€” `BrowserRouter` ve `Routes` yapÄ±sÄ±nÄ± kur
- [x] **15.2.** `AuthProvider` ile tÃ¼m route'larÄ± sar
- [x] **15.3.** AÅŸaÄŸÄ±daki route'larÄ± tanÄ±mla:
  - [x] **15.3.1.** `/login` â†’ `<AuthPage />`
  - [x] **15.3.2.** `/dashboard` â†’ `<ProtectedRoute allowedRoles={['Doctor','Admin']}><DashboardPage /></ProtectedRoute>` (placeholder)
  - [x] **15.3.3.** `/` â†’ Login sayfasÄ±na yÃ¶nlendir (`<Navigate to="/login" />`)
- [x] **15.4.** HenÃ¼z oluÅŸturulmamÄ±ÅŸ sayfalar iÃ§in geÃ§ici placeholder bileÅŸenler oluÅŸtur

---

## 16. Frontend â€” Form Validation

> ğŸ“ Referans: [phases.md â€” Frontend Test, Validation satÄ±rlarÄ±](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L403-L405)

- [x] **16.1.** Login formu validasyonlarÄ±:
  - [x] **16.1.1.** Email boÅŸsa "E-posta adresi gereklidir" hatasÄ± gÃ¶ster
  - [x] **16.1.2.** Email formatÄ± geÃ§ersizse uyarÄ± gÃ¶ster
  - [x] **16.1.3.** Åifre boÅŸsa "Åifre gereklidir" hatasÄ± gÃ¶ster
- [x] **16.2.** Register formu validasyonlarÄ±:
  - [x] **16.2.1.** Ad boÅŸsa "Ad-soyad gereklidir" hatasÄ± gÃ¶ster
  - [x] **16.2.2.** Email boÅŸsa veya geÃ§ersiz formatsa uyarÄ± gÃ¶ster
  - [x] **16.2.3.** Åifre 6 karakterden kÄ±saysa uyarÄ± gÃ¶ster
  - [x] **16.2.4.** Åifre ve onay eÅŸleÅŸmiyorsa "Åifreler eÅŸleÅŸmiyor" hatasÄ± gÃ¶ster
  - [x] **16.2.5.** Rol seÃ§ilmediyse uyarÄ± gÃ¶ster
- [x] **16.3.** Hata mesajlarÄ± iÃ§in kÄ±rmÄ±zÄ± renkli alert bileÅŸeni kullan

---

## 17. Frontend DoÄŸrulama Testleri

> ğŸ“ Referans: [phases.md â€” Test tablosu (Frontend satÄ±rlarÄ±)](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L393-L405)

- [x] **17.1.** âœ… **Sekme geÃ§iÅŸi:** Login tab'a tÄ±kla â†’ Login formu gÃ¶rÃ¼nÃ¼r; Register tab'a tÄ±kla â†’ Register formu gÃ¶rÃ¼nÃ¼r
- [x] **17.2.** âœ… **BaÅŸarÄ±lÄ± Login:** DoÄŸru bilgilerle giriÅŸ yap â†’ Dashboard sayfasÄ±na yÃ¶nlendirildiÄŸini doÄŸrula
- [x] **17.3.** âœ… **BaÅŸarÄ±sÄ±z Login:** YanlÄ±ÅŸ ÅŸifreyle giriÅŸ yap â†’ Hata mesajÄ±nÄ±n ekranda gÃ¶sterildiÄŸini doÄŸrula
- [x] **17.4.** âœ… **BoÅŸ form Login:** HiÃ§bir alan doldurmadan Login butonuna bas â†’ Validation hatalarÄ±nÄ±n gÃ¶sterildiÄŸini doÄŸrula
- [x] **17.5.** âœ… **BaÅŸarÄ±lÄ± Register:** GeÃ§erli bilgilerle kayÄ±t ol â†’ Token dÃ¶ndÃ¼ÄŸÃ¼nÃ¼ ve yÃ¶nlendirme yapÄ±ldÄ±ÄŸÄ±nÄ± doÄŸrula
- [x] **17.6.** âœ… **Åifre uyumsuzluÄŸu:** Register formda farklÄ± ÅŸifreler gir â†’ "Åifreler eÅŸleÅŸmiyor" hatasÄ±nÄ± doÄŸrula
- [x] **17.7.** âœ… **Duplicate Email:** Zaten kayÄ±tlÄ± bir email ile register dene â†’ Uygun hata mesajÄ±nÄ± doÄŸrula
- [x] **17.8.** âœ… **Token kontrolÃ¼:** BaÅŸarÄ±lÄ± giriÅŸ sonrasÄ± `localStorage`'da `token` key'inin oluÅŸtuÄŸunu doÄŸrula
- [x] **17.9.** âœ… **KorumalÄ± route:** Token olmadan `/dashboard`'a gitmeyi dene â†’ Login'e yÃ¶nlendirildiÄŸini doÄŸrula
- [x] **17.10.** âœ… **Sayfa yenileme:** Login sonrasÄ± sayfayÄ± yenile â†’ KullanÄ±cÄ±nÄ±n hÃ¢lÃ¢ giriÅŸ yapmÄ±ÅŸ durumda olduÄŸunu doÄŸrula (AuthContext token kontrolÃ¼)

---

## 18. Backend â€” Unit Test'ler

> ğŸ“ Referans: [phases.md â€” Test tablosu, Unit satÄ±rlarÄ±](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L397-L399)

- [x] **18.1.** Test projesi oluÅŸtur: `dotnet new xunit -n PQMS.Tests` (opsiyonel ama Ã¶nerilir)
- [x] **18.2.** âœ… **GenerateJwtToken testi:** GeÃ§erli user ile Ã§aÄŸÄ±r â†’ geÃ§erli JWT string dÃ¶ndÃ¼ÄŸÃ¼nÃ¼ doÄŸrula
- [x] **18.3.** âœ… **Register duplicate testi:** AynÄ± email ile iki kez `Register()` Ã§aÄŸÄ±r â†’ ikincisinde hata fÄ±rlatÄ±ldÄ±ÄŸÄ±nÄ± doÄŸrula
- [x] **18.4.** âœ… **Login yanlÄ±ÅŸ ÅŸifre testi:** YanlÄ±ÅŸ ÅŸifre ile `Login()` Ã§aÄŸÄ±r â†’ null veya exception dÃ¶ndÃ¼ÄŸÃ¼nÃ¼ doÄŸrula
- [x] **18.5.** âœ… **Åifre hash testi:** Register sonrasÄ± veritabanÄ±ndaki `PasswordHash` alanÄ±nÄ±n dÃ¼z metin olmadÄ±ÄŸÄ±nÄ± doÄŸrula

---

## ğŸ“‹ Faz 1 Ä°lerleme Ã–zeti

| #  | GÃ¶rev Grubu                           | Durum |
|----|---------------------------------------|-------|
| 1  | User Entity Modeli                   | ✅    |
| 2  | Fluent API KonfigÃ¼rasyonu            | ✅    |
| 3  | Auth DTO'larÄ±                        | ✅    |
| 4  | JWT YapÄ±landÄ±rmasÄ±                   | ✅    |
| 5  | AuthService (Ä°ÅŸ MantÄ±ÄŸÄ±)            | ✅    |
| 6  | AuthController (API)                 | ✅    |
| 7  | Migration GÃ¼ncelleme                 | ✅    |
| 8  | Backend API Testleri (Swagger)       | ✅    |
| 9  | AuthPage TasarÄ±mÄ± (UI/UX)           | ✅    |
| 10 | React State YÃ¶netimi                 | ✅    |
| 11 | Auth API Servis DosyasÄ±              | ✅    |
| 12 | Login/Register FonksiyonlarÄ±         | ✅    |
| 13 | AuthContext (Global State)           | ✅    |
| 14 | ProtectedRoute BileÅŸeni             | ✅    |
| 15 | React Router KonfigÃ¼rasyonu          | ✅    |
| 16 | Form Validation                      | ✅    |
| 17 | Frontend DoÄŸrulama Testleri          | ✅    |
| 18 | Backend Unit Test'ler                | ✅    |

> âœ… TÃ¼m maddeler tamamlandÄ±ÄŸÄ±nda Faz 1 bitmiÅŸtir. Faz 2 gÃ¶revleri iÃ§in yeni bir task dosyasÄ± oluÅŸturulacaktÄ±r.
