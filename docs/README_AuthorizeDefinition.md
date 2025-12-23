# AuthorizeDefinition Attribute Sistemi

Bu sistem, ASP.NET Core MVC projelerinde aksiyon bazlı yetkilendirme için tasarlanmıştır.

## Bileşenler

### 1. ActionType Enum
`BioWings.Domain.Enums.ActionType` - Yetki türlerini tanımlar:
- `Read` - Okuma yetkisi
- `Write` - Yazma/Ekleme yetkisi  
- `Update` - Güncelleme yetkisi
- `Delete` - Silme yetkisi

### 2. AreaNames
`BioWings.Domain.Constants.AreaNames` - Area isimleri için sabit değerler:
- `Admin` - Admin alanı
- `Public` - Public alan (boş string)

### 3. AuthorizeDefinitionAttribute
`BioWings.Domain.Attributes.AuthorizeDefinitionAttribute` - Controller action'larına eklenebilecek attribute.

## Parametreler

- **MenuName**: Hangi menüye ait olduğu (örn: "Kullanıcı Yönetimi")
- **ActionType**: Yetki türü (ActionType enum'undan)
- **Definition**: Açıklayıcı tanım (örn: "Kullanıcı listesini görüntüleme")
- **AreaName**: Hangi Area'ya ait olduğu (AreaNames'ten alınmalı)

## Gelecek Adımlar

1. Bu attribute'ları okuyup yetkilendirme kontrolü yapacak middleware/filter geliştirilebilir
2. Reflection ile tüm controller'ları tarayıp yetki matrisi oluşturulabilir
3. Dinamik menü yapısı bu attribute'lara göre şekillendirilebilir 