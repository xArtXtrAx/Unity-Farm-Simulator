# Cozy Farm — recepción piloto

## Estado actual

- Rama: `chore/cozy-farm-art-intake`.
- Head funcional A3.1: `39abe438bb6068b21438fb836b5eea01295f0db3`.
- El héroe actual se conserva sin cambios.
- El paquete completo `full version.zip` permanece fuera del repositorio.
- A1, recepción de cinco hojas fuente: **VALIDADA LOCALMENTE**.
- A2, slicing curado y pruebas automáticas: **VALIDADO LOCALMENTE**.
- A3, primera exhibición: **VALIDADA TÉCNICAMENTE; COMPOSICIÓN VISUAL RECHAZADA POR DESPROPORCIÓN**.
- A3.1, exhibición reequilibrada: **IMPLEMENTADA REMOTAMENTE; REGENERACIÓN Y VALIDACIÓN LOCAL PENDIENTES**.

## A1 — Fuente piloto validada

Commit local publicado por Arturo:

```text
e4540b42d275b650f726bad41d4546787ae544e9
```

Archivos fuente:

```text
Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/
├── items.png
├── seeds.png
├── tools.png
├── crops.png
└── tiles.png
```

Arturo confirmó el 5 de agosto de 2026 que la importación inicial quedó correcta. La configuración permanece en 16 PPU, Point, sin mipmaps, Clamp y sin compresión.

## A2 — Slicing curado validado

No se duplicaron ni recortaron los PNG. Se conservaron las hojas originales y se añadieron únicamente rectángulos nombrados en sus `.meta`.

### Objetos y semillas

`items.png` expone:

- `cozy_turnip`;
- `cozy_carrot`;
- `cozy_cabbage`.

`seeds.png` expone:

- `cozy_turnip_seeds`;
- `cozy_carrot_seeds`;
- `cozy_cabbage_seeds`.

### Cultivos

`crops.png` expone seis etapas para cada cultivo:

- `cozy_turnip_stage_0` a `cozy_turnip_stage_5`;
- `cozy_carrot_stage_0` a `cozy_carrot_stage_5`;
- `cozy_cabbage_stage_0` a `cozy_cabbage_stage_5`.

Los cultivos usan pivote inferior central para futura colocación sobre la parcela.

### Terreno

`tiles.png` expone solo cuatro muestras piloto:

- `cozy_grass`;
- `cozy_dirt`;
- `cozy_water`;
- `cozy_tilled_soil`.

`cozy_tilled_soil` se conserva como muestra aislada provisional. A3.1 deja de repetirla bajo cada cultivo porque su silueta circular dominaba la lectura visual de la exhibición.

### Validación local A2 — 2026-08-05

Arturo comprobó dentro de Unity exactamente:

- `items.png`: **3** sprites;
- `seeds.png`: **3** sprites;
- `crops.png`: **18** sprites;
- `tiles.png`: **4** sprites;
- `tools.png`: una sola planilla sin cortes.

Después ejecutó todas las pruebas:

- EditMode: **130/130**;
- PlayMode: **6/6**;
- errores: **0**.

A2 queda aprobado completamente.

### Alias visuales provisionales

El paquete denomina **radish** al recurso usado provisionalmente para el ID de dominio `turnip`, y **lettuce** al usado provisionalmente para `cabbage`. Los IDs de dominio no cambiaron. Estos alias son reversibles.

### Herramientas

`tools.png` contiene máquinas de procesamiento y mobiliario, no iconos adecuados de azada y regadera. Permanece en modo Single y sin slicing. No se asignaron sustitutos falsos a `hoe` o `watering-can`.

## A3 — Primera escena artística de exhibición

El generador Editor reproducible crea:

```text
Assets/_Project/Scenes/CozyFarmShowcase.unity
```

También puede regenerarse desde:

```text
Tools > Farm Simulator > Rebuild Cozy Farm Showcase
```

La escena es independiente de `Lab`. La cámara usa `SpatialModel.CameraOrthographicSize` (**4.21875**) y `ReferenceAspectCamera`.

