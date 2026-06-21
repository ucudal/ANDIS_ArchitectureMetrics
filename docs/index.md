![Build
status](https://github.com/ucudal/ANDIS_ArchitectureMetrics/actions/workflows/build-test.yml/badge.svg)
[![Descargar desde
NuGet](https://img.shields.io/badge/NuGet-Ucu.Andis.ArchitectureMetrics-004880?logo=nuget)](https://www.nuget.org/packages/Ucu.Andis.ArchitectureMetrics/)

# Introducción

Esta librería proporciona herramientas para analizar y medir la estabilidad y
calidad de una arquitectura de software, utilizando las [métricas y principios
definidos](http://objectmentor.com/resources/articles/stability.pdf) por Robert
C. Martin, usando [ArchUnit](https://archunitnet.readthedocs.io/en/stable/).

Para cada componente -donde en este contexto componente es
un ensamblado- calcula las siguientes métricas:

- **Fan-in** o **acoplamiento aferente**: cuántos otros componentes dependen del
  componente dado.

- **Fan-out** o **acoplamiento eferente**: de cuántos otros componentes depende
  un componente dado.

- **Abstractness**: Proporción de tipos abstractos o extensibles
  -interfaces, clases abstractas, registros abstractos, tipos genéricos-
  respecto del total de tipos

- **Instability**: Medida de qué tan susceptible es el componente al cambio
  $I = \frac{Ce}{Ca + Ce}$

- **Distance**: Distancia de la secuencia principal $D = |A + I - 1|$

<!-- markdownlint-disable MD032 -->
\note Para el cálculo de las métricas de abstracción se consideran tipos abstractos:
- Interfaces `interface`
- Clases abstractas `abstract class`
- Records abstractos `abstract record`
- Tipos genéricos `class<T>`
<!-- markdownlint-enable MD032 -->

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

# Notas

It is natural for people to make changes to packages that are easy to change,
and to avoid changes to packages that are hard to change. In doing so, they make
the easy-to-change packages easier to change and the hard-to-change packages
harder to change. The principle provides a guideline for determining if things
are getting worse or getting better as these natural tendencies take effect. If
things are getting worse, then some refactoring may be in order.

<https://wiki.c2.com/?StableDependenciesPrinciple>
