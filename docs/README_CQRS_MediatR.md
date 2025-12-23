# 🏗️ BioWings CQRS & MediatR Architecture

## 📋 Ne Kullanıyoruz?

BioWings projesinde **CQRS (Command Query Responsibility Segregation)** pattern'ini **MediatR** kütüphanesi ile implement ettik.

### 🎯 Neden CQRS?
- **Separation of Concerns**: Read ve Write işlemleri ayrı
- **Scalability**: Query ve Command'lar bağımsız optimize edilebilir  
- **Maintainability**: Her handler tek sorumluluğa sahip
- **Testability**: Her use case izole test edilebilir

## 🏛️ Mimari Yapı

```
API Controller → MediatR → Handler → Repository → Database
```

### Klasör Yapısı
```
📦 BioWings.Application/
├── 📁 Features/
│   ├── 📁 Commands/           # Write işlemleri
│   │   ├── 📁 ObservationCommands/
│   │   ├── 📁 SpeciesCommands/
│   │   └── 📁 ExportCommands/
│   ├── 📁 Queries/            # Read işlemleri  
│   │   ├── 📁 ObservationQueries/
│   │   ├── 📁 SpeciesQueries/
│   │   └── 📁 ExportQueries/
│   ├── 📁 Handlers/           # Business Logic
│   │   ├── 📁 ObservationHandlers/
│   │   │   ├── 📁 Read/
│   │   │   └── 📁 Write/
│   │   └── 📁 SpeciesHandlers/
│   └── 📁 Results/            # Response Models
└── 📁 DTOs/                   # Data Transfer Objects
```

## 🤔 Request Nedir?

**Request**, MediatR'da tüm command ve query'lerin uyguladığı temel interface'dir. 

### 🎯 IRequest Interface'leri:

#### 1. **IRequest** - Response döndürmeyen işlemler
```csharp
public class DeleteObservationCommand : IRequest
{
    public int Id { get; set; }
}
// Handler'ı: IRequestHandler<DeleteObservationCommand>
```

#### 2. **IRequest<TResponse>** - Response döndüren işlemler  
```csharp
public class GetObservationQuery : IRequest<ServiceResult<ObservationDto>>
{
    public int Id { get; set; }
}
// Handler'ı: IRequestHandler<GetObservationQuery, ServiceResult<ObservationDto>>
```

### 🔍 Request'in Rolü:
- **Giriş Noktası**: Controller'dan gelen data'yı taşır
- **Validasyon**: FluentValidation ile otomatik doğrulama
- **Serialization**: JSON'dan object'e dönüşüm
- **Routing**: MediatR hangi handler'ı çağıracağını bilir

### 📝 Request Örneği:
```csharp
// Bu bir Request (Command)
public class CreateObservationCommand : IRequest<ServiceResult>
{
    public string SpeciesName { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public DateTime ObservationDate { get; set; }
}

// Controller'da kullanımı:
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateObservationCommand request)
{
    var result = await _mediator.Send(request); // Request MediatR'a gönderiliyor
    return Ok(result);
}
```

## 🔧 Temel Bileşenler

### 1. **Commands** - Write İşlemleri
```csharp
public class ObservationCreateCommand : IRequest<ServiceResult>
{
    // Bu property'ler parametre olarak kullanılır
    public string? AuthorityName { get; set; }    // Parametre: Yazar adı
    public string? GenusName { get; set; }        // Parametre: Cins adı
    public string? SpeciesName { get; set; }      // Parametre: Tür adı
    public decimal Latitude { get; set; }         // Parametre: Enlem
    public decimal Longitude { get; set; }        // Parametre: Boylam
}
```

**💡 Property'ler nasıl parametre oluyor?**
- Controller'dan gelen JSON data bu property'lere bind edilir
- Handler'da `request.SpeciesName` şeklinde kullanılır
- Validation rule'ları bu property'ler üzerinde çalışır

### 2. **Queries** - Read İşlemleri
```csharp
public class ObservationGetByIdQuery : IRequest<ServiceResult<ObservationGetByIdQueryResult>>
{
    public int Id { get; set; }    // Parametre: Hangi ID'li kaydı getireceğiz?
}

// Daha karmaşık query örneği:
public class ObservationGetAllQuery : IRequest<ServiceResult<List<ObservationDto>>>
{
    public string? SpeciesName { get; set; }      // Parametre: Tür filtresi (opsiyonel)
    public DateTime? StartDate { get; set; }      // Parametre: Başlangıç tarihi
    public DateTime? EndDate { get; set; }        // Parametre: Bitiş tarihi
    public int Page { get; set; } = 1;            // Parametre: Sayfa numarası
    public int PageSize { get; set; } = 10;       // Parametre: Sayfa boyutu
}
```

**💡 Query parametreleri nasıl kullanılır?**
- URL'den: `/api/observations/5` → `Id = 5`
- Query string'den: `/api/observations?speciesName=Papilio&page=2`
- Body'den: POST request'lerde JSON olarak