### Validación local A3 — 2026-08-05

Arturo confirmó:

- EditMode: **134/134**;
- PlayMode: **6/6**;
- errores: **0**.

La implementación técnica quedó aprobada, pero la captura visual mostró una composición desproporcionada:

- objetos y semillas tratados como sprites físicos de mundo;
- bases circulares repetidas bajo las 18 etapas;
- muestras de terreno 2×2 demasiado grandes;
- exceso de espacio vacío;
- comparación de escala poco clara.

A3 queda registrada como validada técnicamente, pero no aceptada visualmente.

## A3.1 — Exhibición reequilibrada

Archivos modificados:

```text
Assets/_Project/Scripts/Editor/CozyFarmShowcaseScenePipeline.cs
Assets/_Project/Tests/EditMode/CozyFarmShowcaseSceneTests.cs
```

Cambios:

- firma de escena elevada a `cozy-farm-showcase-scene-v2` para forzar regeneración;
- objetos y semillas colocados sobre una referencia de 3×2 slots y escalados a **0.55**;
- cultivos conservados a escala de mundo **1.0**;
- eliminación de los 18 marcadores `soil_for_*`;
- las 18 etapas se colocan sobre una cama compartida de tierra de **6×3 tiles**;
- las cuatro muestras de terreno pasan de bloques 2×2 a tiles individuales alineados;
- el héroe conserva escala **1.0** y se compara sobre una referencia de tierra de **2×2 tiles**;
- distribución compactada dentro del contrato visible 960×540;
- si la escena antigua está abierta durante la recompilación, el pipeline no la sobrescribe: solicita cerrarla y ejecutar el comando de reconstrucción.

No se modificaron:

- `tiles.png.meta` ni los slices A2;
- PNG del paquete;
- `Lab`;
- prefab, spritesheet o animaciones del héroe;
- Domain, inventario, Input System o runtime de juego;
- Tilemaps, paletas, hotbar o UI funcional.

### Pruebas A3.1

`CozyFarmShowcaseSceneTests.cs` pasa de cuatro a **seis** casos. Los dos casos nuevos verifican:

- separación explícita entre iconos de interfaz a escala 0.55 y sprites de mundo a escala 1.0;
- panel compartido, cama de cultivo compartida, ausencia de objetos `soil_for_*`, muestras de un solo tile y referencia 2×2 del héroe.

Resultado esperado, todavía no confirmado localmente:

- EditMode: **136/136**;
- PlayMode: **6/6**.

## Exclusiones vigentes

No incluir ZIP, GIF, `global.png`, `item_carry.png`, personajes adicionales, animales, edificios, enemigos, máquinas adicionales ni variantes estacionales completas. No crear todavía Tilemaps, paletas funcionales, UI o conexión con el inventario.

## Próximo paso local

1. Cerrar `CozyFarmShowcase` en Unity antes de actualizar la rama.
2. En GitHub Desktop, hacer Fetch/Pull de `chore/cozy-farm-art-intake`.
3. Abrir Unity y esperar compilación e importación completas.
4. La firma `v2` debe regenerar `Assets/_Project/Scenes/CozyFarmShowcase.unity` automáticamente.
5. Si Unity avisa que la escena antigua estaba abierta, cerrarla y ejecutar `Tools > Farm Simulator > Rebuild Cozy Farm Showcase`.
6. Abrir la escena nueva y pulsar Play.
7. Confirmar visualmente:
   - iconos notablemente menores que el héroe;
   - tres filas compactas de crecimiento sin círculos repetidos;
   - cuatro muestras de un solo tile;
   - héroe intacto sobre referencia 2×2.
8. Ejecutar EditMode completo; esperado **136/136**.
9. Ejecutar PlayMode completo; esperado **6/6**.
10. Reportar captura, conteos y cualquier error o advertencia.
11. No hacer commit todavía de la escena generada ni avanzar a Tilemaps/hotbar hasta revisar el resultado visual.
