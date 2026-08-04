# ✅ Faz 7 — Admin Paneli (Kullanıcı ve Sistem Yönetimi): Görev Listesi

> **Referans:** [phases.md — Faz 7](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1448-L1547)
>
> **Ön Koşul:** Faz 1 (Yetkilendirme) ve Faz 2-5 (Temel Sistem) tamamlanmış olmalıdır. Sadece "Admin" rolündeki kullanıcılar erişebilir.
>
> **Ekran Referansı:** Users, Visit Reasons, Queue History sekmelerine sahip Yönetim Paneli.

---

## 1. Veri Yapısı ve DTO'lar
> 📎 Referans: [phases.md — Veri Yapısı](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1463-L1476)

- [x] **1.1.** `DTOs/Admin` klasörünü oluştur.
- [x] **1.2.** `UserListDto.cs` ekle: `Id`, `FullName`, `Email`, `Role`, `IsActive`, `CreatedAt`
- [x] **1.3.** `UpdateUserRoleDto.cs` ekle: `Role`, `IsActive`
- [x] **1.4.** `DTOs/Queue/QueueHistoryFilterDto.cs` ekle: `StartDate` (nullable), `EndDate` (nullable), `Status` (nullable)

---

## 2. Backend — İş Mantığı Katmanı (AdminService)
- [x] **2.1.** `IAdminService.cs` ve `AdminService.cs` dosyalarını oluştur.
- [x] **2.2.** Kullanıcı yönetimi metotlarını implemente et:
  - `Task<List<UserListDto>> GetAllUsers()`
  - `Task UpdateUserRole(int id, UpdateUserRoleDto dto)`
  - `Task ToggleUserStatus(int id)` (Aktif/Pasif yapma)
- [x] **2.3.** Ziyaret sebepleri (Visit Reasons) yönetimi metotlarını implemente et:
  - CRUD işlemleri (Add, Update, Soft Delete)
- [x] **2.4.** Kuyruk Geçmişi (Queue History) metodunu implemente et:
  - `Task<List<QueueEntry>> GetQueueHistory(QueueHistoryFilterDto filter)`

---

## 3. Backend — API Katmanı (AdminController)
> 📎 Referans: [phases.md — API Endpoint'leri](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1480-L1503)

- [x] **3.1.** `AdminController.cs` dosyasını oluştur ve sınıfa `[Authorize(Roles = "Admin")]` ekle.
- [x] **3.2.** Kullanıcı Endpoints:
  - `GET /api/admin/users`
  - `PUT /api/admin/users/{id}/role`
  - `PUT /api/admin/users/{id}/status`
- [x] **3.3.** Visit Reasons Endpoints:
  - `GET /api/admin/visit-reasons`
  - `POST /api/admin/visit-reasons`
  - `PUT /api/admin/visit-reasons/{id}`
  - `DELETE /api/admin/visit-reasons/{id}`
- [x] **3.4.** Queue History Endpoints:
  - `GET /api/admin/queue-history` (Query param olarak filtreleri alsın)

---

## 4. Frontend — API Servis Entegrasyonu
- [x] **4.1.** `src/services/adminService.js` dosyasını oluştur.
- [x] **4.2.** `getUsers`, `updateUserRole`, `toggleUserStatus` fonksiyonlarını ekle.
- [x] **4.3.** `getVisitReasons`, `addVisitReason`, `updateVisitReason`, `deleteVisitReason` fonksiyonlarını ekle.
- [x] **4.4.** `getQueueHistory(filters)` fonksiyonunu ekle.

---

## 5. Frontend — UI Bileşenleri ve Sayfa Tasarımı
> 📎 Referans: [phases.md — Frontend React Durum Yönetimi](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1505-L1535)

- [x] **5.1.** `src/pages/AdminPage.jsx` ve `src/styles/AdminPage.css` oluştur.
- [x] **5.2.** Üst kısma (veya sol tarafa) Tab Menü ekle: `Users` | `Visit Reasons` | `Queue History`
- [x] **5.3.** **Users Tab:**
  - Kullanıcı tablosunu oluştur (Verileri API'den çek).
  - Kullanıcı rolünü değiştirmek ve aktif/pasif yapmak için eylem butonları (Edit/Toggle) ekle.
- [x] **5.4.** **Visit Reasons Tab:**
  - Mevcut sebepleri listeleyen tablo.
  - Yeni ekleme ve düzenleme/silme butonları ve modal (veya inline edit).
- [x] **5.5.** **Queue History Tab:**
  - Başlangıç, Bitiş ve Durum filtrelerini içeren bir arama çubuğu.
  - Filtrelenmiş sonuçları gösteren tablo.
- [x] **5.6.** `App.jsx` içine `/admin` rotasını `ProtectedRoute` kullanarak sadece `Admin` erişebilecek şekilde ekle.

---

## 6. Test ve Doğrulama
> 📎 Referans: [phases.md — Test Tablosu](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L1537-L1547)

- [x] **6.1.** Entegrasyon: Admin token ile `GET /api/admin/users` isteği 200 OK dönüyor mu test et.
- [x] **6.2.** Entegrasyon: Normal "Doctor" rolündeki (veya anonim) biri `/api/admin/...` adresine istek atınca `403 Forbidden` / `401 Unauthorized` hatası dönüyor mu test et.
- [x] **6.3.** Frontend Eylem: Admin panelinde sekme değiştirince ilgili API'ye istek gidip tablonun dolduğunu onayla.
- [x] **6.4.** Frontend Eylem: Bir kullanıcının rolünü Doctor'dan Admin'e (veya tam tersi) değiştirmeyi UI üzerinden test et.

---

## 📋 Faz 7 İlerleme Özeti

| # | Görev Grubu | Durum |
|---|-------------|-------|
| 1 | Veri Yapısı ve DTO'lar | ✅ |
| 2 | Backend İş Mantığı (AdminService) | ✅ |
| 3 | API Katmanı (AdminController) | ✅ |
| 4 | Frontend API Servis Entegrasyonu | ✅ |
| 5 | UI Bileşenleri ve Sayfa Tasarımı | ✅ |
| 6 | Test ve Doğrulama | ✅ |

> ✅ Tüm maddeler tamamlandığında Faz 7 bitmiştir. Son aşama olan Faz 8 (Kuyruk Gösterim Ekranı) için yeni bir task dosyası oluşturulacaktır.
