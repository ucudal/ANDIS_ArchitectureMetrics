![Build
status](https://github.com/ucudal/ANDIS_ArchitectureMetrics/actions/workflows/build-test.yml/badge.svg)
[![Descargar desde
NuGet](https://img.shields.io/badge/NuGet-Ucu.Andis.ArchitectureMetrics-004880?logo=nuget)](https://www.nuget.org/packages/Ucu.Andis.ArchitectureMetrics/)

# Introducción

Esta librería proporciona herramientas para analizar y medir la estabilidad y
calidad de una arquitectura de software, utilizando las [métricas y
principios](http://objectmentor.com/resources/articles/stability.pdf) definidos
por Robert C. Martin, usando
[ArchUnit](https://archunitnet.readthedocs.io/en/stable/).

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

<!-- It is natural for people to make changes to packages that are easy to change,
and to avoid changes to packages that are hard to change. In doing so, they make
the easy-to-change packages easier to change and the hard-to-change packages
harder to change. The principle provides a guideline for determining if things
are getting worse or getting better as these natural tendencies take effect. If
things are getting worse, then some refactoring may be in order.

<https://wiki.c2.com/?StableDependenciesPrinciple> -->
