# 🦋 BioWings - Butterfly Observation & Taxonomy System

BioWings, **kelebek gözlemleri** ve taksonomisi üzerine özelleşmiş, **biyolojik veri yönetimi** amacıyla geliştirilmiş kapsamlı bir web uygulamasıdır. Proje, kelebek türlerinin (Familya, Cins, Tür, Alt Tür) sistematik kaydını tutmak, saha gözlemlerini harita üzerinde görselleştirmek ve büyük veri setlerini yüksek performansla yönetmek için tasarlanmıştır.

Proje, **Clean Architecture** prensiplerine uygun olarak, **CQRS** tasarım kalıbı ve **MediatR** kütüphanesi ile modern ve ölçeklenebilir bir kurumsal mimari üzerine inşa edilmiştir.

---

## 🛠 Mimari ve Teknoloji Yığını

Proje modern .NET ekosistemi ve açık kaynak teknolojiler üzerine kurulmuştur.

### Backend & Core
| Teknoloji | Açıklama |
|-----------|----------|
| **.NET 8.0** | ASP.NET Core Web API |
| **Clean Architecture** | CQRS & MediatR Pattern |
| **MySQL** | Entity Framework Core ORM |
| **Redis** | Distributed Cache |
| **SignalR** | Real-time ilerleme bildirimleri |
| **Nominatim** | Geocoding/Reverse Geocoding (Docker) |
| **MailHog** | Development e-posta testi |
| **EPPlus & ExcelDataReader** | Excel import/export |

### Frontend (UI)
| Teknoloji | Açıklama |
|-----------|----------|
| **ASP.NET Core MVC** | Razor Views |
| **Bootstrap & jQuery** | Responsive tasarım |
| **OpenStreetMap & Leaflet.js** | Interaktif harita |

### DevOps & Infrastructure
| Teknoloji | Açıklama |
|-----------|----------|
| **Docker & Docker Compose** | Container orchestration |
| **Serilog** | Structured logging |

---

## ✨ Temel Özellikler

### 📥 1. Gelişmiş Veri Import Sistemi
5 farklı Excel formatını destekleyen, esnek ve yüksek performanslı içe aktarım modülü.
- **Format Analizi:** .xls ve .xlsx desteği, stream bazlı asenkron okuma
- **Akıllı Doğrulama:** Duplicate kontrolü, Dictionary ile cache karşılaştırması
- **Batch Processing:** 1500 kayıtlık gruplar, transaction yönetimi
- **Performans:** `MySqlBulkCopy` ile optimize edilmiş toplu veri ekleme
- **Canlı Takip:** SignalR üzerinden anlık ilerleme gösterimi

### 📤 2. Dinamik Veri Export Sistemi
- **Özelleştirilebilir:** İstenilen sütunların dinamik seçimi
- **Yüksek Performans:** Streaming desteği ve memory optimizasyonu
- **Formatlar:** .xlsx ve .csv çıktı desteği
- **Filtreleme:** İl, Tarih, Tür bazlı veri dışa aktarımı

### 🗺️ 3. Harita Entegrasyonu
Kelebek gözlemlerinin OpenStreetMap üzerinde interaktif gösterimi.
- **Kümeleme (Clustering):** Yakın gözlemlerin gruplandırılması
- **Detaylı Pop-up:** Tür, tarih, gözlemci ve koordinat bilgileri
- **Gelişmiş Filtreleme:** Harita üzerinde multi-kriter filtreleme
- **Geocoding:** Local Nominatim sunucusu ile adres/koordinat dönüşümü

### 🦋 4. Taksonomi ve Gözlem Yönetimi
Kelebek hiyerarşisinin (Familya → Cins → Tür → Alt Tür) tam yönetimi.
- **Server-side Pagination:** Büyük veri setleri için optimize edilmiş sayfalama
- **Anlık Arama:** Bilimsel ad, Türkçe ad vb. üzerinden filtreleme
- **Excel Şablonu:** Toplu tür yükleme desteği
- **Validasyon:** Koordinat doğrulama ve hiyerarşik tutarlılık kontrolü

### ⚡ 5. Performans ve Cache
- **Redis Cache:** Sık kullanılan verilerin önbelleğe alınması
- **Database Indexing:** Composite ve spatial indexler
- **Memory Yönetimi:** Batch işlemlerde Change Tracker optimizasyonu
- **Service Result Pattern:** Standartlaştırılmış API yanıtları

---

## 🔒 Güvenlik Mimarisi

