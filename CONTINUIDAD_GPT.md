# CONTINUIDAD GPT — Unity Farm Simulator

Este archivo es el **punto de entrada permanente** para retomar el desarrollo sin depender de la memoria de un chat.

> Un chat nuevo debe leer este archivo desde `main` antes de buscar la bitácora técnica.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** ninguna todavía
- **Última fase:** Fase 6 — Catálogo de objetos e inventario de dominio
- **Estado:** implementada, validada e integrada
- **PR de integración:** #6 — `Add domain item catalog and inventory foundation`
- **Squash commit de integración:** `4abce7561215a28e7a37e082cbaacf3825021e92`
- **Registro técnico posterior:** `945ae3d8a86346e45b8b83d3da2d8e614c52d259`
- **Bugs activos:** ninguno
- **Bugs verificados:** `BUG-0001` a `BUG-0006`
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`

### Línea base integrada

- Fases 1 a 6 integradas en `main`.
- EditMode: **124/124**.
- Casos nuevos de la Fase 6: **88/88**.
- Fallos reportados: **0**.
- PlayMode conserva la última regresión confirmada de **6/6**.
- Player prefab, sorting layers, profundidad y oclusión aprobados.
- Catálogo puro e inventario de dominio integrados.
- `FarmSimulator.Domain` permanece independiente de `UnityEngine`.

### Fase 6 integrada

`main` contiene ahora:

- catálogo puro con ocho IDs estables, categorías, stacks y precios;
- `ItemId`, `ItemCategory`, `ItemDefinition` e `ItemCatalog`;
- inventario puro de ocho slots;
- selección y ciclo con wrap-around;
- capacidad y stacking deterministas;
- adición totalmente atómica;
- consumo y limpieza de slots;
- snapshots defensivos y restauración prevalidada;
- estado inicial con azada, regadera y veinte semillas de nabo;
- 25 casos EditMode de catálogo;
- 63 casos EditMode de inventario.

No se añadieron escenas, UI, ScriptableObjects, almacenamiento, cambios de Input System ni recursos externos.

### Decisión artística vigente

Arturo proporcionó el paquete completo Cozy Farm para evaluación.

- El héroe actual se conserva: `Assets/_Project/Resources/Characters/Farmer/farmer-spritesheet.png`.
- No cambiar el prefab, animaciones, pivote, collider ni profundidad del jugador salvo decisión visual explícita posterior.
- Cozy Farm es candidato para mundo, agricultura, objetos, edificios, animales y UI.
- No subir el ZIP completo ni GIF de referencia.
- Cualquier incorporación debe realizarse en una rama artística independiente mediante un piloto mínimo.
- Rama sugerida: `chore/cozy-farm-art-intake`.

### Próxima acción

1. Sincronizar `main` localmente mediante GitHub Desktop.
2. No continuar sobre `feature/domain-item-inventory-foundation`; la rama está cerrada.
3. Leer `MIGRACION_DESDE_FARMING_GAME_A.MD` antes de elegir el siguiente bloque funcional.
4. Para Cozy Farm, crear una rama independiente y probar solo iconos de objetos, semillas, herramientas, un cultivo y una pequeña muestra de terreno, sin reemplazar al héroe.

---

## Orden obligatorio de lectura

1. Leer este archivo desde `main`.
2. Leer `BITÁCORA_GPT.MD` desde `main`.
3. Leer `BUGS.MD`.
4. Leer `MIGRACION_DESDE_FARMING_GAME_A.MD`.
5. Revisar ramas y commits recientes.
6. Continuar desde **“Próximo paso exacto”** en la bitácora.

---

## Reglas críticas

- La Fase 6 ya está integrada; no reabrir su rama para añadir funcionalidad.
- `FarmSimulator.Domain` no debe depender de `UnityEngine`.
- No traducir TypeScript línea por línea; conservar comportamientos, IDs y atomicidad.
- El héroe actual se mantiene durante el desarrollo.
- Cozy Farm debe permanecer separado de las fases funcionales y entrar mediante un piloto controlado.
- Después de cada implementación, corrección o integración realizada por GPT, actualizar `BITÁCORA_GPT.MD` en el mismo bloque.
- Cada vez que cambie la rama activa, fase, integración o estado de validación, actualizar este archivo en `main`.
- No afirmar que Unity compila o que pruebas pasan sin una validación ejecutada y reportada.

---

## Prompt mínimo para un chat nuevo

```text
Continúa trabajando en mi repositorio:
https://github.com/xArtXtrAx/Unity-Farm-Simulator

Lee primero CONTINUIDAD_GPT.md y BITÁCORA_GPT.MD desde main; después sigue exactamente su orden de lectura y el “Próximo paso exacto”. La Fase 6 está integrada mediante PR #6 y squash commit 4abce7561215a28e7a37e082cbaacf3825021e92, validada con 124/124 EditMode y cero fallos. No existe todavía una nueva rama activa. Conserva el héroe actual. Cozy Farm solo debe incorporarse en una rama artística separada y mediante un piloto mínimo; no subas el ZIP completo ni los GIF de referencia. Tras cada implementación, corrección o integración, actualiza BITÁCORA_GPT.MD y mantén CONTINUIDAD_GPT.md sincronizado.
```

---

## Mantenimiento de este archivo

Mantener breve. Actualizar únicamente rama activa, fase, estado, commits, pruebas, cambios locales y cualquier advertencia necesaria para reanudar sin ambigüedad.
