# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `feature/inventory-hotbar-presentation`
- **Head remoto registrado de la rama:** `19c3d73a349426990be90b7ba99bf8baa2a04664`
- **Commit funcional inicial:** `9084ab05874ae8e7f013d701135d8e9ce1cef762`
- **Corrección NUnit:** `6cb50b63a91cc35463419e9ad0b594036a56165c`
- **Bloque actual:** presentación de inventario y hotbar
- **Estado:** compilación, prefab y suites completas validados; inspección visual y controles manuales en `Lab` pendientes
- **Validación técnica de la rama:** **166/166 EditMode**, **8/8 PlayMode**, **0 errores**
- **Última integración:** PR #7 — `Add Cozy Farm pilot art and visual calibration`
- **Squash commit PR #7:** `7860095d0d165c83585f21579e9794ea57ec0a35`
- **Bugs activos:** ninguno
- **Bugs verificados:** `BUG-0001` a `BUG-0006`
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`

## Línea base integrada

- Proyecto Unity/URP y arquitectura por capas.
- Calibración cenital XY, cámara ortográfica y resolución lógica 960×540.
- Movimiento por teclado y DualSense.
- Héroe animado y prefab reutilizable con profundidad por Y.
- Catálogo de objetos e inventario puro en Domain.
- Piloto Cozy Farm con fuentes curadas, slicing nombrado, pruebas y pipeline de exhibición reproducible.

`FarmSimulator.Domain` permanece independiente de `UnityEngine`.

## Piloto Cozy Farm integrado

Fuentes:

```text
Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/
├── items.png
├── seeds.png
├── tools.png
├── crops.png
└── tiles.png
```

Slicing aprobado:

- 3 objetos cosechados;
- 3 bolsas de semillas;
- 18 etapas de cultivo;
- 4 muestras de terreno;
- `tools.png` permanece Single.

Configuración: Sprite 2D/UI, 16 PPU, Point, sin mipmaps, Clamp y sin compresión.

Alias provisionales:

- `radish → turnip`;
- `lettuce → cabbage`.

## Decisiones visuales aprobadas

- conservar el héroe actual;
- raíz y collider del jugador en **1.0**;
- visual del héroe en **1.5** frente al mundo Cozy Farm;
- tiles y cultivos en **1.0**;
- iconos del catálogo en **0.75** dentro de la exhibición;
- `stage_0` de los tres cultivos con offset vertical **−0.3**;
- `cozy_tilled_soil` es provisionalmente un hoyo/montículo, no una parcela rectangular completa.

La escala 1.5 todavía no se ha trasladado al prefab real.

## Hotbar — validación técnica completada

Fuente congelada estudiada en `farming-game-A`:

- `src/game/inventory/HotbarView.ts`;
- `src/game/inventory/InventoryManager.ts`;
- `src/game/inventory/InventoryState.ts`;
- commit `dd32056c9f8142a2322bc2c1d41f0b05b002598f`.

La rama activa añade:

- `HotbarPresentationModel` puro en Application;
- ocho slots con índice, ID, nombre, abreviatura, cantidad y selección;
- prefab uGUI generado en Resources;
- Canvas de referencia 960×540 y anclaje inferior central;
- selección con 1–8, teclado numérico, rueda y L1/R1;
- nombre del objeto seleccionado;
- cantidad visible cuando es mayor que uno;
- iconos Cozy Farm para seis semillas/cosechas;
- placeholders explícitos `AZ` y `RG` para azada y regadera;
- instalación automática de la hotbar al cargar `Lab`;
- inventario inicial: azada, regadera, 20 semillas de nabo y cinco slots vacíos.

Prefab generado localmente:

```text
Assets/_Project/Resources/Prefabs/UI/InventoryHotbar.prefab
```

Comando manual:

```text
Tools > Farm Simulator > Rebuild Inventory Hotbar
```

Pruebas añadidas y confirmadas:

- **28 casos EditMode nuevos**;
- **2 casos PlayMode nuevos**;
- total final: **166/166 EditMode**;
- total final: **8/8 PlayMode**;
- errores: **0**.

## Incidencia de NUnit — resuelta

Primera ejecución local:

- EditMode: **165/166**;
- único fallo: `HotbarMapsOnlyApprovedCozyFarmIcons`;
- error: `System.ArgumentException: Property Count was not found`;
- ubicación: `InventoryHotbarAssetPipelineTests.cs:108`.

Diagnóstico:

- la hotbar y el mapeo no fallaron;
- NUnit de Unity no pudo aplicar `Has.Count` sobre el arreglo `Sprite[]`;
- se sustituyó por una comparación directa de `view.IconSprites.Count`.

Corrección publicada:

```text
6cb50b63a91cc35463419e9ad0b594036a56165c
```

Después de la corrección, Arturo confirmó todas las suites sin errores.

Guía operativa de la rama:

```text
HOTBAR_PRESENTATION.md
```

## Exclusiones de la rama activa

No se implementaron:

- agricultura;
- herramientas funcionales;
- consumo de semillas;
- economía;
- almacenamiento;
- guardado;
- Tilemaps;
- inventario completo desplegable;
- cambios a `Lab.unity`;
- cambios a `InputSystem_Actions.inputactions`;
- escala 1.5 en el prefab real del héroe.

El prefab, collider, spritesheet, animaciones y movimiento del héroe permanecen intactos.

## Próxima acción local

1. Abrir `Bootstrap` o `Lab` y pulsar Play.
2. Confirmar visualmente:
   - hotbar inferior centrada;
   - slot 1 `AZ` seleccionado;
   - slot 2 `RG`;
   - slot 3 con icono de semillas y `×20`;
   - slots 4–8 vacíos;
   - nombre del objeto seleccionado legible.
3. Probar selección con teclas 1–8 y teclado numérico.
4. Probar ciclo con rueda del mouse.
5. Probar L1/R1 con DualSense.
6. Confirmar que el movimiento del héroe permanezca intacto.
7. Compartir una captura y observaciones de tamaño, posición, contraste o legibilidad.
8. No abrir PR ni hacer commit manual de assets generados hasta cerrar la revisión visual.

---

## Orden obligatorio de lectura

1. Leer este archivo desde `main`.
2. Leer `HOTBAR_PRESENTATION.md` desde `feature/inventory-hotbar-presentation`.
3. Leer `BITÁCORA_GPT.MD` y `COZY_FARM_INTAKE.md` desde `main`.
4. Leer `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`.
5. Revisar ramas y commits recientes.

## Reglas críticas

- No reemplazar automáticamente el héroe.
- No subir el ZIP completo ni GIF.
- No asignar imágenes falsas a azada o regadera.
- No escalar la raíz o el collider del jugador.
- No crear Tilemaps o agricultura funcional dentro del bloque de hotbar.
- No abrir PR ni fusionar la rama antes de cerrar la inspección visual y de controles.
- Mantener documentación sincronizada después de cada transacción.

## Prompt mínimo para un chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee CONTINUIDAD_GPT.md desde main y HOTBAR_PRESENTATION.md desde feature/inventory-hotbar-presentation. Fases 1–6 y Cozy Farm están integrados. La rama activa está en 19c3d73a349426990be90b7ba99bf8baa2a04664. La hotbar compiló, generó prefab y pasó 166/166 EditMode y 8/8 PlayMode sin errores. El único fallo inicial de NUnit fue corregido en 6cb50b63a91cc35463419e9ad0b594036a56165c. Falta revisar visualmente la hotbar en Lab y probar 1–8, rueda y L1/R1 antes del PR. No avances a agricultura o Tilemaps.
```
