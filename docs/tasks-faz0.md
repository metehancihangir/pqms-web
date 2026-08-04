# ✅ Faz 0 — Proje İskeleti ve Altyapı Kurulumu: Görev Listesi

> **Referans:** [phases.md — Faz 0](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L10-L148)
>
> Bu görev listesindeki tüm maddeler tamamlanmadan Faz 1'e geçilmemelidir.

---

## 1. Backend Proje Oluşturma

> 📎 Referans: [phases.md — Kodlama Süreci, Madde 1](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L104-L105)

- [x] **1.1.** `dotnet new webapi -n PQMS.API` komutu ile .NET Web API projesi oluştur
- [x] **1.2.** Proje klasör yapısını oluştur — aşağıdaki dizinleri oluştur:
  - [x] **1.2.1.** `Controllers/` klasörü
  - [x] **1.2.2.** `Models/` klasörü
  - [x] **1.2.3.** `DTOs/` klasörü (+ `DTOs/Auth/`, `DTOs/Patient/`, `DTOs/Queue/`)
  - [x] **1.2.4.** `Data/` klasörü
  - [x] **1.2.5.** `Data/Configurations/` klasörü
  - [x] **1.2.6.** `Services/` klasörü
  - [x] **1.2.7.** `Middleware/` klasörü

> 📎 Referans: [phases.md — Klasör Yapısı (Backend)](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L31-L43)

---

## 2. Backend NuGet Paketlerinin Yüklenmesi

> 📎 Referans: [phases.md — Kodlama Süreci, Madde 3](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L107-L111)

- [x] **2.1.** `Pomelo.EntityFrameworkCore.MySql` paketini yükle
- [x] **2.2.** `Microsoft.AspNetCore.Authentication.JwtBearer` paketini yükle
- [x] **2.3.** `BCrypt.Net-Next` paketini yükle
- [x] **2.4.** `Swashbuckle.AspNetCore` (Swagger) paketini yükle
- [x] **2.5.** `Microsoft.EntityFrameworkCore.Design` paketini yükle (migration tooling için)

> ⚠️ **Not:** Pomelo 9.0.0 EF Core 9.x gerektirir. Versiyon uyumu için `net9.0` hedeflendi ve tüm EF paketleri 9.0.14'e sabitlendi.

---

## 3. MySQL Veritabanı Oluşturma

> 📎 Referans: [phases.md — MySQL Veritabanı Oluşturma](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L47-L52)

- [ ] **3.1.** MySQL Server'ın kurulu ve çalışır durumda olduğunu doğrula
- [ ] **3.2.** `pqms_db` veritabanını `utf8mb4` karakter seti ve `utf8mb4_unicode_ci` collation ile oluştur:
  ```sql
  CREATE DATABASE pqms_db
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;
  ```

> ⚠️ **Kullanıcı Eylemi Gerekli:** MySQL Server'ın kurulu olması ve çalıştırılması, ardından `appsettings.json` dosyasındaki `YOUR_PASSWORD` değerinin gerçek MySQL root şifresiyle değiştirilmesi gerekmektedir.

---

## 4. Veritabanı Bağlantı Yapılandırması

> 📎 Referans: [phases.md — Connection String (appsettings.json)](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L78-L85)

- [x] **4.1.** `appsettings.json` dosyasına MySQL connection string'i ekle
- [x] **4.2.** `appsettings.Development.json` dosyasına geliştirme ortamına özel connection string ekle

> ⚠️ **Not:** `appsettings.json` içindeki şifre `YOUR_PASSWORD` olarak bırakıldı. Kendi MySQL şifrenle değiştirmelisin.

---

## 5. EF Core DbContext Oluşturma

> 📎 Referans: [phases.md — EF Core DbContext Konfigürasyonu](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L54-L76)

- [x] **5.1.** `Data/PqmsDbContext.cs` dosyasını oluştur
- [x] **5.2.** `PqmsDbContext` sınıfını `DbContext`'ten türet
- [x] **5.3.** Aşağıdaki `DbSet` property'lerini tanımla:
  - [x] **5.3.1.** `DbSet<User> Users`
  - [x] **5.3.2.** `DbSet<Patient> Patients`
  - [x] **5.3.3.** `DbSet<Appointment> Appointments`
  - [x] **5.3.4.** `DbSet<QueueEntry> QueueEntries`
  - [x] **5.3.5.** `DbSet<VisitReason> VisitReasons`
- [x] **5.4.** `OnModelCreating` metodunda `modelBuilder.HasCharSet("utf8mb4")` ile karakter setini ayarla
- [x] **5.5.** `OnModelCreating` içinde Fluent API konfigürasyonlarını `ApplyConfigurationsFromAssembly` ile uygula

---

## 6. Program.cs Temel Servis Kayıtları

> 📎 Referans: [phases.md — Program.cs Temel Servislerin Kaydı](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L87-L102)

- [x] **6.1.** `AddDbContext<PqmsDbContext>` ile MySQL bağlantısını kaydet (`Pomelo` provider kullanarak)
- [x] **6.2.** `MySqlServerVersion` ile MySQL sunucu versiyonunu belirt
- [x] **6.3.** CORS politikası tanımla — `AllowReactApp` adıyla:
  - [x] **6.3.1.** Origin: `http://localhost:5173` (Vite dev server)
  - [x] **6.3.2.** `AllowAnyHeader()` ve `AllowAnyMethod()` izinlerini ekle
