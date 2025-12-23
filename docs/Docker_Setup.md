# 🐳 BioWings Docker Kurulum ve Kullanım Kılavuzu

Bu döküman, BioWings uygulamasının Docker ile nasıl çalıştırılacağını açıklamaktadır.

## 📋 İçindekiler

- [Gereksinimler](#gereksinimler)
- [Hızlı Başlangıç](#hızlı-başlangıç)
- [Servisler](#servisler)
- [Ortam Değişkenleri](#ortam-değişkenleri)
- [Veritabanı İşlemleri](#veritabanı-işlemleri)
- [E-posta Test (MailHog)](#e-posta-test-mailhog)
- [Sık Kullanılan Komutlar](#sık-kullanılan-komutlar)
- [Sorun Giderme](#sorun-giderme)

---

## Gereksinimler

- **Docker Desktop** (Windows/Mac) veya **Docker Engine** (Linux)
- **Docker Compose** v2.0+
- En az **8GB RAM** (Nominatim için önerilir)
- En az **20GB disk alanı**

---

## Hızlı Başlangıç

### 1. Ortam Değişkenlerini Yapılandır

```bash
# .env.example dosyasını kopyala
cp .env.example .env

# .env dosyasını düzenle ve gerekli değerleri gir
```

### 2. Docker Container'larını Başlat

```bash
# Tüm servisleri build et ve başlat
docker-compose up -d --build

# Logları izle
docker-compose logs -f
```

### 3. Veritabanını İçeri Aktar

MySQL Workbench veya benzeri bir araç kullanarak:
- **Host:** `localhost`
- **Port:** `3307`
- **User:** `root`
- **Password:** `.env` dosyasındaki `MYSQL_ROOT_PASSWORD`

SQL yedeğinizi `db_biowings` şemasına import edin.

### 4. Uygulamaya Eriş

| Servis | URL |
|--------|-----|
| **UI (Frontend)** | http://localhost:5000 |
| **API (Backend)** | http://localhost:7128 |
| **MailHog (Email Test)** | http://localhost:8025 |
| **MySQL** | localhost:3307 |
| **Redis** | localhost:6379 |
| **Nominatim** | http://localhost:8081 |

---

## Servisler

### 🦋 biowings-ui
ASP.NET Core MVC tabanlı kullanıcı arayüzü.

### 🔧 biowings-webapi
RESTful API sunucusu. Tüm iş mantığı burada.

### 🗄️ biowings-db
MySQL 8.0 veritabanı. Veriler `db_data` volume'ünde saklanır.

### ⚡ biowings-redis
Redis cache sunucusu. Performans optimizasyonu için kullanılır.

### 🗺️ biowings-nominatim
OpenStreetMap tabanlı geocoding servisi. Türkiye harita verilerini kullanır.

### 📧 biowings-mailhog
Development ortamında e-posta testleri için. Gönderilen tüm e-postalar yakalanır.

---

## Ortam Değişkenleri

Tüm konfigürasyon `.env` dosyasından okunur. Ana kategoriler:

### Veritabanı
```env
MYSQL_ROOT_PASSWORD=güçlü_şifre
MYSQL_DATABASE=db_biowings
MYSQL_USER=user
MYSQL_PASSWORD=güçlü_şifre
DB_PORT=3307
```

### JWT Güvenlik
```env
JWT_ISSUER=http://localhost:7128
JWT_AUDIENCE=http://localhost:5000
JWT_SECRET_KEY=en_az_32_karakter_güçlü_anahtar
JWT_ACCESS_TOKEN_EXPIRATION=60
JWT_REFRESH_TOKEN_EXPIRATION=7
```

### E-posta (Production)
```env
EMAIL_SMTP_SERVER=smtp.gmail.com
EMAIL_PORT=587
EMAIL_USERNAME=email@gmail.com
EMAIL_PASSWORD=app_password
```

### MailHog (Development)
```env
MAILHOG_SMTP_PORT=1025
MAILHOG_UI_PORT=8025
```

> ⚠️ **ÖNEMLİ:** `.env` dosyası asla Git'e commit edilmemelidir!

---

## Veritabanı İşlemleri

### Yedek Alma

```bash
docker exec biowings-biowings-db-1 mysqldump -u root -p$MYSQL_ROOT_PASSWORD db_biowings > backup.sql
```

### Yedek Geri Yükleme

```bash
docker exec -i biowings-biowings-db-1 mysql -u root -p$MYSQL_ROOT_PASSWORD db_biowings < backup.sql
```

### MySQL Shell'e Bağlanma

```bash
docker exec -it biowings-biowings-db-1 mysql -u root -p
```

### Case Sensitivity

Docker'da MySQL Linux üzerinde çalıştığı için tablo isimleri case-sensitive'dir. `docker-compose.yml` dosyasında `--lower_case_table_names=1` ayarı ile bu sorun çözülmüştür.

---

## E-posta Test (MailHog)

Development ortamında tüm e-postalar MailHog'a gönderilir:

1. **MailHog UI:** http://localhost:8025 adresini aç
2. Uygulamadan bir e-posta gönder (şifre sıfırlama, kayıt onayı vb.)
3. MailHog'da e-postayı görüntüle

> 💡 **Not:** MailHog gerçek e-posta göndermez, sadece yakalar ve görüntüler.

---

## Sık Kullanılan Komutlar

### Servisleri Yönet

```bash
# Tüm servisleri başlat
docker-compose up -d

# Tüm servisleri durdur
docker-compose down

# Servisleri yeniden başlat
docker-compose restart

# Belirli bir servisi yeniden build et
docker-compose up -d --build biowings-webapi
```

### Logları Görüntüle

```bash
# Tüm loglar
docker-compose logs -f

# Belirli servis logu
docker-compose logs -f biowings-webapi
docker-compose logs -f biowings-ui

# Son 50 satır
docker-compose logs --tail 50 biowings-webapi
```

### Container'a Bağlan

```bash
# WebAPI container'a bash ile bağlan
docker exec -it biowings-biowings-webapi-1 bash

# MySQL container'a bağlan
docker exec -it biowings-biowings-db-1 bash
```

### Temizlik

```bash
# Servisleri ve volume'leri sil (VERİLER SİLİNİR!)
docker-compose down -v

# Kullanılmayan image'ları temizle
docker image prune -a

# Tüm Docker cache'i temizle
docker system prune -a
```

---

## Sorun Giderme

### ❌ "Table doesn't exist" hatası

MySQL case-sensitivity sorunu. Çözüm:
```bash
docker-compose down -v
docker-compose up -d --build
# Sonra veritabanını yeniden import et
```

### ❌ API'ye bağlanılamıyor (ERR_SSL_PROTOCOL_ERROR)

Tarayıcıdan JavaScript, HTTPS ile API'ye bağlanmaya çalışıyor. `.env` dosyasında:
```env
FRONTEND_API_URL=http://localhost:7128/api
```
değerinin doğru olduğundan emin ol.

### ❌ E-posta gönderilemiyor

1. MailHog container'ının çalıştığını kontrol et:
   ```bash
   docker ps | grep mailhog
   ```

2. WebAPI container'ında MailHog ayarlarını kontrol et:
   ```bash
   docker exec biowings-biowings-webapi-1 printenv | grep MailHog
   ```

### ❌ Container başlamıyor

Logları kontrol et:
```bash
docker-compose logs biowings-webapi
```

Yaygın nedenler:
- Port zaten kullanımda
- Veritabanı bağlantı hatası
- Eksik ortam değişkeni

### ❌ Tarayıcı eski dosyaları gösteriyor

Tarayıcı cache'i temizle:
- **Ctrl+Shift+Delete** → Cache sil
- Veya **Incognito mod** kullan

---

## 📁 Dosya Yapısı

```
BioWings/
├── docker-compose.yml      # Docker servis tanımları
├── .env                    # Ortam değişkenleri (gitignore'da)
├── .env.example            # Örnek ortam değişkenleri
├── BioWings.WebAPI/
│   └── Dockerfile          # WebAPI Docker image tanımı
├── BioWings.UI/
│   └── Dockerfile          # UI Docker image tanımı
└── docs/
    └── Docker_Setup.md     # Bu döküman
```

---

## 🔗 İlgili Dökümanlar

- [README.md](../README.md) - Proje genel bakış
- [Security_Architecture.md](./README_Security_Architecture.md) - Güvenlik mimarisi
- [Api_Versioning.md](./Api_Versioning.md) - API versiyonlama

---

*Son güncelleme: Aralık 2025*
