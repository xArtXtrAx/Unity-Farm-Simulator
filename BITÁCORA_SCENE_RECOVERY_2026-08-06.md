# Bitácora — recuperación moderna de Farm y HouseInterior

Fecha: 2026-08-06
Rama: `dev/farm-scene-authoring`

## Incidente

`Assets/_Project/Scenes/Farm.unity` y `HouseInterior.unity` no estaban versionadas en Git. El proyecto conservaba código y pruebas que asumían su existencia, mientras `HouseAndSleepScenePipeline` podía regenerarlas automáticamente con una composición antigua basada en sprites y tiles obsoletos.

## Realizado

- Se retiró el comportamiento destructivo y automático de `HouseAndSleepScenePipeline`; ahora es un stub Legacy sin generación durante recarga de dominio.
- Se añadió `ModernFarmSceneAuthoring` bajo:

```text
Tools
→ Farm Simulator
→ Farm Development Kit
→ Scene Recovery
```

Acciones disponibles:

- `Generate Missing Farm + HouseInterior`: crea únicamente las escenas faltantes.
- `Replace Farm + HouseInterior (with backup)`: crea copias con timestamp en `Assets/_Project/SceneBackups` y reemplaza las escenas después de confirmación explícita.
- `Configure Art Profile`: abre el perfil curado de referencias visuales.

La Farm reconstruida incluye:

```text
Farm World
├── Farm Authoring Grid
│   ├── Ground
│   ├── Paths
│   ├── Soil
│   └── Decoration
├── Scene Authoring Bounds
├── Movement Boundary
├── Spawn FarmStart
├── Spawn FarmHouseDoor
├── House Entrance Portal
└── Player
```

`HouseInterior` incluye un grid moderno para `Ground`, `Walls` y `Decoration`, spawns de entrada/despertar, cama interactiva, portal de salida, cámara y límites físicos.

## Corrección después de la primera validación visual

Arturo ejecutó el autor desde `Lab` y confirmó que las escenas se creaban, pero la primera versión resolvía tiles y sprites mediante coincidencias parciales de nombre (`grass`, `wood`, `bed`). Con el catálogo grande del proyecto, esas búsquedas seleccionaron assets incorrectos y produjeron mosaicos repetidos y fondos visualmente inválidos.

La estrategia heurística quedó eliminada por completo.

Se añadió `SceneRecoveryArtProfile`, guardado en:

```text
Assets/_Project/Editor/Scene Recovery Art Profile.asset
```

El perfil contiene referencias exactas para:

- Farm Ground Tile;
- Farm Path Tile;
- Farm House Sprite;
- House Floor Tile;
- House Wall Tile;
- Bed Sprite.

El autor moderno ahora cumple este contrato:

- solo usa las referencias exactas asignadas en el perfil;
- nunca busca assets por nombre parcial;
- nunca sustituye una referencia faltante por otro asset;
- cuando una referencia no está asignada, deja la capa u objeto visual vacío;
- al terminar muestra un reporte indicando escenas generadas, escenas omitidas y referencias faltantes;
- cualquier excepción se muestra en diálogo y queda registrada completa en Console.

## Comprobado por inspección

- No existe llamada a `FarmSceneGridLayoutResetter` desde el nuevo autor.
- No existe inicialización automática que reconstruya Farm o HouseInterior.
- No existen `AssetDatabase.FindAssets` ni búsquedas de texto para seleccionar arte durante la generación.
- Los nombres de escenas, spawns, portales y cama utilizan las APIs runtime vigentes.
- Las escenas se añaden habilitadas a Build Settings.
- Las escenas existentes se respaldan antes del reemplazo.

## Pendiente de validación local

No se ejecutó Unity desde este entorno. Arturo debe:

1. actualizar `dev/farm-scene-authoring`;
2. abrir Unity y confirmar compilación sin errores;
3. abrir `Scene Recovery > Configure Art Profile`;
4. asignar referencias exactas desde las paletas y bibliotecas Cozy vigentes;
5. cerrar Farm y HouseInterior si están abiertas;
6. ejecutar `Replace Farm + HouseInterior (with backup)`;
7. abrir ambas escenas y comprobar jerarquías, cámara, colliders y arte;
8. ejecutar EditMode y PlayMode completos;
9. probar Farm → HouseInterior → dormir → nuevo día → Farm;
10. versionar `Farm.unity`, `Farm.unity.meta`, `HouseInterior.unity`, `HouseInterior.unity.meta` y el perfil de arte.

## Commits del incremento

- `0bd2b36f6c5f8a0ce55a785cbec2bf08750e491e` — autor moderno inicial.
- `36f2341190776420f7c47935b7a94da79bb84589` — retiro definitivo del generador obsoleto.
- `ebd1a02829a3d8e19fa8520190e5bed2d843c2c2` — pruebas de arquitectura moderna.
- `205688feea447148a6c095dbc5f269fa0c170f99` — perfil curado de referencias visuales.
- `63146918747984219b850dfa80b03cf6a21398e4` — eliminación de búsquedas heurísticas y reporte visible de generación.

## Siguiente paso exacto

Asignar referencias exactas en el perfil local, reemplazar ambas escenas con backup y revisar visualmente el resultado. No marcar este incremento como verificado hasta completar compilación, suites y flujo jugable local.