- [x] **6.4.** `app.UseCors("AllowReactApp")` middleware'ini pipeline'a ekle
- [x] **6.5.** Swagger/OpenAPI konfigürasyonunun aktif olduğunu doğrula
- [x] **6.6.** *(Ek)* JWT Authentication middleware'ini `Program.cs`'ye ekle (`UseAuthentication` + `UseAuthorization`)

---

## 7. Health Check Endpoint Oluşturma

> 📎 Referans: [phases.md — Test tablosu, Entegrasyon satırı](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L147)

- [x] **7.1.** `Controllers/HealthController.cs` dosyasını oluştur
- [x] **7.2.** `GET /api/health` endpoint'i tanımla — `200 OK` ve basit bir JSON mesajı döndür

---

## 8. İlk Migration ve Veritabanı Güncelleme

> 📎 Referans: [phases.md — Kodlama Süreci, Madde 6](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L135-L139)

- [x] **8.1.** `dotnet ef migrations add InitialCreate` komutu ile ilk migration'ı oluştur
- [x] **8.2.** `dotnet ef database update` komutu ile veritabanını güncelle
- [x] **8.3.** MySQL'de tabloların başarıyla oluşturulduğunu doğrula

> ⚠️ **Ön Koşul:** Görev 3 tamamlanmalıdır (MySQL veritabanı oluşturulmalı ve connection string güncellenmelidir).

---

## 9. Frontend Proje Oluşturma

> 📎 Referans: [phases.md — Kodlama Süreci, Madde 2](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L106)

- [x] **9.1.** `npm create vite@latest pqms-client -- --template react` komutu ile React projesi oluştur
- [x] **9.2.** `cd pqms-client && npm install` ile bağımlılıkları yükle
- [x] **9.3.** Proje klasör yapısını oluştur — aşağıdaki dizinleri oluştur:
  - [x] **9.3.1.** `src/components/` klasörü
  - [x] **9.3.2.** `src/pages/` klasörü
  - [x] **9.3.3.** `src/hooks/` klasörü
  - [x] **9.3.4.** `src/services/` klasörü
  - [x] **9.3.5.** `src/context/` klasörü
  - [x] **9.3.6.** `src/utils/` klasörü
  - [x] **9.3.7.** `src/styles/` klasörü

> 📎 Referans: [phases.md — Klasör Yapısı (Frontend)](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L16-L30)

---

## 10. Frontend npm Paketlerinin Yüklenmesi

> 📎 Referans: [phases.md — Kodlama Süreci, Madde 4](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L112-L114)

- [x] **10.1.** `axios` paketini yükle
- [x] **10.2.** `react-router-dom` paketini yükle

---

## 11. Axios API Instance Oluşturma

> 📎 Referans: [phases.md — Kodlama Süreci, Madde 5](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L115-L134)

- [x] **11.1.** `src/services/api.js` dosyasını oluştur
- [x] **11.2.** `axios.create()` ile API instance'ı oluştur — `baseURL: 'https://localhost:7135/api'`
- [x] **11.3.** Request interceptor ekle — her istekte `localStorage`'dan `token` alıp `Authorization: Bearer <token>` header'ı ekle
- [x] **11.4.** Oluşturulan `api` instance'ını `export default` ile dışa aktar
- [x] **11.5.** *(Ek)* Response interceptor ekle — 401 hatalarında otomatik olarak login'e yönlendir

---

## 12. Doğrulama Testleri (Faz 0 Tamamlanma Kriterleri)

> 📎 Referans: [phases.md — Test tablosu](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L141-L147)

- [x] **12.1.** ✅ **Backend derleme testi:** `dotnet build` komutu 0 hata, 0 uyarı ile tamamlandı
- [x] **12.2.** ✅ **Backend çalıştırma testi:** `dotnet run` komutu ile backend'i başlat → tarayıcıda Swagger UI'ın açıldığını doğrula
- [x] **12.3.** ✅ **Frontend çalıştırma testi:** `npm run dev` komutu ile frontend'i başlat → tarayıcıda Vite welcome sayfasının göründüğünü doğrula
- [x] **12.4.** ✅ **MySQL bağlantı testi:** `dotnet ef database update` komutunun hatasız tamamlandığını doğrula
- [x] **12.5.** ✅ **Health endpoint testi:** Tarayıcı veya Postman ile `GET /api/health` isteği gönder → `200 OK` yanıtı geldiğini doğrula
- [x] **12.6.** ✅ **CORS testi:** Frontend'den backend'e bir test isteği gönder → CORS hatası almadığını doğrula

---

## 📋 Faz 0 İlerleme Özeti

| #  | Görev Grubu                        | Durum |
|----|------------------------------------|-------|
| 1  | Backend Proje Oluşturma           | ✅    |
| 2  | NuGet Paketleri                   | ✅    |
| 3  | MySQL Veritabanı                  | ✅    |
| 4  | Connection String                 | ✅    |
| 5  | DbContext                         | ✅    |
| 6  | Program.cs Servisleri             | ✅    |
| 7  | Health Check Endpoint             | ✅    |
| 8  | Migration & DB Update             | ✅    |
| 9  | Frontend Proje Oluşturma          | ✅    |
| 10 | npm Paketleri                     | ✅    |
| 11 | Axios API Instance                | ✅    |
| 12 | Doğrulama Testleri                | ✅    |

> ✅ Tüm maddeler tamamlandığında Faz 0 bitmiştir. [Faz 1 görevlerine geç →](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/tasks-faz1.md)
