# ✅ Faz 5 — Hasta Check-In Ekranı: Görev Listesi

> **Referans:** [phases.md — Faz 5](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1017-L1318)
>
> **Ön Koşul:** Faz 4 (Hasta Arama) görevleri tamamlanmış olmalıdır.
>
> **Ekran Referansı:** "Welcome to Our Clinic" — Appointment/Walk-In sekmeli check-in formu (Ekran Görüntüsü 6)

---

## 1. Veri Modelleri ve DTO'lar
> 📎 Referans: [phases.md — Veri Yapısı](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1044-L1101)

- [x] **1.1.** `Models/VisitReason.cs` dosyasını oluştur:
  - `Id`, `Name` (Required, MaxLength 100), `IsActive` özelliklerini ekle
- [x] **1.2.** `Data/Configurations/VisitReasonConfiguration.cs` oluştur:
  - Tablo adını `VisitReasons` olarak belirle
  - 7 adet örnek ziyaret sebebini `HasData` ile seed et (General Checkup, Follow-up Visit, vb.)
- [x] **1.3.** `PqmsDbContext.cs` dosyasına `DbSet<VisitReason> VisitReasons` ekle ve `OnModelCreating` içinde Configuration'ı çağır
- [x] **1.4.** DTOs/Queue klasörüne `CheckInRequestDto.cs` ekle (`PatientId`, `CheckInType`, `VisitReason`, `AdditionalInfo`, `TermsAccepted`)
- [x] **1.5.** DTOs/Queue klasörüne `CheckInResponseDto.cs` ekle (`QueueEntryId`, `QueueNumber`, `PatientName`, `Status`, `CheckInTime`)
- [x] **1.6.** Migration oluştur (`dotnet ef migrations add AddVisitReasons`) ve veritabanını güncelle (`dotnet ef database update`)

---

## 2. Backend — İş Mantığı Katmanı (QueueService Güncellemesi)
> 📎 Referans: [phases.md — Kodlama Süreci Backend](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1103-L1163)

- [x] **2.1.** `IQueueService.cs` içerisine yeni metodu ekle: `Task<CheckInResponseDto> CheckIn(CheckInRequestDto request)`
- [x] **2.2.** `QueueService.cs` içinde `CheckIn` metodunu implemente et:
  - [x] **Adım 1:** PatientId'nin veritabanında var olup olmadığını kontrol et (yoksa NotFoundException/BusinessException fırlat)
  - [x] **Adım 2:** Hastanın aynı gün içinde durumu "Completed" olmayan başka bir kaydı var mı diye bak (varsa çift kayıt engelle)
  - [x] **Adım 3:** `TermsAccepted` true mu kontrol et
  - [x] **Adım 4:** Mevcut `GenerateQueueNumber` metodunu kullanarak numara üret
  - [x] **Adım 5:** Yeni `QueueEntry` nesnesi oluştur ve veritabanına kaydet
  - [x] **Adım 6:** `CheckInResponseDto` döndür
- [x] **2.3.** `PqmsDbContext` üzerinden `VisitReasons` listesini çekecek basit bir metot eklenebilir veya Controller'da doğrudan context kullanılabilir

---

## 3. Backend — API Katmanı (QueueController & VisitReasons)
> 📎 Referans: [phases.md — API Endpoint'leri](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1107-L1111)

- [x] **3.1.** `QueueController.cs` dosyasına `POST /api/queue/checkin` endpoint'i ekle
- [x] **3.2.** Yeni bir `VisitReasonsController.cs` oluştur (`GET /api/visit-reasons` endpoint'i ile aktif olan sebepleri dönsün)

---

## 4. Frontend — API Servis Entegrasyonu
> 📎 Referans: [phases.md — Frontend React Durum Yönetimi](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1165-L1231)

- [x] **4.1.** `src/services/queueService.js` dosyasına `checkIn(data)` fonksiyonunu ekle
- [x] **4.2.** `queueService.js` dosyasına `getVisitReasons()` fonksiyonunu ekle

---

## 5. Frontend — UI Bileşenleri ve Sayfa Tasarımı
- [x] **5.1.** `src/pages/PatientCheckInPage.jsx` dosyasını oluştur (Placeholder rotayı aslıyla değiştir)
- [x] **5.2.** `PatientCheckInPage.jsx` içerisine formu ve state'leri ekle:
  - `checkInType` (Appointment / WalkIn) Tab'leri
  - `visitReason` (API'den çekilen dropdown)
  - `additionalInfo` (Textarea)
  - `termsAccepted` (Checkbox)
- [x] **5.3.** `useEffect` ile sayfa açılışında `getVisitReasons` API'sini çağırıp dropdown'ı doldur
- [x] **5.4.** Form Submit edildiğinde validasyonları yap (Reason seçilmiş mi? Terms kabul edilmiş mi?) ve API'ye gönder
- [x] **5.5.** Başarılı işlem sonrası ekranda "CheckInSuccess" bileşenini/mesajını göster (Queue Number ve Patient Name ile)
- [x] **5.6.** `src/styles/PatientCheckInPage.css` dosyasını oluştur ve premium tasarıma uygun stillendir (Tab yapısı, select ve buton hover efektleri vb.)

---

## 6. Test ve Doğrulama
> 📎 Referans: [phases.md — Test Tablosu](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1302-L1317)

- [x] **6.1.** Unit Test: `QueueService.CheckIn` metodunun çift kayıt engellemesini ve geçersiz hasta kontrolünü doğrulama
- [x] **6.2.** Backend API: Postman/Swagger ile `GET /api/visit-reasons` üzerinden statik listeyi alabildiğini doğrula
- [x] **6.3.** Backend API: `POST /api/queue/checkin` çağrısı yapıp 200 OK ve Queue Number alabildiğini onayla
- [x] **6.4.** Frontend Eylem: Tablar arası (Appointment / WalkIn) geçiş yapınca `checkInType` state'inin değiştiğini doğrula
- [x] **6.5.** Frontend Eylem: Ziyaret sebebi seçmeden veya koşulları onaylamadan butona basınca hata mesajı (validation) çıktığını gör
- [x] **6.6.** Frontend Eylem: Başarılı submit sonrası ekranda hastanın kuyruk numarasının başarıyla gösterildiğini test et

---

## 📋 Faz 5 İlerleme Özeti

| # | Görev Grubu | Durum |
|---|-------------|-------|
| 1 | Veri Modelleri ve DTO'lar | ✅ |
| 2 | İş Mantığı Katmanı (QueueService) | ✅ |
| 3 | API Katmanı (QueueController & VisitReasons) | ✅ |
| 4 | Frontend API Servis Entegrasyonu | ✅ |
| 5 | UI Bileşenleri ve Sayfa Tasarımı | ✅ |
| 6 | Test ve Doğrulama | ✅ |

> ✅ Tüm maddeler tamamlandığında Faz 5 bitmiştir. Faz 6 görevleri için yeni bir task dosyası oluşturulacaktır.
