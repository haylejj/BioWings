# 🦋 BioWings - Butterfly Observation & Taxonomy System

BioWings, **kelebek gözlemleri** ve taksonomisi üzerine özelleşmiş, **biyolojik veri yönetimi** amacıyla geliştirilmiş kapsamlı bir web uygulamasıdır. Proje, kelebek türlerinin (Familya, Cins, Tür, Alt Tür) sistematik kaydını tutmak, saha gözlemlerini harita üzerinde görselleştirmek ve büyük veri setlerini yüksek performansla yönetmek için tasarlanmıştır.

Proje, **Clean Architecture** prensiplerine uygun olarak, **CQRS** tasarım kalıbı ve **MediatR** kütüphanesi ile modern ve ölçeklenebilir bir kurumsal mimari üzerine inşa edilmiştir.

## 🛠 Mimari ve Teknoloji Yığını

Proje modern .NET ekosistemi ve açık kaynak teknolojiler üzerine kurulmuştur.

### Backend & Core
- **Framework:** .NET 8.0.11 (ASP.NET Core Web API)
- **Mimari:** Clean Architecture (Onion Architecture), CQRS & MediatR Pattern
- **Veritabanı:** MySQL (Entity Framework Core ORM)
- **Real-time İletişim:** SignalR (Canlı ilerleme bildirimleri için)
- **Coğrafi Servisler:** 
  - **Nominatim API:** Geocoding ve Reverse Geocoding işlemleri için.
  - **Docker:** Rate limit sorunlarını aşmak için local Nominatim sunucusu.
- **Excel İşlemleri:** EPPlus ve ExcelDataReader.

### Frontend (UI)
- **Framework:** ASP.NET Core MVC (Razor Views)
- **UI Kütüphaneleri:** Bootstrap, jQuery (Responsive Tasarım)
- **Harita:** OpenStreetMap & Leaflet.js

## ✨ Temel Özellikler

### � 1. Gelişmiş Veri Import Sistemi
5 farklı Excel formatını destekleyen, esnek ve yüksek performanslı içe aktarım modülü.
- **Format Analizi:** .xls ve .xlsx desteği, stream bazlı asenkron okuma (ExcelDataReader).
- **Akıllı Doğrulama:** 
  - Veritabanı entegrasyonu ile duplicate (tekrar eden) kayıt kontrolü.
  - `Dictionary` yapısı ile mevcut verilerin önbelleğe (cache) alınarak karşılaştırılması.
- **Batch Processing:** 1500 kayıtlık gruplar halinde işleme ve her entity için transaction yönetimi.
- **Performans:** `MySqlBulkCopy` ile optimize edilmiş toplu veri ekleme.
- **Canlı Takip:** Import işlemi sırasında **SignalR** üzerinden anlık yüzde (%) ilerleme durumu gösterimi.

### � 2. Dinamik Veri Export Sistemi
- **Özelleştirilebilir:** İstenilen sütunların dinamik olarak seçilebilmesi.
- **Yüksek Performans:** Büyük veri setleri için streaming desteği ve memory optimizasyonu.
- **Formatlar:** .xlsx ve .csv formatlarında çıktı alabilme.
- **Filtreleme:** Mevcut filtre kriterlerine (İl, Tarih, Tür vb.) göre veri dışa aktarımı.

### 🗺️ 3. Harita Entegrasyonu ve Görselleştirme
Kelebek gözlemlerinin OpenStreetMap üzerinde interaktif gösterimi.
- **Kümeleme (Clustering):** Yakın konumdaki gözlemlerin gruplandırılarak (Cluster) gösterilmesi ve zoom seviyesine göre dinamik ayrışma.
- **Detaylı Pop-up:** Gözlem noktasına tıklandığında tür, tarih, gözlemci ve koordinat bilgilerinin gösterimi.
- **Gelişmiş Filtreleme:** Harita üzerinde Tarih, Tür ve Lokasyon bazlı filtreleme.
- **Geocoding:** Adres bilgisinden koordinat, koordinattan adres dönüşümü (Local Nominatim sunucusu ile).

### 🦋 4. Taksonomi ve Gözlem Yönetimi
Kelebek hiyerarşisinin (Familya -> Cins -> Tür -> Alt Tür) ve gözlem verilerinin tam yönetimi.
- **Gözlem (Observation):** 
  - Sunucu taraflı sayfalama (Server-side pagination).
  - Anlık arama ve filtreleme.
  - Koordinat doğrulama ve detaylı form validasyonları.
- **Tür (Species) Yönetimi:** 
  - Excel şablonu ile toplu tür yükleme.
  - Detaylı arama (Bilimsel ad, Türkçe ad).
  - Genus ve Authority ilişkilerinin tutarlılık kontrolü.
- **Familya & Cins (Family & Genus):** Tam CRUD operasyonları ve hiyerarşik validasyonlar.

### ⚡ 5. Performans ve Optimizasyon
- **Veritabanı İndeksleme:** `Species(Name, GenusId)` composite index, `Location` spatial index ve Tarih bazlı indexler.
- **Memory Yönetimi:** Batch işlemler sırasında Change Tracker temizliği ve Dictionary kullanımı.
- **Global Hata Yönetimi:** Service Result pattern ile standartlaştırılmış API yanıtları ve detaylı loglama.

## 🔒 Güvenlik Mimarisi

Güvenlik sistemi, UI ve API arasındaki iletişimi güvenli hale getirmek için **Hibrit (Hybrid)** bir yapı kullanır.

- **Dual Authentication:**
  - **UI Katmanı:** Cookie-based Authentication.
  - **API Katmanı:** JWT (JSON Web Token) based Authentication.
- **Otomatik Token Yönetimi:** UI, API'ye istek atarken araya giren **TokenHandler** mekanizması ile JWT token'ı otomatik olarak header'a ekler.
- **Dinamik Yetkilendirme (RBAC):**
  - Veritabanı tabanlı Rol ve İzin (Permission) yönetimi.
  - `[AuthorizeDefinition]` attribute'u ile kod tarafında deklaratif izin tanımlamaları.
  - Her istekte anlık izin kontrolü sağlayan **PermissionAuthorizationFilter**.

