# ✅ Faz 2 — Yönetici Dashboard (Kuyruk Yönetim Ekranı): Görev Listesi

> **Referans:** [phases.md — Faz 2](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L409-L681)
>
> **Ön Koşul:** Faz 1 (Kimlik Doğrulama) görevleri tamamlanmış olmalıdır.
>
> **Ekran Referansı:** Today's Queue (3 kolonlu Kanban görünümü)

---

## 1. Veri Modellerinin (Entity) Oluşturulması

> 📎 Referans: [phases.md — Veri Yapısı](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L454-L511)

- [x] **1.1.** `Models/Patient.cs` dosyasını oluştur
- [x] **1.2.** `Patient` sınıfına aşağıdaki özellikleri ekle:
  - [x] **1.2.1.** `int Id` (Primary Key)
  - [x] **1.2.2.** `string FullName` — `[Required, MaxLength(100)]`
  - [x] **1.2.3.** `string? PhoneNumber` — `[MaxLength(20)]`
  - [x] **1.2.4.** `DateTime? DateOfBirth`
  - [x] **1.2.5.** `DateTime CreatedAt` — Varsayılan: `DateTime.UtcNow`
  - [x] **1.2.6.** Navigation properties: `ICollection<QueueEntry>` ve `ICollection<Appointment>`
- [x] **1.3.** `Models/QueueEntry.cs` dosyasını oluştur
- [x] **1.4.** `QueueEntry` sınıfına aşağıdaki özellikleri ekle:
  - [x] **1.4.1.** `int Id` (Primary Key)
  - [x] **1.4.2.** `int PatientId` ve `Patient Patient` (Foreign Key İlişkisi)
  - [x] **1.4.3.** `string QueueNumber` — `[Required, MaxLength(15)]` (Örn: "WALK-6")
  - [x] **1.4.4.** `string Status` — `[Required, MaxLength(20)]` (Waiting, InProgress, Completed)
  - [x] **1.4.5.** `string CheckInType` — `[Required, MaxLength(20)]` (WalkIn, Appointment)
  - [x] **1.4.6.** `string? VisitReason` — `[MaxLength(100)]`
  - [x] **1.4.7.** `string? AdditionalInfo` — `[MaxLength(500)]`
  - [x] **1.4.8.** `DateTime CheckInTime` — Varsayılan: `DateTime.UtcNow`
  - [x] **1.4.9.** `DateTime? CalledAt` (Muayeneye başlama zamanı)
  - [x] **1.4.10.** `DateTime? CompletedAt` (Tamamlanma zamanı)
  - [x] **1.4.11.** `DateTime QueueDate` — Hangi günün kuyruğu olduğu. Varsayılan: `DateTime.UtcNow.Date`
- [x] **1.5.** `PqmsDbContext.cs` dosyasına `DbSet<Patient>` ve `DbSet<QueueEntry>` özelliklerini ekle

---

## 2. Fluent API Konfigürasyonları

> 📎 Referans: [phases.md — Fluent API](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L513-L534)

- [x] **2.1.** `Data/Configurations/QueueEntryConfiguration.cs` dosyasını oluştur
- [x] **2.2.** `IEntityTypeConfiguration<QueueEntry>` interface'ini implemente et
- [x] **2.3.** Tablo adını `QueueEntries` olarak belirle
- [x] **2.4.** Patient ilişkisini yapılandır (`HasOne.WithMany.HasForeignKey.OnDelete(DeleteBehavior.Restrict)`)
- [x] **2.5.** Hızlı arama için Index tanımla: `HasIndex(q => q.QueueDate)` ve `HasIndex(q => q.Status)`
- [x] **2.6.** Alan sınırlarını belirle (`QueueNumber`, `Status`, `CheckInType` için `HasMaxLength`)
- [x] **2.7.** `PqmsDbContext.OnModelCreating` içerisinde bu yapılandırmayı çağır (Veya `ApplyConfigurationsFromAssembly` kullanıyorsa otomatik tanınmasını sağla)

