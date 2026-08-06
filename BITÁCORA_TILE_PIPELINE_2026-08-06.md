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

## Reconstrucción de la casa con el atlas completo

La fachada modular basada en paneles del paquete piloto fue retirada. La casa exterior ahora se obtiene directamente de:

```text
Assets/_Project/Art/ThirdParty/CozyFarm/Full/Buildings/buildings.png
```

`CozyFarmBuildingCatalog` centraliza los rectángulos de origen del atlas y genera sprites transparentes reutilizables en:

```text
Assets/_Project/Art/Generated/CozyFarm/Buildings
```

La primera variante es `starter-green-gable-house.png`. El generador:

- lee el PNG original sin alterar su importador;
- extrae una casa completa mediante un `RectInt` documentado;
- detecta los límites alfa visibles;
- recorta el espacio vacío;
- conserva transparencia real;
- importa a 16 PPU, filtro Point, sin mipmaps ni compresión;
- utiliza pivote inferior centrado para apoyar la fachada sobre el suelo.

`CozyFarmHouseExteriorUpgrader` usa un único `SpriteRenderer` para la casa completa y conserva el root funcional, portal, collider y puntos de aparición. La estructura generada es:

```text
Hero House Exterior
└── Cozy Full-Pack House v4
    ├── Starter Green Gable House
    └── Entrance Grounding Shadow
```

Commits:

- `f4ddd073822f93493ca4f16eb717ddd3499d7d9b`
- `3782565ee49a30235ba17b23308c7a38a894c525`
- `e0d8c96a471ff191abb040351c1f1b00013d210b`

La selección visual exacta del rectángulo del atlas queda pendiente de validación local en Unity. Si requiere ajuste, solo se modifica `StarterHouseSource`; la escena y la lógica no cambian.

## Próximo paso exacto

1. Hacer Pull de `agent/cozy-art-pipeline`.
2. Esperar compilación e importación.
3. Ejecutar `Generate Cozy Full-Pack Building Sprites`.
4. Ejecutar `Apply Cozy House Exterior To Farm Scene`.
5. Abrir `Farm` y validar la casa completa, transparencia, escala y alineación de la puerta.
6. Entrar en Play Mode y comprobar portal, collider y regreso desde el interior.
7. Ejecutar EditMode y PlayMode completos.
8. Si el recorte incluye piezas vecinas o corta parte del edificio, ajustar exclusivamente `CozyFarmBuildingCatalog.StarterHouseSource`.

## Exclusiones

No se modificaron reglas de crecimiento, consumo de semillas, inventario persistente, sueño, portales, interiores ni collider del héroe.
