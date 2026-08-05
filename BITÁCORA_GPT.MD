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

La recepción inicial se limita a iconos de objetos, semillas, cultivos y terreno. No organizar todavía todos los edificios, animales, enemigos y estaciones.

### Bloque artístico A1 — Recepción piloto de Cozy Farm

Estado: **VALIDADO LOCALMENTE**.

#### Fuente y selección

Archivo local no versionado: `full version.zip`, Cozy Farm de shubibubi, copia comprada proporcionada por Arturo el 5 de agosto de 2026.

Inventario observado:

- 445 entradas;
- 63 PNG;
- 344 GIF;
- 3 TXT.

Los GIF se consideran únicamente referencia temporal y no entran en Unity.

Se preparó y versionó un bundle con cinco hojas PNG originales:

- `Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/items.png` — 160 × 192;
- `Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/seeds.png` — 112 × 96;
- `Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/tools.png` — 592 × 64;
- `Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/crops.png` — 96 × 592;
- `Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/tiles.png` — 864 × 800.

No se incluyeron ZIP, GIF, `global.png`, `item_carry.png`, personajes, animales, edificios, enemigos ni variantes estacionales completas.

#### Configuración inicial

- Texture Type: Sprite (2D and UI).
- Sprite Mode: Single temporal.
- Pixels Per Unit: 16.
- Filter Mode: Point.
- Mip Maps: desactivados.
- Wrap Mode: Clamp.
- Alpha Is Transparency: activado.
- Compresión por defecto: desactivada.

#### Publicación y validación

- Commit local de assets publicado por Arturo: `e4540b42d275b650f726bad41d4546787ae544e9` — `Add Cozy Farm pilot art sources`.
- El commit contiene exactamente 17 archivos del piloto.
- Arturo confirmó el 5 de agosto de 2026 que la importación y todas las pruebas solicitadas quedaron OK.
- Como A1 no añadió casos, la línea base confirmada permaneció en **124/124 EditMode** y **6/6 PlayMode**.
- No se reportaron errores de compilación ni ciclos de reimportación.
- El héroe, escenas, prefabs, scripts funcionales, asmdefs, Input System y Domain permanecieron sin cambios.

### Bloque artístico A2 — Slicing curado de Cozy Farm

Estado: **IMPLEMENTADO REMOTAMENTE; VALIDACIÓN LOCAL PENDIENTE**.

Objetivo: convertir únicamente recursos con consumidor próximo o criterio de prueba en sub-sprites nombrados, conservando los PNG originales sin duplicarlos ni editarlos.

#### Objetos y semillas

`items.png` expone tres sprites 16 × 16:

- `cozy_turnip`;
- `cozy_carrot`;
- `cozy_cabbage`.

`seeds.png` expone tres sprites 16 × 16:

- `cozy_turnip_seeds`;
- `cozy_carrot_seeds`;
- `cozy_cabbage_seeds`.

Alias visuales provisionales:

- el paquete denomina `radish` al recurso asociado provisionalmente con el ID de dominio `turnip`;
- el paquete denomina `lettuce` al usado provisionalmente con `cabbage`;
- los IDs de dominio no cambiaron y los alias pueden reemplazarse después sin tocar la lógica.

#### Cultivos

`crops.png` expone 18 sprites 16 × 16:

- `cozy_turnip_stage_0` a `cozy_turnip_stage_5`;
- `cozy_carrot_stage_0` a `cozy_carrot_stage_5`;
- `cozy_cabbage_stage_0` a `cozy_cabbage_stage_5`.

Los 18 sprites usan pivote inferior central para su colocación futura sobre parcelas.

#### Terreno

`tiles.png` expone cuatro sprites 16 × 16:

- `cozy_grass`;
- `cozy_dirt`;
- `cozy_water`;
- `cozy_tilled_soil`.

#### Herramientas

`tools.png` contiene máquinas de procesamiento y mobiliario, no iconos adecuados de azada o regadera. Permanece en modo Single y no se crearon aliases `cozy_hoe` ni `cozy_watering_can`.

#### Pruebas A2

Archivos nuevos:

- `Assets/_Project/Tests/EditMode/CozyFarmPilotArtTests.cs`;
- `Assets/_Project/Tests/EditMode/CozyFarmPilotArtTests.cs.meta`.

Seis casos nuevos comprueban:

