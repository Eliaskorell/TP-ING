## Objeto de dominio agregado: Categoría

### Descripción

Una **Categoría** agrupa productos del catálogo (por ejemplo "Bebidas", "Limpieza"). El sistema no tenía ninguna forma de clasificar productos, así que no duplica funcionalidad existente. Es un aggregate propio (`Domain/CategoriaAggregate`) y se relaciona con Producto en una relación **1 a muchos**: una categoría tiene muchos productos y cada producto tiene como máximo una categoría.

### Reglas de negocio

| Regla | Dónde se controla |
|---|---|
| El nombre es obligatorio (no vacío ni en blanco) | Validator del endpoint y entidad (`Guard.Against.NullOrWhiteSpace`) |
| El nombre (y la descripción) se guardan sin espacios de más al principio ni al final | Entidad (`.Trim()`), y el handler de creación busca duplicados ya con el nombre recortado |
| El nombre tiene como máximo 100 caracteres | Validator, entidad y base (`HasMaxLength`) |
| La descripción es opcional y tiene como máximo 500 caracteres | Validator, entidad y base |
| Si la descripción viene vacía o con solo espacios, se guarda como null (no como cadena vacía) | Entidad |
| No puede haber dos categorías con el mismo nombre, sin importar mayúsculas | Handler (`CategoriaPorNombreSpec`) → 409, e índice único en la base |
| El Id lo genera el sistema y no puede ser negativo | `CategoriaId` (Vogen) y `VogenIntIdValueGenerator` |
| Un producto puede no tener categoría | `Product.CategoriaId` es nullable; FK opcional en la base |

### Endpoints

#### `POST /Categorias` — Crear una categoría

Request:

```json
{
  "nombre": "Bebidas",
  "descripcion": "Gaseosas, jugos y aguas"
}
```

| Respuesta | Cuándo | Ejemplo |
|---|---|---|
| `201 Created` | Se creó la categoría. Header `Location: /Categorias/1` | `{"id":1,"nombre":"Bebidas","descripcion":"Gaseosas, jugos y aguas"}` |
| `400 Bad Request` | Nombre vacío o demasiado largo, o descripción demasiado larga | `{"statusCode":400,"message":"One or more errors occurred!","errors":{"nombre":["El nombre es obligatorio"]}}` |
| `409 Conflict` | Ya existe una categoría con ese nombre | `{"title":"No se pudo crear","status":409,"detail":"Ya existe una categoria con ese nombre"}` |

#### `GET /Categorias/{id}` — Obtener una categoría por su Id

| Respuesta | Cuándo | Ejemplo |
|---|---|---|
| `200 OK` | La categoría existe | `{"id":1,"nombre":"Bebidas","descripcion":"Gaseosas, jugos y aguas"}` |
| `400 Bad Request` | El Id es 0 o negativo | `{"statusCode":400,"message":"One or more errors occurred!","errors":{"categoriaId":["El Id de la categoria debe ser mayor a 0"]}}` |
| `404 Not Found` | No existe una categoría con ese Id | (sin cuerpo) |

#### `PUT /Products/{ProductId}/Categoria` — Asignarle una categoría a un producto

Request:

```json
{
  "categoriaId": 1
}
```

| Respuesta | Cuándo | Ejemplo |
|---|---|---|
| `200 OK` | Se asignó la categoría al producto | `{"id":1,"name":"Coffee Mug","unitPrice":9.99,"categoriaId":1}` |
| `400 Bad Request` | `ProductId` o `categoriaId` es 0 o negativo | `{"statusCode":400,"message":"One or more errors occurred!","errors":{"categoriaId":["El Id de la categoria debe ser mayor a 0"]}}` |
| `404 Not Found` | No existe el producto, o no existe la categoría (se verifica antes de guardar, para que nunca salte un error por la clave foránea) | (sin cuerpo) |

Todas estas solicitudes están listas para ejecutar en `src/ISII.Web/api.http`.

### Recorrido de una solicitud (crear)

1. **Endpoint** (`CategoriaFeatures/Crear/CrearEndpoint.cs`): FastEndpoints recibe el JSON y lo convierte en `CrearCategoriaRequest`.
2. **Validator** (`CrearCategoriaValidator`): antes de ejecutar el endpoint se validan los datos con FluentValidation. Si algo falla responde 400 y no sigue.
3. **Mediator**: el endpoint arma un `CrearCategoriaCommand` y lo envía con `mediator.Send`.
4. **Handler** (`CrearCategoriaHandler`): busca con `CategoriaPorNombreSpec` si el nombre ya existe (→ `Result.Conflict`, 409). Si no, crea la entidad con `Categoria.Crear(...)`, que vuelve a validar las reglas de dominio.
5. **Repositorio** (`EfRepository<Categoria>`): `AddAsync` agrega la entidad y guarda los cambios.
6. **EF Core + SQLite**: `CategoriaConfiguration` define cómo se mapea a la tabla `Categorias`; el `VogenIntIdValueGenerator` asigna el próximo Id.
7. **Respuesta**: el handler devuelve un `CategoriaDto`, que el endpoint convierte en `CategoriaRecord` y responde 201 con `ToCreatedResult`.

La consulta por Id sigue el mismo camino con `ObtenerPorIdEndpoint` → `ObtenerCategoriaPorIdValidator` → `ObtenerCategoriaQuery` → `ObtenerCategoriaHandler` (usa `IReadRepository`, de solo lectura, y `CategoriaPorIdSpec`).

### Pruebas (`tests/ISII.Tests`)

