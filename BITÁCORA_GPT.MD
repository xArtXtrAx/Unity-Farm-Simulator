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
- **Rama activa:** `feature/player-sprite-animation`
- **Proyecto fuente:** `xArtXtrAx/farming-game-A`
- **Commit fuente congelado:** `dd32056c9f8142a2322bc2c1d41f0b05b002598f`
- **Identidad Unity:** `jarturoga` / `Unity Farm Simulator` / `com.jarturoga.unityfarmsimulator`

---

## 2. Principios rectores

- Unity resuelve jerarquía, inspector, escenas, prefabs, sprites, Tilemaps, snapping, colisiones, animación, física y depuración.
- Migrar comportamientos, reglas, datos e identificadores; no traducir Phaser/TypeScript línea por línea.
- `main` debe permanecer estable.
- Arturo usa principalmente GitHub Desktop.
- Todo código propio vive bajo `Assets/_Project` y fuera de `Assembly-CSharp`.
- Las reglas importantes permanecen fuera de `MonoBehaviour` cuando sea razonable.
- Cada implementación o corrección de GPT actualiza esta bitácora en el mismo bloque.

---

## 3. Arquitectura vigente

- `FarmSimulator.Domain`: reglas puras, sin `UnityEngine`.
- `FarmSimulator.Application`: casos de uso y contratos; depende de Domain, sin `UnityEngine`.
- `FarmSimulator.Infrastructure`: persistencia y adaptadores.
- `FarmSimulator.Presentation`: escenas, UI, input y representación visual.
- `FarmSimulator.Editor`: herramientas exclusivas del Editor.
- `FarmSimulator.Tests.EditMode`.
- `FarmSimulator.Tests.PlayMode`.

Control de versiones confirmado: `Visible Meta Files`, `Force Text`, `.meta`, `.gitignore` y `.gitattributes` adecuados para Unity.

---

## 4. Dirección visual y modelo espacial

- Vista cenital basada en sprites y Tilemaps 2D.
- Plano lógico: **XY**.
- Profundidad visual: **Z** y sorting order.
- Cámara: **ortográfica frontal**.
- Física: **Physics 2D**.
- Se permiten partículas, luces, trayectorias y objetos 3D cuando aporten valor.

### Contrato de pantalla y pixel art

- Resolución lógica: **960 × 540**.
- Relación: **16:9**.
- Pixels Per Unit: **64**.
- Celda: **64 × 64 px = 1 × 1 unidad**.
- Campo visible: **15 × 8.4375 unidades**.
- Tamaño ortográfico: **4.21875**.
- Frame del personaje: **64 × 72 px**.
- Personaje en Unity: **1 × 1.125 unidades**.
- Letterbox o pillarbox fuera de 16:9.

---

## 5. Fases integradas

### Fase 1 — Cimentación

Proyecto Unity/URP, assemblies, `Bootstrap`, `Lab`, transición automática e identidad. PR #1 fusionado mediante `ece618c`.

### Fase 2 — Calibración visual y espacial

Vista cenital XY, Physics 2D, 960 × 540, 64 PPU y cámara 16:9. Validación final: EditMode **4/4**, PlayMode **2/2**, consola limpia. PR #2 fusionado mediante `7ac3f91`.

### Fase 3 — Movimiento del jugador

Physics 2D, WASD, flechas, stick izquierdo, D-pad, zona muerta, magnitud analógica, diagonales normalizadas, cuatro orientaciones, collider de pies y límites visuales. Validación final: EditMode **11/11**, PlayMode **4/4**, consola limpia. PR #3 fusionado mediante `32186f1`; registro en `main` `3df4d04`.

Bugs verificados: `BUG-0001`, `BUG-0002`, `BUG-0003`.

---

## 6. Fase 4 — Sprite y animación: IMPLEMENTADA REMOTAMENTE, PENDIENTE DE VALIDACIÓN

### Fuente correcta congelada

- Archivo funcional: `src/assets/hero_hd.png`.
- Repositorio: `xArtXtrAx/farming-game-A`.
- Commit: `dd32056c9f8142a2322bc2c1d41f0b05b002598f`.
- Dimensiones: **192 × 288 px**.
- Tamaño conocido: **84,514 bytes**.
- Distribución: **3 columnas × 4 filas**.
- Frame: **64 × 72 px**.
- Filas: abajo, izquierda, derecha, arriba.
- Frame central: idle.
- Caminata a 8 FPS:
  - abajo `[0, 1, 2, 1]`;
  - izquierda `[3, 4, 5, 4]`;
  - derecha `[6, 7, 8, 7]`;
  - arriba `[9, 10, 11, 10]`.
