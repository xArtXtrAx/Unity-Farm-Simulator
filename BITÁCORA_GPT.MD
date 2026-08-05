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

## 4. Fase 6 — Catálogo de objetos e inventario de dominio: EN IMPLEMENTACIÓN; CATÁLOGO COMPLETADO

Rama:

```text
feature/domain-item-inventory-foundation
```

Objetivo: iniciar el núcleo de dominio puro recomendado por la migración. Este bloque no añade hotbar visual, ScriptableObjects, almacenamiento ni interacción en escenas.

### Fuente congelada estudiada

Commit `dd32056c9f8142a2322bc2c1d41f0b05b002598f` de `xArtXtrAx/farming-game-A`:

- `src/game/data/items.ts`;
- `src/game/inventory/InventoryState.ts`;
- `src/game/inventory/InventoryManager.ts`;
- `tests/unit/data/catalogs.test.ts`;
- `tests/unit/inventory/InventoryState.test.ts`;
- `src/game/core/GameState.ts`;
- `tests/integration/core/GameState.integration.test.ts`.

No existe una prueba unitaria dedicada a `InventoryManager`; su responsabilidad de presentación no se migra en este bloque.

### Datos confirmados de la fuente

Ocho IDs iniciales:

- `hoe`;
- `watering-can`;
- `turnip-seeds`;
- `turnip`;
- `carrot-seeds`;
- `carrot`;
- `cabbage-seeds`;
- `cabbage`.

Categorías conocidas: herramienta, semilla, cultivo, material, comida y regalo.

Reglas de catálogo:

- herramientas: stack máximo 1;
- semillas y cosechas actuales: stack máximo 99;
- semillas comprables: nabo 30, zanahoria 50, col 75;
- cosechas vendibles: nabo 60, zanahoria 100, col 150;
- IDs desconocidos se rechazan de forma explícita.

Reglas de inventario:

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

### Tabla de paridad aprobada

| Regla fuente | Tipo o método C# | Cobertura EditMode prevista |
|---|---|---|
| Ocho IDs estables y ordinales | `ItemId` | IDs exactos, parseo y rechazo de desconocidos |
| Categorías, nombres, etiquetas, stacks y precios | `ItemCategory`, `ItemDefinition`, `ItemCatalog` | ocho definiciones exactas y validación de invariantes |
| Comprable/vendible solo con precio válido | `ItemDefinition.IsPurchasable`, `IsSellable` | conjuntos exactos de semillas y cosechas |
| Ocho slots, selección y wrap-around | `InventoryState` | construcción, índices, selección repetida y ciclos |
| Capacidad y stacking deterministas | `GetItemCapacity`, `CanAddItem`, `AddItem` | stacks parciales, slots vacíos, orden y distribución |
| Adición completamente atómica | `AddItemResult`, prevalidación de capacidad | inventario idéntico cuando no cabe todo |
| Consumo y limpieza | `ConsumeSelected`, `ConsumeFromSlot`, `ClearSlot` | parcial, exacto, insuficiente e índices inválidos |
| Snapshot serializable y defensivo | `InventorySlotSnapshot`, `InventorySnapshot` | colección inmutable y ausencia de referencias compartidas |
| Restauración validada | `RestoreSlots`, `Restore` | clamp de selección, slots omitidos y rollback local ante datos inválidos |
| Estado inicial | `CreateInitialPlayerInventory` | paridad exacta de los ocho slots y selección |

### Implementación 6A — Catálogo puro completado

Archivos añadidos en la rama activa:

- `Assets/_Project/Scripts/Domain/ItemCatalog.cs`;
- `Assets/_Project/Scripts/Domain/ItemCatalog.cs.meta`.

Contenido:

- `ItemId` como value object ordinal y case-sensitive con los ocho IDs registrados;
- `ItemCategory` con las seis categorías conocidas;
- `ItemDefinition` inmutable con validación de ID, textos, categoría, stack y precios;
- `ItemCatalog` de solo lectura con búsquedas tipadas y por cadena;
- propiedades y consultas de comprable/vendible;
- rechazo explícito de IDs nulos, vacíos o desconocidos.

Commits correctos en `feature/domain-item-inventory-foundation`:

- `deb03849826a5bfb5ecc07e303440e9277de70b3` — `feat(domain): add pure item catalog`;
- `1264f1996ec462f802316b916471197cfb1fcfc0` — `chore(unity): track item catalog asset`.