Güvenlik sistemi, UI ve API arasındaki iletişimi güvenli hale getirmek için **Hibrit (Hybrid)** bir yapı kullanır.

| Katman | Yöntem | Açıklama |
|--------|--------|----------|
| **UI** | Cookie-based Auth | Secure, HttpOnly cookie |
| **API** | JWT Token | Access & Refresh token |
| **Yetkilendirme** | RBAC | Veritabanı tabanlı rol/izin |

### Temel Özellikler
- **TokenHandler:** JWT token'ın otomatik header enjeksiyonu
- **PermissionAuthorizationFilter:** Her istekte anlık izin kontrolü
- **Secure by Default:** `[AllowAnonymous]` olmayan endpoint'ler korunur
- **Encryption:** AES-256 ile hassas veri şifreleme

> 📖 Detaylı bilgi: [docs/README_Security_Architecture.md](docs/README_Security_Architecture.md)

---

## 🐳 Docker ile Kurulum

Proje, Docker ile kolayca çalıştırılabilir. Tüm servisler tek komutla ayağa kalkar.

### Gereksinimler
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Windows/Mac) veya Docker Engine (Linux)
- En az **8GB RAM**
- En az **20GB disk alanı**

### 🚀 Hızlı Başlangıç

```bash
# 1. Repo'yu klonla
git clone https://github.com/Butterfly-Lovers/BioWings.git
cd BioWings

# 2. Ortam değişkenlerini yapılandır
cp .env.example .env
# .env dosyasını düzenle ve gerekli değerleri gir

# 3. Tüm servisleri başlat
docker-compose up -d --build
```

### Servis Adresleri

| Servis | URL | Açıklama |
|--------|-----|----------|
| 🌐 **UI** | http://localhost:5000 | Web arayüzü |
| 🔧 **API** | http://localhost:7128 | REST API |
| 📧 **MailHog** | http://localhost:8025 | E-posta test arayüzü |
| 🗄️ **MySQL** | localhost:3307 | Veritabanı |
| ⚡ **Redis** | localhost:6379 | Cache sunucusu |
| 🗺️ **Nominatim** | http://localhost:8081 | Geocoding servisi |

### ✅ Veritabanı Otomatik Kurulum

Docker ilk başlatıldığında veritabanı **örnek verilerle birlikte** otomatik olarak oluşturulur. `db_init/init.sql` dosyası MySQL container'ı başlarken çalıştırılır.

> ⚠️ **Not:** Otomatik import sadece veritabanı volume'ü boşken çalışır. Verileri sıfırlamak için:
> ```bash
> docker-compose down -v
> docker-compose up -d --build
> ```

### 📧 E-posta Testi (MailHog)

Development ortamında tüm e-postalar MailHog'a gönderilir:
1. http://localhost:8025 adresini aç
2. Uygulamadan bir e-posta gönder (şifre sıfırlama vb.)
3. MailHog'da e-postayı görüntüle

### Sık Kullanılan Komutlar

```bash
# Servisleri durdur
docker-compose down

# Servisleri yeniden başlat
docker-compose restart

# Belirli servisi yeniden build et
docker-compose up -d --build biowings-webapi

# Logları izle
docker-compose logs -f biowings-webapi
```

> 📖 Detaylı bilgi: [docs/Docker_Setup.md](docs/Docker_Setup.md)

---

## 📁 Proje Yapısı

```
BioWings/
├── BioWings.Domain/           # Entity'ler, Interface'ler
├── BioWings.Application/      # CQRS Handlers, Business Logic
├── BioWings.Infrastructure/   # External Services (Email, Cache, Geocoding)
├── BioWings.Persistence/      # EF Core, Repository Implementations
├── BioWings.WebAPI/           # REST API Controllers
├── BioWings.UI/               # MVC Frontend
├── db_init/                   # Veritabanı init script
├── docs/                      # Dokümantasyon
├── docker-compose.yml         # Docker servis tanımları
└── .env.example               # Örnek ortam değişkenleri
```

---

## 📚 Dokümantasyon

| Döküman | Açıklama |
|---------|----------|
| [Docker Kurulum Kılavuzu](docs/Docker_Setup.md) | Container kurulum ve yönetimi |
| [Güvenlik Mimarisi](docs/README_Security_Architecture.md) | Authentication & Authorization |
| [API Versiyonlama](docs/Api_Versioning.md) | API versiyonlama stratejisi |

---

## 📄 Lisans

Bu proje açık kaynak olarak geliştirilmektedir.

---

*Son güncelleme: Aralık 2025*
