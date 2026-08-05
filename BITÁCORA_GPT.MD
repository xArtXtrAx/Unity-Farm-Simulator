# BITÁCORA GPT — Unity Farm Simulator

Documento maestro de continuidad. Debe permitir reanudar el proyecto sin depender de la memoria del chat.

> **Regla de continuidad:** antes de modificar el repositorio, leer `CONTINUIDAD_GPT.md` desde `main`; después leer este archivo, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, ramas y commits recientes.

> **Regla transaccional:** ninguna implementación, corrección o integración realizada por GPT se considera terminada hasta registrar aquí lo realizado, lo comprobado, lo pendiente y el siguiente paso exacto.

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
- **Rama funcional cerrada:** `feature/domain-item-inventory-foundation`
- **Rama activa nueva:** `chore/cozy-farm-art-intake`
- **Proyecto fuente:** `xArtXtrAx/farming-game-A`
- **Commit fuente congelado:** `dd32056c9f8142a2322bc2c1d41f0b05b002598f`
- **Identidad Unity:** `jarturoga` / `Unity Farm Simulator` / `com.jarturoga.unityfarmsimulator`

---

## 2. Contrato técnico vigente

- Vista cenital y plano lógico XY.
- Physics 2D.
- Cámara ortográfica frontal.
- Resolución lógica 960 × 540, 16:9.
- 64 Pixels Per Unit para el héroe actual.
- Recursos Cozy Farm piloto: 16 Pixels Per Unit.
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

### Fase 6 — Catálogo de objetos e inventario de dominio

Estado: **IMPLEMENTADA, VALIDADA E INTEGRADA**.

Rama cerrada:

```text
feature/domain-item-inventory-foundation
```

Objetivo cumplido: núcleo de catálogo e inventario puro en `FarmSimulator.Domain`, sin hotbar visual, ScriptableObjects, almacenamiento, UI ni cambios de escenas.

#### Fuente congelada estudiada

Commit `dd32056c9f8142a2322bc2c1d41f0b05b002598f` de `xArtXtrAx/farming-game-A`:

- `src/game/data/items.ts`;
- `src/game/inventory/InventoryState.ts`;
- `src/game/inventory/InventoryManager.ts`;
- `tests/unit/data/catalogs.test.ts`;
- `tests/unit/inventory/InventoryState.test.ts`;
- `src/game/core/GameState.ts`;
- `tests/integration/core/GameState.integration.test.ts`.

No existía una prueba unitaria dedicada a `InventoryManager`; su responsabilidad de presentación no se migró en este bloque.

#### Implementación 6A — Catálogo puro

Archivos:

- `Assets/_Project/Scripts/Domain/ItemCatalog.cs`;
- `Assets/_Project/Scripts/Domain/ItemCatalog.cs.meta`.

Incluye:

- `ItemId` ordinal y case-sensitive con ocho IDs estables;
- `ItemCategory`, `ItemDefinition` e `ItemCatalog`;
- stacks 1 para herramientas y 99 para semillas/cosechas actuales;
- semillas comprables por 30/50/75;
- cosechas vendibles por 60/100/150;
- catálogo de solo lectura y búsquedas tipadas/por cadena;
- rechazo explícito de IDs nulos, vacíos o desconocidos.

#### Implementación 6B — Inventario puro

Archivos:

- `Assets/_Project/Scripts/Domain/InventoryState.cs`;
- `Assets/_Project/Scripts/Domain/InventoryState.cs.meta`.

Incluye:

- `InventorySlot`, `InventorySlotSnapshot`, `AddItemResult`, `InventorySnapshot` e `InventoryState`;
- ocho slots por defecto y selección inicial 0;
- selección directa y ciclo con wrap-around;
- copias defensivas;
- stacking determinista: stacks existentes antes de slots vacíos;
- adición completamente atómica por prevalidación de capacidad;
- consumo parcial/exacto y limpieza de slots;
- snapshots serializables y restauración prevalidada;
- estado inicial: azada ×1, regadera ×1, semillas de nabo ×20 y cinco slots vacíos.

#### Implementación 6C — Pruebas EditMode

Cobertura integrada:

- catálogo: **25** casos;
- inventario: **63** casos;
- nuevos: **88** casos;
- línea base existente: **36** casos;
- total EditMode: **124** casos.

#### Validación e integración

- EditMode completo: **124/124**.
- Fallos: **0**.
- PlayMode: **6/6**.
- PR #6: `Add domain item catalog and inventory foundation`.
- Head validado de la rama: `c91b3063f9c06fb949842d64d730694a84d4f416`.
- Método: **Squash and merge**.
- Squash commit en `main`: `4abce7561215a28e7a37e082cbaacf3825021e92`.
- No se integraron escenas, UI, Input System, ScriptableObjects, almacenamiento ni recursos artísticos externos.
- El héroe existente permanece intacto junto con su prefab, animaciones, pivote, collider y profundidad.

---

## 4. Decisión artística vigente

Arturo proporcionó un ZIP del paquete completo **Cozy Farm** para evaluación.

Decisiones:

- el héroe actual **se conserva** durante el desarrollo;
- no se sustituirá el sprite del jugador automáticamente;
- Cozy Farm se considera biblioteca candidata para mundo, agricultura, objetos, edificios, animales y UI;
- el ZIP completo y sus GIF no se versionan;
- las incorporaciones se realizan en `chore/cozy-farm-art-intake` mediante bloques piloto;
- no se organizan todavía todos los edificios, animales, enemigos y estaciones.

### Bloque artístico A1 — Recepción piloto

