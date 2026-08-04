# ✅ Faz 3 — Hasta Karşılama (Welcome Ekranı): Görev Listesi

> **Referans:** [phases.md — Faz 3](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L684-L780)
>
> **Ön Koşul:** Faz 2 (Kuyruk Yönetimi) görevleri tamamlanmış olmalıdır.
>
> **Ekran Referansı:** İki seçenekli hasta karşılama ekranı ("Have you been here before?")

---

## 1. Veri Modelleri ve Backend API
> 📎 Referans: [phases.md — Veri Yapısı ve Backend](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L706-L716)

- [x] **1.1.** Bu faz sadece frontend (ön yüz) yönlendirmelerini içerdiği için backend gereksinimi yoktur. (Herhangi bir kod yazılmayacak)

---

## 2. Frontend — UI Bileşenleri ve Sayfa Tasarımı
> 📎 Referans: [phases.md — UI/UX Tasarımı ve Kodlama Süreci](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L688-L752)

- [x] **2.1.** `src/pages/PatientWelcomePage.jsx` dosyasını oluştur
- [x] **2.2.** `src/styles/PatientWelcomePage.css` dosyasını oluştur (veya CSS module/tailwind ayarlarını kullan)
- [x] **2.3.** Sayfa tasarımını referans dokümana uygun oluştur:
  - [x] **2.3.1.** Minimal bir Navbar (Sol tarafta `PQMS` logosu, sağda `Patient Welcome` yazısı)
  - [x] **2.3.2.** Ekranı ortalayacak bir kart yapısı (Beyaz kart, hafif gölge)
  - [x] **2.3.3.** Mavi daire içinde `?` ikonu
  - [x] **2.3.4.** Başlık metnini ekle: `Have you been here before, or is this your first visit?`
  - [x] **2.3.5.** Alt başlık metnini ekle: `Please select one of the options below`
- [x] **2.4.** Seçenek butonlarını oluştur ve stillendir:
  - [x] **2.4.1.** Evet butonu: Tam genişlikte, mavi arka planlı (`✓ Yes, I've been here before`)
  - [x] **2.4.2.** Hayır butonu: Tam genişlikte, outlined tarzında (`👤+ No, this is my first visit`)
- [x] **2.5.** Mobil uyumluluğu sağla (Dar ekranlarda kartın düzgün ortalanması)

---

## 3. Frontend — Durum Yönetimi ve Yönlendirme (Routing)
> 📎 Referans: [phases.md — React Durum Yönetimi](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L717-L770)

- [x] **3.1.** `PatientWelcomePage` bileşenine `useNavigate` (React Router) hook'unu ekle
- [x] **3.2.** Evet ("Yes") butonuna tıklandığında `/patient/search` rotasına yönlendirecek `handleExistingPatient` fonksiyonunu yaz
- [x] **3.3.** Hayır ("No") butonuna tıklandığında `/patient/register` rotasına yönlendirecek `handleNewPatient` fonksiyonunu yaz
- [x] **3.4.** `src/App.jsx` dosyasını aç ve Router ayarlarını güncelle:
  - [x] **3.4.1.** `<Route path="/patient/welcome" element={<PatientWelcomePage />} />` rotasını ekle
  - [x] **3.4.2.** (Şimdilik placeholder olarak) `<Route path="/patient/search" element={<div>Search Page</div>} />` rotasını ekle
  - [x] **3.4.3.** (Şimdilik placeholder olarak) `<Route path="/patient/register" element={<div>Register Page</div>} />` rotasını ekle

---

## 4. Test ve Doğrulama
> 📎 Referans: [phases.md — Test Tablosu](file:///c:/Users/Metehan/Desktop/work01-fullstack-app/docs/phases.md#L771-L780)

- [x] **4.1.** `http://localhost:5173/patient/welcome` adresine gidildiğinde sayfanın doğru, ortalanmış 2 butonlu şekilde açıldığını doğrula
- [x] **4.2.** "Yes" butonuna tıklandığında adres çubuğunda `/patient/search` göründüğünü doğrula
- [x] **4.3.** "No" butonuna tıklandığında adres çubuğunda `/patient/register` göründüğünü doğrula
- [x] **4.4.** Geliştirici araçlarından mobil görünüme geçerek sayfanın responsive yapısını kontrol et

---

## 📋 Faz 3 İlerleme Özeti

| # | Görev Grubu | Durum |
|---|-------------|-------|
| 1 | Veri Modelleri ve Backend | ✅ N/A |
| 2 | UI Bileşenleri ve Sayfa Tasarımı | ✅ |
| 3 | Durum Yönetimi ve Yönlendirme | ✅ |
| 4 | Test ve Doğrulama | ✅ |

> ✅ Tüm maddeler tamamlandığında Faz 3 bitmiştir. Faz 4 görevleri için yeni bir task dosyası oluşturulacaktır.
