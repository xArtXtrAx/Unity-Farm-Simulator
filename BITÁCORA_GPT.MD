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

Bugs verificados: `BUG-0001` a `BUG-0006`.

---

## 4. Fase 5 — Prefab reutilizable y profundidad visual: VALIDADA LOCALMENTE; VERSIONADO LOCAL PENDIENTE

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
- [x] El laboratorio contiene una referencia 2D solapable que usa el mismo sistema de sorting por pies.

### Bugs de la fase

#### BUG-0005 — compilación

Unity abrió inicialmente en Safe Mode por la resolución ambigua de `Application.isPlaying`. Se corrigió con `global::UnityEngine.Application.isPlaying`.

Verificación:

- Unity abrió fuera de Safe Mode.
- Consola: **0 errores y 0 advertencias**.
- Prefab reutilizable y sorting layers generados.
- Estado: **VERIFICADO**.

#### BUG-0006 — no existía una referencia visual compatible

El jugador modificaba correctamente su `sortingOrder`, pero las primitivas 3D opacas del laboratorio no podían intercalarse con un `SpriteRenderer` transparente.

Corrección:

- `LabDepthSortingReference`.
- Rectángulo morado 2D de **1.5 × 2.25 unidades**.
- Pivote inferior como punto de pies.
- Pies en `(0, 0.65)`.
- Sin collider.
- `SpriteRenderer`, sorting layer `Actors` y `TopDownSpriteSorting` iguales al jugador.
- Prueba PlayMode que exige ambas relaciones delante/detrás.

Verificación de Arturo:

- el rectángulo morado apareció correctamente;
- al caminar hacia arriba y superar su línea de pies, el personaje se ocultó detrás;
- al regresar hacia abajo, el personaje volvió a mostrarse delante;
- estado: **VERIFICADO**.

### Validación automatizada final — 2026-08-05

Arturo confirmó:

- EditMode: **36/36**.
- PlayMode: **6/6**.
- Todas las pruebas pasaron sin errores.
- La profundidad dinámica varió aproximadamente entre **9600 y 10400** al recorrer el laboratorio.
- La oclusión visual delante/detrás funcionó en ambos sentidos.

### Recursos generados localmente

Unity generó o actualizó:

- `ProjectSettings/TagManager.asset` con las sorting layers;
- Player prefab bajo la ruta definida en `PlayerPrefabAssetCatalog`;
- metadatos de carpetas y del prefab asociados.

Estos cambios todavía son locales y no están protegidos por GitHub hasta hacer commit y push. No hacer commit antes de revisar la lista completa en GitHub Desktop.

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

1. Detener Play Mode.
2. Cerrar Unity para que todos los archivos generados queden liberados y escritos.
3. Abrir GitHub Desktop en `feature/player-prefab-depth-sorting`.
4. Revisar la lista completa de cambios locales.
5. Enviar una captura donde se vean todos los archivos modificados o añadidos.
6. No desmarcar archivos ni hacer commit todavía.
7. Tras confirmar que solo contiene el prefab, sus metadatos y `ProjectSettings/TagManager.asset`, crear un commit local para los recursos generados.
8. Hacer push, revisar el commit remoto y preparar el PR de Fase 5 hacia `main`.

---

## 7. Línea base pendiente de migrar

Granja, casa, mercado, hotbar, inventario, almacenamiento, doce parcelas, herramientas, tres cultivos, tienda, economía, envíos, reloj, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee primero `CONTINUIDAD_GPT.md` desde `main`; después sigue el orden de lectura y el “Próximo paso exacto” que ahí se indican. No asumas que los cambios locales pendientes ya fueron versionados. Tras cada implementación o corrección, actualiza `BITÁCORA_GPT.MD` y mantén actualizado `CONTINUIDAD_GPT.md` cuando cambie la rama activa, la fase o el estado.
