# BUGS — Unity Farm Simulator

Registro histórico de errores detectados, investigados y corregidos durante el desarrollo.

Este archivo complementa `BITÁCORA_GPT.MD`: la bitácora conserva el contexto general y las decisiones; este documento conserva la trazabilidad específica de fallos.

---

## 1. Estados

- **ABIERTO:** confirmado y pendiente de corrección.
- **INVESTIGANDO:** se está buscando la causa raíz.
- **BLOQUEADO:** no puede continuar hasta disponer de información, recurso o dependencia.
- **CORREGIDO:** existe una solución implementada.
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

No hay bugs registrados todavía. El proyecto Unity aún no ha sido creado o sincronizado con este repositorio.

---

## 5. Bugs corregidos y verificados

No hay entradas todavía.

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

- Se creó el registro de bugs.
- Se definieron estados, niveles de severidad y plantilla estándar.
- Estado inicial: sin bugs, porque todavía no existe implementación Unity dentro del repositorio.
