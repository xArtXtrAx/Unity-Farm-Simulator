# Bitácora — Pipeline de Tilemaps y cultivos como entidades

Fecha: 2026-08-06  
Rama: `agent/cozy-art-pipeline`  
PR: #14 — `Automate Cozy Tile Palette authoring`

Este documento es un anexo transaccional de `BITÁCORA_GPT.MD`.

## Estado validado antes de este cambio

- Unity 6.3 LTS `6000.3.21f1`.
- `com.unity.2d.tilemap` instalado.
- Tile Manager y paletas automáticas operativas.
- EditMode completo aprobado antes del último ajuste de prefabs.
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

Los sprites generados usan pivote inferior central. Los prefabs reutilizables se normalizan ahora alrededor de esa base visual:

- `Building Visual` queda en posición local `(0, 0)`;
- portal, spawn y collider se convierten desde los metadatos de escena al espacio local de base;
- la huella lógica se autoriza independientemente del techo y del tamaño total del sprite;
- `Regenerate + place on scene grid` reconstruye el prefab antes de instanciarlo, evitando máscaras desactualizadas;
- el Footprint Editor superpone sobre el sprite la misma máscara exacta usada por Scene, snap y colisiones.

### Corrección del lienzo de ancho par

La discrepancia restante no estaba en el gizmo ni en el prefab. El lienzo de `4 × 3` utilizaba columnas distintas según el sistema:

```text
Runtime / máscara guardada: -2, -1, 0, 1
Panel del editor anterior:  -1,  0, 1, 2
```

Por eso el contador informaba diez celdas, la previsualización mostraba las diez, pero el panel solo podía representar ocho de ellas. Se centralizó la fórmula en `GridBuildingFootprint.GetCanvasMinimumX()` y `CreateRectangleOffsets()`. El editor reutiliza ahora esas mismas funciones, de modo que la celda ancla `(0,0)` aparece en la tercera columna de un lienzo de ancho cuatro y todas las celdas guardadas son visibles.

Commits del ajuste final:

- `dc44fc52663c0303ae6af6657f723c8d21c3091d`
- `225157451d83db5ff65161d40c3696328c29b14d`
- `90dc5b8010853ed7d71b950caed63cce19b73e50`

## Cierre del incremento — overlays y origen editable

Antes de integrar la rama se completó la autoría visual de edificios:

- el Footprint Editor separa y muestra `Visual bounds`, `Collider` y `Footprint`;
- la huella de las casas usa por defecto un ancla `(0, 0.5)`, de modo que el borde inferior de la primera fila coincide con la base visual y la celda frontal queda libre para caminos y escalones;
- `Footprint Origin` permite elegir manualmente dónde comienza la huella, editar el borde inferior, mover el origen con incrementos configurables y regenerar el prefab;
- la previsualización, el gizmo de Scene, el snap y las colisiones consumen la misma máscara y el mismo origen;
- Arturo confirmó visualmente que la herramienta quedó mucho mejor y suficientemente estable para integrar el incremento.

Commits finales:

- `927a7659559a1b98233eb34ba4488244ae3520a6` — overlays de visual, collider y footprint;
- `b4a4965add30604778dcf93c605243e9c9653161` — borde inferior alineado con la base visual;
- `58792b0201eda41787e88d7a586225b36fbaa82c` — editor explícito del origen de huella;
- `255ddd6059a3e6272ae1b702289d1d7c9fb84d4b` — metadata del editor de origen.

Estado al integrar: **IMPLEMENTADO y validado visualmente por Arturo**. `BUG-0014` no se marca como `VERIFICADO` en este documento porque no se recibió una nueva confirmación explícita de la suite EditMode después de los dos últimos ajustes de editor.

## Próxima validación en `main`

1. Actualizar `main` después del merge.
2. Abrir una casa en `Footprint Editor` y confirmar overlays y origen editable.
3. Regenerar y colocar un prefab.
4. Confirmar que la celda frontal queda libre para caminos.
5. Ejecutar EditMode completo.
6. Marcar `BUG-0014` como `VERIFICADO` únicamente después de esa confirmación.

## Regla que se mantiene

Los Tilemaps se usan para autoría del mundo estático. Los cultivos siguen siendo entidades runtime con `SpriteRenderer`. Los edificios completos se extraen del atlas Full-Pack mediante un catálogo reproducible y sus huellas lógicas se autorizan por separado del tamaño visual del sprite.