---

## 3. Migration ve Veritabanı Güncellemesi

- [x] **3.1.** Terminalden `dotnet ef migrations add AddPatientAndQueueEntities` komutunu çalıştır
- [x] **3.2.** `dotnet ef database update` ile değişiklikleri MySQL veritabanına yansıt
- [x] **3.3.** Veritabanında `Patients` ve `QueueEntries` tablolarının doğru şekilde oluştuğunu kontrol et

---

## 4. Backend — İş Mantığı Katmanı (QueueService)

> 📎 Referans: [phases.md — İş Mantığı ve Kuyruk Numarası](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L547-L569)

- [x] **4.1.** `Services/IQueueService.cs` interface'ini oluştur:
  - [x] `Task<string> GenerateQueueNumber(string checkInType, DateTime date)`
  - [x] `Task<IEnumerable<QueueEntry>> GetTodayQueue()`
  - [x] `Task<QueueEntry> CallNextPatient()`
  - [x] `Task<QueueEntry> CompletePatient(int queueId)`
- [x] **4.2.** `Services/QueueService.cs` sınıfını oluştur ve interface'i implemente et
- [x] **4.3.** `GenerateQueueNumber` metodunu yaz: `WalkIn` için `WALK-`, `Appointment` için `APPT-` prefixi ile o günkü kayıtlara göre sayı üret
- [x] **4.4.** `GetTodayQueue` metodunu yaz: `QueueDate`'i bugün olan kayıtları getir
- [x] **4.5.** `CallNextPatient` metodunu yaz:
  - [x] **4.5.1.** En eski `CheckInTime`'a sahip `Waiting` durumundaki ilk hastayı bul
  - [x] **4.5.2.** Zaten `InProgress` olan hasta varsa, onu `Completed` yap ve `CompletedAt` ata
  - [x] **4.5.3.** Bulunan hastanın durumunu `InProgress` yap ve `CalledAt` ata
  - [x] **4.5.4.** Veritabanını güncelle ve dön
- [x] **4.6.** `Program.cs` dosyasına `builder.Services.AddScoped<IQueueService, QueueService>();` ekle

---

## 5. Backend — API Katmanı (QueueController)

> 📎 Referans: [phases.md — Endpoint Listesi](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L538-L545)

- [x] **5.1.** `Controllers/QueueController.cs` oluştur (`[ApiController]`, `[Route("api/queue")]`)
- [x] **5.2.** `[Authorize(Roles = "Doctor,Admin")]` attribute'u ekleyerek bu controller'ı yetkilendir
- [x] **5.3.** `GET /api/queue/today` endpoint'ini oluştur
- [x] **5.4.** `POST /api/queue/call-next` endpoint'ini oluştur
- [x] **5.5.** `PUT /api/queue/{id}/complete` endpoint'ini oluştur

---

## 6. Frontend — API Servis Entegrasyonu

> 📎 Referans: [phases.md — Frontend React](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L571-L610)

