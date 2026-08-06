# BUGS — Unity Farm Simulator

Registro histórico de errores detectados, investigados y corregidos durante el desarrollo. Complementa `BITÁCORA_GPT.MD`.

## Estados

- **ABIERTO:** confirmado y pendiente de corrección.
- **INVESTIGANDO:** buscando causa raíz.
- **BLOQUEADO:** falta información o dependencia.
- **CORREGIDO:** solución implementada, pendiente de validación local.
- **VERIFICADO:** solución probada satisfactoriamente.
- **REABIERTO:** reapareció después de corregirse.
- **DESCARTADO:** no se corregirá por una razón documentada.

## Severidad

- **S0 — Bloqueante:** impide abrir, compilar o continuar.
- **S1 — Crítica:** corrupción, pérdida de datos o función principal inutilizable.
- **S2 — Alta:** comportamiento importante incorrecto con alternativa temporal.
- **S3 — Media:** fallo visible o funcional limitado.
- **S4 — Baja:** problema menor o de pulido.

---

## Bugs activos

## BUG-0014 — El panel del Footprint Editor no representaba todas las celdas guardadas

- **Estado:** CORREGIDO
- **Severidad:** S3 — Media
- **Detectado:** 2026-08-06
- **Sistema:** Farm Development Kit / huellas lógicas de edificios
- **Rama afectada:** `agent/cozy-art-pipeline`
- **Correcciones:** `dc44fc52663c0303ae6af6657f723c8d21c3091d`, `225157451d83db5ff65161d40c3696328c29b14d`
- **Prueba de regresión:** `90dc5b8010853ed7d71b950caed63cce19b73e50`
- **Reportado por:** Arturo

### Comportamiento observado

En una huella `4 × 3`, el contador y la superposición sobre el sprite indicaban diez celdas ocupadas, pero el panel de botones solo mostraba ocho. La máscara de Scene parecía distinta de la matriz editada.

### Causa raíz

La máscara runtime y el valor predeterminado de las casas utilizaban columnas `-2, -1, 0, 1`. El panel del editor calculaba el inicio horizontal con otra fórmula y mostraba `-1, 0, 1, 2`. Las dos celdas de la columna `-2` existían y participaban en colisiones, pero no podían verse ni editarse desde el panel.

### Solución aplicada

Se centralizó el origen horizontal en `GridBuildingFootprint.GetCanvasMinimumX()` y la enumeración del lienzo en `CreateRectangleOffsets()`. Runtime, relleno rectangular y panel editor comparten ahora la misma convención. En un ancho de cuatro, el ancla `(0,0)` aparece en la tercera columna y las diez celdas guardadas deben ser visibles.

### Validación pendiente

Confirmar en Unity que contador, matriz, previsualización y Scene muestran el mismo patrón; regenerar el prefab y ejecutar EditMode completo. No pasar a **VERIFICADO** hasta recibir confirmación local.

---

## BUG-0011 — Regresión de compilación por uso de LINQ en `FarmPlotBehaviour`

- **Estado:** CORREGIDO
- **Severidad:** S0 — Bloqueante
- **Detectado:** 2026-08-06
- **Sistema:** agricultura runtime / validación de sprites
- **Rama afectada:** `agent/cozy-art-pipeline`
- **Corrección funcional:** `02d1d696137a2692983e02d342ded5a7f41020df`
- **Reportado por:** Arturo

### Comportamiento observado

Unity detenía la compilación con:

```text
CS1061: 'Sprite[]' does not contain a definition for 'Any'
```

El error aparecía en `FarmPlotBehaviour.ValidateStages()` después de migrar los cultivos desde Tilemap a entidades visuales con `SpriteRenderer`.

### Causa raíz

La validación utilizaba `copy.Any(sprite => sprite == null)` sin importar `System.Linq`. El código dependía implícitamente de LINQ dentro de un ensamblado que no tenía esa extensión disponible.

### Solución aplicada

Se reemplazó `Any()` por un ciclo explícito que recorre el arreglo y lanza la misma excepción cuando encuentra una etapa nula. La solución elimina la dependencia de LINQ y evita asignaciones innecesarias.

### Validación pendiente

Confirmar que Unity compila sin errores y ejecutar las suites completas EditMode y PlayMode. El bug pasará a **VERIFICADO** después de esa validación local.

---

## BUG-0010 — Los cultivos no conservan transparencia ni una base visual común

