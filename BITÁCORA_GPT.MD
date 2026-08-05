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

Validación local de Arturo:

- EditMode: **27/27**.
- PlayMode: **5/5**.
- Consola: **0 errores y 0 advertencias**.
- Teclado y DualSense aprobados.
- Sprite, animación, pivote y límites aprobados.

Integración:

- PR #4: `Add animated farmer sprite and import pipeline`.
- Merge en `main`: `d04a35934df167db1c6366ee59846c36a0228470`.
- Registro de integración: `8ee1e35d689f7ea71a528da5d6f8fdec7ae45d42`.

Bugs verificados: `BUG-0001` a `BUG-0004`.

---

## 4. Fase 5 — Prefab reutilizable y profundidad visual: IMPLEMENTADA REMOTAMENTE, PENDIENTE DE VALIDACIÓN

Rama: `feature/player-prefab-depth-sorting`.

Commits funcionales:

- `9e09fdb` — implementación inicial del prefab y orden visual.
- `489851d` — endurece el ciclo de generación del prefab y sorting layers.

### Implementación

- [x] `TopDownSortingModel` puro en Application.
- [x] Los pies más abajo en pantalla reciben un `sortingOrder` mayor y se renderizan delante.
- [x] Cuantización determinista de profundidad.
- [x] Sorting layers requeridas para suelo, objetos, actores, efectos y UI.
- [x] `TopDownSpriteSorting` actualiza el orden del `SpriteRenderer` desde la Y de los pies.
- [x] `PlayerPrefabIdentity` identifica el prefab generado.
- [x] `PlayerPrefabAssetCatalog` centraliza rutas, nombres y firma de generación.
- [x] `PlayerPrefabAssetPipeline` genera un Player prefab reutilizable.
- [x] El prefab conserva:
  - `Rigidbody2D`;
  - `CapsuleCollider2D` de pies;
  - `UnifiedMovementInput`;
  - `TopDownPlayerMotor`;
  - `SpriteRenderer`;
  - `Animator`;
  - `PlayerSpriteAnimator`;
  - `TopDownSpriteSorting`;
  - marcador de orientación y `PlayerProxyFacingView`.
- [x] `PlayerSpriteRuntimeInstaller` pasa a instanciar el prefab en lugar de construir cada componente manualmente.
- [x] El pipeline espera una recarga de AssetDatabase después de modificar sorting layers antes de generar el prefab.
- [x] `PlayerSpriteAnimator` evita intentar reproducir estados durante la generación fuera de Play Mode.
- [x] La firma del prefab permite regenerarlo cuando cambie su composición.

### Pruebas añadidas

EditMode:

- sorting layers requeridas;
- composición completa del prefab;
- firma vigente del prefab;
- orden inferior delante del superior;
- orden base en Y = 0;
- tres casos de cuantización;
- rechazo de precisión inválida.

Total nuevo EditMode: **9 pruebas**.

PlayMode:

- `Lab` instancia el prefab reutilizable y cambia el orden visual al modificar la Y del jugador.

Total nuevo PlayMode: **1 prueba**.

### Archivos principales

- `Assets/_Project/Scripts/Application/Spatial/TopDownSortingModel.cs`
- `Assets/_Project/Scripts/Editor/PlayerPrefabAssetPipeline.cs`
- `Assets/_Project/Scripts/Presentation/Player/PlayerPrefabAssetCatalog.cs`
- `Assets/_Project/Scripts/Presentation/Player/PlayerPrefabIdentity.cs`
- `Assets/_Project/Scripts/Presentation/Player/TopDownSortingLayers.cs`
- `Assets/_Project/Scripts/Presentation/Player/TopDownSpriteSorting.cs`
- `Assets/_Project/Tests/EditMode/PlayerPrefabAssetPipelineTests.cs`
- `Assets/_Project/Tests/EditMode/TopDownSortingModelTests.cs`
- `Assets/_Project/Tests/PlayMode/PlayerPrefabSortingTests.cs`

### Recursos que Unity debe generar localmente

Al abrir la rama, Unity debe modificar o crear:

