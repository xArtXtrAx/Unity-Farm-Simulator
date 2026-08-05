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

## 4. Fase 6 — Catálogo de objetos e inventario de dominio: VALIDADA LOCALMENTE; INTEGRACIÓN PENDIENTE

Rama:

```text
feature/domain-item-inventory-foundation
```

Objetivo cumplido: núcleo de catálogo e inventario puro en `FarmSimulator.Domain`, sin hotbar visual, ScriptableObjects, almacenamiento, UI ni cambios de escenas.

### Fuente congelada estudiada

Commit `dd32056c9f8142a2322bc2c1d41f0b05b002598f` de `xArtXtrAx/farming-game-A`:

- `src/game/data/items.ts`;
- `src/game/inventory/InventoryState.ts`;
- `src/game/inventory/InventoryManager.ts`;
- `tests/unit/data/catalogs.test.ts`;
- `tests/unit/inventory/InventoryState.test.ts`;
- `src/game/core/GameState.ts`;
- `tests/integration/core/GameState.integration.test.ts`.

No existe una prueba unitaria dedicada a `InventoryManager`; su responsabilidad de presentación no se migró en este bloque.

### Tabla de paridad ejecutada

| Regla fuente | Tipo o método C# | Cobertura EditMode publicada |
|---|---|---|
| Ocho IDs estables y ordinales | `ItemId` | IDs exactos, parseo y rechazo de desconocidos |
| Categorías, nombres, etiquetas, stacks y precios | `ItemCategory`, `ItemDefinition`, `ItemCatalog` | ocho definiciones exactas y validación de invariantes |
| Comprable/vendible solo con precio válido | `ItemDefinition.IsPurchasable`, `IsSellable` | conjuntos exactos de semillas y cosechas |
| Ocho slots, selección y wrap-around | `InventoryState` | construcción, índices, selección repetida y ciclos |
| Capacidad y stacking deterministas | `GetItemCapacity`, `CanAddItem`, `AddItem` | stacks parciales, slots vacíos, orden y distribución |
| Adición completamente atómica | `AddItemResult`, prevalidación de capacidad | inventario idéntico cuando no cabe todo |
| Consumo y limpieza | `ConsumeSelected`, `ConsumeFromSlot`, `ClearSlot` | parcial, exacto, insuficiente e índices inválidos |
| Snapshot serializable y defensivo | `InventorySlotSnapshot`, `InventorySnapshot` | colección inmutable y ausencia de referencias compartidas |
| Restauración validada | `RestoreSlots`, `Restore` | clamp de selección, slots omitidos y atomicidad ante datos inválidos |
| Estado inicial | `CreateInitialPlayerInventory` | paridad exacta de los ocho slots y selección |

### Implementación 6A — Catálogo puro

Archivos:

- `Assets/_Project/Scripts/Domain/ItemCatalog.cs`;
- `Assets/_Project/Scripts/Domain/ItemCatalog.cs.meta`.

Incluye:

- `ItemId` ordinal y case-sensitive con los ocho IDs aprobados;
- `ItemCategory` con herramienta, semilla, cultivo, material, comida y regalo;
- `ItemDefinition` inmutable;
- stacks 1 para herramientas y 99 para semillas/cosechas actuales;
- semillas comprables por 30/50/75;
- cosechas vendibles por 60/100/150;
- catálogo de solo lectura, búsquedas tipadas y por cadena;
- rechazo explícito de IDs nulos, vacíos o desconocidos.

Commits:

- `deb03849826a5bfb5ecc07e303440e9277de70b3` — implementación;
- `1264f1996ec462f802316b916471197cfb1fcfc0` — meta;
- `88ee0e71acc2dbca944f6b04233be96a1772365a` — corrección de asignación explícita del parámetro `out` en `TryGet(ItemId, ...)` detectada durante revisión estática.

### Implementación 6B — Inventario puro

Archivos:

- `Assets/_Project/Scripts/Domain/InventoryState.cs`;
- `Assets/_Project/Scripts/Domain/InventoryState.cs.meta`.

Tipos:

- `InventorySlot` inmutable y validado;
- `InventorySlotSnapshot` como DTO de cadena sin dependencias Unity;
- `AddItemResult` inmutable;
- `InventorySnapshot` con copia propia y colección de solo lectura;
- `InventoryState`.

Comportamiento:

- ocho slots por defecto, selección inicial 0 y capacidad configurable;
- selección directa y ciclo positivo/negativo con wrap-around;
- copia defensiva al construir y consultar slots;
- completar stacks existentes por índice antes de abrir slots vacíos;
- adición completamente atómica por prevalidación de capacidad;
- herramientas con stack 1 y slots separados;
- consumo parcial, exacto, seleccionado o por índice;
- limpieza del slot al llegar a cero;
- snapshots con IDs serializables;
- restauración prevalidada en un arreglo temporal antes de mutar;
- slots omitidos vacíos, excedentes ignorados y selección limitada al rango;
- estado inicial exacto: azada ×1, regadera ×1, semillas de nabo ×20, cinco slots vacíos, selección 0.

Commits:

- `dcc20f14e4eca17ca99eccc3342d17e3f2e33bbc` — implementación;
- `e909d9aa295caf4d62db905d6d2cc6202e3104bd` — meta.

