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

- El catálogo genera 18 PNG derivados en `Assets/_Project/Tiles/Generated/Crops`.
- El color de la esquina inferior izquierda se utiliza como máscara cromática cuando es opaco.
- Los PNG derivados se importan como Sprite Single, 16 PPU, Point, sin mipmaps, sin compresión y con transparencia.
- Todos usan pivote inferior centrado `(0.5, 0.0)`.
- Los `Tile` agrícolas referencian estos sprites generados, no los recortes opacos originales.

### Validación pendiente

Regenerar el catálogo, comprobar visualmente semillas y etapas sobre tierra arada y ejecutar EditMode/PlayMode completos.

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

La jerarquía de autoría pasa a ser:

```text
Farm Authoring Grid
├── Ground
├── Paths
├── Soil
├── Crops
└── Decoration
```

`Soil` y `Crops` tienen órdenes de render independientes; `Crops` se dibuja sobre `Soil`. El Tile Manager expone botones y paletas separadas para ambas capas.

### Validación pendiente

Pintar tierra y cultivo en la misma coordenada, guardar, reabrir la escena y confirmar que ambos persisten.

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

- Se verificaron `BUG-0007` y `BUG-0008`.
- Se registraron `BUG-0009` y `BUG-0010` como corregidos pendientes de validación local.
- Se separaron las capas agrícolas y se añadió generación de sprites transparentes.

### 2026-08-05

- Se verificaron `BUG-0001` a `BUG-0006`.