- **Estado:** CORREGIDO
- **Severidad:** S2 — Alta
- **Detectado:** 2026-08-06
- **Sistema:** Cozy Farm Tile Palette / agricultura
- **Rama afectada:** `agent/cozy-art-pipeline`
- **Corrección funcional:** `3fbd03dafb59aeb53f8e313e9d9e13eb531adeeb`
- **Prueba de regresión:** `067aa4f43416c6ff836eb68c44e12e125f481513`
- **Reportado por:** Arturo

### Comportamiento observado

Las semillas y algunas etapas de crecimiento aparecían como cuadros opacos, reemplazaban visualmente la tierra y quedaban desplazadas hacia arriba respecto de la celda.

### Causa raíz

Los tiles usaban directamente los recortes de `crops.png`. Algunas regiones conservaban un color de fondo opaco y, aunque el importador tenía `Alpha Is Transparency`, esos píxeles seguían siendo opacos. La paleta tampoco generaba una versión normalizada con pivote verificable.

### Solución aplicada

La solución final abandona los cultivos como tiles pintables. El catálogo genera 18 sprites transparentes para runtime y cada parcela los representa mediante `Crop Entity Visual`, un `SpriteRenderer` controlado por `FarmPlotBehaviour`. El cultivo ya no puede sustituir el suelo porque no ocupa una celda del Tilemap.

### Validación pendiente

Arar, sembrar, dormir y cosechar en Play Mode; confirmar que el suelo permanece visible y que las etapas cambian sobre la parcela sin cuadros opacos.

---

## BUG-0009 — Tierra arada y cultivo comparten una sola celda del mismo Tilemap

- **Estado:** CORREGIDO
- **Severidad:** S2 — Alta
- **Detectado:** 2026-08-06
- **Sistema:** Farm Tile Manager / Tilemaps agrícolas
- **Rama afectada:** `agent/cozy-art-pipeline`
- **Corrección funcional:** `94bb3fba22b8e609cf568a4abe9fe880dda3dee5`, `d1a7654d3b6c574eddd252c63067b9bdeb0727ef`, `5f30046020c8e6181c2f596b0b9f0812436e65cb`
- **Prueba de regresión:** `067aa4f43416c6ff836eb68c44e12e125f481513`
- **Reportado por:** Arturo

### Comportamiento observado

Al colocar una etapa de cultivo sobre una celda de tierra arada, el cultivo sustituía al tile de tierra. No era posible conservar ambos simultáneamente.

### Causa raíz

La escena y el Tile Manager utilizaban un único Tilemap `Farming`. Unity permite un solo tile por celda y Tilemap, por lo que la segunda pintura reemplazaba la primera.

### Solución aplicada

La solución final separa responsabilidades:

```text
Farm Authoring Grid
├── Ground
├── Paths
├── Soil
└── Decoration

Farm Plot Field
└── Crop Entity Visual (SpriteRenderer)
```

El suelo permanece en Tilemap y los cultivos son entidades runtime. Ya no existe una paleta ni un Tilemap `Crops`.

### Validación pendiente

Comprobar el ciclo agrícola completo y verificar que sembrar nunca modifica el Tilemap de suelo.

---

## BUG-0008 — Las pruebas PlayMode del hotbar dependen del orden de ejecución

- **Estado:** VERIFICADO
- **Severidad:** S3 — Media
- **Detectado:** 2026-08-06
- **Corregido/verificado:** 2026-08-06
- **Sistema:** inventario persistente / pruebas PlayMode
- **Rama afectada:** `agent/cozy-art-pipeline`
- **Corrección funcional:** `d6e7e1c3ef898cbae62120ad98680a7e9c3c80b0`
- **Reportado por:** Arturo

Una prueba dejaba seleccionada la regadera y otra esperaba el slot cero. Al persistir el inventario entre escenas, el resultado dependía del orden. Ambas pruebas normalizan ahora explícitamente la selección inicial. Arturo confirmó **10/10 PlayMode**.

---

## BUG-0007 — Refactor de paletas rompe la compilación de pruebas EditMode

- **Estado:** VERIFICADO
- **Severidad:** S0 — Bloqueante
- **Detectado:** 2026-08-06
- **Corregido/verificado:** 2026-08-06
- **Sistema:** catálogo de Tile Palettes / pruebas EditMode
- **Rama afectada:** `agent/cozy-art-pipeline`
- **Correcciones:** `40a5fc823b912e3261e77e307dfe9c56a2aa43a5`, `8b7bc2241469cc227677b2885d76d901884e25d3`
- **Reportado por:** Arturo

