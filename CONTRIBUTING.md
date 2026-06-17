Para publicar una nueva versión del paquete NuGet:

Los números de versión tienen la forma `x.y.z` donde `x.y` lo indica el
desarrollador y `z` se incrementa automáticamente. La publicación del paquete es
a través de una [GitHub Action](.github/workflows/publish-nuget.yml).

## Ejecución automática

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

## Ejecución manual

La acción se puede ejecutar manualmente. En el momento de ejecutarla va a pedir
el numero de versión `0.0`.