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

## BUG-0006 — El `sortingOrder` cambia, pero ningún objeto del laboratorio puede ocultar al jugador

- **Estado:** CORREGIDO
- **Severidad:** S3 — Media
- **Detectado:** 2026-08-05
- **Corrección implementada:** 2026-08-05
- **Versión de Unity:** `6000.3.21f1`
- **Sistema:** laboratorio de calibración / profundidad visual cenital
- **Rama afectada:** `feature/player-prefab-depth-sorting`
- **Commit afectado:** `080b487`
- **Corrección funcional:** `19147bd`, `b2e746a`
- **Prueba de regresión:** `e93efe9`
- **Reportado por:** Arturo

### Comportamiento observado

Durante Play Mode, `Playable Player Sprite` cambiaba correctamente su `Order in Layer` aproximadamente entre **9600 y 10400**, pero el personaje se dibujaba siempre por encima de todos los objetos visibles del laboratorio. No existía una referencia con la que pudiera comprobarse visualmente el cambio delante/detrás.

### Comportamiento esperado

El laboratorio debe contener al menos un objeto 2D solapable que participe en el mismo sistema de sorting por Y. Al mover los pies del jugador por encima de la referencia, el jugador debe verse detrás; al moverlos por debajo, debe verse delante.

### Causa raíz

Las referencias existentes se construyen con `GameObject.CreatePrimitive`, `MeshRenderer` y materiales URP opacos. El personaje usa `SpriteRenderer`; su cola transparente y `sortingOrder` no se intercalan con esas primitivas opacas. El valor numérico cambiaba correctamente, pero la visibilidad no podía cambiar frente a esos objetos.

### Solución aplicada

- Se añadió `LabDepthSortingReference`, una referencia morada 2D de **1.5 × 2.25 unidades**.
- Su pivote está en los pies y su posición de pies es `Y = 0.65`.
- Usa `SpriteRenderer`, sorting layer `Actors` y `TopDownSpriteSorting`, igual que el jugador.
- `LabSpatialCalibrationInstaller` la instala tanto si crea la calibración como si esta ya existe.
- No tiene collider, para permitir atravesarla y observar el cambio visual.
- La prueba PlayMode ahora exige:
  - misma sorting layer;
  - orden correcto de la referencia;
  - jugador detrás con pies más arriba;
  - jugador delante con pies más abajo.

### Pendiente para pasar a VERIFICADO

- Hacer pull y confirmar compilación limpia.
- Comprobar que aparece `Depth Sorting Reference` dentro de `Generated Calibration`.
- Caminar desde abajo hacia arriba atravesando el rectángulo morado.
- Confirmar el cambio visual delante/detrás.
- Ejecutar EditMode **36/36** y PlayMode **6/6**.

---

## Bugs corregidos y verificados

## BUG-0005 — La Fase 5 abre Unity con errores de compilación y solicita Safe Mode

- **Estado:** VERIFICADO
- **Severidad:** S0 — Bloqueante
- **Detectado:** 2026-08-05
- **Corregido/verificado:** 2026-08-05
- **Versión de Unity:** `6000.3.21f1`
- **Sistema:** prefab reutilizable y orden visual por profundidad
- **Rama afectada:** `feature/player-prefab-depth-sorting`
- **Commit afectado:** `489851d`
- **Corrección funcional:** `a9a3fe2`
- **Reportado por:** Arturo

Unity entró en Safe Mode porque `Application.isPlaying` se resolvía como el namespace `FarmSimulator.Application`. Se cambió a `global::UnityEngine.Application.isPlaying`. Arturo verificó apertura normal, consola limpia y generación del prefab y sorting layers.

---

## BUG-0004 — El pipeline descarga un spritesheet ilegible y entra en un ciclo de reimportación

- **Estado:** VERIFICADO
- **Severidad:** S2 — Alta
- **Detectado/verificado:** 2026-08-05
- **Sistema:** importación y animación del personaje
- **Rama afectada:** `feature/player-sprite-animation`
- **Commit afectado:** `2099118`
- **Corrección funcional:** `ed840bc`
- **Prueba de regresión:** `70ec28c`

La fuente inicial era incorrecta y no se validaba antes de importar. Se cambió a `src/assets/hero_hd.png`, se validaron firma PNG y dimensiones **192 × 288 px**, y se impidió escribir descargas inválidas. Arturo verificó importación, animación, pruebas y límites visuales.

---

## BUG-0003 — El límite físico permite que el personaje se recorte en los bordes

- **Estado:** VERIFICADO
- **Severidad:** S3 — Media
- **Detectado/cerrado:** 2026-08-05
- **Sistema:** `Lab` / límites de movimiento
- **Corrección:** `9b55ade`
- **Prueba:** `6c96d10`

El perímetro usaba el collider pequeño de los pies e ignoraba el volumen visual. Se centralizaron dimensiones, marcador, collider y offsets, con límites verticales asimétricos por el pivote. Arturo verificó los cuatro extremos.

---

## BUG-0002 — Marcador de orientación descentrado

- **Estado:** VERIFICADO
- **Severidad:** S4 — Baja
- **Detectado/cerrado:** 2026-08-05
- **Sistema:** `Lab` / referencia visual del jugador
- **Corrección:** `dd7357c`

Se reposicionó el marcador desde las dimensiones visuales: laterales centrados, superior sobre la cabeza e inferior bajo los pies.

---

## BUG-0001 — Conflicto entre BoxCollider 3D y BoxCollider2D

- **Estado:** VERIFICADO
- **Severidad:** S3 — Media
- **Detectado/cerrado:** 2026-08-05
- **Sistema:** `Lab` / suelo de calibración
- **Corrección:** `cdb6cdd`
- **Prueba:** `c8342a2`

`GameObject.CreatePrimitive` añadía un `BoxCollider` 3D y Unity rechazaba agregar `BoxCollider2D` durante el mismo frame. Se separaron el suelo lógico 2D y su representación visual.

---

## Reglas de mantenimiento

1. Usar identificadores incrementales `BUG-0001`, `BUG-0002`, etc.
2. No borrar registros corregidos.
3. Documentar causa raíz, solución y prueba.
4. Un bug solo pasa a **VERIFICADO** después de validación satisfactoria.
5. Si reaparece, conservar el registro y marcarlo **REABIERTO**.

## Historial

### 2026-08-05

- Se verificaron `BUG-0001` a `BUG-0005`.
- Se añadió `BUG-0006` por falta de una referencia visual compatible con el sorting del jugador.
- Se implementó una referencia 2D con la misma sorting layer y cálculo por pies; queda pendiente de validación local.
