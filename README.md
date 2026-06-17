<!-- markdownlint-disable MD033 MD041 -->
<img alt="UCU" src="./assets/logo_ucu.svg" width="150"/>
<!-- markdownlint-enable MD033 MD041 -->

# Universidad Católica del Uruguay

## Facultad de Ingeniería y Tecnologías

### Análisis y diseño de aplicaciones I y II

# ArchitectureMetrics: Librería para calcular métricas de arquitectura

![Build
status](https://github.com/ucudal/ANDIS_ArchitectureMetrics/actions/workflows/build-test.yml/badge.svg)
[![Descargar desde
NuGet](https://img.shields.io/badge/NuGet-Ucu.Andis.ArchitectureMetrics-004880?logo=nuget)](https://www.nuget.org/packages/Ucu.Andis.ArchitectureMetrics/)

Esta librería .NET permite calcular las siguientes métricas de arquitectura
basadas en los principios definidos por Robert C. Martin en Clean Architecture-A
Craftsman's Guide to Software:

## Descripción general

Esta librería proporciona herramientas para analizar y medir la estabilidad y
calidad de su arquitectura de software usando
[ArchUnit](https://archunitnet.readthedocs.io/en/stable/). Calcula las
siguientes métricas, para cada componente -donde en este contexto componente es
un ensamblado-:

- **Fan-In**: Acoplamiento aferente - cuántos componentes dependen del
  componente dado

- **Fan-Out**: Acoplamiento eferente - de cuántos componentes este depende
  un componente dado

- **Abstractness**: Proporción de tipos abstractos o extensibles
  -interfaces, clases abstractas, registros abstractos, tipos genéricos-
  respecto del total de tipos

- **Instability**: Medida de qué tan susceptible es el componente al cambio
  (I = Ce / (Ca + Ce))

- **Distance**: Distancia de la secuencia principal (D = |A + I - 1|)

## Estructura del proyecto

```text
ANDIS_ArchitectureMetrics/
├── src/
│   └── Ucu.Andis.ArchitectureMetrics/           # Librería principal
│       ├── Models/                              # Modelos de datos
│       │   ├── Component.cs                     # Definición de componente
│       │   ├── ComponentMetrics.cs              # Contenedor de métricas
│       │   ├── ComponentDependencies.cs         # Grafo de dependencias
│       │   └── ComponentCouplings.cs            # Métricas de acoplamiento
│       └── Calculators/                         # Motores de cálculo
│           └── MetricsCalculator.cs             # Calculador principal
│
├── tests/
│   └── Ucu.Andis.ArchitectureMetrics.Tests/     # Casos de prueba
│
├── docs/                                        # Documentación
├── README.md                                    # Este archivo
└── ANDIS_ArchitectureMetrics.sln                # Archivo de solución
```

## Uso

### Uso básico

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

- .NET 8.0 o posterior
- TngTech.ArchUnitNET 0.13.3 o compatible

## Instalación desde NuGet

```bash
dotnet add package Ucu.Andis.ArchitectureMetrics
```

O en tu archivo `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="Ucu.Andis.ArchitectureMetrics" Version="1.0.0"/>
</ItemGroup>
```

## Compilación

```bash
dotnet build
```

## Ejecutar tests

```bash
dotnet test
```

## Publicación

Los números de versión tienen la forma `x.y.z` donde `x.y` lo indica el
desarrollador y `z` se incrementa automáticamente. La publicación del paquete es
a través de una [GitHub Action](.github/workflows/publish-nuget.yml).

### Ejecución automática

Esta acción se ejecuta automáticamente de la siguiente forma:

```bash
git tag v0.0
git push origin v0.0
```

donde `0.0` es el número de versión deseado.

En caso de que el tag ya exista, eliminarlo primero con:

```bash
git tag -d v0.0
git push origin --delete v0.0
```

### Ejecución manual

La acción se puede ejecutar manualmente. En el momento de ejecutarla va a pedir
el numero de versión `0.0`.

## Documentación

La documentación de la API se genera usando Doxygen. Consulte la carpeta `docs/`
para documentación detallada.

## Referencias

- Libro de Arquitectura de Robert C. Martin: "Clean Architecture"
- ArchUnit: <https://www.archunit.org/>
