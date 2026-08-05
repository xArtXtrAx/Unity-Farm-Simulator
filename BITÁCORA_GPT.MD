# BITÁCORA GPT — Unity Farm Simulator

Documento maestro de continuidad. Debe permitir retomar el proyecto sin depender de la memoria del chat.

> **Regla de continuidad:** antes de modificar el repositorio, leer este archivo, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, ramas y commits recientes.

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
- **Rama activa siguiente:** `feature/player-prefab-depth-sorting`
- **Proyecto fuente:** `xArtXtrAx/farming-game-A`
- **Commit fuente congelado:** `dd32056c9f8142a2322bc2c1d41f0b05b002598f`
- **Identidad Unity:** `jarturoga` / `Unity Farm Simulator` / `com.jarturoga.unityfarmsimulator`

---

## 2. Principios rectores

- Migrar comportamientos, reglas, datos e identificadores; no traducir Phaser/TypeScript línea por línea.
- `main` debe permanecer estable.
- Arturo usa principalmente GitHub Desktop.
- Todo código propio vive bajo `Assets/_Project` y fuera de `Assembly-CSharp`.
- Reglas importantes permanecen fuera de `MonoBehaviour` cuando sea razonable.
- Cada implementación o corrección de GPT actualiza esta bitácora en el mismo bloque.

Arquitectura vigente:

- `FarmSimulator.Domain`
- `FarmSimulator.Application`
- `FarmSimulator.Infrastructure`
- `FarmSimulator.Presentation`
- `FarmSimulator.Editor`
- `FarmSimulator.Tests.EditMode`
- `FarmSimulator.Tests.PlayMode`

---

## 3. Dirección visual y contrato espacial

- Vista cenital basada en sprites y Tilemaps 2D.
- Plano lógico: **XY**.
- Profundidad visual: **Z** y sorting order.
- Cámara: **ortográfica frontal**.
- Física: **Physics 2D**.
- Resolución lógica: **960 × 540**, relación **16:9**.
- Pixels Per Unit: **64**.
- Celda: **64 × 64 px = 1 × 1 unidad**.
- Campo visible: **15 × 8.4375 unidades**.
- Tamaño ortográfico: **4.21875**.
- Frame del personaje: **64 × 72 px = 1 × 1.125 unidades**.
- Pivote del sprite: **X 0.5 / Y 0.12**, cerca de los pies.
- Letterbox o pillarbox fuera de 16:9.
- Se permiten partículas, luces, trayectorias y objetos 3D cuando aporten valor.

---

## 4. Fases completadas e integradas

### Fase 1 — Cimentación

Proyecto Unity/URP, assemblies, `Bootstrap`, `Lab`, transición automática e identidad. PR #1, merge `ece618c`.

### Fase 2 — Calibración visual y espacial

Vista cenital XY, Physics 2D, 960 × 540, 64 PPU y cámara 16:9. Validación: EditMode **4/4**, PlayMode **2/2**, consola limpia. PR #2, merge `7ac3f91`.

### Fase 3 — Movimiento del jugador

Physics 2D, WASD, flechas, stick izquierdo, D-pad, zona muerta radial, magnitud analógica, diagonales normalizadas, cuatro orientaciones, collider de pies y límites visuales. Validación: EditMode **11/11**, PlayMode **4/4**, consola limpia. PR #3, merge `32186f1`.

### Fase 4 — Sprite y animación del jugador

Fuente congelada:

- `xArtXtrAx/farming-game-A`
- commit `dd32056c9f8142a2322bc2c1d41f0b05b002598f`
- archivo `src/assets/hero_hd.png`
- PNG **192 × 288 px**, 3 columnas × 4 filas
- frames **64 × 72 px**
- filas: abajo, izquierda, derecha, arriba
- idle: frame central
- caminata a 8 FPS: `[0,1,2,1]` por fila

Implementado:

- `PlayerAnimationModel` puro.
- Idle y walk en cuatro direcciones.
- `PlayerSpriteAnimator` conectado al motor validado.
- `PlayerSpriteRuntimeInstaller` con `SpriteRenderer + Animator`.
- Pipeline de Editor para importación Multiple Sprite, 64 PPU, Point, sin mipmaps ni compresión.
- Validación de firma PNG y dimensiones.
- Ocho clips `.anim` y `FarmerAnimator.controller` versionados.
- El rectángulo rojo queda oculto; Rigidbody2D, collider, límites y controles no cambiaron.
- El marcador amarillo continúa temporalmente como depuración.

Validación local de Arturo — 2026-08-05:

- EditMode: **27/27 aprobadas**.
- PlayMode: **5/5 aprobadas**.
- Consola: **0 errores y 0 advertencias**.
- Idle y caminata funcionan en las cuatro direcciones.
- Teclado y DualSense siguen funcionando.
- Pixel art nítido y pivote estable.
- Sprite y marcador contenidos en los cuatro extremos.

Integración:

- Commit de recursos locales: `d3c65b0` — `feat: add generated farmer animation assets`.
- Cabeza final de rama: `365953d`.
- PR #4: `Add animated farmer sprite and import pipeline`.
- Merge en `main`: `d04a35934df167db1c6366ee59846c36a0228470`.
- Registro posterior en `main`: este commit documental.

Bugs verificados: `BUG-0001`, `BUG-0002`, `BUG-0003`, `BUG-0004`.

---

## 5. Próxima fase — Prefab reutilizable y profundidad visual

Objetivo: dejar de depender de que `LabSpatialCalibration` y un instalador temporal construyan la representación del jugador en ejecución.

Alcance previsto:

1. Crear un **Player prefab** reutilizable con:
   - `Rigidbody2D`;
   - `CapsuleCollider2D` de pies;
   - input unificado;
   - motor de movimiento;
   - `SpriteRenderer`;
   - `Animator`;
   - controlador de animación;
   - marcador de depuración desactivable.
2. Hacer que `Lab` instancie el prefab, sin recrear manualmente sus componentes.
3. Establecer orden visual dinámico basado en la coordenada Y de los pies.
4. Preparar sorting layers estables para suelo, objetos, actores, efectos y UI.
5. Añadir pruebas de composición del prefab, spawn e intercambio de profundidad.
6. Mantener intactos controles, velocidad, collider, animación y límites ya validados.

Después de esta fase podrá comenzar el primer mapa real basado en Tilemap.

---

## 6. Próximo paso exacto

1. Cerrar Unity.
2. En GitHub Desktop cambiar a `main`.
3. Ejecutar **Fetch origin** y **Pull origin** para recibir el PR #4 y este registro.
4. Ejecutar nuevamente **Fetch origin**.
5. Cambiar a `feature/player-prefab-depth-sorting` cuando aparezca publicada.
6. Abrir Unity y confirmar que `main` integrado sigue con consola limpia antes de validar la nueva implementación.

---

## 7. Línea base pendiente de migrar

Granja, casa, mercado, hotbar, inventario, almacenamiento, doce parcelas, herramientas, tres cultivos, tienda, economía, envíos, reloj, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee `BITÁCORA_GPT.MD`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, revisa ramas y commits, resume el estado y continúa desde el próximo paso documentado.
