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
## BUG-0000 — Título breve

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

No hay bugs activos.

---

## 5. Bugs corregidos y verificados

## BUG-0003 — El límite físico permite que el personaje se recorte en los bordes

- **Estado:** VERIFICADO
- **Severidad:** S3 — Media
- **Detectado:** 2026-08-05
- **Verificado/cerrado:** 2026-08-05
- **Versión de Unity:** `6000.3.21f1`
- **Escena o sistema:** `Lab` / límites de movimiento del jugador
- **Rama/commit afectado:** `feature/player-movement-foundation` en `e3f5430`
- **Corrección:** `9b55ade`
- **Prueba de regresión:** `6c96d10`
- **Reportado por:** Arturo

### Descripción

El jugador se detenía cuando el collider pequeño de los pies tocaba el `EdgeCollider2D`, pero el rectángulo rojo y el marcador amarillo podían quedar parcialmente fuera del terreno visible. El recorte era evidente en los límites izquierdo, derecho y superior.

### Pasos para reproducir

1. Ejecutar `Bootstrap → Lab` en el estado `e3f5430`.
2. Mover al jugador hasta cada uno de los cuatro extremos.
3. Observar que el cuerpo se corta lateralmente y que en el límite superior gran parte del cuerpo queda fuera de pantalla.

### Resultado esperado

El límite debe detener el pivote de los pies con suficiente anticipación para que el cuerpo y el marcador de orientación permanezcan completamente dentro del terreno de calibración.

### Evidencia

Cuatro capturas iniciales enviadas por Arturo mostraron el personaje recortado. Después de la corrección, cuatro capturas nuevas mostraron el cuerpo rojo y el marcador amarillo completamente contenidos en arriba, abajo, izquierda y derecha.

### Causa raíz

El perímetro se colocaba casi exactamente sobre los bordes del mapa. Physics 2D limitaba el `CapsuleCollider2D` de los pies, cuyo tamaño es mucho menor que el sprite de referencia y cuyo pivote está en la base del personaje. El cálculo ignoraba:

- la mitad del ancho del cuerpo;
- la altura completa sobre el pivote de los pies;
- el espacio adicional ocupado por el marcador amarillo;
- los tamaños y offsets del collider físico.

### Solución aplicada

- `PlayerProxyFacingView` expone las extensiones visuales máximas horizontales, superior e inferior.
- `LabSpatialCalibration` centraliza tamaño y offset del `CapsuleCollider2D`.
- El `EdgeCollider2D` se calcula de manera asimétrica desde el volumen visual completo y el collider físico.
- Los límites superior e inferior respetan el pivote situado en los pies.
- La prueba PlayMode deriva las posiciones extremas posibles del pivote y comprueba que cuerpo y marcador permanezcan dentro del mapa.

### Verificación

Validación local de Arturo el 2026-08-05:

- EditMode: **11/11 aprobadas**.
- PlayMode: **4/4 aprobadas**.
- Consola: sin errores.
- Cuerpo y marcador permanecen completamente visibles en los cuatro extremos.
- La separación vertical asimétrica se comporta correctamente debido al pivote situado en los pies.

### Notas de regresión

No usar únicamente el collider de los pies para decidir límites visuales. Al sustituir el rectángulo por el sprite definitivo, conservar el pivote en los pies y recalcular el volumen visual desde las dimensiones reales del sprite o su configuración de presentación.

---

## BUG-0002 — Marcador de orientación descentrado respecto al personaje provisional

- **Estado:** VERIFICADO
- **Severidad:** S4 — Baja
- **Detectado:** 2026-08-05
- **Verificado/cerrado:** 2026-08-05
- **Versión de Unity:** `6000.3.21f1`
- **Escena o sistema:** `Lab` / referencia visual del jugador
- **Rama/commit afectado:** `feature/player-movement-foundation` en `724152e`
- **Corrección:** `dd7357c`
- **Reportado por:** Arturo

### Descripción

El cuadrado amarillo que indica la orientación del jugador cambiaba correctamente de lado, pero en izquierda y derecha quedaba alineado con el pivote de los pies en lugar de quedar centrado verticalmente respecto al rectángulo rojo.

### Pasos para reproducir

1. Ejecutar `Bootstrap → Lab` en el estado `724152e`.
2. Mover el personaje hacia la izquierda o derecha.
3. Observar que el marcador lateral aparece demasiado abajo respecto al cuerpo rojo.

### Resultado esperado

Los marcadores laterales deben estar centrados verticalmente respecto al cuerpo; el marcador superior debe quedar sobre la cabeza y el inferior bajo los pies.

### Evidencia

Observación visual reportada por Arturo después de validar teclado y DualSense.

### Causa raíz

El jugador usa los pies como pivote lógico. `PlayerProxyFacingView` calculaba todas las posiciones del marcador alrededor de ese pivote, sin considerar que el centro visual del cuerpo se encuentra a la mitad de su altura sobre los pies.

