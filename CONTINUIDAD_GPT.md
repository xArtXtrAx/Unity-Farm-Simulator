# CONTINUIDAD GPT — Unity Farm Simulator

Punto de entrada permanente para retomar el desarrollo sin depender de la memoria del chat.

---

## Estado actual

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `chore/cozy-farm-art-intake`
- **Head funcional A3.1:** `39abe438bb6068b21438fb836b5eea01295f0db3`
- **Último registro de rama:** `22c73158556255e4f55b9ba8e9577fc4b2354746`
- **Bloque actual:** A3.2 — separar viñeta de mundo, interfaz y referencia técnica
- **A3.1:** validada con **136/136 EditMode**, **6/6 PlayMode** y cero errores
- **A3 original:** validada con **134/134 EditMode**, **6/6 PlayMode** y cero errores
- **A2:** validada con **130/130 EditMode**, **6/6 PlayMode** y cero errores
- **Commit de assets fuente:** `e4540b42d275b650f726bad41d4546787ae544e9`
- **Última fase funcional:** Fase 6, integrada mediante PR #6
- **Squash commit Fase 6:** `4abce7561215a28e7a37e082cbaacf3825021e92`
- **Bugs activos:** ninguno
- **Bugs verificados:** `BUG-0001` a `BUG-0006`
- **Unity:** `6000.3.21f1`
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`

## Línea base integrada

- Fases 1 a 6 integradas en `main`.
- Catálogo e inventario de dominio integrados.
- `FarmSimulator.Domain` permanece independiente de `UnityEngine`.
- El héroe actual, su prefab, animaciones, pivote, collider y sorting permanecen intactos.

## Cozy Farm A1 y A2

- Cinco hojas fuente versionadas en `Pilot/Source`.
- Configuración: Sprite, 16 PPU, Point, sin mipmaps, Clamp y sin compresión.
- Slicing: 3 objetos, 3 semillas, 18 etapas y 4 muestras de terreno.
- `tools.png` permanece Single y sin cortes.
- Alias provisionales: `radish → turnip`, `lettuce → cabbage`.

## A3 y A3.1 — conclusiones

El pipeline genera localmente:

```text
Assets/_Project/Scenes/CozyFarmShowcase.unity
```

A3.1 separó iconos a escala 0.55 de cultivos/héroe a escala 1.0, eliminó `soil_for_*`, creó cama 6×3 y redujo las muestras de terreno a un tile.

Validación A3.1 de Arturo:

- EditMode: **136/136**;
- PlayMode: **6/6**;
- errores: **0**.

La segunda captura demuestra que la geometría básica es razonable:

- héroe ≈ 1 tile de ancho y algo más de 1 de alto;
- cultivos maduros dentro de una celda;
- cuadrícula 6×3 correcta;
- iconos menores que el héroe.

No se debe seguir escalando todo indiscriminadamente. La sensación restante proviene de mezclar:

- mundo;
- interfaz;
- lámina técnica de crecimiento.

Además:

- `cozy_tilled_soil` parece un hoyo/montículo circular, no una parcela cuadrada;
- `cozy_grass` no se distingue sobre un fondo idéntico;
- existe diferencia de detalle entre el héroe 64×72 y el arte base Cozy Farm 16×16; cambiar Transform no elimina esa diferencia estilística.

## Próximo bloque — A3.2

Separar la evaluación en tres contextos:

1. **Viñeta de mundo:** héroe, césped, parcela pequeña, agua y solo tres etapas representativas.
2. **Interfaz:** seis iconos dentro de Canvas Screen Space usando un panel/slot real del UI de Cozy Farm.
3. **Referencia técnica:** mantener las 18 etapas en una vista secundaria.

Decisiones vigentes:

- no cambiar por ahora la escala 1.0 del héroe ni de los cultivos;
- usar `cozy_dirt` como parcela provisional;
- reclasificar `cozy_tilled_soil` como hoyo/montículo de plantación hasta verificar un tile mejor;
- importar únicamente el mínimo recurso UI necesario;
- no crear todavía Tilemap final, hotbar conectada o agricultura funcional.

## Próxima acción

1. Leer `COZY_FARM_INTAKE.md` desde la rama activa.
2. Preparar un bundle mínimo con `ui/inventory_chopped.png` o un fragmento equivalente de `UI_all.png`.
3. Añadir el panel/slot con slicing curado y pruebas.
4. Rehacer la escena como viñeta real + Canvas separado.
5. No hacer commit de `CozyFarmShowcase.unity` antes de la validación visual.

---

## Orden obligatorio de lectura

1. Este archivo desde `main`.
2. `BITÁCORA_GPT.MD` desde `chore/cozy-farm-art-intake`.
3. `COZY_FARM_INTAKE.md` desde esa rama.
4. `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`.
5. Ramas y commits recientes.

## Reglas críticas

- No reemplazar el héroe automáticamente.
- No subir el ZIP completo ni GIF.
- No asignar imágenes falsas a azada o regadera.
- No confundir iconos UI con objetos físicos de mundo.
- No afirmar que A3.2 está validada antes del reporte local de Arturo.
- Mantener documentación sincronizada después de cada transacción.

## Prompt mínimo para chat nuevo

```text
Continúa xArtXtrAx/Unity-Farm-Simulator. Lee CONTINUIDAD_GPT.md desde main y después BITÁCORA_GPT.MD y COZY_FARM_INTAKE.md desde chore/cozy-farm-art-intake. Fase 6 está integrada. Cozy Farm A1/A2 están validados. A3.1 pasó 136/136 EditMode y 6/6 PlayMode sin errores. La geometría héroe/tile/cultivo ya es razonable; el problema restante es mezclar mundo, UI y lámina técnica, además de una diferencia estilística entre héroe 64×72 y arte 16×16. El próximo bloque A3.2 debe crear una viñeta de mundo, un Canvas con UI real de Cozy Farm y dejar las 18 etapas como referencia secundaria. No cambies todavía la escala del héroe ni avances a Tilemap/hotbar funcional.
```