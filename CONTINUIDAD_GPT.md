# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** ninguna; la fase de hotbar ya fue integrada
- **Integración más reciente:** PR #8 — `Add inventory hotbar presentation foundation`
- **Squash commit PR #8:** `bf89a9a5ee9ea8f45e9e48b751b9b027922dbe3a`
- **Estado local de Arturo:** `main` sincronizado después de la fusión
- **Validación técnica:** **166/166 EditMode**, **8/8 PlayMode**, **0 errores**
- **Validación manual:** hotbar, teclado, teclado numérico, rueda, L1/R1 y movimiento del héroe aprobados
- **Integración anterior:** PR #7 — `Add Cozy Farm pilot art and visual calibration`
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
- Hotbar uGUI funcional de ocho slots conectada al `InventoryState`.

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

La escala 1.5 todavía no se ha trasladado al prefab real del héroe.

## Hotbar integrada

Fuente congelada estudiada en `farming-game-A`:

- `src/game/inventory/HotbarView.ts`;
- `src/game/inventory/InventoryManager.ts`;
- `src/game/inventory/InventoryState.ts`;
- commit `dd32056c9f8142a2322bc2c1d41f0b05b002598f`.

Implementación integrada:

- `HotbarPresentationModel` puro en Application;
- ocho slots con índice, ID, nombre, abreviatura, cantidad y selección;
- prefab uGUI generado y versionado en Resources;
- Canvas de referencia 960×540 y anclaje inferior central;
- selección con 1–8, teclado numérico, rueda y L1/R1;
- nombre del objeto seleccionado;
- cantidad visible cuando es mayor que uno;
- iconos Cozy Farm para seis semillas/cosechas;
- placeholders explícitos `AZ` y `RG` para azada y regadera;
- instalación automática al cargar `Lab`;
- inventario inicial: azada, regadera, 20 semillas de nabo y cinco slots vacíos.

Prefab versionado:

```text
Assets/_Project/Resources/Prefabs/UI.meta
Assets/_Project/Resources/Prefabs/UI/InventoryHotbar.prefab
Assets/_Project/Resources/Prefabs/UI/InventoryHotbar.prefab.meta
```

Firma:

```text
inventory-hotbar-prefab-v1
```

Comando de reconstrucción:

```text
Tools > Farm Simulator > Rebuild Inventory Hotbar
```

Pruebas añadidas y confirmadas:

- **28 casos EditMode nuevos**;
- **2 casos PlayMode nuevos**;
- total integrado: **166/166 EditMode**;
- total integrado: **8/8 PlayMode**;
- errores: **0**.

## Incidencia de NUnit — resuelta

La primera ejecución local obtuvo **165/166 EditMode**. El único fallo fue `HotbarMapsOnlyApprovedCozyFarmIcons` por aplicar `Has.Count` sobre `Sprite[]` en la versión de NUnit incluida con Unity.

Se corrigió con:

```text
Assert.That(view.IconSprites.Count, Is.EqualTo(expectedIds.Length));
```

Después pasaron todas las suites.

## Integración del PR #8

- Rama fuente: `feature/inventory-hotbar-presentation`.
- Head validado: `e1898756cb28a0049b6a45b11367d94d617b1ad9`.
- PR: #8.
- Método: **Squash and merge**.
- Commit integrado: `bf89a9a5ee9ea8f45e9e48b751b9b027922dbe3a`.
- Arturo confirmó después que su `main` local quedó sincronizado.

Los PR borrador #9, #10 y #11 fueron creados accidentalmente durante el cierre documental, se cerraron de inmediato, no fueron fusionados y no modificaron `main`.

## Exclusiones de la fase integrada

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

## Próxima acción

La fase de hotbar está cerrada. Antes de escribir nueva funcionalidad, revisar `main`, escoger el siguiente bloque acotado y crear una rama nueva desde la línea estable.

---

## Orden obligatorio de lectura

1. Leer este archivo desde `main`.
2. Leer `HOTBAR_PRESENTATION.md` desde `main`.
3. Leer `BITÁCORA_GPT.MD` y `COZY_FARM_INTAKE.md`.
4. Leer `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`.
5. Revisar ramas, commits y PR recientes antes de escribir.

## Reglas críticas

- No reemplazar automáticamente el héroe.
- No subir el ZIP completo ni GIF.
- No asignar imágenes falsas a azada o regadera.
- No escalar la raíz o el collider del jugador.
- Separar agricultura, Tilemaps y herramientas funcionales en fases propias.
- Crear cada nueva fase desde el `main` actualizado.
- Mantener documentación sincronizada después de cada integración.

## Prompt mínimo para un chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee CONTINUIDAD_GPT.md, HOTBAR_PRESENTATION.md, BITÁCORA_GPT.MD, COZY_FARM_INTAKE.md, BUGS.MD y MIGRACION_DESDE_FARMING_GAME_A.MD desde main. Fases 1–6, Cozy Farm y la hotbar están integradas. PR #8 fue fusionado mediante squash en bf89a9a5ee9ea8f45e9e48b751b9b027922dbe3a. La línea validada pasó 166/166 EditMode y 8/8 PlayMode sin errores; teclado, rueda, L1/R1 y movimiento fueron aprobados. Arturo tiene main sincronizado. Revisa el estado actual y propone el siguiente bloque acotado antes de crear una rama nueva.
```
