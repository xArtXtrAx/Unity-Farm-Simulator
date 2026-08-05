# CONTINUIDAD GPT — Unity Farm Simulator

Este archivo es el **punto de entrada permanente** para retomar el desarrollo sin depender de la memoria de un chat.

> Un chat nuevo debe leer este archivo desde `main` antes de buscar la bitácora técnica.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `feature/domain-item-inventory-foundation`
- **Fase actual:** Fase 6 — Catálogo de objetos e inventario de dominio
- **Estado:** implementación y validación local completas; integración mediante PR pendiente
- **Cabeza remota registrada de la rama activa:** `c91b3063f9c06fb949842d64d730694a84d4f416`
- **Última integración:** PR #5, merge `341051f097d9d70796e69b9cddc9277ba3902ed0`
- **Registro de integración en main:** `8a85534526ad1e491cb0687d817f2924acee2d4d`
- **Bugs activos:** ninguno
- **Bugs verificados:** `BUG-0001` a `BUG-0006`
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`

### Línea base integrada

- Fase 5 integrada en `main`.
- EditMode previo: **36/36**.
- PlayMode previo: **6/6**.
- Consola de la Fase 5: **0 errores y 0 advertencias**.
- Player prefab, sorting layers, profundidad y oclusión aprobados.

### Fase 6 implementada

En `feature/domain-item-inventory-foundation` existen:

- catálogo puro con los ocho IDs, categorías, stacks y precios aprobados;
- `ItemId`, `ItemCategory`, `ItemDefinition` e `ItemCatalog`;
- inventario puro de ocho slots;
- selección y ciclo con wrap-around;
- capacidad y stacking deterministas;
- adición totalmente atómica;
- consumo y limpieza de slots;
- snapshots defensivos y restauración prevalidada;
- estado inicial con azada, regadera y veinte semillas de nabo;
- **25 casos EditMode de catálogo**;
- **63 casos EditMode de inventario**.

No se añadieron UI, ScriptableObjects, almacenamiento, cambios de escenas ni referencias `UnityEngine` en Domain.

### Validación local — 2026-08-05

Arturo confirmó que todas las pruebas del bloque pasaron sin errores:

- EditMode: **124/124**.
- Fallos: **0**.
- Pruebas nuevas aprobadas: **88/88**.
- Unity ejecutó el conjunto sin errores de compilación reportados.
- PlayMode no fue requisito nuevo; se conserva la regresión previa confirmada de **6/6**.

La puerta de salida de la Fase 6 está cumplida. La implementación todavía no está integrada en `main`.

### Próxima acción

Revisar la diferencia final, crear el PR de la Fase 6 hacia `main` y fusionarlo después de confirmar que solo incluye Domain, pruebas EditMode, metas y documentación. No añadir más funcionalidad a esta rama validada.

---

## Orden obligatorio de lectura

1. Leer este archivo desde `main`.
2. Cambiar conceptualmente a `feature/domain-item-inventory-foundation`.
3. Leer `BITÁCORA_GPT.MD` desde esa rama.
4. Leer `BUGS.MD` desde esa rama.
5. Leer `MIGRACION_DESDE_FARMING_GAME_A.MD`.
6. Comparar `main` con la rama activa y revisar commits recientes.
7. Continuar desde **“Próximo paso exacto”** en la bitácora.

---

## Reglas críticas

- La Fase 6 está implementada y validada localmente, pero aún no integrada en `main`.
- No añadir más funcionalidad a `feature/domain-item-inventory-foundation` antes del merge.
- `FarmSimulator.Domain` no debe depender de `UnityEngine`.
- No traducir TypeScript línea por línea; conservar comportamientos, IDs y atomicidad.
- `main` debe mantenerse estable y todavía no contiene la implementación funcional de la Fase 6.
- Después de cada implementación o corrección realizada por GPT, actualizar `BITÁCORA_GPT.MD` en el mismo bloque.
- Cada vez que cambie la rama activa, la fase, la integración o el estado de validación, actualizar este archivo en `main`.

---

## Prompt mínimo para un chat nuevo

```text
Continúa trabajando en mi repositorio:
https://github.com/xArtXtrAx/Unity-Farm-Simulator

Lee primero CONTINUIDAD_GPT.md desde main y sigue exactamente su orden de lectura y el “Próximo paso exacto” de la bitácora en la rama activa. La rama activa es feature/domain-item-inventory-foundation. La Fase 6 está implementada y validada localmente con 124/124 EditMode y cero fallos, pero todavía no está integrada en main. No añadas más funcionalidad a esta rama; revisa el alcance y prepara su PR. Tras cada implementación o corrección, actualiza BITÁCORA_GPT.MD y mantén CONTINUIDAD_GPT.md sincronizado.
```

---

## Mantenimiento de este archivo

Mantener breve. Actualizar únicamente rama activa, fase, estado, commit remoto, pruebas, cambios locales y cualquier advertencia necesaria para reanudar sin ambigüedad.
