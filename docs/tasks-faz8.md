# ✅ Faz 8 — Kuyruk Gösterim Ekranı (Queue Display): Görev Listesi

> **Referans:** [phases.md — Faz 8](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1550-L1660)
>
> **Ön Koşul:** Dashboard (Faz 2) ve Check-In (Faz 5) ekranlarının tamamlanmış olması (Tamamlandı).
>
> **Ekran Referansı:** Bekleme salonlarında gösterilmek üzere, anlık çağrılan hastayı ve bekleme listesini gösteren, otomatik güncellenen büyük fontlu ekran.

---

## 1. Veri Yapısı ve DTO'lar
> 📎 Referans: [phases.md — Veri Yapısı](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1576-L1588)

- [x] **1.1.** `DTOs/Queue` klasörüne `QueueDisplayItemDto.cs` ekle: `QueueNumber`, `PatientName`, `CheckInTime`
- [x] **1.2.** `DTOs/Queue` klasörüne `QueueDisplayDto.cs` ekle: `CurrentPatient` (QueueDisplayItemDto?), `WaitingList` (List<QueueDisplayItemDto>)

---

## 2. Backend — İş Mantığı Katmanı (QueueService)
- [x] **2.1.** `IQueueService.cs` içerisine `Task<QueueDisplayDto> GetQueueDisplay()` metodunu tanımla.
- [x] **2.2.** `QueueService.cs` içerisinde metodu implemente et:
  - `CurrentPatient`: Durumu `InProgress` olan en son/geçerli kaydı getir.
  - `WaitingList`: Durumu `Waiting` olan kayıtları `CheckInTime` veya `QueueNumber` sırasına göre getir.

---

## 3. Backend — API Katmanı (QueueController)
> 📎 Referans: [phases.md — API Endpoint](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1570-L1574)

- [x] **3.1.** `QueueController.cs` içerisine `GET /api/queue/display` endpoint'i ekle.
- [x] **3.2.** Bu endpoint'in `[AllowAnonymous]` (veya public) erişilebilir olmasını sağla (Bekleme salonu ekranı login gerektirmeyebilir).

---

## 4. Frontend — API Servis Entegrasyonu
- [x] **4.1.** `src/services/queueService.js` dosyasına `getQueueDisplay` fonksiyonunu ekle.

---

## 5. Frontend — UI Bileşenleri ve Sayfa Tasarımı (Otomatik Yenileme)
> 📎 Referans: [phases.md — React Durum Yönetimi](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1590-L1649)

- [x] **5.1.** `src/pages/QueueDisplayPage.jsx` sayfasını oluştur.
- [x] **5.2.** `src/styles/QueueDisplayPage.css` ile büyük puntolu, dikkat çekici, premium bir TV ekranı tasarımı hazırla.
- [x] **5.3.** React `useEffect` ve `setInterval` (Polling) kullanarak veriyi her **10 saniyede bir** otomatik güncelleyecek mantığı kur.
- [x] **5.4.** Ekranda "Now Serving" (Çağrılan Hasta) bölümünü oluştur.
- [x] **5.5.** Ekranda "Up Next" (Sıradaki Bekleyenler) listesini oluştur.
- [x] **5.6.** `App.jsx` içerisine `/queue-display` rotasını ekle (Public veya Protected). Navbar'da linki zaten güncellenmişti.

---

## 6. Test ve Doğrulama
> 📎 Referans: [phases.md — Test Tablosu](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1651-L1660)

- [x] **6.1.** Dashboard ekranından yeni bir hasta "Call" (InProgress) durumuna çekildiğinde, 10 saniye içinde Display ekranının "Now Serving" kısmında belirdiğini doğrula.
- [x] **6.2.** Yeni hastalar Check-In yaptıkça "Up Next" listesine eklendiğini doğrula.
- [x] **6.3.** Hiç InProgress hasta olmadığında sistemin "No patient currently being served" gibi bir mesajı doğru şekilde gösterdiğini test et.

---

## 📋 Faz 8 İlerleme Özeti

| # | Görev Grubu | Durum |
|---|-------------|-------|
| 1 | Veri Yapısı ve DTO'lar | ✅ |
| 2 | Backend İş Mantığı (QueueService) | ✅ |
| 3 | API Katmanı (QueueController) | ✅ |
| 4 | Frontend API Servis Entegrasyonu | ✅ |
| 5 | UI Bileşenleri (QueueDisplay) | ✅ |
| 6 | Test ve Doğrulama | ✅ |

> ✅ Tüm maddeler tamamlandığında Faz 8 bitmiş olacak ve proje tamamlanacaktır!
