# ANDIS Architecture Metrics

Librería .NET para calcular métricas de estabilidad arquitectónica basadas en los principios de análisis de arquitectura de Robert C. Martin.

## Descripción General

Esta librería proporciona herramientas para analizar y medir la estabilidad y calidad de su arquitectura de software usando ArchUnit. Calcula métricas clave como:

- **Fan-In (Ca)**: Acoplamiento aferente - cuántos componentes dependen de este
- **Fan-Out (Ce)**: Acoplamiento eferente - de cuántos componentes este depende
- **Abstractness (A)**: Proporción de tipos abstractos o extensibles (interfaces, clases abstractas, records abstractos, tipos genéricos)
- **Instability (I)**: Medida de qué tan susceptible es el componente al cambio (I = Ce / (Ca + Ce))
- **Distance (D)**: Distancia de la Secuencia Principal (D = |A + I - 1|)

## Estructura del Proyecto

```
ANDIS_ArchitectureMetrics/
├── src/
│   └── Ucu.Andis.ArchitectureMetrics/          # Librería principal
│       ├── Models/                              # Modelos de datos
│       │   ├── Component.cs                     # Definición de componente
│       │   ├── ComponentMetrics.cs              # Contenedor de métricas
│       │   ├── ComponentDependencies.cs         # Grafo de dependencias
│       │   └── ComponentCouplings.cs            # Métricas de acoplamiento
│       └── Calculators/                         # Motores de cálculo
│           └── MetricsCalculator.cs             # Calculador principal
│
├── tests/
│   └── Ucu.Andis.ArchitectureMetrics.Tests/    # Suite de tests
│
├── docs/                                        # Documentación
├── README.md                                    # Este archivo
└── ANDIS_ArchitectureMetrics.sln                # Archivo de solución
```

## Uso

### Uso Básico

```csharp
using Ucu.Andis.ArchitectureMetrics;
using ArchUnitNET.Loader;

// Cargar assemblies
var architecture = new ArchLoader()
    .LoadAssemblies(
        typeof(Domain.Entity).Assembly,
        typeof(Application.Command).Assembly,
        typeof(Infrastructure.Repository).Assembly,
        typeof(API.Controller).Assembly
    )
    .Build();

// Construir componentes desde los assemblies
var components = MetricsCalculator.BuildAssemblyComponents(
    architecture,
    "MyApp.Domain",
    "MyApp.Application",
    "MyApp.Infrastructure",
    "MyApp.API"
);

// Calcular métricas
var metrics = MetricsCalculator.CalculateMetrics(components);

// Usar métricas para análisis
foreach (var metric in metrics)
{
    Console.WriteLine($"{metric.Name}:");
    Console.WriteLine($"  Fan-In: {metric.FanIn}");
    Console.WriteLine($"  Fan-Out: {metric.FanOut}");
    Console.WriteLine($"  Abstractness: {metric.Abstractness:F4}");
    Console.WriteLine($"  Instability: {metric.Instability:F4}");
    Console.WriteLine($"  Distance: {metric.Distance:F4}");
}
```

### Interpretación de Métricas

**Inestabilidad (I)**

- I ≈ 0: Componente estable (dependendido, poca dependencia de otros)
- I ≈ 1: Componente inestable (depende de muchos otros)
- Lo ideal depende del rol del componente (el dominio central debe ser estable)

**Abstractness (A)**

Se consideran tipos abstractos/extensibles:

- Interfaces - contrato sin implementación
- Clases abstractas - contrato parcial
- Records abstractos - registros con implementación parcial
- Tipos genéricos - extensibles vía parametrización de tipos

Valores:

- A = 0: Puramente concreto (solo clases y records concretos no genéricos)
- A = 1: Puramente abstracto (interfaces, clases abstractas, records abstractos)
- A > 0: Flexible para extensión

**Distance (D)**

- D ≈ 0 o 1: En la secuencia principal (bien balanceado)
- D ≈ 0.5: Fuera de la secuencia principal (posibles problemas de diseño)

## Requisitos

- .NET 9.0 o posterior
- TngTech.ArchUnitNET 0.13.3 o compatible

## Instalación

### Desde NuGet (Recomendado)

```bash
dotnet add package Ucu.Andis.ArchitectureMetrics
```

O en tu archivo `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Ucu.Andis.ArchitectureMetrics" Version="1.0.0"/>
</ItemGroup>
```

### Desde Proyecto Local

```bash
dotnet add reference /path/to/Ucu.Andis.ArchitectureMetrics/Ucu.Andis.ArchitectureMetrics.csproj
```

## Compilación

```bash
dotnet build
```

## Ejecutar Tests

```bash
dotnet test
```

## Publicación

El paquete NuGet se publica automáticamente al crear un tag de versión en GitHub. Para más detalles, consulte [PUBLISHING.md](PUBLISHING.md).

### Publicar una Nueva Versión

```bash
git tag v1.0.1
git push origin v1.0.1
```

El workflow de GitHub Actions se encargará del resto.

## Documentación

La documentación de la API se genera usando Doxygen. Consulte la carpeta `docs/` para documentación detallada.

## Referencias

- Libro de Arquitectura de Robert C. Martin: "Clean Architecture"
- ArchUnit: <https://www.archunit.org/>
- Métricas de Estabilidad y Distancia: <https://en.wikipedia.org/wiki/Architecture_metric>

## Licencia

Consulte el archivo LICENSE para más detalles.

## Contribuciones

Las contribuciones son bienvenidas. Por favor asegúrese de:

- El código sigue los principios de clean architecture
- Se incluyen tests para nuevas características
- Se actualiza la documentación según corresponda
