# BUGS — Unity Farm Simulator

Registro histórico de errores detectados, investigados y corregidos durante el desarrollo.

Este archivo complementa `BITÁCORA_GPT.MD`: la bitácora conserva el contexto general y las decisiones; este documento conserva la trazabilidad específica de fallos.

---

## 1. Estados

- **ABIERTO:** confirmado y pendiente de corrección.
- **INVESTIGANDO:** se está buscando la causa raíz.
- **BLOQUEADO:** no puede continuar hasta disponer de información, recurso o dependencia.
- **CORREGIDO:** existe una solución implementada, pendiente de validación local.
- **VERIFICADO:** la solución fue probada satisfactoriamente.
- **NO REPRODUCIBLE:** no pudo reproducirse con la información disponible.
- **DESCARTADO:** no es un bug, es comportamiento esperado o no se corregirá por una razón documentada.
- **REABIERTO:** reapareció después de haberse considerado corregido.

---

## 2. Severidad

- **S0 — Bloqueante:** impide abrir, compilar, ejecutar o continuar el desarrollo.
- **S1 — Crítica:** pérdida de datos, corrupción, bloqueo frecuente o función principal inutilizable.
- **S2 — Alta:** comportamiento importante incorrecto, pero existe una alternativa temporal.
- **S3 — Media:** fallo visible o funcional de alcance limitado.
- **S4 — Baja:** problema menor, visual, de pulido o documentación.

---

## 3. Formato de cada registro

```markdown
## BUG-0001 — Título breve

- **Estado:** ABIERTO
- **Severidad:** S2 — Alta
- **Detectado:** AAAA-MM-DD
- **Verificado/cerrado:** —
- **Versión de Unity:** 6000.x.xf1
- **Escena o sistema:** Nombre
- **Rama/commit:** rama o SHA
- **Reportado por:** Arturo / GPT / prueba automática

### Descripción
Qué ocurre y por qué constituye un problema.

### Pasos para reproducir
1. Paso uno.
2. Paso dos.
3. Resultado observado.

### Resultado esperado
Qué debería ocurrir.

### Evidencia
Mensaje de consola, captura, video, log o archivo relacionado.

### Causa raíz
Completar cuando se determine.

### Solución aplicada
Archivos modificados, enfoque elegido y cualquier compromiso técnico.

### Verificación
Pruebas realizadas y resultado.

### Notas de regresión
Qué debe comprobarse en futuras versiones para evitar que reaparezca.
```

---

## 4. Bugs activos

## BUG-0001 — Conflicto entre BoxCollider 3D y BoxCollider2D en el suelo de Lab

- **Estado:** CORREGIDO
- **Severidad:** S3 — Media
- **Detectado:** 2026-08-05
- **Verificado/cerrado:** —
- **Versión de Unity:** `6000.3.21f1`
- **Escena o sistema:** `Lab` / calibración espacial cenital
- **Rama/commit afectado:** `feature/lab-spatial-calibration` en `5b3dbda`
- **Corrección:** `cdb6cdd`
- **Prueba de regresión:** `c8342a2`
- **Reportado por:** Arturo y prueba PlayMode

### Descripción

La prueba `LabBuildsFrontFacingOrthographicXYCalibration` fallaba porque el objeto `Calibration Ground` no lograba recibir su `BoxCollider2D`. La consola registraba mensajes de conflicto entre componentes de colisión 3D y 2D.

### Pasos para reproducir

1. Sincronizar la rama en el estado `5b3dbda`.
2. Abrir Unity y ejecutar `Test Runner → PlayMode → Run All`.
3. Observar resultado **1/2** y el mensaje de conflicto en la consola.

### Resultado esperado

El suelo lógico debe contener un `BoxCollider2D`, no debe conservar un `Collider` 3D y las dos pruebas PlayMode deben aprobarse.

### Evidencia

Mensaje observado:

```text
Can't add component 'BoxCollider2D' to Calibration Ground because it conflicts with the existing 'BoxCollider'-derived component!
```

EditMode terminó correctamente en **3/3**; PlayMode terminó en **1/2**.

### Causa raíz

`GameObject.CreatePrimitive(PrimitiveType.Cube)` agrega automáticamente un `BoxCollider` 3D. El código llamaba `Destroy` sobre ese collider, pero `Destroy` se procesa al final del frame. Antes de que desapareciera, `BuildGround` intentaba añadir un `BoxCollider2D` al mismo objeto, y Unity rechazaba esa combinación.

### Solución aplicada

- `Calibration Ground` ahora es un `GameObject` lógico sin primitiva ni collider 3D.
- Ese objeto recibe directamente el `BoxCollider2D` y conserva las dimensiones del mapa.
- La representación verde se crea como un objeto visual separado llamado `Calibration Ground Visual`.
- Los colliders 3D automáticos de las primitivas visuales continúan eliminándose.
- La prueba PlayMode verifica además que las referencias visuales generadas no conserven ningún `Collider` 3D.

### Verificación

Pendiente de validación local. Resultado esperado después de sincronizar:

- EditMode: **3/3**.
- PlayMode: **2/2**.
- Consola: sin el mensaje de conflicto.

### Notas de regresión

No añadir colliders 2D a un objeto creado mediante `GameObject.CreatePrimitive` durante el mismo frame. Separar siempre la representación visual 3D del objeto lógico que contiene componentes Physics 2D, o usar una representación 2D nativa.

---

## 5. Bugs corregidos y verificados

No hay bugs verificados todavía.

---

## 6. Reglas de mantenimiento

1. Asignar un identificador incremental: `BUG-0001`, `BUG-0002`, etc.
2. No borrar registros corregidos; moverlos a la sección histórica correspondiente.
3. Documentar la causa raíz cuando sea posible, no solo el síntoma.
4. Vincular el commit o pull request que contiene la corrección.
5. Un bug solo pasa de **CORREGIDO** a **VERIFICADO** después de una prueba satisfactoria.
6. Si reaparece, conservar el registro original y marcarlo como **REABIERTO**.
7. Evitar registrar solicitudes de funciones como bugs; esas deben quedar en la planificación o en issues separados.

---

## 7. Historial del documento

### 2026-08-05

- Se creó el registro de bugs y se definieron estados y severidades.
- Se añadió `BUG-0001` por el conflicto de colliders detectado en la calibración cenital.