- [x] **6.1.** `src/services/queueService.js` dosyasını oluştur
- [x] **6.2.** `getTodayQueue()` fonksiyonunu ekle (`GET /api/queue/today`)
- [x] **6.3.** `callNextPatient()` fonksiyonunu ekle (`POST /api/queue/call-next`)
- [x] **6.4.** İsteklerin header'ında token'ın doğru gönderildiğinden emin ol (`api.js` interceptor'ı aracılığıyla halihazırda sağlanmış olmalı)

---

## 7. Frontend — UI Bileşenleri (Components)

> 📎 Referans: [phases.md — QueueColumn ve PatientCard](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L629-L666)

- [x] **7.1.** `src/components/Navbar.jsx` oluştur:
  - [x] **7.1.1.** Logo, Navigation Linkleri (Dashboard vs) ve Kullanıcı Adı gösterimi
  - [x] **7.1.2.** AuthContext'ten alınan `logout` fonksiyonuna bağlı bir Çıkış butonu
- [x] **7.2.** `src/components/PatientCard.jsx` oluştur:
  - [x] **7.2.1.** Hasta adı, bekleme/giriş saati ve ziyaret nedeni gösterimi
  - [x] **7.2.2.** `CheckInType`'a göre mavi (`WalkIn`) veya yeşil (`Appointment`) Badge içinde Kuyruk Numarası (`QueueNumber`) gösterimi
- [x] **7.3.** `src/components/QueueColumn.jsx` oluştur:
  - [x] **7.3.1.** Kolon başlığı (title) ve alt başlığı (toplam hasta sayısı) prop'u alacak
  - [x] **7.3.2.** `patients` array prop'u alarak, içindeki hastalar için `PatientCard` bileşenini map ile render edecek

---

## 8. Frontend — Yönetici Dashboard Tasarımı (DashboardPage)

> 📎 Referans: [phases.md — Ekran Bileşenleri ve Sayfa İçeriği](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L415-L452)

- [x] **8.1.** `src/pages/DashboardPage.jsx` dosyasını `Faz 2` gereksinimlerine göre yeniden yapılandır
- [x] **8.2.** `src/styles/DashboardPage.css` oluştur ve modern tasarım dillerine uygun CSS yaz
- [x] **8.3.** Sayfa tasarımında `Navbar`'ı en üste yerleştir
- [x] **8.4.** "Today's Queue" başlığı, günün tarihi (`formatDate` helper'ı ile) ve "Call Next Patient" butonunu konumlandır
- [x] **8.5.** Kanban tahtası için CSS Grid kullanarak 3 eşit kolon (Waiting, In Progress, Completed) oluştur
- [x] **8.6.** React State entegrasyonu:
  - [x] **8.6.1.** `queueEntries`, `loading`, `error` state'lerini tanımla
  - [x] **8.6.2.** `useEffect` ile sayfa açılışında kuyruk datasını getir (`fetchTodayQueue`)
  - [x] **8.6.3.** Verileri `waitingPatients`, `inProgressPatients`, `completedPatients` olarak filterele ve ilgili `QueueColumn` bileşenlerine gönder
  - [x] **8.6.4.** "Call Next Patient" butonuna basıldığında ilgili API'yi çağır ve tabloyu yenile

---

## 9. Doğrulama ve Testler

> 📎 Referans: [phases.md — Test Tablosu](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/phases.md#L668-L681)

- [x] **9.1.** Unit Test: Backend'de `QueueService.GenerateQueueNumber`'ın doğru formatta (WALK-1, APPT-3 vs) ürettiğini doğrula
- [x] **9.2.** Backend API: `/api/queue/today` yetkisiz çağrılırsa `401 Unauthorized` döndüğünü Swagger ile test et
- [x] **9.3.** Frontend UI: Yönetici hesabı (veya Yetkili Kullanıcı) ile giriş yapıldığında Dashboard ekranının 3 Kanban kolonuyla hatasız yüklendiğini doğrula
- [x] **9.4.** Frontend Eylem: "Call Next Patient" butonuna tıklandığında Waiting'deki ilk hastanın InProgress'e geçtiğini ve arayüzün anlık yenilendiğini doğrula

---

## 📋 Faz 2 İlerleme Özeti

| # | Görev Grubu | Durum |
|---|-------------|-------|
| 1 | Modellerin Oluşturulması | ✅ |
| 2 | Fluent API Konfigürasyonları | ✅ |
| 3 | Veritabanı Migration İşlemleri | ✅ |
| 4 | İş Mantığı Katmanı (QueueService) | ✅ |
| 5 | API Endpoint'leri (QueueController)| ✅ |
| 6 | Frontend — API Servisi | ✅ |
| 7 | Frontend — UI Bileşenleri | ✅ |
| 8 | Dashboard Sayfa Entegrasyonu | ✅ |
| 9 | Test ve Doğrulama | ✅ |

> ✅ Tüm maddeler tamamlandığında Faz 2 bitmiştir. Faz 3 görevleri için yeni bir task dosyası oluşturulacaktır.
