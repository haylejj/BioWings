# BioWings Yetkilendirme Sistemi

Bu dokümantasyon, BioWings projesinde kullanılan veritabanı tabanlı yetkilendirme sistemini açıklar.

## Sistem Özellikleri

- **Veritabanı Tabanlı**: Her istekte veritabanından yetki kontrolü yapar (claim tabanlı değil)
- **AuthorizeDefinition Attribute**: Controller action'larına attribute ile yetki tanımlaması
- **PermissionCode Üretimi**: Otomatik olarak unique izin kodları oluşturur
- **Role-Based**: Kullanıcıların rolleri üzerinden izin kontrolü
- **Global Filter**: Tüm controller'lara otomatik olarak uygulanır

## Sistem Bileşenleri

### 1. AuthorizeDefinition Attribute

```csharp
[AuthorizeDefinition("Kullanıcı Yönetimi", ActionType.Read, "Kullanıcı listesini görüntüleme", AreaNames.Admin)]
[HttpGet]
public async Task<IActionResult> GetAll()
{
    // Action implementasyonu
}
```

**Parametreler:**
- `MenuName`: Hangi menüye ait olduğu
- `ActionType`: Yetki türü (Read, Write, Update, Delete)
- `Definition`: Açıklayıcı tanım
- `AreaName`: Hangi Area'ya ait olduğu

### 2. PermissionCode Formatı

```
{AreaName}.{ControllerName}.{ActionName}.{ActionType}.{HttpMethod}
```

**Örnek:**
```
Admin.Users.GetAll.Read.GET
Admin.Users.Create.Write.POST
Admin.Users.Update.Update.PUT
Admin.Users.Remove.Delete.DELETE
```

### 3. Veritabanı Yapısı

```sql
-- Kullanıcılar
User -> UserRole -> Role -> RolePermission -> Permission

-- İzin kontrolü
SELECT p.PermissionCode 
FROM Users u
JOIN UserRoles ur ON u.Id = ur.UserId
JOIN Roles r ON ur.RoleId = r.Id
JOIN RolePermissions rp ON r.Id = rp.RoleId
JOIN Permissions p ON rp.PermissionId = p.Id
WHERE u.Id = @UserId AND p.PermissionCode = @RequiredPermissionCode
```

## Kullanım Kılavuzu

### 1. Controller'a Yetki Ekleme

```csharp
using BioWings.Domain.Attributes;
using BioWings.Domain.Constants;
using BioWings.Domain.Enums;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseController
{
    [HttpGet]
    [AuthorizeDefinition("Ürün Yönetimi", ActionType.Read, "Ürün listesini görüntüleme", AreaNames.Admin)]
    public async Task<IActionResult> GetAll()
    {
        // Implementation
    }

    [HttpPost]
    [AuthorizeDefinition("Ürün Yönetimi", ActionType.Write, "Yeni ürün oluşturma", AreaNames.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        // Implementation
    }
}
```

### 2. JWT Token'da Kullanıcı ID'si

Filter, `ClaimTypes.NameIdentifier` claim'inden kullanıcı ID'sini alır:

```csharp
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Email, user.Email),
    // Diğer claim'ler...
};
```

### 3. Yetki Kontrolü Akışı

1. **Attribute Tespiti**: Action'dan `AuthorizeDefinition` attribute'u bulunur
2. **Kullanıcı Kontrolü**: JWT token'dan kullanıcı ID'si alınır
3. **PermissionCode Üretimi**: Attribute bilgilerinden unique kod oluşturulur
4. **Veritabanı Kontrolü**: Kullanıcının bu izne sahip olup olmadığı kontrol edilir
5. **Sonuç**: İzin varsa devam, yoksa `403 Access Denied`

### 4. Hata Yönetimi

Yetkisiz erişim durumunda kullanıcı şu endpoint'e yönlendirilir:

```
GET /api/Error/access-denied
```

Response:
```json
{
    "error": "Access Denied",
    "message": "Bu işlemi gerçekleştirmek için yetkiniz bulunmamaktadır.",
    "timestamp": "2024-01-15T10:30:00Z",
    "path": "/api/users"
}
```

## Konfigürasyon

### 1. DI Container Kayıtları

`Program.cs`:
```csharp
// Global filter ekleme
builder.Services.AddControllers(options =>
{
    options.Filters.Add<PermissionAuthorizationFilter>();
});
```

`ApplicationExtensions.cs`:
```csharp
// Repository'ler zaten Persistence layer'da kayıtlı
// Filter otomatik olarak constructor injection ile repository'leri alır
```

### 2. Gerekli Repository'ler

- `IUserRoleRepository`: Kullanıcı-rol ilişkileri
- `IRolePermissionRepository`: Rol-izin ilişkileri  
- `IPermissionRepository`: İzin işlemleri
- Filter direkt olarak repository'lerle çalışır, ekstra service layer gerektirmez

## Güvenlik Özellikleri

### 1. Veritabanı Tabanlı Kontrol
- Her istekte fresh data kullanılır
- Gerçek zamanlı yetki değişiklikleri desteklenir
- Cache poisoning riski yoktur

### 2. Loglama
- Tüm yetki kontrolleri loglanır
- Yetkisiz erişim denemeleri kaydedilir
- Debug modda detaylı bilgi sağlanır

### 3. Hata İşleme
- Exception'lar graceful şekilde handle edilir
- Güvenlik açığı vermeyecek şekilde hata mesajları döndürülür

## Performans Optimizasyonu

### 1. Veritabanı Optimizasyonu
```sql
-- Önerilen indexler
CREATE INDEX IX_UserRoles_UserId ON UserRoles (UserId);
CREATE INDEX IX_RolePermissions_RoleId ON RolePermissions (RoleId);
CREATE INDEX IX_Permissions_PermissionCode ON Permissions (PermissionCode);
```

### 2. Caching (Opsiyonel)
Gelecekte performans için cache eklenebilir:
- Redis ile kullanıcı izinleri cache'lenebilir
- Permission değişikliklerinde cache invalidation

## Örnek Kullanım Senaryoları

### 1. Admin Panel
```csharp
[AuthorizeDefinition("Dashboard", ActionType.Read, "Ana sayfa görüntüleme", AreaNames.Admin)]
public IActionResult Dashboard() => View();
```

### 2. Public API
```csharp
[AuthorizeDefinition("Profile", ActionType.Update, "Profil güncelleme", AreaNames.Public)]
public async Task<IActionResult> UpdateProfile(UpdateProfileCommand command)
```

### 3. Hibrit Yetkilendirme
```csharp
// Sadece belirli action'lara yetki kontrolü
[AuthorizeDefinition("Reports", ActionType.Read, "Raporları görüntüleme", AreaNames.Admin)]
public IActionResult ViewReports() => View();

// Bu action'a herkes erişebilir (attribute yok)
public IActionResult PublicInfo() => View();
``` 