- dimensiones exactas de las cinco hojas;
- importación pixel-art a 16 PPU, Point, sin mipmaps, Clamp y sin compresión;
- que `tools.png` siga Single y no exponga aliases falsos de herramienta;
- los seis nombres de objetos/semillas;
- las 18 etapas de cultivo;
- los cuatro tiles piloto.

La aserción de `tools.png` se endureció en `d88eeacb38ff50963b31d1d8a0f2c91b4297bf49` para no depender de cómo Unity represente internamente el sprite principal en modo Single.

#### Alcance remoto confirmado

Desde el commit validado de A1 hasta el head de A2 solo cambian:

- cuatro `.meta` de texturas (`items`, `seeds`, `crops`, `tiles`);
- la prueba A2 y su `.meta`;
- `COZY_FARM_INTAKE.md`;
- esta bitácora al cerrar la transacción documental.

No se modificaron PNG, héroe, escenas, prefabs, runtime, asmdefs, Input System ni Domain.

---

## 5. Protocolo permanente de continuidad

Existe en `main` `CONTINUIDAD_GPT.md` y el README apunta a él.

Cadena de recuperación:

```text
README.md en main
  → CONTINUIDAD_GPT.md en main
  → BITÁCORA_GPT.MD en la rama activa o main
  → COZY_FARM_INTAKE.md
  → BUGS.MD + MIGRACION_DESDE_FARMING_GAME_A.MD
  → comparación main/rama y commits recientes
  → Próximo paso exacto
```

Los cambios exclusivamente locales siguen siendo el único estado que GitHub no protege.

---

## 6. Próximo paso exacto

1. En GitHub Desktop, permanecer en `chore/cozy-farm-art-intake` y ejecutar **Fetch origin / Pull origin**.
2. Confirmar que el head remoto incluido es `d88eeacb38ff50963b31d1d8a0f2c91b4297bf49` o un registro documental posterior sin cambios funcionales.
3. Abrir Unity `6000.3.21f1` y esperar la reimportación de `items.png`, `seeds.png`, `crops.png` y `tiles.png`.
4. Confirmar en el Project Browser o Sprite Editor:
   - 3 sprites de objetos;
   - 3 sprites de semillas;
   - 18 sprites de cultivos;
   - 4 sprites de terreno.
5. Confirmar visualmente que los cultivos tienen pivote inferior central.
6. Ejecutar EditMode completo; resultado esperado, todavía no confirmado: **130/130**.
7. Ejecutar PlayMode completo; resultado esperado, todavía no confirmado: **6/6**.
8. Evaluar si `radish → turnip` y `lettuce → cabbage` son aceptables como alias visuales provisionales.
9. Comunicar pruebas, consola y percepción visual antes de crear Tilemaps, paletas, prefabs, UI o conexión con inventario.
10. No añadir más sprites ni funcionalidad a esta rama hasta cerrar la validación A2.
11. Mantener `FarmSimulator.Domain` independiente de `UnityEngine`.

---

## 7. Línea base pendiente de migrar

Hotbar visual, almacenamiento, economía, tiempo, agricultura, herramientas, tres cultivos, interacciones, granja, casa, mercado, envíos, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee primero `CONTINUIDAD_GPT.md` desde `main`; después `BITÁCORA_GPT.MD` y `COZY_FARM_INTAKE.md` desde `chore/cozy-farm-art-intake`, seguido de `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`. La Fase 6 está integrada mediante PR #6. A1 de Cozy Farm fue validado con 124/124 EditMode y 6/6 PlayMode. A2 está implementado en `d88eeacb38ff50963b31d1d8a0f2c91b4297bf49`, con 3 objetos, 3 semillas, 18 etapas de cultivo, 4 tiles y 6 pruebas EditMode nuevas; su validación esperada es 130/130 EditMode y 6/6 PlayMode. Conserva el héroe actual y no avances a Tilemaps o UI antes de la validación de Arturo.

---

## 9. Actualización posterior — A2 validado y A3 implementado

> **Esta sección sustituye operativamente los apartados 6 y 8 anteriores, que se conservan como registro histórico del estado previo a la validación.**

### A2 — Validación final local

Arturo confirmó el 5 de agosto de 2026:

- `items.png`: **3** sprites;
- `seeds.png`: **3** sprites;
- `crops.png`: **18** sprites;
- `tiles.png`: **4** sprites;
- `tools.png`: una planilla Single sin cortes;
- EditMode: **130/130**;
- PlayMode: **6/6**;
- errores: **0**.

