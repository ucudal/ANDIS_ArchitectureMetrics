using ArchUnitNET.Domain;

namespace Ucu.Andis.ArchitectureMetrics;

/// <summary>
/// Calcula métricas de estabilidad arquitectónica para componentes basadas en
/// sus dependencias.
/// </summary>
/// <remarks>
/// Esta calculadora implementa las métricas de arquitectura de Robert C. Martin:
/// - Ca: Fan-in o acoplamiento aferente
/// - Ce: Fan-out o acoplamiento eferente
/// - A: Abstacción; proporción de tipos abstractos
/// - I: Inestabilidad; indicador de estabilidad, como $I = \frac{Ce}{Ca + Ce}$
/// - D: Distancia; distancia de la secuencia principal, como $D = |A + I - 1|$
/// </remarks>
public static class MetricsCalculator
{
    /// <summary>
    /// Construye una colección de instancias de <c>Component</c> agrupando
    /// tipos por ensamblado.
    /// </summary>
    /// <param name="architecture">La arquitectura cargada a analizar.</param>
    /// <param name="assemblyNames">Los nombres de los ensamblados a agrupar
    /// como componentes.</param>
    /// <returns>Una colección de componentes, cada uno contiene todos los tipos
    /// del ensamblado.</returns>
    /// <remarks>
    /// Este método es fundamental para organizar la arquitectura en capas
    /// lógicas a nivel de despliegue. Los ensamblados son unidades de
    /// compilación y despliegue, por lo que son el nivel correcto para medir
    /// acoplamiento e inestabilidad arquitectónica.
    /// </remarks>
    public static IReadOnlyCollection<Component> BuildAssemblyComponents(
        Architecture architecture,
        params string[] assemblyNames)
    {
        return assemblyNames
            .Select(assemblyName =>
                new Component(
                    assemblyName,
                    architecture.Types
                        .Where(t => t.Assembly.Name == assemblyName)
                        .ToList()))
            .ToList();
    }

    /// <summary>
    /// Construye un grafo de dependencias entre instancias de <c>Component</c>.
    /// </summary>
    /// <param name="components">Los componentes para los cuales construir el
    /// grafo.</param>
    /// <returns>Una colección de <c>ComponentDependencies</c> donde cada
    /// elemento contiene el nombre del componente y la colección de componentes
    /// de los que depende.</returns>
    /// <remarks>
    /// El grafo resultante se utiliza para calcular acoplamiento aferente
    /// -fan-in- y eferente -fan-out-. Solo se incluyen dependencias externas al
    /// componente -se ignoran dependencias internas-. Cuentan como
    /// dependencias:
    /// <ul>
    /// <li>Referencias a tipos en variables o propiedades de instancia o de
    /// clase, parámetros de métodos y sus resultados.</li>
    /// <li>Heredar de tipos</li>
    /// <li>Implementar interfaces</li>
    /// </ul>
    /// </remarks>
    public static IReadOnlyCollection<ComponentDependencies>
        BuildDependencyGraph(IEnumerable<Component> components)
    {
        var componentMap = components
            .SelectMany(
                c => c.Types,
                (component, type) => (type, component.Name))
            .ToDictionary(x => x.type, x => x.Name);

        var graph = components.ToDictionary(
            c => c.Name,
            _ => new HashSet<string>());

        foreach (var component in components)
        {
            foreach (var type in component.Types)
            {
                foreach (var dependency in type.Dependencies)
                {
                    var targetType = dependency.Target;

                    if (!componentMap.TryGetValue(
                            targetType,
                            out var targetComponent))
                    {
                        continue;
                    }

                    if (targetComponent != component.Name)
                    {
                        graph[component.Name]
                            .Add(targetComponent);
                    }
                }
            }
        }

        return graph
            .Select(kv => new ComponentDependencies(kv.Key, kv.Value))
            .ToList();
    }

    /// <summary>
    /// Calcula el acoplamiento aferente -fan-in- y eferente -fan-out- para
    /// todos los componentes.
    /// </summary>
    /// <param name="graph">El grafo de dependencias entre componentes.</param>
    /// <returns>Un <c>ComponentCouplings</c> con dos diccionarios: FanIn
    /// -acoplamiento aferente- y FanOut -acoplamiento eferente-.</returns>
    /// <remarks>
    /// FanIn: Cuántos otros componentes dependen de este.
    /// FanOut: De cuántos otros componentes este depende.
    ///
    /// Estos valores son necesarios para calcular la inestabilidad, como $I =
    /// \frac{Ce}{Ca + Ce}$.
    /// </remarks>
    public static ComponentCouplings CalculateCouplings(
        IReadOnlyCollection<ComponentDependencies> graph)
    {
        var fanOut = graph.ToDictionary(
            cd => cd.ComponentName,
            cd => cd.Dependencies.Count);

        var fanIn = graph.ToDictionary(
            cd => cd.ComponentName,
            _ => 0);

        foreach (var componentDep in graph)
        {
            foreach (var target in componentDep.Dependencies)
            {
                fanIn[target]++;
            }
        }

        return new ComponentCouplings(fanIn, fanOut);
    }

