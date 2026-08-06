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

- El autor intenta usar tiles modernos ya generados por el navegador estacional; si no encuentra un tile adecuado, conserva las capas vacías para pintarlas desde el Tile Palette actual sin recurrir a assets viejos.
- Se actualizaron las pruebas EditMode del pipeline para comprobar la arquitectura moderna y para impedir que el pipeline Legacy vuelva a generar escenas.
- Se añadió limpieza de marcadores de editor antes de serializar escenas.

## Comprobado por inspección

- No existe llamada a `FarmSceneGridLayoutResetter` desde el nuevo autor.
- No existe inicialización automática que reconstruya Farm o HouseInterior.
- Los nombres de escenas, spawns, portales y cama utilizan las APIs runtime vigentes.
- Las escenas se añaden habilitadas a Build Settings.

## Pendiente de validación local

No se ejecutó Unity desde este entorno. Arturo debe:

1. actualizar `dev/farm-scene-authoring`;
2. abrir Unity y confirmar compilación sin errores;
3. ejecutar `Generate Missing Farm + HouseInterior`;
4. abrir ambas escenas y comprobar jerarquías, cámara y colliders;
5. decorar/pintar con el Tile Palette y las bibliotecas Cozy actuales;
6. ejecutar EditMode y PlayMode completos;
7. probar Farm → HouseInterior → dormir → nuevo día → Farm;
8. versionar `Farm.unity`, `Farm.unity.meta`, `HouseInterior.unity` y `HouseInterior.unity.meta`.

## Commits del incremento

- `0bd2b36f6c5f8a0ce55a785cbec2bf08750e491e` — autor moderno inicial.
- `36f2341190776420f7c47935b7a94da79bb84589` — retiro definitivo del generador obsoleto.
- `ebd1a02829a3d8e19fa8520190e5bed2d843c2c2` — pruebas de arquitectura moderna.
- commits auxiliares posteriores — metadata, sanitización y limpieza del guard redundante.

## Siguiente paso exacto

Ejecutar la generación local en Unity, validar las dos escenas y publicar inmediatamente los cuatro archivos de escena y metadata en `dev/farm-scene-authoring`. No marcar este incremento como verificado hasta completar esa validación.
