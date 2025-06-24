# Kullanıcı Giriş Takip Sistemi

Bu sistem, admin paneli için kullanıcı giriş aktivitelerini takip eder ve şu özellikleri içerir:

## Özellikler

✅ **Kullanıcı bilgileri**: Kullanıcı ID'si ve adı  
✅ **IP adresi tespiti**: Gerçek IP'yi algılar (proxy arkasındaysa X-Forwarded-For header'ını kontrol eder)  
✅ **Giriş tarihi ve saati**: UTC timezone ile  
✅ **User Agent bilgisi**: Tarayıcı/cihaz bilgisi  
✅ **Giriş durumu**: Başarılı/başarısız durumları  
✅ **Hata sebebi**: Başarısız girişlerde hata detayı  

## API Endpoints

### POST /api/Login
Kullanıcı girişi yapar ve otomatik olarak giriş kaydı oluşturur.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh_token_here",
    "expiration": "2024-06-25T10:00:00Z",
    "roles": ["User"]
  },
  "success": true,
  "message": null
}
```

### GET /api/LoginLog/recent?count=100
Son giriş denemelerini görüntüler (Admin yetkisi gerekli).

**Response:**
```json
[
  {
    "userId": 1,
    "userName": "John Doe",
    "ipAddress": "192.168.1.1",
    "loginDateTime": "2024-06-24T18:30:00Z",
    "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
    "isSuccessful": true,
    "failureReason": null
  }
]
```

### GET /api/LoginLog/user/{userId}
Belirli bir kullanıcının giriş geçmişini görüntüler (Admin yetkisi gerekli).

## Veritabanı Tablosu

`LoginLogs` tablosu şu kolonları içerir:

- `Id` (int, Primary Key)
- `UserId` (int, Foreign Key to Users)
- `UserName` (varchar(100))
- `IpAddress` (varchar(45)) - IPv4 ve IPv6 destekli
- `LoginDateTime` (datetime)
- `UserAgent` (varchar(500))
- `IsSuccessful` (boolean)
- `FailureReason` (varchar(200), nullable)
- `CreatedDateTime` (datetime)
- `UpdatedDateTime` (datetime)

### Indexler
- `IX_LoginLogs_UserId` - Kullanıcıya göre hızlı sorgular
- `IX_LoginLogs_LoginDateTime` - Tarihe göre sıralama
- `IX_LoginLogs_IsSuccessful` - Başarılı/başarısız filtreleme

## IP Adresi Tespiti

Sistem şu sırayla IP adresini belirler:

1. **X-Forwarded-For** header (proxy arkasındaysa)
2. **X-Real-IP** header
3. **CF-Connecting-IP** header (Cloudflare için)
4. **Remote IP** adresi
5. IPv6 localhost (::1) varsa IPv4'e (127.0.0.1) çevirir

## Güvenlik Özellikleri

- ✅ Hassas bilgiler (şifre) log'lanmaz
- ✅ User Agent bilgisi XSS'e karşı güvenli şekilde saklanır
- ✅ IP adresi doğrulama ve temizleme yapılır
- ✅ Admin yetkisi olmadan log'lar görüntülenemez

## Kullanım Örnekleri

### Başarılı Giriş
```http
POST /api/Login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "AdminPass123!"
}
```

### Başarısız Giriş
```http
POST /api/Login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "WrongPassword"
}
```

Her iki durumda da sistem otomatik olarak giriş kaydı oluşturur.

### Admin Panelinde Log Görüntüleme
```http
GET /api/LoginLog/recent?count=50
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Test Etme

1. **Başarılı giriş testi:**
   - Geçerli kullanıcı bilgileriyle POST /api/Login
   - LoginLogs tablosunda kayıt oluştuğunu kontrol et

2. **Başarısız giriş testi:**
   - Yanlış şifre ile POST /api/Login
   - LoginLogs tablosunda `IsSuccessful: false` kaydı oluştuğunu kontrol et

3. **IP adresi testi:**
   - X-Forwarded-For header'ı ile istek gönder
   - Doğru IP'nin kaydedildiğini kontrol et

4. **Admin log görüntüleme:**
   - Admin yetkisiyle GET /api/LoginLog/recent
   - Tüm giriş kayıtlarının listelendiğini kontrol et

## Migration

Sistem otomatik olarak `LoginLogs` tablosunu oluşturur. Manuel migration gerekiyorsa:

```bash
dotnet ef migrations add LoginLogSystem --project BioWings.Persistence --startup-project BioWings.WebAPI
dotnet ef database update --project BioWings.Persistence --startup-project BioWings.WebAPI
``` 