Las pruebas conservaban referencias a `PalettePrefabPath` y después a constantes de categoría inexistentes. Se migraron a `GetPalettePath(CozyPaletteCategory)`. Arturo confirmó que todas las pruebas EditMode volvieron a aparecer y pasaron.

---

## Bugs corregidos y verificados

## BUG-0006 — El `sortingOrder` cambia, pero ningún objeto del laboratorio puede ocultar al jugador

- **Estado:** VERIFICADO
- **Severidad:** S3 — Media
- **Detectado:** 2026-08-05
- **Corregido/verificado:** 2026-08-05
- **Versión de Unity:** `6000.3.21f1`
- **Sistema:** laboratorio de calibración / profundidad visual cenital
- **Rama afectada:** `feature/player-prefab-depth-sorting`
- **Commit afectado:** `080b487`
- **Corrección funcional:** `19147bd`, `b2e746a`
- **Prueba de regresión:** `e93efe9`
- **Reportado por:** Arturo

Durante Play Mode, `Playable Player Sprite` cambiaba correctamente su `Order in Layer`, pero no existía una referencia visual 2D compatible. Se añadió una referencia con `SpriteRenderer`, la misma sorting layer y cálculo por pies. Arturo verificó la oclusión delante/detrás y ambas suites completas.

---

## BUG-0005 — La Fase 5 abre Unity con errores de compilación y solicita Safe Mode

- **Estado:** VERIFICADO
- **Severidad:** S0 — Bloqueante
- **Detectado:** 2026-08-05
- **Corregido/verificado:** 2026-08-05
- **Sistema:** prefab reutilizable y orden visual por profundidad
- **Corrección funcional:** `a9a3fe2`

`Application.isPlaying` se resolvía como el namespace `FarmSimulator.Application`. Se cambió a `global::UnityEngine.Application.isPlaying`.

---

## BUG-0004 — El pipeline descarga un spritesheet ilegible y entra en un ciclo de reimportación

- **Estado:** VERIFICADO
- **Severidad:** S2 — Alta
- **Detectado/verificado:** 2026-08-05
- **Sistema:** importación y animación del personaje
- **Corrección funcional:** `ed840bc`
- **Prueba de regresión:** `70ec28c`

Se cambió a la fuente correcta `src/assets/hero_hd.png`, se validaron firma PNG y dimensiones 192 × 288 px y se impidió escribir descargas inválidas.

---

## BUG-0003 — El límite físico permite que el personaje se recorte en los bordes

- **Estado:** VERIFICADO
- **Severidad:** S3 — Media
- **Detectado/cerrado:** 2026-08-05
- **Corrección:** `9b55ade`
- **Prueba:** `6c96d10`

Se centralizaron dimensiones, marcador, collider y offsets, con límites verticales asimétricos por el pivote.

---

## BUG-0002 — Marcador de orientación descentrado

- **Estado:** VERIFICADO
- **Severidad:** S4 — Baja
- **Detectado/cerrado:** 2026-08-05
- **Corrección:** `dd7357c`

Se reposicionó el marcador desde las dimensiones visuales.

---

## BUG-0001 — Conflicto entre BoxCollider 3D y BoxCollider2D

- **Estado:** VERIFICADO
- **Severidad:** S3 — Media
- **Detectado/cerrado:** 2026-08-05
- **Corrección:** `cdb6cdd`
- **Prueba:** `c8342a2`

Se separaron el suelo lógico 2D y su representación visual.

---

## Reglas de mantenimiento

1. Usar identificadores incrementales `BUG-0001`, `BUG-0002`, etc.
2. No borrar registros corregidos.
3. Documentar causa raíz, solución y prueba.
4. Un bug solo pasa a **VERIFICADO** después de validación satisfactoria.
5. Si reaparece, conservar el registro y marcarlo **REABIERTO**.

## Historial

### 2026-08-06

- Se registró `BUG-0014` por la discrepancia entre el panel, la previsualización y la máscara runtime en lienzos de ancho par; quedó corregido pendiente de validación local.
- Se registró `BUG-0011` por la regresión de compilación en `FarmPlotBehaviour` y quedó corregido pendiente de validación local.
- Se documentó la decisión de representar cultivos como entidades runtime con `SpriteRenderer` y reservar Tilemaps para el mundo.
- Se verificaron `BUG-0007` y `BUG-0008`.
- Se registraron `BUG-0009` y `BUG-0010` como corregidos pendientes de validación local.
- Se separaron las capas agrícolas y se añadió generación de sprites transparentes.

### 2026-08-05

- Se verificaron `BUG-0001` a `BUG-0006`.