| Archivo | Qué verifica |
|---|---|
| `CategoriaTest.cs` | Reglas de dominio: valores asignados, Id nuevo, nombre vacío/null/largo, descripción larga, Id negativo |
| `CrearCategoriaValidatorTest.cs` | Validación del request de creación (casos válidos e inválidos) |
| `ObtenerCategoriaPorIdValidatorTest.cs` | Validación del Id en la consulta |
| `CategoriaPorNombreSpecTest.cs` | La búsqueda por nombre no distingue mayúsculas |
| `CrearCategoriaHandlerTest.cs` | Lógica de aplicación: crea si el nombre no existe; devuelve Conflict y no guarda si ya existe (repositorio simulado con NSubstitute) |
| `ObtenerCategoriaHandlerTest.cs` | Devuelve la categoría si existe o NotFound si no |
| `ProductTest.cs` | Un producto arranca sin categoría y `AsignarCategoria` la asigna |
| `AsignarCategoriaValidatorTest.cs` | Validación de los Ids del request (producto y categoría) |
| `AsignarCategoriaHandlerTest.cs` | NotFound si no existe el producto, NotFound y no guarda si no existe la categoría, asigna y guarda en el caso feliz |

Se ejecutan con `dotnet test`.

## Getting Started

### Build and Run

```powershell
# Build the solution
dotnet build

# Run the application
dotnet run --project src/ISII.Web

```

### Database Setup

#### Run Web project directly (SQLite LocalDB)

If running the Web project without Aspire, update `appsettings.json` to use LocalDB:

```powershell
dotnet ef database update -c AppDbContext -p src/ISII.Web -s src/ISII.Web
dotnet run --project src/ISII.Web
```

## Project Structure

This template uses a **single Web project** organized by **vertical slices** (features):

```text
src/ISII.Web/
├── Domain/                    # Domain entities and aggregates
│   ├── CartAggregate/
│   ├── OrderAggregate/
│   └── ProductAggregate/
├── Infrastructure/            # Data access and external services
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   ├── Config/           # EF Core configurations
│   │   └── Migrations/
│   └── Email/                # Email services
├── Endpoints/                 # API endpoints (FastEndpoints)
│   ├── Cart/
│   ├── Order/
│   └── Product/
└── Program.cs                # Application startup
```

### Key Design Decisions

```text
Single project vertical slice architecture
```

- **Single Project**: All code in one Web project - simpler dependencies, faster builds
- **Vertical Slices**: Organized by feature (Cart, Order, Product) not layer
- **Domain-Driven Design**: Entities use proper encapsulation and business logic
- **FastEndpoints**: REPR pattern for clean, testable API endpoints
- **Entity Framework Core**: Simple data access with SQLite (easily switched to SQL Server)
- **Mediator Pattern**: Optional - use for cross-cutting concerns or remove for simplicity

## What's Different from Full Clean Architecture?

This minimal template simplifies the full Clean Architecture template:

| Full Template | Minimal Template |
|--------------|------------------|
| 4+ projects (Core, UseCases, Infrastructure, Web) | 1 Web project |
| Repository pattern with Specifications | Repository pattern with Specifications if needed |
| Extensive use of interfaces and abstractions | Pragmatic abstractions where needed |
| Separate Use Cases project with Mediator | Optional Mediator; logic can be in endpoints |
| Complex domain patterns (Aggregates, Value Objects, Domain Events) | Pragmatic DDD patterns (Aggregates, Value Objects) |

## When to Use This Template

**Use Minimal Clean Architecture when:**

- ✓ Building MVPs or smaller applications
- ✓ You want architectural guidance without project ceremony
- ✓ Team prefers simplicity and fast iteration
- ✓ Vertical slice architecture appeals to you
- ✓ You may grow into full Clean Architecture later

**Use Full Clean Architecture when:**

- × Building large, complex enterprise applications
- × Multiple teams working on different layers
- × Need strict separation of concerns and dependencies
- × Domain complexity requires extensive DDD patterns
- × Long-term maintenance and evolution expected

## Technology Stack

- **.NET 10**: Latest LTS framework
- **FastEndpoints**: REPR pattern for API endpoints
- **Entity Framework Core**: Data access with migrations
- **SQLite**: Database
- **Serilog**: Structured logging
- **FluentValidation**: Request validation

## Common Tasks

### Adding a New Feature (Vertical Slice)

1. **Create Domain Entity**: Add to `Domain/YourFeatureAggregate/`
2. **Add EF Configuration**: Create config in `Infrastructure/Data/Config/`
3. **Create Migration**: `dotnet ef migrations add AddYourFeature`
4. **Create Endpoints**: Add FastEndpoints in `Endpoints/YourFeature/`

### SQLite

The project is configured to use SQLite by default. Update `appsettings.json` if needed:

```json
{
  "ConnectionStrings": {
    "AppDb": "DataSource=ISII.db;Cache=Shared"
  }
}
```

### Running Tests

```powershell
dotnet test
```

## Migration Path to Full Clean Architecture

As your application grows, you can migrate to the full Clean Architecture template:

1. **Extract Core**: Move Domain entities to separate Core project
2. **Extract UseCases**: Move business logic to UseCases project with Mediator
3. **Extract Infrastructure**: Move data access to Infrastructure project
4. **Update Dependencies**: Set up proper dependency flow (Core ← UseCases ← Infrastructure)

Learn more: [Clean Architecture Template](https://github.com/ardalis/CleanArchitecture)

## Resources

- [Clean Architecture Template Repository](https://github.com/ardalis/CleanArchitecture)
- [Vertical Slice Architecture](https://jimmybogard.com/vertical-slice-architecture/)
- [FastEndpoints Documentation](https://fast-endpoints.com/)
- [Ardalis on YouTube](https://www.youtube.com/@Ardalis)

---

**Happy Coding!** 🚀
