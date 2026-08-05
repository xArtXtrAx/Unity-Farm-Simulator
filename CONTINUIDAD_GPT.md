# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `feature/inventory-hotbar-presentation`
- **Head remoto de la rama:** `e1898756cb28a0049b6a45b11367d94d617b1ad9`
- **Pull request abierto:** PR #8 — `Add inventory hotbar presentation foundation`
- **Estado del PR:** abierto, no draft y fusionable
- **Commit funcional inicial:** `9084ab05874ae8e7f013d701135d8e9ce1cef762`
- **Corrección NUnit:** `6cb50b63a91cc35463419e9ad0b594036a56165c`
- **Prefab generado:** `a6e84425b72d9cfa656adf756bb69bfdbf6e599b`
- **Cierre documental de la rama:** `e1898756cb28a0049b6a45b11367d94d617b1ad9`
- **Bloque actual:** presentación de inventario y hotbar
- **Estado:** fase técnica, visual y funcionalmente aprobada; PR listo para Squash and merge
- **Validación técnica:** **166/166 EditMode**, **8/8 PlayMode**, **0 errores**
- **Validación manual:** hotbar, teclado, teclado numérico, rueda, L1/R1 y movimiento del héroe aprobados
- **Última integración estable:** PR #7 — `Add Cozy Farm pilot art and visual calibration`
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

## Hotbar — validación completa

Fuente congelada estudiada en `farming-game-A`:

- `src/game/inventory/HotbarView.ts`;
- `src/game/inventory/InventoryManager.ts`;
- `src/game/inventory/InventoryState.ts`;
- commit `dd32056c9f8142a2322bc2c1d41f0b05b002598f`.

La rama activa añade:

- `HotbarPresentationModel` puro en Application;
- ocho slots con índice, ID, nombre, abreviatura, cantidad y selección;
- prefab uGUI generado y versionado en Resources;
- Canvas de referencia 960×540 y anclaje inferior central;
- selección con 1–8, teclado numérico, rueda y L1/R1;
- nombre del objeto seleccionado;
- cantidad visible cuando es mayor que uno;
- iconos Cozy Farm para seis semillas/cosechas;
- placeholders explícitos `AZ` y `RG` para azada y regadera;
- instalación automática de la hotbar al cargar `Lab`;
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

Validación manual confirmada por Arturo el 5 de agosto de 2026:

- hotbar inferior centrada;
- `AZ`, `RG`, semillas de nabo `×20` y cinco slots vacíos correctos;
- nombre del objeto seleccionado legible;
- teclas 1–8 y teclado numérico correctos;
- rueda del mouse correcta;
- L1/R1 con DualSense correctos;
- movimiento del héroe intacto;
- sin errores observados.

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

## Pull previo al push del prefab

GitHub Desktop solicitó un Pull porque la documentación remota avanzó mientras el prefab se preparaba localmente. La integración automática creó:

```text
85e0365bad0df07c2d3b79a51b3de3047ecb6226
```

No hubo conflictos manuales ni pérdida de archivos. El historial intermedio se consolidará mediante Squash and merge.

## Pull request #8

```text
https://github.com/xArtXtrAx/Unity-Farm-Simulator/pull/8
```

- Título: `Add inventory hotbar presentation foundation`.
- Base: `main`.
- Head: `feature/inventory-hotbar-presentation`.
- Head SHA: `e1898756cb28a0049b6a45b11367d94d617b1ad9`.
- Archivos cambiados: 26.
- Estado: abierto y fusionable.
- Método recomendado: **Squash and merge**.

La revisión remota confirmó que el alcance contiene únicamente:

- modelo de presentación;
- vista, instalador y catálogo de la hotbar;
- pipeline Editor;
- prefab y metadatos;
- pruebas EditMode y PlayMode;
- referencias `Unity.ugui` necesarias;
- guía `HOTBAR_PRESENTATION.md`.

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

## Próxima acción exacta

1. Abrir el PR #8.
2. Confirmar que GitHub siga mostrando el PR como fusionable.
3. Elegir **Squash and merge**.
4. Usar como título sugerido:

```text
Add inventory hotbar presentation foundation (#8)
```

5. Confirmar la fusión.
6. En GitHub Desktop, cambiar a `main` y hacer Fetch/Pull.
7. No borrar la rama hasta confirmar el nuevo head de `main` y actualizar esta continuidad.

---

## Orden obligatorio de lectura

1. Leer este archivo desde `main`.
2. Leer `HOTBAR_PRESENTATION.md` desde `feature/inventory-hotbar-presentation` mientras el PR #8 siga abierto.
3. Leer `BITÁCORA_GPT.MD` y `COZY_FARM_INTAKE.md` desde `main`.
4. Leer `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`.
5. Revisar ramas, commits recientes y PR #8.

## Reglas críticas

- No reemplazar automáticamente el héroe.
- No subir el ZIP completo ni GIF.
- No asignar imágenes falsas a azada o regadera.
- No escalar la raíz o el collider del jugador.
- No crear Tilemaps o agricultura funcional dentro del bloque de hotbar.
- Integrar PR #8 mediante Squash and merge.
- No afirmar que la rama está integrada hasta verificar el nuevo head de `main`.
- Mantener documentación sincronizada después de la fusión.

## Prompt mínimo para un chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee CONTINUIDAD_GPT.md desde main y HOTBAR_PRESENTATION.md desde feature/inventory-hotbar-presentation. Fases 1–6 y Cozy Farm están integrados. El PR #8 está abierto y fusionable en e1898756cb28a0049b6a45b11367d94d617b1ad9. La hotbar pasó 166/166 EditMode y 8/8 PlayMode sin errores; teclado, rueda, L1/R1 y movimiento fueron aprobados manualmente. El prefab está versionado. Próximo paso: Squash and merge del PR #8, verificar main y actualizar continuidad. No avances a agricultura o Tilemaps antes de cerrar esa integración.
```
