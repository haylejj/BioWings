# 🔐 BioWings Güvenlik Mimarisi Dokümantasyonu

## 📋 İçindekiler
- [Genel Bakış](#-genel-bakış)
- [Güvenlik Mimarisi](#-güvenlik-mimarisi)
- [Authentication (Kimlik Doğrulama)](#-authentication-kimlik-doğrulama)
- [Authorization (Yetkilendirme)](#-authorization-yetkilendirme)
- [API Client ve BaseUrl Yönetimi](#-api-client-ve-baseurl-yönetimi)
- [JavaScript Token Yönetimi](#-javascript-token-yönetimi)
- [AuthorizeDefinition Attribute Sistemi](#-authorizedefinition-attribute-sistemi)
- [Permission Authorization Filter](#-permission-authorization-filter)
- [JWT Token Sistemi](#-jwt-token-sistemi)
- [Güvenlik Konfigürasyonları](#-güvenlik-konfigürasyonları)
- [Kullanım Örnekleri](#-kullanım-örnekleri)
- [Best Practices](#-best-practices)

## 🎯 Genel Bakış

BioWings projesi, modern web uygulamaları için tasarlanmış **hibrit güvenlik mimarisi** kullanmaktadır. Bu mimari, hem kullanıcı deneyimini hem de güvenliği maksimize etmek için tasarlanmıştır.

### Temel Özellikler
- ✅ **Dual Authentication**: UI için Cookie-based, API için JWT-based
- ✅ **Dynamic Authorization**: Veritabanı tabanlı rol-izin sistemi
- ✅ **Automatic Token Management**: Otomatik JWT token yönetimi
- ✅ **Attribute-Based Security**: Declarative güvenlik tanımlamaları
- ✅ **Real-time Permission Check**: Her istekte canlı izin kontrolü

## 🏗️ Güvenlik Mimarisi

### Mimari Akışı

```
Client Request → UI Layer → Authentication → TokenHandler → API Layer → PermissionFilter → Database Check → Response
```

### Mimari Katmanları

| Katman | Teknoloji | Sorumluluk |
|--------|-----------|------------|
| **Frontend** | ASP.NET Core MVC | Cookie-based authentication |
| **API Gateway** | TokenHandler | Otomatik JWT token ekleme |
| **Authorization** | PermissionAuthorizationFilter | Dinamik izin kontrolü |
| **Backend** | ASP.NET Core Web API | JWT-based authentication |
| **Database** | Entity Framework Core | Rol-izin veri yönetimi |

## 🔑 Authentication (Kimlik Doğrulama)

### 1. UI Tarafı - Cookie Authentication

```csharp
// Program.cs - UI Project
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = true;
        options.LoginPath = "/Login/Login";
        options.LogoutPath = "/Logout/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });
```

**Özellikler:**
- **HttpOnly**: XSS saldırılarına karşı koruma
- **SecurePolicy**: HTTPS zorunluluğu
- **SlidingExpiration**: Otomatik oturum yenileme
- **Custom Paths**: Özel login/logout sayfaları

### 2. API Tarafı - JWT Authentication

```csharp
// Program.cs - WebAPI Project
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
    };
});
```

## 🛡️ Authorization (Yetkilendirme)

### Veritabanı Şeması

```
Users
├── UserRoles (Many-to-Many)
│   └── Roles
│       └── RolePermissions (Many-to-Many)
│           └── Permissions
```

### İzin Kontrolü Akışı

1. **Kullanıcı Rollerini Al**: `UserRoles` tablosundan kullanıcının rollerini getir
2. **Rol İzinlerini Al**: `RolePermissions` tablosundan rollerin izinlerini getir
3. **Permission Code Kontrolü**: Gerekli izin kodunu veritabanında ara
4. **Sonuç**: İzin varsa devam et, yoksa 403 döndür

## 🌐 API Client ve BaseUrl Yönetimi

### ApiSettings Konfigürasyonu

```csharp
// BioWings.Domain/Configuration/ApiSettings.cs
public class ApiSettings
{
    public string BaseUrl { get; set; }      // API base URL
    public string FrontendUrl { get; set; }  // Frontend URL
}
```

### Ortam Bazlı Konfigürasyon

#### Development Environment
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7128/api",
    "FrontendUrl": "https://localhost:7269"
  }
}
```

#### Production Environment
```json
{
  "ApiSettings": {
    "BaseUrl": "https://your-production-api.com/api",
    "FrontendUrl": "https://your-production-frontend.com"
  }
}
```

### HttpClient Konfigürasyonu

```csharp
// Program.cs - UI Project
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<TokenHandler>();

builder.Services.AddHttpClient("ApiClient")
   .AddHttpMessageHandler<TokenHandler>();

builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));
```

### Neden API Client Kullanıyoruz?

| Avantaj | Açıklama |
|---------|----------|
| **Merkezi Yönetim** | Tüm API URL'leri tek yerden kontrol edilir |
| **Environment Flexibility** | Farklı ortamlar için farklı konfigürasyonlar |
| **Automatic Token Injection** | Her API isteğine otomatik token eklenir |
| **Error Handling** | Merkezi hata yönetimi |
| **Logging** | Tüm API istekleri loglanabilir |

## 📱 JavaScript Token Yönetimi

### Otomatik Token Ekleme Sistemi

Frontend'de tüm AJAX ve Fetch istekleri otomatik olarak JWT token alır:

#### jQuery AJAX için Global Setup

```javascript
$(document).ready(function() {
    $.ajaxSetup({
        beforeSend: function(xhr) {
            const token = '@(User.FindFirst("AccessToken")?.Value ?? "")';
            if (token) {
                xhr.setRequestHeader('Authorization', 'Bearer ' + token);
            }
        }
    });
});
```

#### Fetch API için Global Override

```javascript
(function() {
    const originalFetch = window.fetch;
    const token = document.querySelector('meta[name="access-token"]')?.content;

    window.fetch = function(url, options = {}) {
        if (token) {
            options.headers = {
                ...options.headers,
                'Authorization': `Bearer ${token}`
            };
        }
        return originalFetch(url, options);
    };
})();
```

### TokenHandler - C# Tarafında Otomatik Token Ekleme

```csharp
// BioWings.UI/Handler/TokenHandler.cs
public class TokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = httpContextAccessor.HttpContext?.User.FindFirst("AccessToken")?.Value;

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
```

## 🎯 AuthorizeDefinition Attribute Sistemi

### Attribute Tanımı

```csharp
// BioWings.Domain/Attributes/AuthorizeDefinitionAttribute.cs
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class AuthorizeDefinitionAttribute : Attribute
{
    public string MenuName { get; }      // Hangi menüye ait
    public ActionType ActionType { get; } // İzin türü
    public string Definition { get; }     // Açıklayıcı tanım
    public string AreaName { get; }      // Hangi area'ya ait

    public AuthorizeDefinitionAttribute(string menuName, ActionType actionType, 
                                      string definition, string areaName)
    {
        MenuName = menuName ?? throw new ArgumentNullException(nameof(menuName));
        ActionType = actionType;
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        AreaName = areaName ?? throw new ArgumentNullException(nameof(areaName));
    }
}
```

### ActionType Enum

```csharp
public enum ActionType
{
    Read,    // Okuma yetkisi
    Write,   // Yazma/Ekleme yetkisi
    Update,  // Güncelleme yetkisi
    Delete   // Silme yetkisi
}
```

### Kullanım Örnekleri

#### Basit Kullanım
```csharp
[HttpGet]
[AuthorizeDefinition("Kullanıcı Yönetimi", ActionType.Read, 
                    "Kullanıcı listesini görüntüleme", AreaNames.Admin)]
public async Task<IActionResult> GetAll()
{
    // Implementation
}
```

#### Farklı İzin Türleri
```csharp
// CRUD Operations için farklı izinler
[HttpGet]
[AuthorizeDefinition("Ürün Yönetimi", ActionType.Read, "Ürün listesi", AreaNames.Public)]
public async Task<IActionResult> GetProducts() { }

[HttpPost]
[AuthorizeDefinition("Ürün Yönetimi", ActionType.Write, "Yeni ürün ekleme", AreaNames.Admin)]
public async Task<IActionResult> CreateProduct() { }

[HttpPut]
[AuthorizeDefinition("Ürün Yönetimi", ActionType.Update, "Ürün güncelleme", AreaNames.Admin)]
public async Task<IActionResult> UpdateProduct() { }

[HttpDelete]
[AuthorizeDefinition("Ürün Yönetimi", ActionType.Delete, "Ürün silme", AreaNames.Admin)]
public async Task<IActionResult> DeleteProduct() { }
```

## ⚡ Permission Authorization Filter

### Filter Yapısı

```csharp
public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // 1. AuthorizeDefinition attribute'unu bul
        var authorizeDefinition = GetAuthorizeDefinitionAttribute(context);

        // 2. Kullanıcı authentication kontrolü
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
            return;
        }

        // 3. Permission Code oluştur
        var permissionCode = GeneratePermissionCode(context, authorizeDefinition);

        // 4. Veritabanından izin kontrolü
        var hasPermission = await HasUserPermissionAsync(userId, permissionCode);

        // 5. Sonuca göre işlem yap
        if (!hasPermission)
        {
            // API request ise JSON response
            if (IsApiRequest(context.HttpContext))
            {
                context.Result = new JsonResult(new
                {
                    error = "Access Denied",
                    message = "Bu işlemi gerçekleştirmek için yetkiniz bulunmamaktadır."
                })
                {
                    StatusCode = 403
                };
            }
            else
            {
                // UI request ise redirect
                context.Result = new RedirectToActionResult("AccessDenied", "Error", null);
            }
        }
    }
}
```

### Permission Code Formatı

```
Format: {AreaName}.{ControllerName}.{ActionName}.{ActionType}.{HttpMethod}

Örnekler:
- Admin.Users.GetAll.Read.GET
- Admin.Users.Create.Write.POST
- Admin.Users.Update.Update.PUT
- Admin.Users.Delete.Delete.DELETE
- Public.Products.GetById.Read.GET
```

### Permission Code Üretim Algoritması

```csharp
private string GeneratePermissionCode(AuthorizationFilterContext context, 
                                    AuthorizeDefinitionAttribute authorizeDefinition)
{
    var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

    var areaName = string.IsNullOrWhiteSpace(authorizeDefinition.AreaName) 
                   ? "Global" : authorizeDefinition.AreaName;
    var controllerName = actionDescriptor?.ControllerName ?? "Unknown";
    var actionName = actionDescriptor?.ActionName ?? "Unknown";
    var actionType = authorizeDefinition.ActionType.ToString();
    var httpMethod = context.HttpContext.Request.Method;

    return $"{areaName}.{controllerName}.{actionName}.{actionType}.{httpMethod}";
}
```

## 🔐 JWT Token Sistemi

### Token Oluşturma

```csharp
public TokenResponse CreateToken(User user, List<UserRoleGetByUserIdDto> roles)
{
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim()),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

    // Rolleri claims'e ekle
    foreach (var role in roles)
    {
        claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
    }

    var authSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]));
    var tokenExpirationHours = Convert.ToDouble(
        configuration["JwtSettings:AccessTokenExpiration"]);
    var tokenExpiration = DateTime.Now.AddHours(tokenExpirationHours);

    var token = new JwtSecurityToken(
        issuer: configuration["JwtSettings:Issuer"],
        audience: configuration["JwtSettings:Audience"],
        expires: tokenExpiration,
        claims: claims,
        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
    );

    return new TokenResponse
    {
        AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
        RefreshToken = CreateRefreshToken(),
        Expiration = tokenExpiration
    };
}
```

### JWT Konfigürasyonu

```json
{
  "JwtSettings": {
    "SecretKey": "V3ryS3cur3K3yF0rJWT_S1gn1ng_BioWings2025!",
    "Issuer": "https://localhost:7128",
    "Audience": "https://localhost:7269",
    "AccessTokenExpiration": 60,
    "RefreshTokenExpiration": 7
  }
}
```

## ⚙️ Güvenlik Konfigürasyonları

### CORS Konfigürasyonu

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvcApp", builder =>
    {
        builder.WithOrigins("https://localhost:7269") // MVC HTTPS port
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
```

### Global Filter Kaydı

```csharp
builder.Services.AddControllers(options =>
{
    options.Filters.Add<PermissionAuthorizationFilter>();
});
```

## 📝 Kullanım Örnekleri

### Controller'da Güvenlik Tanımlaması

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseController
{
    [HttpGet]
    [AuthorizeDefinition("Ürün Yönetimi", ActionType.Read, 
                        "Ürün listesini görüntüleme", AreaNames.Public)]
    public async Task<IActionResult> GetAll()
    {
        // Tüm kullanıcılar görebilir
    }

    [HttpPost]
    [AuthorizeDefinition("Ürün Yönetimi", ActionType.Write, 
                        "Yeni ürün ekleme", AreaNames.Admin)]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        // Sadece admin rolündeki kullanıcılar ekleyebilir
    }

    [HttpPut("{id}")]
    [AuthorizeDefinition("Ürün Yönetimi", ActionType.Update, 
                        "Ürün güncelleme", AreaNames.Admin)]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
    {
        // Sadece admin rolündeki kullanıcılar güncelleyebilir
    }

    [HttpDelete("{id}")]
    [AuthorizeDefinition("Ürün Yönetimi", ActionType.Delete, 
                        "Ürün silme", AreaNames.Admin)]
    public async Task<IActionResult> Delete(int id)
    {
        // Sadece admin rolündeki kullanıcılar silebilir
    }
}
```

### UI Controller'da API Kullanımı

```csharp
public class ProductController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    public ProductController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = options.Value.BaseUrl;
    }

    public async Task<IActionResult> Index()
    {
        // ApiClient otomatik olarak JWT token ekleyecek
        var client = _httpClientFactory.CreateClient("ApiClient");
        var response = await client.GetAsync($"{_baseUrl}/Products");
        
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<ProductViewModel>>(content);
            return View(products);
        }
        
        return View(new List<ProductViewModel>());
    }
}
```

### JavaScript'te API Kullanımı

```javascript
// Otomatik token ekleme sayesinde normal fetch kullanabilirsiniz
async function getProducts() {
    try {
        const response = await fetch('/api/Products');
        if (response.ok) {
            const products = await response.json();
            console.log('Products:', products);
        } else {
            console.error('API Error:', response.status);
        }
    } catch (error) {
        console.error('Network Error:', error);
    }
}