### Implementación 6C — Pruebas EditMode exhaustivas

Archivos:

- `Assets/_Project/Tests/EditMode/ItemCatalogTests.cs`;
- `Assets/_Project/Tests/EditMode/ItemCatalogTests.cs.meta`;
- `Assets/_Project/Tests/EditMode/InventoryStateTests.cs`;
- `Assets/_Project/Tests/EditMode/InventoryStateTests.cs.meta`.

Commits:

- `00c804a3b19258ecbdf71e3cd8bafd7e47b94de4` — 25 casos de catálogo;
- `75cb1918485e8fb8ecc02d801360866654586fa6` — meta de catálogo;
- `cedaa70982c5dec0da6bf6fa18a7addf9e36200c` — 63 casos de inventario;
- `718000a776ac109dfbdd9d053f3aa93ee591e690` — meta de inventario.

Cobertura nueva:

- catálogo: **25** casos;
- inventario: **63** casos;
- nuevos: **88** casos;
- línea base existente: **36** casos;
- total EditMode: **124** casos.

PlayMode no cambió y conserva la línea base previamente validada de **6/6**.

### Revisión estática realizada

- llaves, paréntesis y corchetes balanceados en los cuatro archivos C#;
- namespaces: `FarmSimulator.Domain.Items`, `FarmSimulator.Domain.Inventory` y `FarmSimulator.Tests.EditMode`;
- cero referencias a `UnityEngine`, `UnityEditor`, `MonoBehaviour`, `ScriptableObject`, escenas, UI o `GameObject`;
- `FarmSimulator.Domain.asmdef` conserva `noEngineReferences: true`;
- el asmdef EditMode ya referencia `FarmSimulator.Domain`; no requirió cambios;
- conteo estático confirmado: 25 + 63 = 88 casos nuevos;
- archivos funcionales y pruebas verificados en la rama activa y ausentes de `main`.

### Incidencia de escritura en rama — resuelta sin residuo

El primer intento del catálogo usó el parámetro antiguo `branch_name`, que el conector ignoró y escribió temporalmente dos archivos en `main`.

- commits accidentales: `a43bc0209de7b9c5e18aedc4f7ec2c02727af908` y `63698bebad19755e07e5eb8c6a15b98f400679c3`;
- reversión inicial: `59edfe528f42c238d71491d950dcd71680597f82` y `7ab7d97f76b8eae5be81bcfdb5b64bae343a6db6`;
- comparación contra el head previo `d1893aa596203546e3915b6babf1f8828d861177`: **cero archivos diferentes**;
- `main` fue restaurada exactamente a `d1893aa596203546e3915b6babf1f8828d861177`, eliminando también el historial accidental;
- todas las escrituras posteriores usaron `branch` y fueron verificadas.

### Validación local de Arturo — 2026-08-05

Arturo sincronizó la rama y confirmó que todas las pruebas del bloque pasaron sin errores.

- Unity pudo cargar y ejecutar el conjunto de pruebas sin errores de compilación reportados.
- EditMode completo: **124/124**.
- Fallos de pruebas: **0**.
- Los **88 casos nuevos** de catálogo e inventario quedan aprobados localmente.
- PlayMode no era requisito de salida de esta fase; se conserva la última regresión confirmada de **6/6** de la Fase 5.
- No se registró en este mensaje un conteo independiente de advertencias de consola.

### Puerta de salida

**Cumplida.** El catálogo y el inventario puros pasan pruebas EditMode exhaustivas sin abrir ni modificar escenas. La Fase 6 puede integrarse a `main` mediante PR.

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

1. No añadir más funcionalidad a `feature/domain-item-inventory-foundation`.
2. Revisar la diferencia final de la rama contra `main`; el alcance permitido es Domain, pruebas EditMode, metas y documentación.
3. Crear el PR de la Fase 6 hacia `main`, con título sugerido: `Add domain item catalog and inventory foundation`.
4. Incluir en el PR la validación local: **124/124 EditMode**, cero fallos y sin cambios de escenas o UI.
5. Revisar el PR y fusionarlo únicamente cuando el alcance sea correcto.
6. Tras el merge, registrar en `BITÁCORA_GPT.MD` el número de PR, commit de merge y estado integrado.
7. Actualizar `CONTINUIDAD_GPT.md` en `main` con la integración y la siguiente rama/fase.
8. Solo después crear la siguiente rama; no continuar el desarrollo sobre esta rama ya validada.
9. El siguiente bloque debe definirse desde `MIGRACION_DESDE_FARMING_GAME_A.MD`, priorizando la capa inmediatamente superior que consuma catálogo e inventario sin adelantar todavía agricultura completa.

---

## 7. Línea base pendiente de migrar

Hotbar visual, almacenamiento, economía, tiempo, agricultura, herramientas, tres cultivos, interacciones, granja, casa, mercado, envíos, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee primero `CONTINUIDAD_GPT.md` desde `main`; después sigue el orden de lectura y el “Próximo paso exacto”. La rama activa es `feature/domain-item-inventory-foundation`. La Fase 6 está implementada y validada localmente con 124/124 EditMode y cero fallos; su integración a `main` mediante PR sigue pendiente. No añadir más funcionalidad a esta rama antes del merge.