A2 queda **VALIDADO**. Los alias `radish → turnip` y `lettuce → cabbage` siguen siendo provisionales y reversibles.

### A3 — Escena artística de exhibición

Estado: **IMPLEMENTADO REMOTAMENTE; GENERACIÓN E INSPECCIÓN LOCAL PENDIENTES**.

Archivos añadidos:

- `Assets/_Project/Scripts/Editor/CozyFarmShowcaseScenePipeline.cs`;
- `Assets/_Project/Scripts/Editor/CozyFarmShowcaseScenePipeline.cs.meta`;
- `Assets/_Project/Tests/EditMode/CozyFarmShowcaseSceneTests.cs`;
- `Assets/_Project/Tests/EditMode/CozyFarmShowcaseSceneTests.cs.meta`.

El pipeline Editor genera reproduciblemente:

```text
Assets/_Project/Scenes/CozyFarmShowcase.unity
```

Características:

- escena separada de `Lab` y `Bootstrap`;
- cámara ortográfica de **4.21875** con `ReferenceAspectCamera`;
- fondo de césped;
- cuatro muestras de terreno;
- tres objetos cosechados y tres bolsas de semillas;
- tres filas con seis etapas de crecimiento cada una;
- instancia conectada al prefab vigente del héroe;
- comando manual `Tools > Farm Simulator > Rebuild Cozy Farm Showcase`;
- no añade Tilemaps, paletas, UI, hotbar, integración funcional ni cambios en Domain.

Pruebas A3 añadidas: **4** casos EditMode sobre generación/firma, grupos curados, prefab del héroe y cámara.

Head remoto al cerrar esta actualización:

```text
088f1589b1ab26e057e1bdc8fe9c43f7a1d08255
```

### Próximo paso exacto vigente

1. En GitHub Desktop, hacer **Fetch origin / Pull origin** en `chore/cozy-farm-art-intake`.
2. Abrir Unity `6000.3.21f1` y esperar compilación e importación.
3. Confirmar que se genere `Assets/_Project/Scenes/CozyFarmShowcase.unity`.
4. Abrir la escena y pulsar Play.
5. Confirmar que `Lab` permanece intacta.
6. Evaluar escala, nitidez y coherencia del héroe frente a terreno, objetos, semillas y cultivos.
7. Ejecutar EditMode; esperado **134/134**.
8. Ejecutar PlayMode; esperado **6/6**.
9. Reportar apariencia, conteos y cualquier error o advertencia.
10. No hacer commit todavía de la escena generada ni avanzar a Tilemaps/hotbar hasta revisar el resultado.

### Prompt vigente para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee `CONTINUIDAD_GPT.md` desde `main`; después `BITÁCORA_GPT.MD` y `COZY_FARM_INTAKE.md` desde `chore/cozy-farm-art-intake`. A1 y A2 de Cozy Farm están validados con 130/130 EditMode, 6/6 PlayMode y cero errores. A3 está implementado hasta `088f1589b1ab26e057e1bdc8fe9c43f7a1d08255`: un pipeline Editor genera `CozyFarmShowcase.unity` y cuatro pruebas elevan el esperado a 134/134 EditMode. La generación local, inspección visual y pruebas A3 siguen pendientes. Conserva el héroe y no avances a Tilemaps o UI antes de la validación de Arturo.

---

## 10. Actualización posterior — A3 validado y A3.1 implementado

> **Esta sección sustituye operativamente el próximo paso y el prompt de la sección 9; las secciones anteriores permanecen como historial.**

### A3 — Validación técnica y diagnóstico visual

Arturo confirmó el 5 de agosto de 2026:

- EditMode: **134/134**;
- PlayMode: **6/6**;
- errores: **0**.

La escena se generó y conservó correctamente `Lab` y el prefab actual del héroe. La captura reveló, sin embargo, una composición visual no aceptable:

- objetos cosechados y semillas mostrados a escala física de mundo;
- 18 bases circulares `cozy_tilled_soil` repetidas y dominantes;
- muestras de terreno 2×2 demasiado grandes;
- distribución dispersa con mucho espacio vacío;
- comparación entre roles de UI y mundo poco clara.

Conclusión: A3 queda **VALIDADO TÉCNICAMENTE**, pero su primera composición fue **RECHAZADA VISUALMENTE**.

### A3.1 — Reequilibrio de escala y composición

Estado: **IMPLEMENTADO REMOTAMENTE; REGENERACIÓN, INSPECCIÓN Y PRUEBAS LOCALES PENDIENTES**.

