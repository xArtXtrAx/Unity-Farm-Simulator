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
- **Rama activa:** `feature/domain-item-inventory-foundation`
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

`FarmSimulator.Domain` no depende de `UnityEngine`.

---

## 3. Fases completadas e integradas

### Fase 1 — Cimentación

Proyecto Unity/URP, assemblies, `Bootstrap`, `Lab`, transición e identidad. PR #1.

### Fase 2 — Calibración visual y espacial

Vista cenital XY, Physics 2D, 960 × 540, 64 PPU y cámara 16:9. Validación: EditMode 4/4, PlayMode 2/2, consola limpia. PR #2.

### Fase 3 — Movimiento del jugador

WASD, flechas, stick, D-pad, zona muerta, diagonales, orientación, collider de pies y límites. Validación: EditMode 11/11, PlayMode 4/4, consola limpia. PR #3.

### Fase 4 — Sprite y animación

Spritesheet congelado `src/assets/hero_hd.png`, 192 × 288 px, frames 64 × 72, idle y walk en cuatro direcciones a 8 FPS. Validación: EditMode 27/27, PlayMode 5/5, consola limpia, teclado y DualSense aprobados. PR #4.

### Fase 5 — Prefab reutilizable y profundidad visual

Player prefab reutilizable, sorting layers, profundidad por Y de los pies y referencia 2D de oclusión.

Validación local de Arturo — 2026-08-05:

- EditMode: **36/36**.
- PlayMode: **6/6**.
- Consola: **0 errores y 0 advertencias**.
- Teclado, DualSense y límites aprobados.
- `Order in Layer`: aproximadamente **9600–10400**.
- Oclusión correcta delante/detrás.
- `BUG-0005` y `BUG-0006`: **VERIFICADOS**.

Integración:

- Commit de recursos: `258de79c40cb8f419f1f9a4b7d737c379015c6e1`.
- PR #5: `Add reusable player prefab and top-down depth sorting`.
- Merge en `main`: `341051f097d9d70796e69b9cddc9277ba3902ed0`.
- Registro posterior: `8a85534526ad1e491cb0687d817f2924acee2d4d`.

Bugs verificados acumulados: `BUG-0001` a `BUG-0006`.

---

## 4. Fase 6 — Catálogo de objetos e inventario de dominio: PREPARADA; IMPLEMENTACIÓN PENDIENTE

Rama:

```text
feature/domain-item-inventory-foundation
```

Objetivo: iniciar el núcleo de dominio puro recomendado por la migración. Este bloque no debe añadir todavía hotbar visual, ScriptableObjects, almacenamiento ni interacción en escenas.

### Fuente congelada identificada

- `src/game/data/items.ts`
- `src/game/inventory/InventoryState.ts`
- `src/game/inventory/InventoryManager.ts`
- pruebas fuente relacionadas con catálogo e inventario, todavía por localizar y estudiar

### Datos confirmados de la fuente

Ocho IDs iniciales:

- `hoe`
- `watering-can`
- `turnip-seeds`
- `turnip`
- `carrot-seeds`
- `carrot`
- `cabbage-seeds`
- `cabbage`

Categorías conocidas: herramienta, semilla, cultivo, material, comida y regalo.

Reglas de catálogo:

- herramientas: stack máximo 1;
- semillas y cosechas actuales: stack máximo 99;
- semillas comprables: nabo 30, zanahoria 50, col 75;
- cosechas vendibles: nabo 60, zanahoria 100, col 150;
- IDs desconocidos deben rechazarse de forma explícita.

Reglas observadas de inventario:

- ocho slots por defecto;
- índice seleccionado inicial 0;
- selección directa validada;
- ciclo con wrap-around;
- los slots vacíos usan cantidad 0;
- completar primero stacks existentes;
- después ocupar slots vacíos;
- adición atómica: si no cabe toda la cantidad, no mutar ningún slot;
- consumo desde slot seleccionado o índice explícito;
- limpiar el slot cuando llega a cero;
- snapshots defensivos de slots e índice seleccionado;
- restauración validada sin exponer referencias mutables internas.