    /// <summary>
    /// Calcula la abstracción —A— de una instancia de <c>Component</c>.
    /// </summary>
    /// <param name="component">El componente para el cual calcular la
    /// abstracción.</param>
    /// <returns>Un valor entre 0 y 1 representando la proporción de tipos
    /// abstractos.</returns>
    /// <remarks>
    /// A -Abstracción- = (número de interfaces + clases abstractas + records
    /// abstractos + tipos genéricos extensibles) / total de tipos.
    ///
    /// Se consideran tipos abstractos o extensibles:
    /// <ul>
    /// <li>Interfaces: contrato sin implementación, abstractas por definición.</li>
    /// <li>Clases abstractas: contrato parcial que requiere implementación.</li>
    /// <li>Records abstractos: registros que requieren implementación en
    /// subclases.</li>
    /// <li>Tipos genéricos -con parámetros de tipo- son extensibles vía
    /// parametrización.</li>
    /// </ul>
    ///
    /// Ejemplos:
    /// <ul>
    /// <li>A = 0: Componente totalmente concreto -solo clases y records
    /// concretos no genéricos-.</li>
    /// <li>A = 1: Componente totalmente abstracto -solo interfaces, clases y
    /// records abstractos-.</li>
    /// <li>A = 0.5: Componente equilibrado con mezcla de tipos abstractos y
    /// concretos.</li>
    /// </ul>
    ///
    /// La abstracción es un indicador de flexibilidad: cuanto más abstracto,
    /// más fácil de extender. Los tipos genéricos aportan extensibilidad
    /// mediante parametrización de tipos.
    /// </remarks>
    public static double CalculateAbstractness(Component component)
    {
        var totalTypes = component.Types.Count;

        if (totalTypes == 0)
            return 0;

        var abstractTypes = component.Types.Count(t =>
            IsAbstractOrExtensible(t));

        return (double)abstractTypes / totalTypes;
    }

    /// <summary>
    /// Determina si un tipo es abstracto o extensible.
    /// </summary>
    /// <param name="type">El tipo a evaluar.</param>
    /// <returns>Verdadero si el tipo es abstracto o extensible.</returns>
    /// <remarks>
    /// Un tipo se considera abstracto o extensible si:
    /// <ul>
    /// <li>Es una interfaz.</li>
    /// <li>Es una clase abstracta.</li>
    /// <li>Es un record abstracto.</li>
    /// <li>Tiene parámetros de tipo genérico.</li>
    /// </ul>
    /// </remarks>
    private static bool IsAbstractOrExtensible(IType type)
    {
        // Interfaces son siempre abstractas
        if (type is Interface)
            return true;

        // Clases abstractas
        if (type is Class c && c.IsAbstract == true)
            return true;

        // Tipos genéricos son extensibles por parametrización
        if (type.GenericParameters.Count > 0)
            return true;

        return false;
    }

    /// <summary>
    /// Calcula todas las métricas de estabilidad arquitectónica para los
    /// componentes.
    /// </summary>
    /// <param name="components">Los componentes para los cuales calcular las
    /// métricas.</param>
    /// <returns>Una colección de métricas para cada componente, incluyendo
    /// FanIn, FanOut, Abstractness, Instability y Distance.</returns>
    /// <remarks>
    /// Este método orquesta el cálculo de todas las métricas fundamentales:
    /// <ul>
    /// <li>FanIn -acoplamiento aferente-</li>
    /// <li>FanOut -acoplamiento eferente-</li>
    /// <li>A -abstracción-</li>
    /// <li>I -inestabilidad- = Fan-out / (Fan-in + Fan-out)</li>
    /// <li>D -distancia de secuencia principal- = |A + I - 1|</li>
    /// </ul>
    /// Estas métricas son indicadores clave de la calidad arquitectónica según
    /// Robert C. Martin.
    /// </remarks>
    public static IReadOnlyCollection<ComponentMetrics> CalculateMetrics(
        IReadOnlyCollection<Component> components)
    {
        var graph = BuildDependencyGraph(components);

        var couplings = CalculateCouplings(graph);

        return components
            .Select(component =>
            {
                var fanIn = couplings.FanIn[component.Name];
                var fanOut = couplings.FanOut[component.Name];

                var a = CalculateAbstractness(component);

                var i = fanIn + fanOut == 0 ? 0 : (double)fanOut / (fanIn + fanOut);

                var d = Math.Abs(a + i - 1);

                return new ComponentMetrics(component.Name, fanIn, fanOut, a, i, d);
            })
            .ToList();
    }
}
