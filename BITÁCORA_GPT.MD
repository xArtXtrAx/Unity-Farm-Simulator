# BITÁCORA GPT — Unity Farm Simulator

Documento maestro de continuidad. Debe permitir reanudar el proyecto sin depender de la memoria del chat.

> **Regla de continuidad:** antes de modificar el repositorio, leer `CONTINUIDAD_GPT.md` desde `main`; después leer este archivo, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, ramas y commits recientes.

> **Regla transaccional:** ninguna implementación o corrección realizada por GPT se considera terminada hasta registrar aquí lo realizado, lo comprobado, lo pendiente y el siguiente paso exacto.

---

## 1. Identidad y entorno

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Propietario:** Arturo García (`xArtXtrAx`)
- **Motor:** Unity 6.3 LTS `6000.3.21f1`
- **Plantilla:** Universal 3D / URP
- **Plataforma inicial:** Windows PC
- **Control prioritario:** DualSense, además de teclado y mouse
- **Ruta local:** `D:\Git\Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Próxima rama:** `feature/domain-item-inventory-foundation`
- **Proyecto fuente:** `xArtXtrAx/farming-game-A`
- **Commit fuente congelado:** `dd32056c9f8142a2322bc2c1d41f0b05b002598f`
- **Identidad Unity:** `jarturoga` / `Unity Farm Simulator` / `com.jarturoga.unityfarmsimulator`

---

## 2. Contrato técnico vigente

- Vista cenital y plano lógico XY.
- Physics 2D.
- Cámara ortográfica frontal.
- Resolución lógica 960 × 540, 16:9.
- 64 Pixels Per Unit.
- Frame del jugador: 64 × 72 px.
- Pivote cercano a los pies: X 0.5 / Y 0.12.
- Teclado y DualSense.
- Orden visual basado en la coordenada Y de los pies.

Arquitectura:

- `FarmSimulator.Domain`
- `FarmSimulator.Application`
- `FarmSimulator.Infrastructure`
- `FarmSimulator.Presentation`
- `FarmSimulator.Editor`
- `FarmSimulator.Tests.EditMode`
- `FarmSimulator.Tests.PlayMode`

Regla de dependencias:

```text
Domain
  ↑
Application
  ↑