Estado: **VALIDADO LOCALMENTE**.

Se versionaron cinco hojas PNG originales:

- `items.png` — 160 × 192;
- `seeds.png` — 112 × 96;
- `tools.png` — 592 × 64;
- `crops.png` — 96 × 592;
- `tiles.png` — 864 × 800.

Configuración:

- Sprite (2D and UI);
- 16 PPU;
- Point;
- sin mipmaps;
- Clamp;
- transparencia habilitada;
- sin compresión por defecto.

Commit de assets publicado por Arturo:

```text
e4540b42d275b650f726bad41d4546787ae544e9
```

### Bloque artístico A2 — Slicing curado

Estado: **VALIDADO LOCALMENTE**.

Contenido:

- tres objetos cosechados;
- tres bolsas de semillas;
- dieciocho etapas de cultivo, seis por cultivo;
- cuatro tiles de terreno;
- `tools.png` deliberadamente Single y sin cortes.

Alias visuales reversibles:

- `turnip` usa provisionalmente el gráfico `radish`;
- `cabbage` usa provisionalmente el gráfico `lettuce`;
- los IDs de dominio no cambiaron.

Pruebas añadidas en `CozyFarmPilotArtTests.cs`: seis casos.

Validación final de Arturo — 2026-08-05:

- conteos visuales: **3, 3, 18, 4** y una planilla sin cortes;
- EditMode: **130/130**;
- PlayMode: **6/6**;
- errores: **0**.

### Bloque artístico A3 — Escena de exhibición

Estado: **IMPLEMENTADO REMOTAMENTE; GENERACIÓN Y VALIDACIÓN LOCAL PENDIENTES**.

Objetivo: evaluar conjuntamente la escala y compatibilidad del héroe actual con el subconjunto Cozy Farm antes de construir Tilemaps, UI o integración funcional.

Implementación:

- `Assets/_Project/Scripts/Editor/CozyFarmShowcaseScenePipeline.cs`;
- `Assets/_Project/Scripts/Editor/CozyFarmShowcaseScenePipeline.cs.meta`;
- `Assets/_Project/Tests/EditMode/CozyFarmShowcaseSceneTests.cs`;
- `Assets/_Project/Tests/EditMode/CozyFarmShowcaseSceneTests.cs.meta`.

El pipeline Editor:

- usa `[InitializeOnLoad]` y `EditorApplication.delayCall`;
- genera de forma reproducible `Assets/_Project/Scenes/CozyFarmShowcase.unity`;
- también puede ejecutarse con `Tools > Farm Simulator > Rebuild Cozy Farm Showcase`;
- crea la escena de forma aditiva, la guarda, restaura la escena activa anterior y la cierra;
- no modifica `Lab` ni `Bootstrap`;
- reutiliza el prefab vigente del héroe sin reemplazarlo;
- usa `ReferenceAspectCamera` y cámara ortográfica de 5.625 unidades;
- muestra fondo de césped, cuatro muestras de terreno, seis iconos de catálogo y tres filas de crecimiento;
- no añade Tilemaps, paletas, UI, hotbar ni lógica de juego.

Pruebas A3: cuatro casos EditMode que verifican firma/generación, grupos curados, prefab vigente y cámara.

Commit funcional A3:

```text
f5c2d5aa6dd66bf7117eca985e46557937736a3f
```

Resultado esperado todavía no confirmado:

- EditMode: **134/134**;
- PlayMode: **6/6**.

---

## 5. Protocolo permanente de continuidad

Cadena de recuperación:

```text
README.md en main
  → CONTINUIDAD_GPT.md en main
  → BITÁCORA_GPT.MD en la rama activa o main
  → BUGS.MD + MIGRACION_DESDE_FARMING_GAME_A.MD
  → comparación main/rama y commits recientes
  → Próximo paso exacto
```

---

## 6. Próximo paso exacto

1. En GitHub Desktop, hacer **Fetch origin / Pull origin** en `chore/cozy-farm-art-intake`.
2. Abrir Unity `6000.3.21f1` y esperar compilación e importación.
3. Confirmar la generación de `Assets/_Project/Scenes/CozyFarmShowcase.unity`.
4. Abrir `CozyFarmShowcase` y pulsar Play.
5. Verificar que `Lab` siga intacta.
6. Evaluar visualmente escala, nitidez y coherencia entre héroe, terreno, objetos, semillas y etapas de cultivo.
7. Ejecutar EditMode; esperado **134/134**.
8. Ejecutar PlayMode; esperado **6/6**.
9. Reportar conteos, consola y percepción visual.
10. No hacer commit todavía de la escena generada ni avanzar a Tilemaps/hotbar hasta revisar el resultado.

---

## 7. Línea base pendiente de migrar

Hotbar visual, almacenamiento, economía, tiempo, agricultura, herramientas, tres cultivos, interacciones, granja, casa, mercado, envíos, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee `CONTINUIDAD_GPT.md` desde `main`; después `BITÁCORA_GPT.MD` y `COZY_FARM_INTAKE.md` desde `chore/cozy-farm-art-intake`. La Fase 6 está integrada. Cozy Farm A1 y A2 están validados con 130/130 EditMode, 6/6 PlayMode y cero errores. A3 está implementado en `f5c2d5aa6dd66bf7117eca985e46557937736a3f`: un pipeline Editor genera `CozyFarmShowcase.unity` y añade cuatro pruebas, con esperado 134/134 EditMode. La generación local, inspección visual y pruebas siguen pendientes. Conserva el héroe y no avances a Tilemaps o UI antes de la validación de Arturo.
