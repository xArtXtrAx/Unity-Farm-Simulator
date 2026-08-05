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

Commits históricos de la rama:

- `deb03849826a5bfb5ecc07e303440e9277de70b3` — implementación;
- `1264f1996ec462f802316b916471197cfb1fcfc0` — meta;
- `88ee0e71acc2dbca944f6b04233be96a1772365a` — corrección de asignación explícita del parámetro `out` en `TryGet(ItemId, ...)`.

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

Commits históricos de la rama:

- `dcc20f14e4eca17ca99eccc3342d17e3f2e33bbc` — implementación;
- `e909d9aa295caf4d62db905d6d2cc6202e3104bd` — meta.

#### Implementación 6C — Pruebas EditMode

Archivos:

- `Assets/_Project/Tests/EditMode/ItemCatalogTests.cs`;
- `Assets/_Project/Tests/EditMode/ItemCatalogTests.cs.meta`;
- `Assets/_Project/Tests/EditMode/InventoryStateTests.cs`;
- `Assets/_Project/Tests/EditMode/InventoryStateTests.cs.meta`.

Commits históricos de la rama:

- `00c804a3b19258ecbdf71e3cd8bafd7e47b94de4` — 25 casos de catálogo;
- `75cb1918485e8fb8ecc02d801360866654586fa6` — meta de catálogo;
- `cedaa70982c5dec0da6bf6fa18a7addf9e36200c` — 63 casos de inventario;
- `718000a776ac109dfbdd9d053f3aa93ee591e690` — meta de inventario.

Cobertura:

- catálogo: **25** casos;
- inventario: **63** casos;
- nuevos: **88** casos;
- línea base existente: **36** casos;
- total EditMode: **124** casos.

#### Validación local de Arturo — 2026-08-05

- Unity cargó y ejecutó el conjunto sin errores de compilación reportados.
- EditMode completo: **124/124**.
- Fallos: **0**.
- Casos nuevos aprobados: **88/88**.
- PlayMode no cambió y conserva la regresión previa confirmada de **6/6**.
- No se reportó en ese mensaje un conteo independiente de advertencias de consola.

#### Integración

- PR #6: `Add domain item catalog and inventory foundation`.
- Head validado de la rama: `c91b3063f9c06fb949842d64d730694a84d4f416`.
- Método: **Squash and merge**.
- Squash commit en `main`: `4abce7561215a28e7a37e082cbaacf3825021e92`.
- Archivos integrados: dos fuentes Domain, cuatro pruebas/metas EditMode y `BITÁCORA_GPT.MD`.
- No se integraron escenas, UI, Input System, ScriptableObjects, almacenamiento ni recursos artísticos externos.
- El héroe existente permanece intacto en `Assets/_Project/Resources/Characters/Farmer/farmer-spritesheet.png`, junto con su prefab, animaciones, pivote, collider y profundidad.

#### Incidencia de escritura en rama — resuelta sin residuo

El primer intento del catálogo usó el parámetro antiguo `branch_name`, que el conector ignoró y escribió temporalmente dos archivos en `main`.

- commits accidentales: `a43bc0209de7b9c5e18aedc4f7ec2c02727af908` y `63698bebad19755e07e5eb8c6a15b98f400679c3`;
- reversión inicial: `59edfe528f42c238d71491d950dcd71680597f82` y `7ab7d97f76b8eae5be81bcfdb5b64bae343a6db6`;
- comparación contra `d1893aa596203546e3915b6babf1f8828d861177`: cero archivos diferentes;
- `main` fue restaurada exactamente al head previo, eliminando también el historial accidental;
- todas las escrituras posteriores usaron `branch` y fueron verificadas.

---

## 4. Decisión artística vigente

Arturo proporcionó un ZIP del paquete completo **Cozy Farm** para evaluación.

Decisiones:

- el héroe actual **se conserva** durante el desarrollo;
- no se sustituirá el sprite del jugador de manera automática;
- Cozy Farm se considera una biblioteca candidata para mundo, agricultura, objetos, edificios, animales y UI;
- el ZIP y sus recursos no formaron parte del PR #6;
- no se importará el paquete completo directamente ni se usarán sus GIF como fuentes Unity;
- cualquier incorporación se hará en una rama dedicada mediante un subconjunto piloto, conservando los originales intactos;
- antes de subir recursos con licencia, el repositorio deberá mantenerse privado mientras estén publicados.

Rama activa del bloque:

```text
chore/cozy-farm-art-intake
```

La primera recepción se limita a iconos de objetos, semillas, herramientas, cultivos y terreno. No organizar todavía todos los edificios, animales, enemigos y estaciones.

### Bloque artístico A1 — Recepción piloto de Cozy Farm

Estado: **RAMA CREADA Y PAQUETE PILOTO PREPARADO; COPIA LOCAL, COMMIT DE ASSETS Y VALIDACIÓN UNITY PENDIENTES**.

#### Fuente y selección