Infrastructure / Presentation
```

`Domain` no depende de `UnityEngine`.

---

## 3. Fases completadas e integradas

### Fase 1 — Cimentación

Proyecto Unity/URP, assemblies, `Bootstrap`, `Lab`, transición e identidad. PR #1.

### Fase 2 — Calibración visual y espacial

Vista cenital XY, Physics 2D, 960 × 540, 64 PPU y cámara 16:9. Validación: EditMode 4/4, PlayMode 2/2, consola limpia. PR #2.

### Fase 3 — Movimiento del jugador

WASD, flechas, stick, D-pad, zona muerta, diagonales, orientación, collider de pies y límites. Validación: EditMode 11/11, PlayMode 4/4, consola limpia. PR #3.

### Fase 4 — Sprite y animación

Spritesheet congelado `src/assets/hero_hd.png`, 192 × 288 px, frames 64 × 72, idle y walk en cuatro direcciones a 8 FPS. PNG, ocho clips y Animator versionados.

Validación:

- EditMode: **27/27**.
- PlayMode: **5/5**.
- Consola: **0 errores y 0 advertencias**.
- Teclado, DualSense, sprite, animación, pivote y límites aprobados.

Integración:

- PR #4: `Add animated farmer sprite and import pipeline`.
- Merge en `main`: `d04a35934df167db1c6366ee59846c36a0228470`.

### Fase 5 — Prefab reutilizable y profundidad visual

Incluye:

- Player prefab reutilizable;
- `Rigidbody2D`, collider de pies, input, motor, sprite y Animator dentro del prefab;
- sorting layers para suelo, mundo, actores, efectos y UI;
- orden dinámico por coordenada Y de los pies;
- referencia morada 2D para comprobar oclusión delante/detrás;
- pipeline de generación y recursos Unity versionados;
- instalación idempotente en `Lab`;
- pruebas de composición, generación y profundidad.

Bugs de la fase:

- `BUG-0005`: colisión entre `FarmSimulator.Application` y `UnityEngine.Application`; **VERIFICADO**.
- `BUG-0006`: no existía referencia 2D compatible para validar oclusión; **VERIFICADO**.

Validación local de Arturo — 2026-08-05:

- EditMode: **36/36**.
- PlayMode: **6/6**.
- Consola: **0 errores y 0 advertencias**.
- Teclado y DualSense aprobados.
- Límites aprobados.
- `Order in Layer`: aproximadamente **9600–10400**.
- Jugador detrás al superar hacia arriba la línea de pies de la referencia y delante al regresar hacia abajo.

Recursos versionados:

- `Assets/_Project/Resources/Prefabs/Player/Player.prefab`;
- sus metadatos de carpetas y asset;
- `ProjectSettings/TagManager.asset`.

Integración:

- Commit de recursos: `258de79c40cb8f419f1f9a4b7d737c379015c6e1`.
- Cabeza final de rama: `b4db4b915119effa2e1f1c5eb2e83f1d51f47929`.
- PR #5: `Add reusable player prefab and top-down depth sorting`.
- Merge en `main`: `341051f097d9d70796e69b9cddc9277ba3902ed0`.

Bugs verificados acumulados: `BUG-0001` a `BUG-0006`.

---

## 4. Próxima fase — Catálogo de objetos e inventario de dominio

Rama prevista:

```text
feature/domain-item-inventory-foundation
```

Este bloque inicia el núcleo de dominio recomendado en la migración. No añade todavía hotbar visual ni interacción en escenas.

### Fuente congelada que debe estudiarse

- `src/game/data/items.ts`
- `src/game/inventory/InventoryState.ts`
- `src/game/inventory/InventoryManager.ts`
- pruebas fuente relacionadas con catálogo e inventario

### Alcance funcional

1. IDs estables de objetos.
2. Categorías y definiciones puras de dominio.
3. Catálogo inicial de ocho objetos:
   - `hoe`;
   - `watering-can`;
   - `turnip-seeds`;
   - `turnip`;
   - `carrot-seeds`;
   - `carrot`;
   - `cabbage-seeds`;
   - `cabbage`.
4. Precios y límites de stack equivalentes a la fuente.
5. `InventoryState` de ocho slots por defecto.
6. Selección directa y ciclo con wrap-around.
7. Adición atómica: si no cabe toda la cantidad, no mutar nada.
8. Completar stacks existentes antes de usar slots vacíos.
9. Consumo desde slot seleccionado o índice explícito.
10. Snapshot y restauración defensiva.
11. Estado inicial: azada, regadera y veinte semillas de nabo en los tres primeros slots.
12. Pruebas EditMode puras; sin cargar escenas.

### Restricciones arquitectónicas

- `FarmSimulator.Domain` no debe depender de `UnityEngine`.
- No traducir TypeScript línea por línea.
- Conservar comportamientos, IDs y atomicidad.
- No crear todavía UI, ScriptableObjects ni almacenamiento; esos consumidores vendrán después.
- Las colecciones expuestas no deben permitir mutar el estado interno accidentalmente.

### Puerta de salida

El catálogo y el inventario pasan pruebas EditMode exhaustivas sin abrir Unity Play Mode, incluyendo límites, datos inválidos, stacking, capacidad, atomicidad, consumo, selección y snapshots.

---

## 5. Protocolo permanente de continuidad

Existe en `main` `CONTINUIDAD_GPT.md` y el README apunta a él.

Cadena de recuperación:

```text
README.md en main
  → CONTINUIDAD_GPT.md en main
  → BITÁCORA_GPT.MD en la rama activa
  → BUGS.MD + MIGRACION_DESDE_FARMING_GAME_A.MD
  → comparación main/rama y commits recientes
  → Próximo paso exacto
```

Los cambios exclusivamente locales siguen siendo el único estado que GitHub no protege.

---

## 6. Próximo paso exacto

1. Crear `feature/domain-item-inventory-foundation` desde el `main` que contiene el PR #5.
2. Actualizar `CONTINUIDAD_GPT.md` para apuntar a la nueva rama.
3. En la nueva rama, estudiar los archivos fuente congelados del catálogo e inventario y localizar sus pruebas.
4. Documentar las reglas extraídas antes de implementar.
5. Implementar primero tipos y catálogo puros en `FarmSimulator.Domain`.
6. Implementar `InventoryState` y snapshots sin `UnityEngine`.
7. Añadir pruebas EditMode de cada regla y regresión.
8. No crear UI ni modificar escenas en este bloque.
9. No afirmar que Unity compila o que las pruebas pasan hasta validación local de Arturo.
10. Actualizar esta bitácora tras cada implementación o corrección.

---

## 7. Línea base pendiente de migrar

Hotbar visual, almacenamiento, economía, tiempo, agricultura, herramientas, tres cultivos, interacciones, granja, casa, mercado, envíos, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee primero `CONTINUIDAD_GPT.md` desde `main`; después sigue el orden de lectura y el “Próximo paso exacto”. La Fase 5 está integrada mediante el PR #5. La siguiente fase comienza en `feature/domain-item-inventory-foundation` y debe implementar catálogo e inventario puros de dominio, sin UI ni escenas.
