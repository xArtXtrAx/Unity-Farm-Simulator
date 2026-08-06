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

## Farm Development Kit — autoría unificada de edificios

Se reemplazó la conversión implícita entre el pivote del prefab y la huella lógica por una fuente única de verdad:

```text
Building Root
├── Building Composition
├── Footprint Anchor
├── Door Anchor
├── Portal Anchor
└── Spawn Anchor
```

`Footprint Anchor` define el origen usado por:

- la previsualización de Scene;
- el ajuste a la cuadrícula;
- la búsqueda de posiciones libres;
- la detección de superposición;
- las celdas ocupadas persistentes.

El `Building Authoring` permite editar la posición local del ancla y las celdas irregulares. Por defecto, las casas usan la posición del portal/puerta como ancla. Los prefabs deben regenerarse después de editar esta información.

Commits del incremento:

- `eccfeb5c77344f859e1c186706c8527625c4cd6d`
- `3658f3d0f7c52936429a2b50609721438e03525f`
- `dd85db8d9f4eec30c30cd9656f024f60f1ca0dcd`
- `8dbfd0e2baa2337b132afe788944882042b20f13`
- `4e6b14189f65532e2635af773deb4c1c4c4f632b`
- `2194e5d553b119457c5c6dbedc9e3f3bf3325422`
- `b63f76458b5bced669fa8f455f95aa9ce0ea7395`

Estado: **IMPLEMENTADO, pendiente de compilación, validación visual y EditMode local**.

## Próxima validación

1. Hacer Pull.
2. Ejecutar `Rebuild Building Definitions`.
3. Abrir `Footprint Editor` y confirmar que el ancla coincide con la puerta/base.
4. Ejecutar `Generate all prefabs`.
5. Eliminar instancias antiguas y colocar prefabs regenerados.
6. Mover una casa y confirmar que la huella sigue el `Footprint Anchor`.
7. Verificar cambio verde/rojo al superponer edificios.
8. Ejecutar EditMode completo.

## Regla que se mantiene

Los Tilemaps se usan para autoría del mundo estático. Los cultivos siguen siendo entidades runtime con `SpriteRenderer`. Los edificios completos se extraen del atlas Full-Pack mediante un catálogo reproducible y sus huellas lógicas se autorizan por separado del tamaño visual del sprite.
