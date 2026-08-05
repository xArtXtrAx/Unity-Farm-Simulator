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

## BUG-0004 — El pipeline descarga un spritesheet ilegible y entra en un ciclo de reimportación

- **Estado:** CORREGIDO
- **Severidad:** S2 — Alta
- **Detectado:** 2026-08-05
- **Verificado/cerrado:** pendiente
- **Versión de Unity:** `6000.3.21f1`
- **Sistema:** importación y animación del personaje
- **Rama afectada:** `feature/player-sprite-animation`
- **Commit afectado:** `2099118`
- **Corrección funcional:** `ed840bc`
- **Prueba de regresión:** `70ec28c`
- **Reportado por:** Arturo

### Descripción

Al abrir Unity, el pipeline descargaba un archivo llamado `farmer-spritesheet.png` que Unity no podía leer. Después intentaba ejecutar `SaveAndReimport` repetidamente sobre el archivo inválido, llenando la consola con cientos de errores e impidiendo generar los sprites, clips y Animator.

### Evidencia

La consola mostró aproximadamente 273 errores repetidos:

```text
Could not create asset from Assets/_Project/Resources/Characters/Farmer/farmer-spritesheet.png: File could not be read
UnityEditor.AssetImporter:SaveAndReimport()
```

Arturo compartió la versión correcta: PNG RGBA de **192 × 288 px** y **84,514 bytes**.

### Causa raíz

El pipeline apuntaba a:

```text
public/assets/farmer/farmer-spritesheet.png
```

Ese archivo del repositorio congelado no era el recurso funcional utilizado por el prototipo. La imagen correcta estaba en:

```text
src/assets/hero_hd.png
```

Además, el pipeline solo comprobaba que el archivo existiera. No validaba la firma PNG ni sus dimensiones antes de solicitar a Unity la reimportación, por lo que un archivo ilegible permanecía atrapado en un ciclo de `SaveAndReimport`.

### Solución aplicada

- La URL congelada ahora apunta a `src/assets/hero_hd.png` en el commit fuente exacto.
- La firma de importación subió a `farm-simulator-farmer-64x72-v2`.
- Antes de importar se valida:
  - firma PNG;
  - ancho de 192 px;
  - alto de 288 px.
- Un archivo local inválido y su `.meta` se eliminan automáticamente.
- Solo se escribe la descarga después de validar sus bytes.
- Una descarga inválida no se escribe ni se reintenta en bucle.
- Se añadieron pruebas para proteger la ruta fuente y la validación de cabecera/dimensiones.

### Verificación pendiente

Después de sincronizar:

1. El pipeline debe borrar el PNG dañado local.
2. Debe descargar `src/assets/hero_hd.png`.
3. Debe generar 12 sprites, ocho clips y el Animator.
4. EditMode debe terminar en **27/27**.
5. PlayMode debe terminar en **5/5**.
6. La consola debe quedar sin errores ni advertencias después de limpiarla.
7. El personaje debe mostrarse y animarse en las cuatro direcciones.

### Notas de regresión

Nunca confiar únicamente en la extensión o en `File.Exists` para recursos descargados. Validar formato y dimensiones antes de escribirlos dentro de `Assets` o llamar a `SaveAndReimport`.

---

## Bugs corregidos y verificados

## BUG-0003 — El límite físico permite que el personaje se recorte en los bordes

- **Estado:** VERIFICADO
- **Severidad:** S3 — Media
- **Detectado/cerrado:** 2026-08-05
- **Sistema:** `Lab` / límites de movimiento
- **Rama afectada:** `feature/player-movement-foundation`
- **Corrección:** `9b55ade`
- **Prueba:** `6c96d10`

El perímetro se calculaba desde el collider pequeño de los pies e ignoraba el volumen visual. Se centralizaron ancho, altura, marcador, collider y offsets; el límite pasó a ser asimétrico verticalmente por el pivote en los pies. Arturo verificó **11/11 EditMode**, **4/4 PlayMode**, consola limpia y visibilidad completa en los cuatro extremos.

---

## BUG-0002 — Marcador de orientación descentrado

- **Estado:** VERIFICADO
- **Severidad:** S4 — Baja
- **Detectado/cerrado:** 2026-08-05
- **Sistema:** `Lab` / referencia visual del jugador
- **Corrección:** `dd7357c`

El marcador lateral se calculaba alrededor del pivote de los pies. Se reposicionó desde las dimensiones visuales: laterales centrados, superior sobre la cabeza e inferior bajo los pies. Arturo confirmó la corrección con pruebas y capturas.

---

## BUG-0001 — Conflicto entre BoxCollider 3D y BoxCollider2D

- **Estado:** VERIFICADO
- **Severidad:** S3 — Media
- **Detectado/cerrado:** 2026-08-05
- **Sistema:** `Lab` / suelo de calibración
- **Corrección:** `cdb6cdd`
- **Prueba:** `c8342a2`

`GameObject.CreatePrimitive` añadía un `BoxCollider` 3D y Unity rechazaba agregar `BoxCollider2D` durante el mismo frame. Se separaron el suelo lógico 2D y su representación visual. Arturo verificó PlayMode **2/2** y consola limpia.

---

## Reglas de mantenimiento

1. Usar identificadores incrementales `BUG-0001`, `BUG-0002`, etc.
2. No borrar registros corregidos.
3. Documentar causa raíz, solución y prueba.
4. Un bug solo pasa a **VERIFICADO** después de validación satisfactoria.
5. Si reaparece, conservar el registro y marcarlo **REABIERTO**.

## Historial

### 2026-08-05

- Se creó el registro.
- Se verificaron `BUG-0001`, `BUG-0002` y `BUG-0003`.
- Se añadió `BUG-0004` por la fuente inválida del spritesheet; está corregido y pendiente de validación local.
