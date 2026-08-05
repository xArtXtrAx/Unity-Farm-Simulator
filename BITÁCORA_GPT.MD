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
- Pivote del sprite: **X 0.5 / Y 0.12**, cerca de los pies.
- Letterbox o pillarbox fuera de 16:9.

---

## 5. Fases integradas

### Fase 1 — Cimentación

Proyecto Unity/URP, assemblies, `Bootstrap`, `Lab`, transición automática e identidad. PR #1 fusionado mediante `ece618c`.

### Fase 2 — Calibración visual y espacial

Vista cenital XY, Physics 2D, 960 × 540, 64 PPU y cámara 16:9. Validación final: EditMode **4/4**, PlayMode **2/2**, consola limpia. PR #2 fusionado mediante `7ac3f91`.

### Fase 3 — Movimiento del jugador

Physics 2D, WASD, flechas, stick izquierdo, D-pad, zona muerta, magnitud analógica, diagonales normalizadas, cuatro orientaciones, collider de pies y límites visuales. Validación final: EditMode **11/11**, PlayMode **4/4**, consola limpia. PR #3 fusionado mediante `32186f1`; registro en `main` `3df4d04`.

---

## 6. Fase 4 — Sprite y animación: VALIDADA LOCALMENTE, RECURSOS PENDIENTES DE VERSIONAR

### Fuente congelada

- Archivo funcional: `src/assets/hero_hd.png`.
- Repositorio: `xArtXtrAx/farming-game-A`.
- Commit: `dd32056c9f8142a2322bc2c1d41f0b05b002598f`.
- Dimensiones: **192 × 288 px**.
- Distribución: **3 columnas × 4 filas**.
- Frame: **64 × 72 px**.
- Filas: abajo, izquierda, derecha, arriba.
- Frame central: idle.
- Caminata a 8 FPS:
  - abajo `[0, 1, 2, 1]`;
  - izquierda `[3, 4, 5, 4]`;
  - derecha `[6, 7, 8, 7]`;
  - arriba `[9, 10, 11, 10]`.

### Implementación

- `PlayerAnimationModel`: layout, estados, nombres y secuencias.
- `PlayerSpriteAnimator`: orientación/movimiento → estado Animator.
- `PlayerSpriteRuntimeInstaller`: instala `SpriteRenderer + Animator` en `Lab`.
- El rectángulo rojo se oculta; motor, Rigidbody2D, collider y límites permanecen sin cambios.
- El marcador amarillo permanece temporalmente como ayuda de depuración.
- Pipeline Editor para importar Multiple Sprite a 64 PPU, Point, sin mipmaps ni compresión.
- Generación de ocho clips y `FarmerAnimator.controller`.
- Menú manual: `Tools → Farm Simulator → Rebuild Farmer Sprite Assets`.

Commits principales:

- `d68b9e8` — implementación de animación.
- `ed840bc` — fuente correcta y validación segura del PNG.
- `70ec28c` — pruebas de fuente, cabecera y dimensiones.
- `31d037b` — documentación de la corrección.

### BUG-0004

La primera fuente descargada era ilegible y quedó atrapada en el importador de Unity. El pipeline fue corregido para usar `src/assets/hero_hd.png` y validar el archivo antes de importarlo. En la instalación local ya afectada fue necesaria una limpieza manual única de la carpeta `Farmer` con Unity cerrado y copiar el PNG correcto sin `.meta`.

`BUG-0004` está **VERIFICADO** en `BUGS.MD`.

### Validación local de Arturo — 2026-08-05

- Todas las pruebas actuales EditMode y PlayMode pasaron.
- El personaje real aparece con pixel art nítido.
- Idle y caminata funcionan en las cuatro orientaciones.
- Teclado y DualSense continúan funcionando sobre el motor ya validado.
- Se revisaron capturas en los cuatro extremos del mapa.
- El sprite y el marcador amarillo permanecen completamente dentro del terreno.
- Los límites son correctos para el frame de **64 × 72 px**.

### Interpretación de los límites con el sprite real

Los límites se calculan desde el rectángulo lógico completo de **1 × 1.125 unidades**, más el marcador amarillo y el margen del terreno; no desde el contorno opaco del personaje. El spritesheet contiene transparencia alrededor del dibujo, por lo que visualmente parece quedar un poco más de espacio libre. Esto es correcto y evita recortes durante cualquier frame de la animación.

El pivote Y `0.12` hace que una pequeña parte del frame exista bajo el origen de los pies. El margen inferior reservado para el marcador es todavía mayor, por lo que sprite y marcador quedan contenidos de manera segura.

---

## 7. Recursos locales pendientes de versionar

Bajo `Assets/_Project/Resources/Characters/Farmer/` deben existir:

- `farmer-spritesheet.png` y `.meta`;
- carpeta `Animations` y `.meta`;
- ocho clips `.anim` y sus `.meta`;
- `FarmerAnimator.controller` y `.meta`.

Estos recursos fueron generados/importados localmente y todavía deben inspeccionarse en GitHub Desktop antes del commit.

---

## 8. Próximo paso exacto

1. Detener Play Mode y cerrar Unity.
2. Abrir GitHub Desktop en `feature/player-sprite-animation`.
3. Enviar una captura con la lista completa de cambios locales dentro de `Assets/_Project/Resources/Characters/Farmer`.
4. Revisar que solo estén el PNG correcto, sus metadatos, ocho clips y el Animator.
5. Versionar los recursos generados con un commit local claro.
6. Hacer Push origin.
7. Volver a abrir Unity y realizar una última prueba desde un árbol limpio de la rama.
8. Actualizar la bitácora, abrir PR y fusionar la Fase 4 en `main` después de la validación final.

---

## 9. Línea base pendiente de migrar

Granja, casa, mercado, hotbar, inventario, almacenamiento, doce parcelas, herramientas, tres cultivos, tienda, economía, envíos, reloj, estaciones, sueño, fin de día y guardado JSON versionado. Detalle en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 10. Protocolo para chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee `BITÁCORA_GPT.MD`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, revisa ramas y commits, resume el estado y continúa desde el próximo paso documentado.