Commits funcionales:

- `4e6f8e756a38c6dd1ba9a74032f943105e73a9e7` — reequilibrio del generador;
- `39abe438bb6068b21438fb836b5eea01295f0db3` — pruebas de roles de escala y composición compacta.

Documentación operativa:

- `23f38f9e2c2791cd61170c2c0e39690ff058eff1` — actualización de `COZY_FARM_INTAKE.md`.

Cambios del generador:

- firma elevada a `cozy-farm-showcase-scene-v2` para invalidar y regenerar la escena anterior;
- iconos de objetos y semillas a escala **0.55**, colocados sobre una referencia de 3×2 slots;
- cultivos conservados a escala de mundo **1.0**;
- eliminación de los 18 objetos `soil_for_*`;
- cama compartida de tierra de **6×3 tiles** para las 18 etapas;
- cuatro muestras de terreno reducidas a **un tile individual** cada una;
- héroe intacto a escala **1.0**, sobre referencia de tierra de **2×2 tiles**;
- distribución compactada dentro del encuadre lógico 960×540;
- protección contra sobrescritura si `CozyFarmShowcase` está abierta: se solicita cerrarla y ejecutar el comando manual.

Decisión sobre `cozy_tilled_soil`:

- no se modificó `tiles.png.meta` ni ningún slice A2;
- la silueta circular queda como muestra aislada provisional;
- deja de utilizarse como base repetida bajo cada etapa para no distorsionar la lectura de escala.

Pruebas A3.1:

- `CozyFarmShowcaseSceneTests.cs` pasa de **4 a 6 casos**;
- verifica iconos UI a 0.55 frente a cultivos/héroe a 1.0;
- verifica panel compartido, cama compartida, ausencia de `soil_for_*`, muestras individuales y referencia 2×2 del héroe.

No se modificaron PNG, `.meta` artísticos, `Lab`, prefab/spritesheet/animaciones del héroe, Domain, inventario, Input System, runtime, Tilemaps, paletas o hotbar.

### Próximo paso exacto vigente

1. Cerrar `CozyFarmShowcase` en Unity antes de hacer Pull.
2. En GitHub Desktop, hacer **Fetch origin / Pull origin** en `chore/cozy-farm-art-intake`.
3. Abrir Unity `6000.3.21f1` y esperar compilación/importación.
4. La firma `v2` debe regenerar `Assets/_Project/Scenes/CozyFarmShowcase.unity`.
5. Si aparece el aviso de escena antigua abierta, cerrarla y ejecutar `Tools > Farm Simulator > Rebuild Cozy Farm Showcase`.
6. Abrir la escena y pulsar Play.
7. Confirmar visualmente iconos menores, filas compactas sin círculos repetidos, cuatro muestras individuales y héroe intacto sobre 2×2 tiles.
8. Ejecutar EditMode completo; esperado **136/136**.
9. Ejecutar PlayMode completo; esperado **6/6**.
10. Reportar captura, conteos y cualquier error o advertencia.
11. No hacer commit todavía de la escena generada ni avanzar a Tilemaps/hotbar antes de la revisión visual.

### Prompt vigente para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee `CONTINUIDAD_GPT.md` desde `main`; después `BITÁCORA_GPT.MD` y `COZY_FARM_INTAKE.md` desde `chore/cozy-farm-art-intake`. A1 y A2 están validados. A3 pasó 134/134 EditMode y 6/6 PlayMode sin errores, pero su primera composición fue rechazada por desproporción. A3.1 está implementado hasta `39abe438bb6068b21438fb836b5eea01295f0db3`: iconos a 0.55, cultivos y héroe a 1.0, cama compartida 6×3, muestras individuales y dos pruebas nuevas. El esperado es 136/136 EditMode y 6/6 PlayMode. Conserva el héroe y no avances a Tilemaps o hotbar antes de la validación visual de Arturo.

---

## 11. Actualización posterior — A3.1 validado y A3.2 implementado

> **Esta sección sustituye operativamente el próximo paso y el prompt de la sección 10. Las conclusiones anteriores que descartaban cambiar la escala del héroe quedan corregidas por evidencia visual posterior más directa.**

### A3.1 — Validación final y corrección del diagnóstico

Arturo confirmó el 5 de agosto de 2026:

- EditMode: **136/136**;
- PlayMode: **6/6**;
- errores: **0**.

