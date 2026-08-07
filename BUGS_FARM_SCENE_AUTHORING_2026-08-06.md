# Reporte de bugs — Autoría de escena Farm

Fecha: 2026-08-06  
Rama: `dev/farm-scene-authoring`

Este archivo complementa `BUGS.MD` y registra errores específicos de la autoría y normalización de la escena `Farm`.

---

## BUG-0015 — Las parcelas cambiaban de posición en la jerarquía después de cada normalización

- **Estado:** VERIFICADO.
- **Severidad:** S3 — Media.
- **Detectado:** 2026-08-06.
- **Corregido/verificado:** 2026-08-06.
- **Sistema:** autoría de escena Farm / layout de parcelas.
- **Rama afectada:** `dev/farm-scene-authoring`.
- **Archivo afectado:** `Assets/_Project/Scripts/Editor/FarmPlotFieldLayoutUpgrader.cs`.
- **Commit que introdujo el comportamiento:** `c7f7a8c32c1af81b084e7cceb101cbdf1b067789`.
- **Commit de corrección:** `85024a2e1f2a53b8e75dc90dbedf7ee71c309060`.
- **Reportado y validado por:** Arturo.

### Comportamiento observado

Las 15 entidades `Plot` se colocaban correctamente como tres filas de cinco en la escena, pero cambiaban de posición dentro de `Farm Plot Field` en la ventana Hierarchy cada vez que el normalizador volvía a ejecutarse.

El orden visible alternaba porque el script primero ordenaba alfabéticamente los nombres y después reasignaba nombres y posiciones usando un recorrido por filas.

### Causa raíz

El orden alfabético de nombres como:

```text
Plot 1-1
Plot 1-2
Plot 1-3
Plot 2-1
...
```

no coincide con el orden espacial deseado por filas:

```text
Plot 1-1
Plot 2-1
Plot 3-1
Plot 4-1
Plot 5-1
Plot 1-2
...
```

Al renombrar los mismos objetos según el índice del arreglo ordenado alfabéticamente, la siguiente ejecución obtenía un arreglo diferente y volvía a permutar las entidades. El normalizador no era idempotente respecto al orden de hermanos.

### Solución aplicada

Se reemplazó la dependencia del orden alfabético por un orden lógico basado en las coordenadas `columna-fila` extraídas del nombre de cada parcela.

Además, el script fija explícitamente el orden de hermanos mediante `Transform.SetSiblingIndex()`.

El orden canónico es ahora:

```text
Plot 1-1
Plot 2-1
Plot 3-1
Plot 4-1
Plot 5-1
Plot 1-2
Plot 2-2
Plot 3-2
Plot 4-2
Plot 5-2
Plot 1-3
Plot 2-3
Plot 3-3
Plot 4-3
Plot 5-3
```

### Resultado

- Las parcelas conservan su posición mundial.
- Los identificadores persistentes permanecen estables.
- El orden de la jerarquía corresponde al orden espacial por filas.
- Las recompilaciones y normalizaciones posteriores ya no permutan los objetos.
- El normalizador queda idempotente para una escena ya corregida.

### Prueba de regresión

1. Abrir `Farm.unity`.
2. Expandir `Farming Core Loop v4/Farm Plot Field`.
3. Confirmar el orden canónico de las 15 parcelas.
4. Ejecutar varias veces:

```text
Tools → Farm Simulator → Farm Development Kit → Farming → Arrange 3 x 5 Plot Field
```

5. Forzar una recompilación de scripts o reabrir Unity.
6. Confirmar que ningún `Plot` cambia de posición en Hierarchy ni en la escena.

### Regla preventiva

Cuando un normalizador administra entidades persistentes de escena:

- no debe inferir identidad a partir de la posición actual en un arreglo mutable;
- debe ordenar por una clave lógica estable;
- debe fijar explícitamente `SiblingIndex` cuando el orden de la jerarquía sea parte del resultado esperado;
- una segunda ejecución sobre una escena normalizada no debe producir cambios.