Estado inicial requerido por paridad:

```text
slot 0: hoe ×1
slot 1: watering-can ×1
slot 2: turnip-seeds ×20
slots 3–7: vacíos
selección: slot 0
```

### Diseño objetivo del bloque

Dominio puro, sin `UnityEngine`:

- identificador estable de objeto;
- categoría de objeto;
- definición inmutable;
- catálogo con búsquedas y validación;
- slot de inventario;
- resultado de adición;
- snapshot de inventario;
- `InventoryState` con ocho slots por defecto;
- fábrica o método para crear el estado inicial.

No decidir todavía el formato de ScriptableObjects. El dominio debe poder probarse y serializarse sin referencias Unity.

### Casos mínimos de prueba EditMode

- catálogo contiene exactamente los ocho objetos iniciales;
- IDs, nombres, etiquetas, categorías, stacks y precios correctos;
- sellable/comprable solo cuando corresponde;
- constructor rechaza cero slots;
- índices inválidos se rechazan;
- cantidades inválidas se rechazan;
- selección repetida no reporta cambio;
- ciclo positivo y negativo envuelve correctamente;
- capacidad incluye espacio parcial y slots vacíos;
- add completa stacks existentes antes de abrir otro;
- add distribuye entre múltiples slots;
- add sin capacidad suficiente no cambia el inventario;
- herramientas no se apilan;
- consumo insuficiente no cambia el inventario;
- consumo exacto vacía el slot;
- snapshots no permiten mutación externa del estado;
- restauración recupera slots y selección;
- estado inicial coincide con la fuente.

### Puerta de salida

El catálogo y el inventario pasan pruebas EditMode exhaustivas sin abrir una escena. No se requiere nueva prueba PlayMode en este bloque salvo que aparezca una integración Unity inevitable, lo cual debe justificarse.

### Estado de validación

- Implementación remota: **pendiente**.
- Compilación local Unity: **pendiente**.
- Pruebas EditMode nuevas: **pendientes**.
- Conteo final esperado: debe calcularse después de implementar y documentarse antes de pedir validación.

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

1. Leer `CONTINUIDAD_GPT.md` desde `main` y confirmar esta rama activa.
2. Revisar `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`.
3. Estudiar completos, desde el commit fuente congelado:
   - `src/game/data/items.ts`;
   - `src/game/inventory/InventoryState.ts`;
   - `src/game/inventory/InventoryManager.ts`.
4. Localizar y leer las pruebas fuente que cubren catálogo e inventario.
5. Redactar una tabla de paridad: regla fuente → tipo/método C# → prueba EditMode.
6. Implementar tipos y catálogo puros en `FarmSimulator.Domain`.
7. Implementar `InventoryState`, resultados y snapshots en `FarmSimulator.Domain`.
8. Añadir pruebas EditMode exhaustivas en `FarmSimulator.Tests.EditMode`.
9. No crear UI, ScriptableObjects, almacenamiento ni modificar escenas.
10. Revisar estáticamente dependencias y asegurar que Domain no usa `UnityEngine`.
11. Actualizar esta bitácora con archivos, commits, pruebas añadidas, conteo esperado y validación local pendiente.
12. Solo entonces pedir a Arturo sincronizar y validar en Unity.
13. No afirmar que Unity compila o que las pruebas pasan hasta que Arturo lo confirme.

---

## 7. Línea base pendiente de migrar

Hotbar visual, almacenamiento, economía, tiempo, agricultura, herramientas, tres cultivos, interacciones, granja, casa, mercado, envíos, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee primero `CONTINUIDAD_GPT.md` desde `main`; después sigue el orden de lectura y el “Próximo paso exacto”. La Fase 5 está integrada mediante el PR #5. La rama activa es `feature/domain-item-inventory-foundation` y la Fase 6 debe implementar catálogo e inventario puros de dominio, sin UI ni escenas.
