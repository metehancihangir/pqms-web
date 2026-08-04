# ✅ Faz 4 — Hasta Arama Ekranı: Görev Listesi

> **Referans:** [phases.md — Faz 4](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L782-L1013)
>
> **Ön Koşul:** Faz 3 (Hasta Karşılama) görevleri tamamlanmış olmalıdır.
>
> **Ekran Referansı:** "Find Patient" formu + sonuç kartları (Ekran Görüntüsü 4 & 5)

---

## 1. Veri Yapısı (DTO'lar ve Konfigürasyon)
> 📎 Referans: [phases.md — Veri Yapısı (Mimari & Veritabanı)](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L816-L845)

- [x] **1.1.** `DTOs/Patient` klasörünü oluştur
- [x] **1.2.** `PatientSearchRequestDto.cs` dosyasını oluştur:
  - `string? SearchTerm`, `DateTime? DateOfBirth` alanlarını içeren record tanımla
- [x] **1.3.** `PatientSearchResultDto.cs` dosyasını oluştur:
  - `int Id`, `string FullName`, `string? PhoneNumber`, `DateTime? DateOfBirth` alanlarını içeren record tanımla
- [x] **1.4.** (İsteğe bağlı, Faz 5'e hazırlık) `PatientCreateDto.cs` oluştur
- [x] **1.5.** `Data/Configurations/PatientConfiguration.cs` dosyasına indeksleri ekle:
  - `builder.HasIndex(p => p.FullName);`
  - `builder.HasIndex(p => p.PhoneNumber);`
  - `builder.HasIndex(p => p.DateOfBirth);`
- [x] **1.6.** Arama indeksleri için Migration oluştur (`dotnet ef migrations add AddPatientSearchIndexes`) ve veritabanını güncelle

---

## 2. Backend — İş Mantığı Katmanı (PatientService)
> 📎 Referans: [phases.md — Kodlama Süreci Backend](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L847-L883)

- [x] **2.1.** `Services/IPatientService.cs` interface'ini oluştur:
  - `Task<List<PatientSearchResultDto>> SearchPatients(string? searchTerm, DateTime? dateOfBirth)` metodu tanımla
- [x] **2.2.** `Services/PatientService.cs` sınıfını oluştur ve implemente et
- [x] **2.3.** `SearchPatients` metodu içinde EF Core ile sorguyu yaz:
  - [x] `SearchTerm` doluysa `FullName` veya `PhoneNumber` üzerinde `Contains` ile arama yap
  - [x] `DateOfBirth` doluysa tam tarih eşleşmesi ara
  - [x] Sonuçları `PatientSearchResultDto`'ya map edip listeye çevir (performans için `Take(20)` limiti koy)
- [x] **2.4.** `Program.cs` içinde `IPatientService`'i DI konteynerine ekle

---

## 3. Backend — API Katmanı (PatientController)
> 📎 Referans: [phases.md — API Endpoint'leri](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L849-L856)

- [x] **3.1.** `Controllers/PatientController.cs` oluştur (`[ApiController]`, `[Route("api/patients")]`)
- [x] **3.2.** `IPatientService` bağımlılığını ekle
- [x] **3.3.** `GET /api/patients/search` endpoint'ini oluştur:
  - Query'den `SearchTerm` ve `DateOfBirth` parametrelerini al
  - `_patientService.SearchPatients` metodunu çağırıp `Ok(results)` dön

---

## 4. Frontend — API Servis Entegrasyonu
> 📎 Referans: [phases.md — Frontend React Durum Yönetimi](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L885-L998)

- [x] **4.1.** `src/services/patientService.js` dosyasını oluştur
- [x] **4.2.** Axios kullanarak `/patients/search` isteklerini atacak `search` fonksiyonunu ekle (`params` üzerinden searchTerm ve dateOfBirth gönderilecek)

---

## 5. Frontend — UI Bileşenleri ve Sayfa Tasarımı
- [x] **5.1.** `src/components/PatientResultCard.jsx` bileşenini oluştur
  - [x] Hasta Adı, Telefon numarası ve Doğum Tarihi bilgilerini göster
  - [x] "Book Appointment" (Mavi) ve "Proceed to Check-In" (Yeşil) butonlarını ekle
- [x] **5.2.** `src/pages/PatientSearchPage.jsx` dosyasını oluştur (Placeholder rotayı aslıyla değiştir)
- [x] **5.3.** `PatientSearchPage.jsx` içerisine Arama Formunu entegre et:
  - `searchTerm` (Text Input)
  - `dateOfBirth` (Date Picker)
  - "Search Patient" Butonu
- [x] **5.4.** React State'lerini (`searchTerm`, `dateOfBirth`, `results`, `hasSearched`, `loading`, `error`) tanımla
- [x] **5.5.** "Search Patient" butonuna tıklanınca (`handleSearch`) verileri API'den çeken ve sonuçları state'e aktaran mantığı yaz
- [x] **5.6.** Sonuçlara göre listeyi map ile döndür veya "No patients found. Register new patient" uyarı kutusunu çıkart
- [x] **5.7.** Kartlardaki buton tıklamalarını (`handleCheckIn`, vb.) uygun react-router yönlendirmelerine bağla (`/patient/checkin/:patientId`)
- [x] **5.8.** `src/styles/PatientSearchPage.css` ile arama formunun ve hasta kartlarının premium tasarımını yap (Gerekirse grid yapıları ve hover efektleri)

---

## 6. Test ve Doğrulama
> 📎 Referans: [phases.md — Test Tablosu](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1001-L1013)

- [x] **6.1.** Unit Test: `PatientService.Search` metodunun eşleşen/eşleşmeyen durumlarda doğru listeleri döndürdüğünü kod bazında (veya swagger'da) kontrol et
- [x] **6.2.** Backend API: Postman veya Swagger üzerinden `GET /api/patients/search?searchTerm=xyz` çağrısı yaparak 200 OK yanıtını onayla
- [x] **6.3.** Frontend UI: Arama formu gönderildiğinde `PatientResultCard` bileşenlerinin göründüğünü doğrula
- [x] **6.4.** Frontend Eylem: "No patients found" durumunda çıkan "Register new patient" linkine tıklayınca Register sayfasına yönlendirildiğini kontrol et

---

## 📋 Faz 4 İlerleme Özeti

| # | Görev Grubu | Durum |
|---|-------------|-------|
| 1 | Veri Yapısı ve Konfigürasyon | ✅ |
| 2 | İş Mantığı Katmanı (PatientService) | ✅ |
| 3 | API Katmanı (PatientController) | ✅ |
| 4 | Frontend — API Servisi | ✅ |
| 5 | UI Bileşenleri ve Sayfa Tasarımı | ✅ |
| 6 | Test ve Doğrulama | ✅ |

> ✅ Tüm maddeler tamamlandığında Faz 4 bitmiştir. Faz 5 görevleri için yeni bir task dosyası oluşturulacaktır.
