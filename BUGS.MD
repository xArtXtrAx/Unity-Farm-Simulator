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

## BUG-0005 — La Fase 5 abre Unity con errores de compilación y solicita Safe Mode

- **Estado:** CORREGIDO
- **Severidad:** S0 — Bloqueante
- **Detectado:** 2026-08-05
- **Corrección implementada:** 2026-08-05
- **Versión de Unity:** `6000.3.21f1`
- **Sistema:** prefab reutilizable y orden visual por profundidad
- **Rama afectada:** `feature/player-prefab-depth-sorting`
- **Commit afectado:** `489851d`
- **Corrección funcional:** `a9a3fe2`
- **Reportado por:** Arturo

### Comportamiento observado

Al abrir el proyecto después de sincronizar la rama de la Fase 5, Unity entró en Safe Mode y mostró un único error de compilación:

```text
Assets/_Project/Scripts/Presentation/Player/PlayerSpriteAnimator.cs(51,17):
error CS0234: The type or namespace name 'isPlaying' does not exist in the namespace 'FarmSimulator.Application'
```

La validación local no podía comenzar porque el proyecto no compilaba.

### Comportamiento esperado

Unity debe resolver el estado de reproducción desde `UnityEngine.Application`, compilar normalmente, crear las sorting layers y generar el Player prefab antes de ejecutar las pruebas.

### Causa raíz

Dentro del namespace `FarmSimulator.Presentation.Player`, la expresión no cualificada:

```csharp
Application.isPlaying
```

se resolvía contra el namespace hermano `FarmSimulator.Application`, no contra la clase `UnityEngine.Application`. Por eso el compilador intentaba encontrar `isPlaying` dentro de un namespace.

### Solución aplicada

Se sustituyó la referencia ambigua por la forma completamente cualificada:

```csharp
global::UnityEngine.Application.isPlaying
```

La corrección está limitada a `PlayerSpriteAnimator.Initialize` y no cambia el comportamiento de animación, prefab, input ni movimiento.

### Prueba de regresión

La propia compilación de `FarmSimulator.Presentation` cubre esta regresión: cualquier retorno a la referencia ambigua vuelve a producir `CS0234` antes de ejecutar tests. Tras sincronizar, se debe validar además:

- salida correcta de Safe Mode;
- consola sin errores de compilación;
- EditMode **36/36**;
- PlayMode **6/6**;
- generación del prefab y sorting layers;
- movimiento, animación y profundidad sin regresiones.

### Pendiente para pasar a VERIFICADO

Arturo debe hacer pull del commit `a9a3fe2`, permitir que Unity recompile y confirmar que Safe Mode puede cerrarse sin errores. Después debe completar la validación de la Fase 5.

---

## Bugs corregidos y verificados

## BUG-0004 — El pipeline descarga un spritesheet ilegible y entra en un ciclo de reimportación

- **Estado:** VERIFICADO
- **Severidad:** S2 — Alta
- **Detectado:** 2026-08-05
- **Verificado/cerrado:** 2026-08-05
- **Versión de Unity:** `6000.3.21f1`
- **Sistema:** importación y animación del personaje
- **Rama afectada:** `feature/player-sprite-animation`
- **Commit afectado:** `2099118`
- **Corrección funcional:** `ed840bc`
- **Prueba de regresión:** `70ec28c`
- **Reportado por:** Arturo

### Descripción y evidencia

El pipeline descargaba un archivo que Unity no podía leer y ejecutaba `SaveAndReimport` repetidamente, llegando a mostrar cientos de errores:

```text
Could not create asset from Assets/_Project/Resources/Characters/Farmer/farmer-spritesheet.png: File could not be read
UnityEditor.AssetImporter:SaveAndReimport()
```

### Causa raíz

La fuente inicial apuntaba a `public/assets/farmer/farmer-spritesheet.png`, pero el recurso funcional congelado era `src/assets/hero_hd.png`. Además, el pipeline confiaba en la existencia/extensión del archivo sin validar su firma PNG ni las dimensiones antes de importarlo.

### Solución aplicada

- Fuente corregida a `src/assets/hero_hd.png` del commit congelado.
- Validación previa de firma PNG y dimensiones exactas de **192 × 288 px**.
- Import signature actualizada a `farm-simulator-farmer-64x72-v2`.
- No se escriben descargas inválidas.
- Se añadieron pruebas de ruta, cabecera y dimensiones.
- En el estado local ya atrapado por `AssetDatabase`, fue necesaria una limpieza manual única de `Assets/_Project/Resources/Characters/Farmer` y copiar el PNG correcto sin `.meta`.

### Verificación

Arturo confirmó el 2026-08-05:

- el PNG correcto fue importado y cortado;
- el personaje real aparece y se anima en las cuatro direcciones;
- todas las pruebas actuales pasaron;
- los cuatro límites visuales son correctos para el sprite de 64 × 72 px y el marcador permanece visible.

### Notas de regresión

No introducir archivos descargados dentro de `Assets` sin validarlos previamente. Una vez que Unity atrapa un archivo corrupto en su importador, puede requerirse retirarlo con Unity cerrado antes de que el código Editor vuelva a ejecutarse.

---

## BUG-0003 — El límite físico permite que el personaje se recorte en los bordes

- **Estado:** VERIFICADO
- **Severidad:** S3 — Media
- **Detectado/cerrado:** 2026-08-05
- **Sistema:** `Lab` / límites de movimiento
- **Corrección:** `9b55ade`
- **Prueba:** `6c96d10`

El perímetro se calculaba desde el collider pequeño de los pies e ignoraba el volumen visual. Se centralizaron ancho, altura, marcador, collider y offsets; el límite pasó a ser asimétrico verticalmente por el pivote en los pies. Arturo verificó la visibilidad completa del proxy y posteriormente del sprite real en los cuatro extremos.

---

## BUG-0002 — Marcador de orientación descentrado

- **Estado:** VERIFICADO
- **Severidad:** S4 — Baja
- **Detectado/cerrado:** 2026-08-05
- **Sistema:** `Lab` / referencia visual del jugador
- **Corrección:** `dd7357c`

El marcador lateral se calculaba alrededor del pivote de los pies. Se reposicionó desde las dimensiones visuales: laterales centrados, superior sobre la cabeza e inferior bajo los pies.

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

- Se verificaron `BUG-0001`, `BUG-0002`, `BUG-0003` y `BUG-0004`.
- Se añadió `BUG-0005` como bloqueo de compilación de la Fase 5.
- Se confirmó la causa raíz de `BUG-0005`: colisión entre `FarmSimulator.Application` y `UnityEngine.Application`.
- Se implementó la corrección `a9a3fe2`; queda pendiente la validación local para marcarla VERIFICADA.
