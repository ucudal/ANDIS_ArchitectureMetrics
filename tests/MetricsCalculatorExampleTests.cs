using ArchUnitNET.Loader;
using Ucu.Andis.ArchitectureMetrics;

namespace Ucu.Andis.ArchitectureMetrics.Tests;

/// <summary>
/// Tests de ejemplo demostrando cómo usar MetricsCalculator.
/// </summary>
/// <remarks>
/// Estos tests muestran el flujo básico para calcular métricas arquitectónicas:
/// 1. Cargar la arquitectura desde assemblies
/// 2. Construir componentes agrupando tipos de los assemblies
/// 3. Calcular métricas usando MetricsCalculator
/// 4. Validar o analizar las métricas calculadas
/// </remarks>
[TestFixture]
public class MetricsCalculatorExampleTests
{
    /// <summary>
    /// Demuestra cómo cargar una arquitectura y construir componentes.
    /// </summary>
    /// <remarks>
    /// Este test carga el assembly de la librería misma y construye un único componente.
    /// Verifica que el componente se creó exitosamente con los tipos correctos.
    /// </remarks>
    [Test]
    public void CanLoadArchitectureAndBuildComponents()
    {
        // Arrange
        var architecture = new ArchLoader()
            .LoadAssemblies(
                typeof(MetricsCalculator).Assembly
            )
            .Build();

        var assemblyName = typeof(MetricsCalculator).Assembly.GetName().Name;

        Assert.That(assemblyName, Is.Not.Null); // Previene warning CS8604

        // Act
        var components = MetricsCalculator.BuildAssemblyComponents(
            architecture,
            assemblyName);

        // Assert
        Assert.That(components.Count, Is.EqualTo(1));
        Assert.That(components.First().Name, Is.EqualTo(assemblyName));
        Assert.That(components.First().Types.Count, Is.GreaterThan(0));
    }

    /// <summary>
    /// Demuestra cómo calcular métricas para componentes.
    /// </summary>
    /// <remarks>
    /// Este test calcula métricas para un único componente y verifica que las
    /// métricas se calcularon correctamente -abstracción entre 0 y 1, etc.-.
    /// </remarks>
    [Test]
    public void CanCalculateMetricsForComponent()
    {
        // Arrange
        var architecture = new ArchLoader()
            .LoadAssemblies(
                typeof(MetricsCalculator).Assembly
            )
            .Build();

        var assemblyName = typeof(MetricsCalculator).Assembly.GetName().Name;

        Assert.That(assemblyName, Is.Not.Null); // Previene warning CS8604

        var components = MetricsCalculator.BuildAssemblyComponents(
            architecture,
            assemblyName);

        // Act
        var metrics = MetricsCalculator.CalculateMetrics(components);

        // Assert
        Assert.That(metrics.Count, Is.EqualTo(1));

        var metric = metrics.First();
        Assert.That(metric.Name, Is.EqualTo(assemblyName));
        Assert.That(metric.FanIn, Is.GreaterThanOrEqualTo(0));
        Assert.That(metric.FanOut, Is.GreaterThanOrEqualTo(0));
        Assert.That(metric.Abstractness, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(1));
        Assert.That(metric.Instability, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(1));
        Assert.That(metric.Distance, Is.GreaterThanOrEqualTo(0).And.LessThanOrEqualTo(1));
    }

    /// <summary>
    /// Demuestra cómo interpretar las métricas calculadas.
    /// </summary>
    /// <remarks>
    /// Este test muestra una tabla formateada de métricas, demostrando cómo
    /// interpretar los valores para diferentes componentes.
    /// </remarks>
    [Test]
    public void DisplayMetricsForLibrary()
    {
        // Arrange
        var architecture = new ArchLoader()
            .LoadAssemblies(
                typeof(MetricsCalculator).Assembly
            )
            .Build();

        var assemblyName = typeof(MetricsCalculator).Assembly.GetName().Name;

        Assert.That(assemblyName, Is.Not.Null); // Previene warning CS8604

        var components = MetricsCalculator.BuildAssemblyComponents(
            architecture,
            assemblyName);

        // Act
        var metrics = MetricsCalculator.CalculateMetrics(components);

        // Assert & Display
        TestContext.Out.WriteLine("\n=== Métricas de componentes ===");
        TestContext.Out.WriteLine(
            "{0,-40} {1,6} {2,6} {3,11} {4,12} {5,10}",
            "Componente", "FanIn", "FanOut", "Abstracción", "Inestabilidad", "Distancia");
        TestContext.Out.WriteLine(new string('-', 95));

        foreach (var metric in metrics.OrderBy(m => m.Name))
        {
            TestContext.Out.WriteLine(
                "{0,-40} {1,6} {2,6} {3,11:F4} {4,12:F4} {5,10:F4}",
                metric.Name,
                metric.FanIn,
                metric.FanOut,
                metric.Abstractness,
                metric.Instability,
                metric.Distance);
        }

        Assert.That(metrics, Is.Not.Empty);
    }
}
