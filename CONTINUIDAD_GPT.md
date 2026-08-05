# CONTINUIDAD GPT — Unity Farm Simulator

Este archivo es el **punto de entrada permanente** para retomar el desarrollo sin depender de la memoria de un chat.

> Un chat nuevo debe leer este archivo desde `main` antes de buscar la bitácora técnica.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `feature/domain-item-inventory-foundation`
- **Fase actual:** Fase 6 — Catálogo de objetos e inventario de dominio
- **Estado:** implementación remota completa; compilación y pruebas Unity pendientes de validación local
- **Cabeza remota registrada de la rama activa:** `441ccb742ee670bcd93ab920e2e0f73c55161081`
- **Última integración:** PR #5, merge `341051f097d9d70796e69b9cddc9277ba3902ed0`
- **Registro de integración en main:** `8a85534526ad1e491cb0687d817f2924acee2d4d`
- **Bugs activos:** ninguno
- **Bugs verificados:** `BUG-0001` a `BUG-0006`
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`

### Línea base validada

- Fase 5 integrada en `main`.
- EditMode: **36/36**.
- PlayMode: **6/6**.
- Consola: **0 errores y 0 advertencias**.
- Player prefab y sorting layers versionados.
- Profundidad y oclusión delante/detrás aprobadas.

### Implementación remota de la Fase 6

En `feature/domain-item-inventory-foundation` ya existen:

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

### Validación pendiente

- Implementación remota: **completa**.
- Pruebas nuevas publicadas: **88**.
- Conteo EditMode esperado: **124** contando los 36 existentes.
- PlayMode esperado sin cambios: **6**.
- Compilación local Unity: **pendiente**.
- Validación EditMode local: **pendiente**.
- Consola local: **pendiente**.

No afirmar que Unity compila ni que 124/124 pasan hasta recibir la validación local de Arturo.

### Próxima acción

Arturo debe sincronizar `feature/domain-item-inventory-foundation`, abrir el proyecto con Unity `6000.3.21f1`, revisar la consola y ejecutar EditMode completo. El resultado esperado, todavía no confirmado, es 124/124.

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

- La implementación remota de la Fase 6 existe, pero sigue sin validación Unity local.
- `FarmSimulator.Domain` no debe depender de `UnityEngine`.
- No traducir TypeScript línea por línea; conservar comportamientos, IDs y atomicidad.
- No afirmar que Unity compila o que pruebas pasan hasta validación local de Arturo.
- `main` debe mantenerse estable y no contiene la implementación funcional de la Fase 6.
- Después de cada implementación o corrección realizada por GPT, actualizar `BITÁCORA_GPT.MD` en el mismo bloque.
- Cada vez que cambie la rama activa, la fase o el estado de validación, actualizar este archivo en `main`.

---

## Prompt mínimo para un chat nuevo

```text
Continúa trabajando en mi repositorio:
https://github.com/xArtXtrAx/Unity-Farm-Simulator

Lee primero CONTINUIDAD_GPT.md desde main y sigue exactamente su orden de lectura y el “Próximo paso exacto” de la bitácora en la rama activa. La rama activa es feature/domain-item-inventory-foundation. La implementación remota de la Fase 6 está completa con catálogo, inventario puro y 88 casos EditMode nuevos; Unity y las pruebas siguen pendientes de validación local. No afirmes que compila o que las pruebas pasan hasta recibir los resultados de Arturo. Tras cada implementación o corrección, actualiza BITÁCORA_GPT.MD y mantén CONTINUIDAD_GPT.md sincronizado.
```

---

## Mantenimiento de este archivo

Mantener breve. Actualizar únicamente rama activa, fase, estado, commit remoto, pruebas, cambios locales y cualquier advertencia necesaria para reanudar sin ambigüedad.
