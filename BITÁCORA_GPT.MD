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

## 4. Fase 5 — Prefab reutilizable y profundidad visual: IMPLEMENTADA, CORRECCIÓN DE COMPILACIÓN PENDIENTE DE VALIDACIÓN

Rama: `feature/player-prefab-depth-sorting`.

Commits funcionales:

- `9e09fdb` — implementación inicial del prefab y orden visual.
- `489851d` — endurece el ciclo de generación del prefab y sorting layers.
- `a9a3fe2` — corrige la colisión de namespace en `PlayerSpriteAnimator`.

### Implementación

- [x] `TopDownSortingModel` puro en Application.
- [x] Los pies más abajo en pantalla reciben un `sortingOrder` mayor y se renderizan delante.
- [x] Cuantización determinista de profundidad.
- [x] Sorting layers requeridas para suelo, objetos, actores, efectos y UI.
- [x] `TopDownSpriteSorting` actualiza el orden del `SpriteRenderer` desde la Y de los pies.
- [x] `PlayerPrefabIdentity` identifica el prefab generado.
- [x] `PlayerPrefabAssetCatalog` centraliza rutas, nombres y firma de generación.
- [x] `PlayerPrefabAssetPipeline` genera un Player prefab reutilizable.
- [x] El prefab conserva `Rigidbody2D`, `CapsuleCollider2D`, input, motor, sprite, Animator, controlador de animación, sorting dinámico y marcador de depuración.
- [x] `PlayerSpriteRuntimeInstaller` pasa a instanciar el prefab en lugar de construir cada componente manualmente.
- [x] El pipeline espera una recarga de AssetDatabase después de modificar sorting layers antes de generar el prefab.
- [x] `PlayerSpriteAnimator` evita intentar reproducir estados durante la generación fuera de Play Mode.
- [x] La firma del prefab permite regenerarlo cuando cambie su composición.

### Pruebas añadidas

EditMode, nueve pruebas nuevas:

- sorting layers requeridas;
- composición completa del prefab;
- firma vigente del prefab;
- orden inferior delante del superior;
- orden base en Y = 0;
- tres casos de cuantización;
- rechazo de precisión inválida.

PlayMode, una prueba nueva:

- `Lab` instancia el prefab reutilizable y cambia el orden visual al modificar la Y del jugador.

Resultado total previsto:

- EditMode: **36/36**.
- PlayMode: **6/6**.
- Consola: **0 errores y 0 advertencias**.

### Archivos principales

- `Assets/_Project/Scripts/Application/Spatial/TopDownSortingModel.cs`
- `Assets/_Project/Scripts/Editor/PlayerPrefabAssetPipeline.cs`
- `Assets/_Project/Scripts/Presentation/Player/PlayerPrefabAssetCatalog.cs`
- `Assets/_Project/Scripts/Presentation/Player/PlayerPrefabIdentity.cs`
- `Assets/_Project/Scripts/Presentation/Player/PlayerSpriteAnimator.cs`
- `Assets/_Project/Scripts/Presentation/Player/TopDownSortingLayers.cs`
- `Assets/_Project/Scripts/Presentation/Player/TopDownSpriteSorting.cs`
- `Assets/_Project/Tests/EditMode/PlayerPrefabAssetPipelineTests.cs`
- `Assets/_Project/Tests/EditMode/TopDownSortingModelTests.cs`
- `Assets/_Project/Tests/PlayMode/PlayerPrefabSortingTests.cs`

### Recursos esperados después de compilar

- `ProjectSettings/TagManager.asset` con las sorting layers requeridas.
- Player prefab bajo la ruta definida en `PlayerPrefabAssetCatalog`.
- `.meta` correspondientes.

No hacer commit local antes de revisar la lista completa de cambios.

### BUG-0005 — error de compilación al abrir la Fase 5

Evidencia local de Arturo — 2026-08-05:

```text
Assets/_Project/Scripts/Presentation/Player/PlayerSpriteAnimator.cs(51,17):
error CS0234: The type or namespace name 'isPlaying' does not exist in the namespace 'FarmSimulator.Application'
```

Causa raíz confirmada:

- `Application.isPlaying` se resolvía como el namespace hermano `FarmSimulator.Application`.
- La intención era consultar `UnityEngine.Application.isPlaying`.

Corrección:

```csharp
global::UnityEngine.Application.isPlaying
```

Estado de `BUG-0005`: **CORREGIDO**, pendiente de validación local. La corrección es acotada y no modifica movimiento, input, animación ni sorting.

---

## 5. Protocolo permanente de continuidad

Existe en `main` el archivo estable `CONTINUIDAD_GPT.md` y el `README.md` apunta a él.

Cadena de recuperación obligatoria:

```text
README.md en main
  → CONTINUIDAD_GPT.md en main
  → BITÁCORA_GPT.MD en la rama activa
  → BUGS.MD + MIGRACION_DESDE_FARMING_GAME_A.MD
  → comparación main/rama y commits recientes
  → “Próximo paso exacto”
```

Los cambios exclusivamente locales siguen siendo el único estado que GitHub no puede proteger; deben comunicarse antes de cambiar de chat.

---

## 6. Próximo paso exacto

1. Cerrar Unity mientras permanece en Safe Mode.
2. En GitHub Desktop, manteniéndose en `feature/player-prefab-depth-sorting`, ejecutar **Fetch origin** y **Pull origin**.
3. Confirmar que se recibió el commit funcional `a9a3fe2` y los commits documentales posteriores.
4. Abrir Unity nuevamente.
5. Esperar a que compile por completo; no pulsar Play durante la importación.
6. Si Unity abre aún en Safe Mode pero la consola queda sin errores, pulsar **Exit Safe Mode**.
7. Limpiar la consola y confirmar **0 errores y 0 advertencias**.
8. Ejecutar EditMode: esperado **36/36**.
9. Ejecutar PlayMode: esperado **6/6**.
10. Ejecutar `Bootstrap → Lab` y comprobar movimiento/animación con teclado y DualSense.
11. Mover al personaje por encima y por debajo de referencias visuales para evaluar el orden de profundidad.
12. Enviar capturas de Test Runner, consola, Game y lista completa de cambios locales en GitHub Desktop.
13. No hacer commit local todavía.

---

## 7. Línea base pendiente de migrar

Granja, casa, mercado, hotbar, inventario, almacenamiento, doce parcelas, herramientas, tres cultivos, tienda, economía, envíos, reloj, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee primero `CONTINUIDAD_GPT.md` desde `main`; después sigue el orden de lectura y el “Próximo paso exacto” que ahí se indican. No asumas que las pruebas locales pendientes ya pasaron. Tras cada implementación o corrección, actualiza `BITÁCORA_GPT.MD` y mantén actualizado `CONTINUIDAD_GPT.md` cuando cambie la rama activa, la fase o el estado.