La composición compacta funcionó técnicamente, pero Arturo movió al héroe junto a cada región y aportó cinco capturas comparativas. Estas comparaciones demostraron que el héroe sigue demasiado pequeño frente a:

- un tile de terreno;
- los cultivos jóvenes e intermedios;
- los cultivos maduros;
- la cama de cultivo completa.

La interpretación anterior de que la geometría era suficiente queda anulada. El diagnóstico correcto es:

- héroe 64 × 72 a 64 PPU = **1 × 1.125 unidades**;
- tile Cozy Farm 16 × 16 a 16 PPU = **1 × 1 unidad**;
- el héroe queda prácticamente de un tile de alto, insuficiente para la lectura humana cenital buscada;
- reducir tiles rompería continuidad de grid;
- escalar la raíz del jugador alteraría collider y referencias técnicas.

A3.1 queda **VALIDADA TÉCNICAMENTE**, pero su relación héroe/mundo permanece **RECHAZADA VISUALMENTE**.

### A3.2 — Calibración visual del héroe a 1.5×

Estado: **IMPLEMENTADO REMOTAMENTE; REGENERACIÓN, INSPECCIÓN Y PRUEBAS LOCALES PENDIENTES**.

Commits funcionales:

- `8c48c55ed5bcc9d6d836f3e83bcc79cc5d7e0200` — firma `v3` y escala del visual del héroe;
- `d4d3757640aa5b4f232bbd35f28d9e924bb328b7` — prueba adicional de escala visual y collider intacto.

Documentación operativa:

- `3b8e057b2f2746fa9732a063e8e4872ffd95dd1d` — actualización de `COZY_FARM_INTAKE.md`.

Cambios:

- firma elevada a `cozy-farm-showcase-scene-v3`;
- nueva constante `HeroVisualScale = 1.5f`;
- raíz `Current Hero` en escala **1.0**;
- hijo `Playable Player Sprite` en escala **1.5**;
- collider, Rigidbody2D, motor, feet sorting, pivote lógico y prefab fuente sin cambios;
- tiles y cultivos conservados en escala **1.0**;
- tamaño visual esperado del héroe: aproximadamente **1.5 × 1.6875 unidades**.

La escala es compatible con la cuadrícula visual: el héroe usa bloques lógicos 4×4 y 1.5× los convierte en bloques 6×6 en la resolución de referencia, sin escalado fraccional de esos bloques.

Pruebas A3.2:

- `CozyFarmShowcaseSceneTests.cs` pasa de **6 a 7 casos**;
- el caso nuevo verifica raíz 1.0, visual 1.5 y collider con ancho, alto y offset originales.

A3.2 solo modifica la instancia de la exhibición. El prefab real no se cambiará antes de la aprobación visual de Arturo.

### Próximo paso exacto vigente

1. Cerrar `CozyFarmShowcase` antes de hacer Pull.
2. Hacer **Fetch origin / Pull origin** en `chore/cozy-farm-art-intake`.
3. Abrir Unity y esperar compilación/importación.
4. La firma `v3` debe regenerar la escena.
5. Si aparece el aviso de escena abierta, cerrarla y ejecutar `Tools > Farm Simulator > Rebuild Cozy Farm Showcase`.
6. Colocar al héroe junto a un tile, un cultivo joven y uno maduro.
7. Compartir una captura para decidir entre 1.5× y una posible prueba 1.25×.
8. Ejecutar EditMode; esperado **137/137**.
9. Ejecutar PlayMode; esperado **6/6**.
10. No hacer commit de la escena generada ni modificar el prefab real antes de aprobar la comparación.

### Prompt vigente para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee `CONTINUIDAD_GPT.md` desde `main`; después `BITÁCORA_GPT.MD` y `COZY_FARM_INTAKE.md` desde `chore/cozy-farm-art-intake`. A1 y A2 están validados. A3.1 pasó 136/136 EditMode y 6/6 PlayMode sin errores, pero cinco comparaciones directas demostraron que el héroe a escala 1.0 sigue demasiado pequeño frente a tiles y cultivos. A3.2 está implementado hasta `d4d3757640aa5b4f232bbd35f28d9e924bb328b7`: firma v3, raíz/collider 1.0 y solo el hijo visual del héroe a 1.5. El esperado es 137/137 EditMode y 6/6 PlayMode. No cambies todavía el prefab real ni avances a Tilemaps/hotbar antes de la aprobación visual de Arturo.