- `ProjectSettings/TagManager.asset` con las sorting layers requeridas;
- el Player prefab bajo la ruta definida en `PlayerPrefabAssetCatalog`;
- `.meta` correspondientes si todavía no existen.

No hacer commit local antes de revisar la lista completa de cambios.

### Estado de validación

La implementación se revisó estructuralmente en GitHub, pero todavía no se ha compilado ni ejecutado en el Unity local de Arturo.

Resultado esperado:

- EditMode: **36/36**.
- PlayMode: **6/6**.
- Consola: **0 errores y 0 advertencias**.
- El jugador se mueve y anima igual que antes.
- Lab contiene una instancia del prefab generado.
- Al mover al personaje verticalmente cambia su `sortingOrder`.
- Teclado, DualSense, collider, límites y animaciones no presentan regresiones.

No existe un bug nuevo confirmado. Cualquier fallo observado debe añadirse a `BUGS.MD` antes de corregirse.

---

## 5. Protocolo permanente de continuidad

Se creó en `main` el archivo estable `CONTINUIDAD_GPT.md` y el `README.md` apunta a él.

Cadena de recuperación obligatoria:

```text
README.md en main
  → CONTINUIDAD_GPT.md en main
  → BITÁCORA_GPT.MD en la rama activa
  → BUGS.MD + MIGRACION_DESDE_FARMING_GAME_A.MD
  → comparación main/rama y commits recientes
  → “Próximo paso exacto”
```

Reglas:

- `CONTINUIDAD_GPT.md` es el puntero breve y siempre vive en `main`.
- Esta bitácora conserva el detalle técnico y vive actualizada en la rama activa.
- Cuando cambie la rama activa, la fase, el estado de validación o el commit remoto de referencia, debe actualizarse `CONTINUIDAD_GPT.md`.
- Los cambios locales sin commit/push siguen siendo el único estado que GitHub no puede proteger; deben comunicarse antes de cambiar de chat.
- Un chat nuevo puede retomar usando únicamente el prompt mínimo incluido en `CONTINUIDAD_GPT.md`.

Commits de creación en `main`:

- `3cbbee5` — crea `CONTINUIDAD_GPT.md`.
- `8088bf8` — enlaza el punto de entrada desde `README.md`.

---

## 6. Próximo paso exacto

1. Cerrar Unity.
2. En GitHub Desktop cambiar a `main`.
3. Ejecutar **Fetch origin** y **Pull origin** para recibir `CONTINUIDAD_GPT.md`, el README actualizado y el cierre de la Fase 4.
4. Ejecutar nuevamente **Fetch origin**.
5. Cambiar a `feature/player-prefab-depth-sorting`.
6. Ejecutar **Pull origin** para recibir la implementación y esta actualización de bitácora.
7. Abrir Unity y esperar compilación, actualización de sorting layers, recarga y generación del prefab.
8. Esperar a que Unity termine por completo antes de pulsar Play.
9. Limpiar la consola y comprobar cero errores/advertencias.
10. Ejecutar EditMode: esperado **36/36**.
11. Ejecutar PlayMode: esperado **6/6**.
12. Ejecutar `Bootstrap → Lab` y comprobar movimiento/animación con teclado y DualSense.
13. Mover al personaje por encima y por debajo de referencias visuales para evaluar el orden de profundidad.
14. Enviar capturas de Test Runner, consola, Game y lista completa de cambios locales en GitHub Desktop.
15. No hacer commit local todavía.

---

## 7. Línea base pendiente de migrar

Granja, casa, mercado, hotbar, inventario, almacenamiento, doce parcelas, herramientas, tres cultivos, tienda, economía, envíos, reloj, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee primero `CONTINUIDAD_GPT.md` desde `main`; después sigue el orden de lectura y el “Próximo paso exacto” que ahí se indican. No asumas que las pruebas locales pendientes ya pasaron. Tras cada implementación o corrección, actualiza `BITÁCORA_GPT.MD` y mantén actualizado `CONTINUIDAD_GPT.md` cuando cambie la rama activa, la fase o el estado.
