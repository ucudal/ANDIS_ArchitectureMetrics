using ArchUnitNET.Domain;

namespace Ucu.Andis.ArchitectureMetrics;

/// <summary>
/// Representa un componente arquitectónico, agrupando tipos por ensamblado
/// -assembly-.
/// </summary>
/// <param name="Name">El nombre del componente -nombre del ensamblado-.</param>
/// <param name="Types">Colección de tipos que conforman el componente.</param>
/// <remarks>
/// Un componente en este contexto es una unidad de despliegue -ensamblado- que
/// puede contener múltiples tipos. El análisis de acoplamiento e inestabilidad
/// arquitectónica se hace para los componentes, no para los tipos individuales.
/// </remarks>
public record Component(
    string Name,
    IReadOnlyCollection<IType> Types);