Archivo local no versionado: `full version.zip`, Cozy Farm de shubibubi, copia comprada proporcionada por Arturo el 5 de agosto de 2026.

Inventario observado:

- 445 entradas;
- 63 PNG;
- 344 GIF;
- 3 TXT.

Los GIF se consideran únicamente referencia temporal y no entran en Unity.

Se preparó un bundle reproducible con cinco hojas PNG originales:

- `Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/items.png` — 160 × 192;
- `Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/seeds.png` — 112 × 96;
- `Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/tools.png` — 592 × 64;
- `Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/crops.png` — 96 × 592;
- `Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/tiles.png` — 864 × 800.

El bundle también contiene:

- `.meta` deterministas para las cinco texturas y sus carpetas;
- `Assets/_Project/Art/ThirdParty/CozyFarm/README.md` con procedencia, hashes, alcance y validación;
- importación inicial no destructiva.

No se incluyeron ZIP, GIF, `global.png`, `item_carry.png`, personajes, animales, edificios, enemigos, máquinas ni variantes estacionales completas.

#### Configuración inicial

- Texture Type: Sprite (2D and UI).
- Sprite Mode: Single temporal.
- Pixels Per Unit: 16.
- Filter Mode: Point.
- Mip Maps: desactivados.
- Wrap Mode: Clamp.
- Alpha Is Transparency: activado.
- Compresión por defecto: desactivada.

El modo Single es temporal y evita fijar slicing y nombres antes de comprobar cuadrículas, escala y estilo dentro de Unity.

#### Comprobaciones remotas

- firmas PNG válidas;
- dimensiones verificadas;
- extracción directa desde las hojas especializadas del ZIP;
- el héroe, escenas, prefabs, scripts funcionales, asmdefs, Input System y Domain no se modificaron;
- el conector GitHub no permite adjuntar directamente estos archivos binarios locales, por lo que su copia y commit deberán realizarse con GitHub Desktop;
- no se afirma todavía que Unity importe sin advertencias ni que las pruebas sigan pasando.

Documentación de operación creada:

- `COZY_FARM_INTAKE.md`;
- commit inicial de rama: `d2845f676ea33260292d0f27f10a6bb578dcd3d5`.

---

## 5. Protocolo permanente de continuidad

Existe en `main` `CONTINUIDAD_GPT.md` y el README apunta a él.

Cadena de recuperación:

```text
README.md en main
  → CONTINUIDAD_GPT.md en main
  → BITÁCORA_GPT.MD en la rama activa o main
  → BUGS.MD + MIGRACION_DESDE_FARMING_GAME_A.MD
  → comparación main/rama y commits recientes
  → Próximo paso exacto
```

Los cambios exclusivamente locales siguen siendo el único estado que GitHub no protege.

---

## 6. Próximo paso exacto

1. En GitHub Desktop, cambiar a `chore/cozy-farm-art-intake` y ejecutar **Fetch origin / Pull origin**.
2. Extraer `cozy-farm-pilot-intake.zip` en la raíz `D:\Git\Unity-Farm-Simulator`, conservando `Assets/_Project/Art/...`.
3. Revisar en GitHub Desktop que solo aparezcan los 17 archivos del piloto Cozy Farm.
4. Crear un único commit local, por ejemplo `Add Cozy Farm pilot art sources`, y ejecutar **Push origin**.
5. Abrir el proyecto con Unity `6000.3.21f1` y esperar la importación completa.
6. Confirmar que no hay errores ni ciclos de reimportación.
7. Revisar que las cinco texturas usan Point, 16 PPU, sin mipmaps y sin compresión.
8. Comparar héroe, objetos y terreno a escala mundial equivalente.
9. Ejecutar EditMode completo; resultado esperado, todavía no confirmado después de la importación: **124/124**.
10. Ejecutar PlayMode completo; resultado esperado, todavía no confirmado: **6/6**.
11. Comunicar consola, pruebas y percepción visual antes de crear slicing, Tilemaps, paletas, prefabs o UI.
12. No agregar más recursos a esta rama hasta cerrar la validación del piloto.
13. Mantener `FarmSimulator.Domain` independiente de `UnityEngine`.

---

## 7. Línea base pendiente de migrar

Hotbar visual, almacenamiento, economía, tiempo, agricultura, herramientas, tres cultivos, interacciones, granja, casa, mercado, envíos, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee primero `CONTINUIDAD_GPT.md` desde `main`; después `BITÁCORA_GPT.MD` y `COZY_FARM_INTAKE.md` desde `chore/cozy-farm-art-intake`, seguido de `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`. La Fase 6 está integrada mediante PR #6 y validada con 124/124 EditMode. La rama artística está activa en `d2845f676ea33260292d0f27f10a6bb578dcd3d5`; existe un bundle local con cinco PNG, metas y documentación, pero su copia, commit y validación Unity siguen pendientes. Conserva el héroe actual y no avances a slicing masivo hasta la validación de Arturo.