### 3. **Handlers** - Business Logic
```csharp
public class ObservationCreateCommandHandler : IRequestHandler<ObservationCreateCommand, ServiceResult>
{
    public async Task<ServiceResult> Handle(ObservationCreateCommand request, CancellationToken cancellationToken)
    {
        // Parametreleri request'ten alıyoruz:
        var observation = new Observation
        {
            SpeciesName = request.SpeciesName,      // ← Parametre kullanımı
            Latitude = request.Latitude,            // ← Parametre kullanımı
            Longitude = request.Longitude,          // ← Parametre kullanımı
            AuthorityName = request.AuthorityName   // ← Parametre kullanımı
        };

        // Validation örneği:
        if (string.IsNullOrEmpty(request.SpeciesName))
            return ServiceResult.Error("SpeciesName parametresi gerekli!");

        return ServiceResult.Success();
    }
}
```

**🎯 Parametreler nasıl Handler'a ulaşır?**
```
JSON Request → Model Binding → Command/Query Properties → Handler.Handle(request)
```

### 🌐 Controller'da Parametre Kullanımı
```csharp
[ApiController]
[Route("api/[controller]")]
public class ObservationsController : BaseController
{
    // URL'den parametre alma: /api/observations/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var query = new ObservationGetByIdQuery { Id = id };  // ← Parametre set ediliyor
        var result = await _mediator.Send(query);
        return CreateResult(result);
    }

    // Query string'den parametre alma: /api/observations?speciesName=Papilio&page=2
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? speciesName, [FromQuery] int page = 1)
    {
        var query = new ObservationGetAllQuery 
        { 
            SpeciesName = speciesName,    // ← Query string parametresi
            Page = page                   // ← Query string parametresi
        };
        var result = await _mediator.Send(query);
        return CreateResult(result);
    }

    // Body'den parametre alma: POST ile JSON
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ObservationCreateCommand command)
    {
        // command object'i zaten JSON'dan bind edilmiş durumda
        // command.SpeciesName, command.Latitude vs. kullanıma hazır
        var result = await _mediator.Send(command);
        return CreateResult(result);
    }
}
```

**📝 Parametre Binding Türleri:**
- `[FromRoute]` → URL path'inden: `/api/observations/{id}`
- `[FromQuery]` → Query string'den: `?name=value&page=1`
- `[FromBody]` → Request body'den: JSON data
- `[FromHeader]` → HTTP header'dan: `Authorization: Bearer token`

### 4. **Results** - Response Models
```csharp
public class ObservationGetByIdQueryResult
{
    public int Id { get; set; }
    public string SpeciesName { get; set; }
    public DateTime ObservationDate { get; set; }
}
```

## 🚀 Yeni API Ekleme Adımları

### Senaryo: Yeni "Family" CRUD API'si ekliyoruz

### 1️⃣ **DTO Oluştur**
```csharp
// BioWings.Application/DTOs/FamilyDtos/FamilyCreateDto.cs
public class FamilyCreateDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
}
```

### 2️⃣ **Commands ve Queries Oluştur**

#### Create Command:
```csharp
// BioWings.Application/Features/Commands/FamilyCommands/FamilyCreateCommand.cs
public class FamilyCreateCommand : IRequest<ServiceResult>
{
    public string Name { get; set; }
    public string? Description { get; set; }
}
```

#### Get Query:
```csharp
// BioWings.Application/Features/Queries/FamilyQueries/FamilyGetByIdQuery.cs
public class FamilyGetByIdQuery : IRequest<ServiceResult<FamilyGetByIdQueryResult>>
{
    public int Id { get; set; }
}
```

### 3️⃣ **Result Models Oluştur**
```csharp
// BioWings.Application/Features/Results/FamilyResults/FamilyGetByIdQueryResult.cs
public class FamilyGetByIdQueryResult
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDateTime { get; set; }
}
```

### 4️⃣ **Handlers Oluştur**

#### Write Handler:
```csharp
// BioWings.Application/Features/Handlers/FamilyHandlers/Write/FamilyCreateCommandHandler.cs
public class FamilyCreateCommandHandler : IRequestHandler<FamilyCreateCommand, ServiceResult>
{
    private readonly IFamilyRepository _familyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FamilyCreateCommandHandler> _logger;

    public FamilyCreateCommandHandler(
        IFamilyRepository familyRepository,
        IUnitOfWork unitOfWork,
        ILogger<FamilyCreateCommandHandler> logger)
    {
        _familyRepository = familyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ServiceResult> Handle(FamilyCreateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var family = new Family
            {
                Name = request.Name,
                Description = request.Description
            };

            await _familyRepository.AddAsync(family, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Family created with ID: {family.Id}");
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating family");
            return ServiceResult.Error("Failed to create family");
        }
    }
}
```

