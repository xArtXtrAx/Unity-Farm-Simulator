# Bitácora — Pipeline de Tilemaps y cultivos como entidades

Fecha: 2026-08-06  
Rama: `agent/cozy-art-pipeline`  
PR: #14 — `Automate Cozy Tile Palette authoring`

Este documento es un anexo transaccional de `BITÁCORA_GPT.MD`.

## Estado validado antes de este cambio

- Unity 6.3 LTS `6000.3.21f1`.
- `com.unity.2d.tilemap` instalado.
- Tile Manager y paletas automáticas operativas.
- EditMode completo aprobado.
- PlayMode aprobado **10/10**.
- Transparencia y centrado manual de cultivos seguían mostrando defectos al pintarlos como tiles.

## Decisión de arquitectura

Los cultivos dejan de ser tiles de autoría.

La estructura final es:

```text
Farm Authoring Grid
├── Ground
├── Paths
├── Soil
└── Decoration

Farm Plot Field
└── Plot x-y
    ├── Soil Visual
    └── Crop Entity Visual (SpriteRenderer)
```

Responsabilidades:

- `Ground`, `Paths`, `Soil` y `Decoration` se pintan con Tile Palette.
- semillas, brotes, plantas maduras y cosechas son sprites runtime propiedad de `FarmPlotBehaviour`.
- el estado de crecimiento sigue viviendo en `FarmPlotState`.
- `FarmPlotBehaviour.Render()` cambia el sprite de la entidad visual al avanzar el día.
- una planta nunca sustituye el tile de suelo porque ya no ocupa una celda del Tilemap.

Esta separación sigue el patrón habitual de los simuladores agrícolas 2D: el mapa estático se autoriza con Tilemaps y los elementos con estado, crecimiento e interacción se representan como entidades runtime. Esto permite cultivos de distintos tamaños, animaciones, sombras, partículas y estados especiales sin quedar limitados a una celda de Tilemap.

## Implementación

- `FarmTilemapLayers` elimina la referencia `Crops`.
- `CozyPaletteCategory` queda en `Ground`, `Paths`, `Soil` y `Decoration`.
- se elimina la paleta agrícola pintable `Cozy Farm - Crops`.
- `LegacyCropPaletteCleanup` borra la paleta antigua al recompilar.
- `CozyFarmTileCatalog` continúa generando 18 sprites transparentes, pero exclusivamente para runtime.
- `FarmSceneFarmingUpgrader` genera `Farming Core Loop v4`.
- cada parcela contiene un `Crop Entity Visual` con `SpriteRenderer`.
- el runtime carga las seis etapas transparentes de nabo, zanahoria y col.
- la posición, escala y etapa se actualizan sin modificar ningún Tilemap.

Commits principales:

- `d731f7d3c1fe9ee1630e95f1e958939a60a3d3ce`
- `0d4796dfeb93bc2dbf7fb35a437ef2682ffd0a30`
- `0132fea7b46c184b19785a630ad7151cfad1b8e2`
- `25027bdcb1f8da0f3e0901e4f42e4eaf97ef1025`
- `552c15054b73507c1b8399940f4a6c656b8234a8`
- `dfefec9eca7a20048af791d1309bca7d9c0adb8f`
- `7232212635f187e489696eb499879e3920c4e186`

## Regresión de compilación detectada

Durante el refactor de `FarmPlotBehaviour`, `ValidateStages()` utilizó `Sprite[].Any(...)` sin importar `System.Linq`. Unity detuvo la compilación con `CS1061`.

Se sustituyó la consulta LINQ por un ciclo explícito que valida cada entrada del arreglo. La solución evita una dependencia innecesaria y no genera enumeradores ni asignaciones adicionales.

- Bug: `BUG-0011`.
- Corrección: `02d1d696137a2692983e02d342ded5a7f41020df`.
- Estado: **CORREGIDO**, pendiente de confirmar compilación y suites completas en Unity.

## Pruebas actualizadas

`FarmingScenePipelineTests` comprueba:

- nueve parcelas configuradas;
- cuatro Tilemaps de autoría;
- ausencia de un Tilemap `Crops`;
- un `SpriteRenderer` de cultivo por parcela;
- cuatro paletas de mundo;
- 18 sprites runtime importados con transparencia, 16 PPU y pivote central.

## Próximo paso exacto

1. Hacer Pull de `agent/cozy-art-pipeline`.
2. Esperar compilación e importación.
3. Ejecutar `Rebuild Cozy Tile Catalog + Palettes`.
4. Ejecutar `Apply Farming Field To Farm Scene`.
5. Confirmar que ya no existe botón ni paleta `Crops`.
6. Confirmar jerarquía `Ground / Paths / Soil / Decoration`.
7. Entrar en Play Mode.
8. Arar, sembrar y verificar que la planta aparece como `Crop Entity Visual` sobre la tierra.
9. Dormir y comprobar el cambio de etapa.
10. Confirmar que el suelo inferior nunca cambia de color ni desaparece.
11. Ejecutar EditMode y PlayMode completos.
12. Cuando Unity compile y las suites pasen, cambiar `BUG-0011` a **VERIFICADO**.

## Exclusiones

No se modificaron reglas de crecimiento, consumo de semillas, inventario persistente, sueño, portales, casa, interiores ni collider del héroe.