### Incidencia transaccional corregida — parámetro de rama del conector

El primer intento usó el nombre antiguo `branch_name`; el conector actual lo ignoró y creó temporalmente los dos archivos en `main`.

Acciones correctivas realizadas de inmediato:

- creación accidental en `main`: `a43bc0209de7b9c5e18aedc4f7ec2c02727af908` y `63698bebad19755e07e5eb8c6a15b98f400679c3`;
- eliminación del meta accidental: `59edfe528f42c238d71491d950dcd71680597f82`;
- eliminación del código accidental: `7ab7d97f76b8eae5be81bcfdb5b64bae343a6db6`;
- verificación posterior: `ItemCatalog.cs` existe en la rama activa y devuelve 404 en `main`.

Regla operativa añadida: para todas las escrituras GitHub posteriores se usará exclusivamente el parámetro `branch` y se verificará la rama después de publicar.

### Comprobación estática realizada

- el archivo funcional se ubica únicamente en `FarmSimulator.Domain` de la rama activa;
- no importa ni menciona `UnityEngine`;
- no cambia asmdefs, escenas, UI, ScriptableObjects ni capas superiores;
- llaves y estructura básica revisadas fuera de Unity;
- no existe compilador C# ni Unity en el entorno remoto, por lo que la compilación continúa pendiente de Arturo.

### Diseño objetivo restante

Dominio puro, sin `UnityEngine`:

- slot de inventario;
- resultado de adición;
- DTO de slot para snapshot;
- snapshot inmutable;
- `InventoryState` con ocho slots por defecto;
- fábrica para crear el estado inicial.

No decidir todavía el formato de ScriptableObjects. El dominio debe poder probarse y serializarse sin referencias Unity.

### Casos EditMode planificados

Se prepararon **25 casos de catálogo** y **63 casos de inventario**, para **88 casos nuevos**. Sumados a los 36 existentes, el conteo EditMode esperado será **124**, sujeto a compilación y descubrimiento reales en Unity.

### Puerta de salida

El catálogo y el inventario deben compilar y pasar pruebas EditMode exhaustivas sin abrir una escena. No se requiere nueva prueba PlayMode porque este bloque no integra Unity.

### Estado de validación

- Investigación fuente: **completada**.
- Tabla de paridad: **completada**.
- Catálogo remoto: **implementado en rama activa; no validado aún en Unity**.
- Inventario remoto: **pendiente**.
- Pruebas EditMode nuevas: **pendientes de publicar**.
- Compilación local Unity: **pendiente**.
- Conteo EditMode esperado al terminar: **124**.
- PlayMode esperado sin cambios: **6**.

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

1. Implementar `InventoryState`, slots, resultado de adición y snapshots en `FarmSimulator.Domain`.
2. Preservar stacking determinista, adición atómica, consumo, selección y restauración validada.
3. Verificar que los archivos existen en `feature/domain-item-inventory-foundation` y no en `main`.
4. Actualizar esta bitácora inmediatamente después de publicar el inventario.
5. Añadir los 25 casos EditMode de catálogo y los 63 de inventario.
6. No crear UI, ScriptableObjects, almacenamiento ni modificar escenas.
7. Revisar estáticamente dependencias y asegurar que Domain no usa `UnityEngine`.
8. Actualizar esta bitácora con archivos, commits, pruebas añadidas, conteo esperado y validación local pendiente.
9. Sincronizar `CONTINUIDAD_GPT.md` en `main` cuando quede cerrado el estado remoto de la fase.
10. Solo entonces pedir a Arturo sincronizar y validar en Unity.
11. No afirmar que Unity compila o que las pruebas pasan hasta que Arturo lo confirme.

---

## 7. Línea base pendiente de migrar

Hotbar visual, almacenamiento, economía, tiempo, agricultura, herramientas, tres cultivos, interacciones, granja, casa, mercado, envíos, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee primero `CONTINUIDAD_GPT.md` desde `main`; después sigue el orden de lectura y el “Próximo paso exacto”. La Fase 5 está integrada mediante el PR #5. La rama activa es `feature/domain-item-inventory-foundation`. La Fase 6 tiene el catálogo puro implementado y debe continuar con inventario y pruebas EditMode, sin UI ni escenas.
