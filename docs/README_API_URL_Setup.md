# 🌐 BioWings API URL Kullanım Kılavuzu

## 📋 Hızlı Başlangıç

Bu projenin API URL'leri **global olarak** yapılandırılmıştır. Hem C# Controller'larda hem de JavaScript'te aynı konfigürasyonu kullanıyoruz.

## ⚙️ API URL Konfigürasyonu

### 1. Konfigürasyon Dosyaları

#### Development Ortamı
```json
// BioWings.UI/appsettings.Development.json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7128/api",
    "FrontendUrl": "https://localhost:7269"
  }
}
```

#### Production Ortamı
```json
// BioWings.UI/appsettings.json
{
  "ApiSettings": {
    "BaseUrl": "https://api.biowings.com/api",
    "FrontendUrl": "https://biowings.com"
  }
}
```

### 2. JavaScript Global Konfigürasyon

```javascript
// wwwroot/js/apiconfig.js
window.API_CONFIG = {
    BASE_URL: 'https://localhost:7128/api'  // Development URL
};
```

## 🎮 Kullanım Örnekleri

### C# Controller'da Kullanım

```csharp
public class SpeciesController : Controller
{
    private readonly string _baseUrl;

    public SpeciesController(IOptions<ApiSettings> options)
    {
        _baseUrl = options.Value.BaseUrl;
    }

    public async Task<IActionResult> Index()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        
        // ✅ Doğru kullanım
        var response = await client.GetAsync($"{_baseUrl}/Species");
        
        // Veya daha kısa
        var response = await client.GetAsync("Species"); // BaseAddress zaten set edilmiş
        
        return View();
    }
}
```

### JavaScript'te Kullanım

```javascript
// Tür listesi getir
async function getSpecies() {
    const response = await fetch(`${API_CONFIG.BASE_URL}/Species`);
    const data = await response.json();
    return data;
}

// jQuery ile
$.ajax({
    url: `${API_CONFIG.BASE_URL}/Species`,
    type: 'GET',
    success: function(data) {
        console.log(data);
    }
});
```

## 🔧 API URL Değiştirme

### Development'dan Production'a Geçiş

1. **UI Project**: `appsettings.json` dosyasını güncelle
```json
{
  "ApiSettings": {
    "BaseUrl": "https://your-api-domain.com/api"
  }
}
```

2. **JavaScript**: `apiconfig.js` dosyasını güncelle
```javascript
window.API_CONFIG = {
    BASE_URL: 'https://your-api-domain.com/api'
};
```


## 🚀 Deployment Checklist

- [ ] `appsettings.json` dosyasında production URL'leri güncelle
- [ ] `apiconfig.js` dosyasında production URL'leri güncelle  
- [ ] CORS ayarlarını production domain'leri için yapılandır
- [ ] SSL sertifikalarının doğru olduğunu kontrol et


**📞 Hızlı Yardım**: API URL sorunları için sadece `appsettings.json` ve `apiconfig.js` dosyalarını kontrol et! 