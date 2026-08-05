# BITÁCORA GPT — Unity Farm Simulator

Documento maestro de continuidad. Debe permitir retomar el proyecto sin depender de la memoria del chat.

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
- **Rama activa:** `feature/player-prefab-depth-sorting`
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

Validación local:

- EditMode: **27/27**.
- PlayMode: **5/5**.
- Consola: **0 errores y 0 advertencias**.
- Teclado y DualSense aprobados.
- Sprite, animación, pivote y límites aprobados.

Integración:

- PR #4: `Add animated farmer sprite and import pipeline`.
- Merge en `main`: `d04a35934df167db1c6366ee59846c36a0228470`.
- Registro de integración: `8ee1e35d689f7ea71a528da5d6f8fdec7ae45d42`.

Bugs verificados: `BUG-0001` a `BUG-0005`.

---

## 4. Fase 5 — Prefab reutilizable y profundidad visual: IMPLEMENTADA; CORRECCIÓN VISUAL PENDIENTE DE VALIDACIÓN

Rama: `feature/player-prefab-depth-sorting`.

### Commits funcionales

- `9e09fdb` — implementación inicial del prefab y orden visual.
- `489851d` — endurece el ciclo de generación del prefab y sorting layers.
- `a9a3fe2` — corrige la colisión de namespace en `PlayerSpriteAnimator`.
- `19147bd` — añade referencia visual 2D de profundidad.
- `b2e746a` — instala la referencia dentro de la calibración del laboratorio.
- `e93efe9` — prueba ambas relaciones delante/detrás del jugador.

### Implementación principal

- [x] `TopDownSortingModel` puro en Application.
- [x] Los pies más abajo reciben un `sortingOrder` mayor y se renderizan delante.
- [x] Cuantización determinista: base 10000 y 100 órdenes por unidad.
- [x] Sorting layers para suelo, objetos, actores, efectos y UI.
- [x] `TopDownSpriteSorting` actualiza el `SpriteRenderer` desde la Y de los pies.
- [x] `PlayerPrefabIdentity` identifica el prefab generado.
- [x] `PlayerPrefabAssetCatalog` centraliza rutas, nombres y firma.
- [x] `PlayerPrefabAssetPipeline` genera el Player prefab reutilizable.
- [x] El prefab conserva Rigidbody2D, collider de pies, input, motor, sprite, Animator, animación, sorting y marcador.
- [x] `PlayerSpriteRuntimeInstaller` instancia el prefab en lugar de construir el jugador manualmente.
- [x] El pipeline espera la recarga de AssetDatabase después de modificar sorting layers.
- [x] La firma del prefab permite regenerarlo cuando cambia su composición.

### BUG-0005 — compilación

Unity abrió inicialmente en Safe Mode por la resolución ambigua de `Application.isPlaying`. Se corrigió con `global::UnityEngine.Application.isPlaying`.

Validación de Arturo:

- Unity abrió fuera de Safe Mode.
- Consola: **0 errores y 0 advertencias**.
- Prefab reutilizable y sorting layers generados.
- `BUG-0005`: **VERIFICADO**.

### Profundidad numérica confirmada

Durante Play Mode, Arturo localizó:

```text
Generated Calibration
└── Playable Player
    └── Playable Player Sprite
```

`Order in Layer` varió aproximadamente entre **9600 y 10400** al recorrer el laboratorio. Esto coincide con:

```text
order = 10000 - round(feetY × 100)
```

Quedó confirmado que bajar aumenta el orden y subir lo disminuye.

### BUG-0006 — no existía una referencia visual compatible

Arturo observó que el jugador seguía siempre por encima de todos los objetos visibles. La causa no estaba en el cálculo del jugador:

- el jugador usa `SpriteRenderer` y sorting por Y;
- las referencias del laboratorio usan `MeshRenderer` y materiales opacos;
- los renderers opacos se dibujan antes que los sprites transparentes, por lo que no pueden intercalarse mediante `sortingOrder`.

Corrección implementada:

- nuevo `LabDepthSortingReference`;
- rectángulo morado 2D de **1.5 × 2.25 unidades**;
- pivote inferior como punto de pies;
- pies en `(0, 0.65)`;
- sin collider para poder atravesarlo;
- `SpriteRenderer`, sorting layer `Actors` y `TopDownSpriteSorting`, iguales al jugador;
- recreación automática si la calibración reconstruye `Generated Calibration`;
- prueba PlayMode que exige jugador detrás con pies más arriba y delante con pies más abajo.

Estado de `BUG-0006`: **CORREGIDO**, pendiente de compilación y validación visual local.

### Pruebas previstas

EditMode: nueve pruebas nuevas de esta fase; total esperado **36/36**.

PlayMode: la prueba existente fue ampliada con la regresión visual lógica; total esperado **6/6**.

La prueba comprueba:

- instancia del prefab reutilizable;
- referencias técnicas transferidas;
- proxy rojo oculto;
- misma sorting layer entre jugador y referencia;
- orden correcto de ambos;
- jugador detrás con pies más arriba;
- jugador delante con pies más abajo;
- instalación idempotente.

### Recursos generados localmente

Unity ya confirmó la generación de:

- `ProjectSettings/TagManager.asset` con las sorting layers;
- Player prefab bajo la ruta definida en `PlayerPrefabAssetCatalog`;
- metadatos asociados.

No hacer commit local antes de revisar la lista completa de cambios.

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

1. Detener Play Mode y cerrar Unity.
2. En GitHub Desktop permanecer en `feature/player-prefab-depth-sorting`.
3. Pulsar **Fetch origin** y **Pull origin**.
4. Abrir Unity y esperar a que compile e importe por completo.
5. Confirmar consola en **0 errores y 0 advertencias**.
6. Abrir `Assets/_Project/Scenes/Bootstrap.unity` y pulsar Play.
7. Esperar la transición a `Lab`.
8. Localizar el rectángulo morado central `Depth Sorting Reference`.
9. Caminar recto hacia arriba atravesándolo:
   - antes de cruzar su línea de pies, el jugador debe verse delante;
   - después de cruzarla, debe verse detrás.
10. Caminar de regreso hacia abajo y confirmar la transición inversa.
11. Probar movimiento y animación en cuatro direcciones con teclado y DualSense.
12. Confirmar límites del terreno y ausencia del rectángulo rojo.
13. Ejecutar EditMode: esperado **36/36**.
14. Ejecutar PlayMode: esperado **6/6**.
15. Limpiar la consola y confirmar **0 errores y 0 advertencias** después de las suites.
16. Enviar capturas de Test Runner, consola, Game y lista completa de cambios locales en GitHub Desktop.
17. No hacer commit local todavía.

---

## 7. Línea base pendiente de migrar

Granja, casa, mercado, hotbar, inventario, almacenamiento, doce parcelas, herramientas, tres cultivos, tienda, economía, envíos, reloj, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee primero `CONTINUIDAD_GPT.md` desde `main`; después sigue el orden de lectura y el “Próximo paso exacto” que ahí se indican. No asumas que las pruebas locales pendientes ya pasaron. Tras cada implementación o corrección, actualiza `BITÁCORA_GPT.MD` y mantén actualizado `CONTINUIDAD_GPT.md` cuando cambie la rama activa, la fase o el estado.