### Solución aplicada

- `PlayerProxyFacingView` calcula las posiciones desde `SpatialModel.ReferenceCharacterWidth` y `ReferenceCharacterHeight`.
- Izquierda y derecha usan el centro vertical real del cuerpo.
- Arriba se coloca después del borde superior y abajo después del borde inferior.
- La prueba PlayMode comprueba que ambos marcadores laterales coincidan con el centro vertical del objeto visual.

### Verificación

Validación local de Arturo el 2026-08-05:

- EditMode: **11/11 aprobadas**.
- PlayMode: **4/4 aprobadas**.
- Consola limpia.
- Capturas del personaje en las cuatro orientaciones y extremos confirmaron el nuevo posicionamiento; el problema posterior correspondía al perímetro, no al anclaje del marcador.

### Notas de regresión

Mantener el pivote lógico del jugador en los pies. Cualquier indicador o sprite hijo que dependa del centro del cuerpo debe derivar su posición de las dimensiones visuales, no asumir que el pivote coincide con el centro.

---

## BUG-0001 — Conflicto entre BoxCollider 3D y BoxCollider2D en el suelo de Lab

- **Estado:** VERIFICADO
- **Severidad:** S3 — Media
- **Detectado:** 2026-08-05
- **Verificado/cerrado:** 2026-08-05
- **Versión de Unity:** `6000.3.21f1`
- **Escena o sistema:** `Lab` / calibración espacial cenital
- **Rama/commit afectado:** `feature/lab-spatial-calibration` en `5b3dbda`
- **Corrección:** `cdb6cdd`
- **Prueba de regresión:** `c8342a2`
- **Reportado por:** Arturo y prueba PlayMode

### Descripción

La prueba `LabBuildsFrontFacingOrthographicXYCalibration` fallaba porque el objeto `Calibration Ground` no lograba recibir su `BoxCollider2D`. La consola registraba un conflicto entre componentes de colisión 3D y 2D.

### Pasos para reproducir

1. Sincronizar la rama en el estado `5b3dbda`.
2. Abrir Unity y ejecutar `Test Runner → PlayMode → Run All`.
3. Observar resultado **1/2** y el mensaje de conflicto en la consola.

### Resultado esperado

El suelo lógico debe contener un `BoxCollider2D`, no debe conservar un `Collider` 3D y las dos pruebas PlayMode deben aprobarse.

### Evidencia

```text
Can't add component 'BoxCollider2D' to Calibration Ground because it conflicts with the existing 'BoxCollider'-derived component!
```

En el estado afectado, EditMode terminó en **3/3** y PlayMode en **1/2**.

### Causa raíz

`GameObject.CreatePrimitive(PrimitiveType.Cube)` agrega automáticamente un `BoxCollider` 3D. El código llamaba `Destroy` sobre ese collider, pero `Destroy` se procesa al final del frame. Antes de que desapareciera, se intentaba añadir `BoxCollider2D` al mismo objeto y Unity rechazaba la combinación.

### Solución aplicada

- `Calibration Ground` se crea como un `GameObject` lógico limpio.
- El objeto lógico recibe directamente `BoxCollider2D`.
- La representación verde vive por separado como `Calibration Ground Visual`.
- Las primitivas visuales eliminan sus colliders 3D automáticos.
- La prueba PlayMode comprueba que las referencias visuales no conserven colliders 3D.

### Verificación

Validación local de Arturo el 2026-08-05:

- PlayMode: **2/2 aprobadas**.
- Consola: sin errores y sin el mensaje de conflicto.
- La escena cenital `Lab` se ejecutó correctamente desde `Bootstrap`.

La prueba EditMode ya se había mantenido verde en **3/3** durante la detección y no dependía de la corrección de collider.

### Notas de regresión

No añadir colliders 2D a un objeto creado mediante `GameObject.CreatePrimitive` durante el mismo frame. Separar la representación visual 3D del objeto lógico que contiene Physics 2D, o usar una representación 2D nativa.

---

## 6. Reglas de mantenimiento

1. Asignar un identificador incremental: `BUG-0001`, `BUG-0002`, etc.
2. No borrar registros corregidos.
3. Documentar la causa raíz cuando sea posible, no solo el síntoma.
4. Vincular el commit o pull request que contiene la corrección.
5. Un bug solo pasa de **CORREGIDO** a **VERIFICADO** después de una prueba satisfactoria.
6. Si reaparece, conservar el registro original y marcarlo como **REABIERTO**.
7. Evitar registrar solicitudes de funciones como bugs.

---

## 7. Historial del documento

### 2026-08-05

- Se creó el registro de bugs y se definieron estados y severidades.
- Se añadió y verificó `BUG-0001` por el conflicto de colliders.
- Se añadió y verificó `BUG-0002` por el marcador de orientación descentrado.
- Se añadió y verificó `BUG-0003` por el recorte visual del jugador en los límites del laboratorio.
