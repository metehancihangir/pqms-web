# ✅ Faz 6 — Yeni Hasta Kayıt Ekranı: Görev Listesi

> **Referans:** [phases.md — Faz 6](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1320-L1445)
>
> **Ön Koşul:** Faz 4 (Hasta Arama) ve Faz 5 (Hasta Check-In) görevleri tamamlanmış olmalıdır.
>
> **Ekran Referansı:** Yeni hasta kayıt formu (Ad, Telefon, Doğum Tarihi)

---

## 1. Veri Yapısı (DTO'lar)
> 📎 Referans: [phases.md — Veri Yapısı](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1337-L1339)

- [x] **1.1.** `DTOs/Patient/PatientCreateDto.cs` dosyasını kontrol et (Faz 4'te eklendiyse onayla, yoksa oluştur):
  - `FullName` (zorunlu), `PhoneNumber`, `DateOfBirth` alanlarını içermeli.

---

## 2. Backend — API ve İş Mantığı (PatientController & Service)
> 📎 Referans: [phases.md — Kodlama Süreci Backend](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1341-L1371)

- [x] **2.1.** `IPatientService.cs` içerisine metot ekle: `Task<PatientSearchResultDto> CreatePatient(PatientCreateDto dto)`
- [x] **2.2.** `PatientService.cs` içinde `CreatePatient` metodunu implemente et:
  - [x] **Adım 1:** `FullName` ve `DateOfBirth` kullanarak çift kayıt (duplicate) kontrolü yap.
  - [x] **Adım 2:** Varsa `BusinessException` / `Exception` fırlat ("Bu isim ve doğum tarihiyle hasta zaten mevcut").
  - [x] **Adım 3:** Yoksa yeni `Patient` entity'sini oluşturup kaydet.
  - [x] **Adım 4:** Oluşan hastayı `PatientSearchResultDto` formatında geri dön.
- [x] **2.3.** `PatientController.cs` içerisine `POST /api/patients` endpoint'ini ekle (AllowAnonymous yap ki hastalar doğrudan kayıt olabilsin).

---

## 3. Frontend — API Servis Entegrasyonu
> 📎 Referans: [phases.md — Frontend React Durum Yönetimi](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1373-L1433)

- [x] **3.1.** `src/services/patientService.js` dosyasına `create(data)` fonksiyonunu ekle (POST isteği atacaksın).

---

## 4. Frontend — UI Bileşenleri ve Sayfa Tasarımı
- [x] **4.1.** `src/pages/PatientRegisterPage.jsx` dosyasını oluştur (App.jsx'teki placeholder'ı değiştir).
- [x] **4.2.** `PatientRegisterPage.jsx` içerisine formu ve state'leri ekle:
  - `fullName` (Text Input)
  - `phoneNumber` (Tel Input)
  - `dateOfBirth` (Date Input)
- [x] **4.3.** Form Submit edildiğinde (`handleRegister`):
  - Boş Ad-Soyad validasyonunu yap.
  - `patientService.create()` çağrısını yap.
  - Başarılı olursa dönen `id` ile `/patient/checkin/{id}` rotasına yönlendir (`navigate`).
  - Hata olursa ekranda göster.
- [x] **4.4.** Sayfa altına "← Back to search" linkini ekle (`/patient/search` sayfasına döner).
- [x] **4.5.** `src/styles/PatientRegisterPage.css` oluşturup sayfanın premium tasarımını (gölgeler, hoverlar, input stilleri) yap.

---

## 5. Test ve Doğrulama
> 📎 Referans: [phases.md — Test Tablosu](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1435-L1445)

- [x] **5.1.** Unit Test: `PatientService.CreatePatient` çift kaydı (duplicate) başarıyla engelliyor mu kontrol et.
- [x] **5.2.** Backend API: Postman/Swagger ile `POST /api/patients` üzerinden başarılı kayıt oluşturulabiliyor mu doğrula.
- [x] **5.3.** Frontend Eylem: İsim girmeden "Register" butonuna basıldığında hatayı doğrula.
- [x] **5.4.** Frontend Eylem: Başarılı kayıt formundan sonra hastanın doğrudan Check-In ekranına (ilgili ID ile) yönlendirildiğini test et.

---

## 📋 Faz 6 İlerleme Özeti

| # | Görev Grubu | Durum |
|---|-------------|-------|
| 1 | Veri Yapısı (DTO'lar) | ✅ |
| 2 | API ve İş Mantığı (PatientService) | ✅ |
| 3 | Frontend API Servisi | ✅ |
| 4 | UI Bileşenleri ve Sayfa Tasarımı | ✅ |
| 5 | Test ve Doğrulama | ✅ |

> ✅ Tüm maddeler tamamlandığında Faz 6 bitmiştir. Faz 7 (Admin Paneli) görevleri için yeni bir task dosyası oluşturulacaktır.