- Pivote: **X 0.5 / Y 0.12**.

### Implementación

- `PlayerAnimationModel`: layout, estados, nombres y secuencias.
- `PlayerSpriteAnimator`: orientación/movimiento → estado Animator.
- `PlayerSpriteRuntimeInstaller`: instala `SpriteRenderer + Animator` en `Lab`.
- El rectángulo rojo se oculta, pero la base física validada permanece intacta.
- El marcador amarillo permanece como depuración.
- Pipeline Editor idempotente.
- Importación Multiple Sprite, 64 PPU, Point, sin mipmaps ni compresión.
- Generación de ocho clips y `FarmerAnimator.controller`.
- Menú manual: `Tools → Farm Simulator → Rebuild Farmer Sprite Assets`.

Commits iniciales:

- `d68b9e8` — implementación de animación.
- `2099118` — registro inicial.

### BUG-0004 — Fuente ilegible del spritesheet

Arturo abrió Unity y observó cerca de **273 errores** `File could not be read`. La causa fue que el pipeline descargaba por error:

```text
public/assets/farmer/farmer-spritesheet.png
```

Arturo compartió el PNG correcto de 192 × 288 RGBA. Coincide con `src/assets/hero_hd.png` del commit congelado.

Corrección publicada:

- `ed840bc` — cambia a `src/assets/hero_hd.png` y reemplaza archivos inválidos de forma segura.
- `70ec28c` — pruebas de la ruta fuente y de validación PNG/dimensiones.
- `a3234ce` — registro de `BUG-0004`.

Protecciones nuevas:

- valida firma PNG antes de escribir;
- valida exactamente 192 × 288 px;
- elimina automáticamente el PNG local ilegible y su `.meta`;
- no escribe descargas inválidas;
- no vuelve a ejecutar `SaveAndReimport` en bucle sobre un archivo corrupto;
- firma de importación actualizada a `farm-simulator-farmer-64x72-v2`.

`BUG-0004` está **CORREGIDO**, pendiente de validación local.

### Recursos locales esperados

Unity debe generar bajo:

`Assets/_Project/Resources/Characters/Farmer/`

- `farmer-spritesheet.png` y `.meta`;
- carpeta `Animations`;
- ocho `.anim` y sus `.meta`;
- `FarmerAnimator.controller` y `.meta`.

No hacer commit de estos recursos hasta revisar pruebas, consola, vista y cambios de GitHub Desktop.

---

## 7. Resultado esperado de validación

- EditMode: **27/27**.
- PlayMode: **5/5**.
- Consola: **0 errores y 0 advertencias** después de limpiar los errores históricos.
- Aparece el personaje real con píxeles nítidos.
- Idle conserva la última dirección.
- Caminata funciona en cuatro direcciones a 8 FPS.
- Teclado y DualSense siguen funcionando.
- Collider y límites no cambian.

No afirmar que esta fase está aprobada hasta la validación de Arturo.

---

## 8. Próximo paso exacto

1. Cerrar Unity por completo.
2. Permanecer en `feature/player-sprite-animation`.
3. Hacer **Fetch origin** y **Pull origin**.
4. No copiar manualmente el PNG: el pipeline corregido borrará el archivo inválido y descargará la fuente correcta.
5. Abrir Unity y esperar a que termine compilación, descarga, importación y generación.
6. Cuando termine, pulsar **Clear** en la consola para retirar los 273 mensajes históricos.
7. Confirmar consola limpia.
8. Ejecutar EditMode: esperado **27/27**.
9. Ejecutar PlayMode: esperado **5/5**.
10. Ejecutar `Bootstrap → Lab` y probar idle/caminar con teclado y DualSense.
11. Enviar capturas de pruebas, consola, Game y cambios locales en GitHub Desktop.
12. No hacer commit local todavía.

Si el reemplazo automático no ocurre, usar una sola vez:

`Tools → Farm Simulator → Rebuild Farmer Sprite Assets`.

---

## 9. Línea base pendiente de migrar

Granja, casa, mercado, hotbar, inventario, almacenamiento, doce parcelas, herramientas, tres cultivos, tienda, economía, envíos, reloj, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 10. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee `BITÁCORA_GPT.MD`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, revisa ramas y commits, resume el estado y continúa desde el próximo paso documentado.