#### Read Handler:
```csharp
// BioWings.Application/Features/Handlers/FamilyHandlers/Read/FamilyGetByIdQueryHandler.cs
public class FamilyGetByIdQueryHandler : IRequestHandler<FamilyGetByIdQuery, ServiceResult<FamilyGetByIdQueryResult>>
{
    private readonly IFamilyRepository _familyRepository;

    public async Task<ServiceResult<FamilyGetByIdQueryResult>> Handle(FamilyGetByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var family = await _familyRepository.GetByIdAsync(request.Id, cancellationToken);
            
            if (family == null)
                return ServiceResult<FamilyGetByIdQueryResult>.Error("Family not found", HttpStatusCode.NotFound);

            var result = new FamilyGetByIdQueryResult
            {
                Id = family.Id,
                Name = family.Name,
                Description = family.Description,
                CreatedDateTime = family.CreatedDateTime
            };

            return ServiceResult<FamilyGetByIdQueryResult>.Success(result);
        }
        catch (Exception ex)
        {
            return ServiceResult<FamilyGetByIdQueryResult>.Error("Failed to get family");
        }
    }
}
```

### 5️⃣ **Repository Interface ve Implementation**

#### Interface:
```csharp
// BioWings.Application/Interfaces/IFamilyRepository.cs
public interface IFamilyRepository : IGenericRepository<Family>
{
    Task<Family?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Family>> GetAllWithGenusCountAsync(CancellationToken cancellationToken = default);
}
```

#### Implementation:
```csharp
// BioWings.Persistence/Repositories/FamilyRepository.cs
public class FamilyRepository : GenericRepository<Family>, IFamilyRepository
{
    public FamilyRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<Family?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(f => f.Name == name, cancellationToken);
    }

    public async Task<List<Family>> GetAllWithGenusCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(f => f.Genera)
            .ToListAsync(cancellationToken);
    }
}
```

### 6️⃣ **API Controller Oluştur**
```csharp
// BioWings.WebAPI/Controllers/FamiliesController.cs
[ApiController]
[Route("api/[controller]")]
public class FamiliesController : BaseController
{
    private readonly IMediator _mediator;

    public FamiliesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [AuthorizeDefinition("Family Management", ActionType.Write, "Create family", AreaNames.Admin)]
    public async Task<IActionResult> Create([FromBody] FamilyCreateCommand command)
    {
        var result = await _mediator.Send(command);
        return CreateResult(result);
    }

    [HttpGet("{id}")]
    [AuthorizeDefinition("Family Management", ActionType.Read, "Get family by ID", AreaNames.Public)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var query = new FamilyGetByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return CreateResult(result);
    }

    [HttpGet]
    [AuthorizeDefinition("Family Management", ActionType.Read, "Get all families", AreaNames.Public)]
    public async Task<IActionResult> GetAll()
    {
        var query = new FamilyGetAllQuery();
        var result = await _mediator.Send(query);
        return CreateResult(result);
    }

    [HttpPut("{id}")]
    [AuthorizeDefinition("Family Management", ActionType.Write, "Update family", AreaNames.Admin)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] FamilyUpdateCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return CreateResult(result);
    }

    [HttpDelete("{id}")]
    [AuthorizeDefinition("Family Management", ActionType.Delete, "Delete family", AreaNames.Admin)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var command = new FamilyDeleteCommand { Id = id };
        var result = await _mediator.Send(command);
        return CreateResult(result);
    }
}
```

### 7️⃣ **Dependency Injection Setup**
```csharp
// BioWings.Persistence/Extensions/PersistenceExtensions.cs
public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
{
    // ... diğer repository'ler
    services.AddScoped<IFamilyRepository, FamilyRepository>();
    
    return services;
}
```

## 🎯 Best Practices

### 1. **Naming Convention**
- **Commands**: `{Entity}{Action}Command` → `ObservationCreateCommand`
- **Queries**: `{Entity}{Action}Query` → `ObservationGetByIdQuery`  
- **Handlers**: `{Command/Query}Handler` → `ObservationCreateCommandHandler`
- **Results**: `{Query}Result` → `ObservationGetByIdQueryResult`

### 2. **Handler Patterns**

#### ✅ Error Handling:
```csharp
try
{
    // Business logic
    return ServiceResult.Success();
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error message");
    return ServiceResult.Error("User-friendly message");
}
```

#### ✅ Validation:
```csharp
public async Task<ServiceResult> Handle(ObservationCreateCommand request, CancellationToken cancellationToken)
{
    // Validation
    if (string.IsNullOrEmpty(request.SpeciesName))
        return ServiceResult.Error("Species name is required");

    // Business logic
}
```

#### ✅ Authorization Check:
```csharp
// Authorization zaten API Controller seviyesinde AuthorizeDefinition ile yapılıyor

```

## 🔄 Integration Flow

### Pratik Örnek:
```
POST /api/Families
{
  "name": "Pieridae",
  "description": "White butterflies"
}

→ FamiliesController.Create()
→ mediator.Send(FamilyCreateCommand)
→ FamilyCreateCommandHandler.Handle()
→ familyRepository.AddAsync()
→ unitOfWork.SaveChangesAsync()
→ ServiceResult.Success()
```