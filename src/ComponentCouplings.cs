namespace Ucu.Andis.ArchitectureMetrics;

/// <summary>
/// Contiene el acoplamiento aferente -FanIn- y eferente -FanOut- para todos los
/// componentes.
/// </summary>
/// <param name="FanIn">Diccionario de acoplamiento aferente: nombre de
/// componente → cantidad de componentes que dependen de él.</param>
/// <param name="FanOut">Diccionario de acoplamiento eferente: nombre de
/// componente → cantidad de componentes de los que depende.</param>
/// <remarks>
/// Estos valores son necesarios para calcular la inestabilidad: I = FanOut /
/// (FanIn + FanOut).
/// </remarks>
public record ComponentCouplings(
    Dictionary<string, int> FanIn,
    Dictionary<string, int> FanOut);
