namespace Ucu.Andis.ArchitectureMetrics.Models;

/// <summary>
/// Contiene todas las métricas de estabilidad arquitectónica para un componente.
/// </summary>
/// <param name="Name">Nombre del componente.</param>
/// <param name="FanIn">Acoplamiento aferente: cuántos componentes dependen de este.</param>
/// <param name="FanOut">Acoplamiento eferente: de cuántos componentes este depende.</param>
/// <param name="Abstractness">Proporción de tipos abstractos o extensibles -A = 0 a 1-.</param>
/// <param name="Instability">Medida de inestabilidad -I = FanOut / (FanIn + FanOut), 0 a 1-.</param>
/// <param name="Distance">Distancia de la secuencia principal -D = |A + I - 1|, 0 a 1-.</param>
/// <remarks>
/// Estas métricas son indicadores clave de la calidad arquitectónica según
/// Robert C. Martin.
/// 
/// La abstracción considera tipos abstractos y extensibles:
/// <ul>
/// <li>Interfaces</li>
/// <li>Clases abstractas</li>
/// <li>Records abstractos</li>
/// <li>Tipos genéricos -extensibles vía parametrización-</li>
/// </ul>
/// 
/// Interpretación de métricas:
/// <ul>
/// <li>Inestabilidad -I- cercano a 0: Componente estable, resistente al
/// cambio.</li>
/// <li>Inestabilidad -I- cercano a 1: Componente inestable, susceptible a
/// cambios.</li>
/// <li>Distancia -D- cercano a 0 o 1: Componente en la secuencia principal
/// -bien diseñado-.</li>
/// <li>Distancia -D- cercano a 0.5: Componente fuera de la secuencia principal
/// -mal diseñado-.</li>
/// </ul>
/// </remarks>
public record ComponentMetrics(
    string Name,
    int FanIn,
    int FanOut,
    double Abstractness,
    double Instability,
    double Distance);
