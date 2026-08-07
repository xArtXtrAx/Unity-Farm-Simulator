# Bitácora de cierre — Farm Scene Authoring

Fecha: 2026-08-06

## Repositorio y ramas

- Repositorio: `xArtXtrAx/Unity-Farm-Simulator`
- Rama de trabajo cerrada: `dev/farm-scene-authoring`
- Rama destino: `main`
- Último commit funcional antes de esta bitácora: `205b8ac768dc3b6c90cb062a46b40baaa25011e2`

## Convención espacial vigente

- 16 píxeles = 1 celda de Unity = 1 unidad de Unity.
- Los elementos deben conservar alineación exacta con la cuadrícula.
- Los cultivos no se pintan como tiles de estado: cada parcela mantiene un `Crop Entity Visual` con `SpriteRenderer`, controlado por `FarmPlotBehaviour`.

## Arquitectura de Tilemaps

```text
Farm Authoring Grid
├── Ground
├── Paths
├── Soil
└── Decoration
```

Cada parcela conserva esta estructura conceptual:

```text
Plot x-y
├── Soil Visual
└── Crop Entity Visual
```

## Trabajo completado en esta etapa

- Herramientas de autoría y normalización de la escena de granja.
- Separación de capas de Tilemap para suelo, caminos, tierra cultivable y decoración.
- Pipeline de arte placeholder libre y herramientas de sustitución semántica.
- Generación, reparación, alineación de base y vinculación de sprites de cultivos.
- Sprites libres disponibles para:
  - nabo: 5 etapas;
  - papa: 6 etapas;
  - rábano: 5 etapas.
- Actualización del dominio agrícola para usar los nombres definitivos:
  - `turnip` / nabo;
  - `potato` / papa;
  - `radish` / rábano.
- IDs definitivos:
  - `turnip-seeds`, `turnip`;
  - `potato-seeds`, `potato`;
  - `radish-seeds`, `radish`.
- `CropCatalog`, `ItemCatalog`, parcelas, inventario, vinculadores y pruebas actualizados.
- Compatibilidad temporal de serialización mediante `FormerlySerializedAs` para preservar referencias visuales antiguas.
- Alias antiguos de C# marcados como `Obsolete` para facilitar la detección de referencias residuales.
- Los IDs de texto antiguos `carrot` y `cabbage` ya no se aceptan como datos válidos.

## Archivos principales afectados en el cierre

- `Assets/_Project/Scripts/Domain/ItemCatalog.cs`
- `Assets/_Project/Scripts/Domain/Farming/CropCatalog.cs`
- `Assets/_Project/Scripts/Presentation/Farming/FarmPlotBehaviour.cs`
- `Assets/_Project/Scripts/Editor/PlaceholderCropSpriteBinder.cs`
- `Assets/_Project/Tests/EditMode/ItemCatalogTests.cs`
- `Assets/_Project/Tests/EditMode/FarmingStateTests.cs`

## Estado de verificación

- La rama `dev/farm-scene-authoring` parte de `main` y puede integrarse mediante avance rápido.
- GitHub no reporta estados de CI configurados para el último commit consultado.
- Validación local todavía recomendada después de actualizar el proyecto:
  1. abrir Unity y esperar la recompilación;
  2. confirmar que la consola quede sin errores;
  3. ejecutar los Edit Mode Tests;
  4. abrir `Farm.unity`;
  5. verificar que las parcelas conserven referencias de sprites y alineación con la cuadrícula;
  6. probar siembra, riego, avance de día y cosecha para nabo, papa y rábano.

## Próxima etapa recomendada

Completar y validar el bucle agrícola jugable:

1. selección de herramienta o semillas desde la hotbar;
2. detección de la celda frente al jugador;
3. arado y riego sobre parcelas válidas;
4. siembra de nabo, papa y rábano;
5. avance de crecimiento al dormir;
6. cosecha e ingreso del producto al inventario;
7. retroalimentación visual y mensajes de interacción;
8. pruebas Edit Mode y Play Mode del flujo completo.

## Reglas para continuar

- Leer antes de cambiar código:
  - `BITÁCORA_CIERRE_FARM_SCENE_AUTHORING_2026-08-06.md`;
  - `BITÁCORA_TILE_PIPELINE_2026-08-06.md`;
  - `BUGS.MD`;
  - `BUGS_TILE_PIPELINE_2026-08-06.md`;
  - `BUGS_FARM_SCENE_AUTHORING_2026-08-06.md`.
- Verificar siempre las herramientas disponibles antes de afirmar que no hay acceso al repositorio.
- No modificar los PNG aprobados sin una petición explícita.
- No volver a introducir cultivos como tiles de estado.
- Mantener 16 PPU y una unidad por celda.
- Trabajar desde una rama nueva creada a partir de `main`.
