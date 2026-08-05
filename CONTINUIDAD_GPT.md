# CONTINUIDAD GPT — Unity Farm Simulator

Este archivo es el **punto de entrada permanente** para retomar el desarrollo sin depender de la memoria de un chat.

> Un chat nuevo debe leer este archivo desde `main` antes de buscar la bitácora técnica.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `feature/domain-item-inventory-foundation`
- **Fase actual:** Fase 6 — Catálogo de objetos e inventario de dominio
- **Estado:** rama y alcance preparados; investigación fuente e implementación pendientes
- **Cabeza remota registrada de la rama activa:** `23ea6b945670c913cecb1b681397964dcc4f349d`
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

### Fase activa

La Fase 6 debe comenzar el núcleo de dominio puro:

- IDs y catálogo inicial de ocho objetos;
- límites de stack, categorías y precios;
- inventario de ocho slots;
- selección y ciclo de hotbar a nivel de dominio;
- stacking y capacidad;
- adición totalmente atómica;
- consumo;
- snapshots y restauración;
- estado inicial con azada, regadera y veinte semillas de nabo;
- pruebas EditMode exhaustivas.

No crear todavía UI, ScriptableObjects, almacenamiento ni modificar escenas.

### Fuente congelada prioritaria

Desde `xArtXtrAx/farming-game-A` en `dd32056c9f8142a2322bc2c1d41f0b05b002598f`:

- `src/game/data/items.ts`
- `src/game/inventory/InventoryState.ts`
- `src/game/inventory/InventoryManager.ts`
- pruebas relacionadas con catálogo e inventario

### Validación pendiente

- Implementación remota: pendiente.
- Conteo de pruebas nuevas: pendiente de determinar.
- Compilación local: pendiente.
- Validación EditMode local: pendiente.
- No se espera PlayMode nuevo salvo dependencia Unity justificada.

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

- No asumir que existe implementación de la Fase 6: la rama solo está preparada y documentada.
- No modificar el repositorio antes de estudiar los módulos y pruebas fuente congelados.
- `FarmSimulator.Domain` no debe depender de `UnityEngine`.
- No traducir TypeScript línea por línea; conservar comportamientos, IDs y atomicidad.
- No afirmar que Unity compila o que pruebas pasan hasta validación local de Arturo.
- `main` debe mantenerse estable.
- Después de cada implementación o corrección realizada por GPT, actualizar `BITÁCORA_GPT.MD` en el mismo bloque.
- Cada vez que cambie la rama activa, la fase o el estado de validación, actualizar este archivo en `main`.

---

## Prompt mínimo para un chat nuevo

```text
Continúa trabajando en mi repositorio:
https://github.com/xArtXtrAx/Unity-Farm-Simulator

Lee primero CONTINUIDAD_GPT.md desde main y sigue exactamente su orden de lectura y el “Próximo paso exacto” de la bitácora en la rama activa. La Fase 5 ya está integrada. La rama activa es feature/domain-item-inventory-foundation y la Fase 6 debe implementar catálogo e inventario puros de dominio, sin UI ni escenas. No asumas que existe implementación ni que las pruebas pasan. Tras cada implementación o corrección, actualiza BITÁCORA_GPT.MD y mantén CONTINUIDAD_GPT.md sincronizado.
```

---

## Mantenimiento de este archivo

Mantener breve. Actualizar únicamente rama activa, fase, estado, commit remoto, pruebas, cambios locales y cualquier advertencia necesaria para reanudar sin ambigüedad.
