namespace Ucu.Andis.ArchitectureMetrics;

/// <summary>
/// Representa las dependencias de un componente hacia otros componentes.
/// </summary>
/// <param name="ComponentName">Nombre del componente que tiene las
/// dependencias.</param>
/// <param name="Dependencies">Colección de nombres de componentes de los que
/// depende este componente.</param>
/// <remarks>
/// Una dependencia se registra cuando un tipo en el componente depende de un
/// tipo en otro componente.
/// Se consideran dependencias:
/// <ul>
/// <li>Referencias a tipos en variables, propiedades, parámetros de métodos y
/// valores de retorno.</li>
/// <li>Herencia de tipos.</li>
/// <li>Implementación de interfaces.</li>
/// </ul>
/// Las dependencias internas al mismo componente se ignoran.
/// </remarks>
public record ComponentDependencies(
    string ComponentName,
    IReadOnlyCollection<string> Dependencies);
