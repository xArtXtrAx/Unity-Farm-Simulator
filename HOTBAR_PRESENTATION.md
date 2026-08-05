# Inventario y hotbar — fase de presentación

## Estado actual

- Rama: `feature/inventory-hotbar-presentation`.
- Base: `main` en `57d7d165cfba47dcc753a6fc9c484a7191ab068a`.
- Commit funcional inicial: `9084ab05874ae8e7f013d701135d8e9ce1cef762`.
- Corrección de compatibilidad NUnit: `6cb50b63a91cc35463419e9ad0b594036a56165c`.
- Estado: **compilación, generación del prefab y suites completas validadas; inspección visual y controles manuales en `Lab` pendientes**.
- Línea base validada de la rama: **166/166 EditMode**, **8/8 PlayMode**, **0 errores**.

## Fuente congelada estudiada

Repositorio `xArtXtrAx/farming-game-A`, commit:

```text
dd32056c9f8142a2322bc2c1d41f0b05b002598f
```

Archivos de referencia:

- `src/game/inventory/HotbarView.ts`;
- `src/game/inventory/InventoryManager.ts`;
- `src/game/inventory/InventoryState.ts`.

Contrato conservado:

- ocho slots;
- selección directa 1–8;
- ciclo con rueda y L1/R1;
- nombre del objeto seleccionado;
- cantidad visible solo cuando es mayor que uno;
- selección visual distinta;
- etiqueta explícita para manos vacías.

## Implementación

### Application

`HotbarPresentationModel` traduce el `InventoryState` puro a una lista inmutable de `HotbarSlotPresentation` con:

- índice;
- ID opcional;
- nombre;
- abreviatura;
- cantidad;
- selección;
- estado vacío.

La UI no replica reglas de catálogo, selección o inventario.

### Presentation

`InventoryHotbarView` presenta:

- ocho slots numerados;
- selección visual;
- nombre seleccionado;
- cantidades;
- seis iconos Cozy Farm aprobados;
- placeholders `AZ` y `RG` para azada y regadera;
- controles 1–8, teclado numérico, rueda, L1 y R1.

`InventoryHotbarInstaller` instala la hotbar al cargar `Lab`, con el inventario inicial:

1. azada ×1;
2. regadera ×1;
3. semillas de nabo ×20;
4. cinco slots vacíos.

### Editor

`InventoryHotbarAssetPipeline` genera reproduciblemente:

```text
Assets/_Project/Resources/Prefabs/UI/InventoryHotbar.prefab
```

Comando manual:

```text
Tools > Farm Simulator > Rebuild Inventory Hotbar
```

Contrato visual inicial:

- Canvas Screen Space Overlay;
- referencia 960×540;
- anclaje inferior central;
- ocho slots de 46 px;
- separación de 6 px;
- iconos de 30 px;
- sorting order 1000.

### Arte

Iconos conectados:

- semillas de nabo, zanahoria y col;
- nabo, zanahoria y col cosechados.

No se asignaron imágenes falsas a:

- azada;
- regadera.

## Pruebas añadidas y validadas

### EditMode

- 22 casos del modelo de presentación;
- 6 casos del prefab/pipeline y su render inicial;
- total nuevo: **28 casos**.

Resultado local confirmado por Arturo el 5 de agosto de 2026:

```text
166/166 EditMode
0 errores
```

### PlayMode

- instalación de la hotbar en `Lab`;
- cambio de selección sin reemplazar al jugador.

Resultado local confirmado por Arturo:

```text
8/8 PlayMode
0 errores
```

## Incidencia de NUnit — resuelta

Primera ejecución local:

- EditMode: **165/166**;
- único fallo: `HotbarMapsOnlyApprovedCozyFarmIcons`;
- excepción: `System.ArgumentException: Property Count was not found`;
- línea afectada: aserción `Has.Count` sobre `Sprite[]`.

Diagnóstico:

- no falló el mapeo de iconos;
- no falló la hotbar ni el prefab;
- la versión de NUnit incluida en Unity intentó resolver una propiedad reflectiva `Count` sobre un arreglo.

Corrección:

```text
Assert.That(view.IconSprites.Count, Is.EqualTo(expectedIds.Length));
```

Commit:

```text
6cb50b63a91cc35463419e9ad0b594036a56165c
```

Después de la corrección, Arturo confirmó **166/166 EditMode** y **8/8 PlayMode**, sin errores.

## Exclusiones

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

El prefab, collider, spritesheet, animaciones y movimiento del héroe siguen intactos.

## Validación visual y manual pendiente

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
7. Compartir una captura y cualquier observación de tamaño, posición, contraste o legibilidad.
8. No abrir PR ni hacer commit manual de assets generados hasta cerrar esta revisión visual.
