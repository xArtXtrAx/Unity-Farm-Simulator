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
- Reglas importantes deben permanecer fuera de `MonoBehaviour` cuando sea razonable para facilitar pruebas.
- Cada implementación o corrección de GPT debe actualizar esta bitácora en el mismo bloque.

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

## 4. Dirección visual y modelo espacial definitivo

- Vista cenital basada en sprites y Tilemaps 2D.
- Plano lógico principal: **XY**.
- Profundidad y orden visual: **Z** y orden de renderizado.
- Cámara: **ortográfica frontal**.
- Física principal: **Physics 2D**.
- Se permiten partículas, luces, trayectorias y objetos 3D cuando aporten valor.

### Contrato de pantalla y pixel art

- Resolución lógica: **960 × 540**.
- Relación de aspecto: **16:9**.
- Pixels Per Unit inicial: **64**.
- Celda de referencia: **64 × 64 px = 1 × 1 unidad**.
- Campo visible: **15 × 8.4375 unidades**.
- Tamaño ortográfico: **4.21875**.
- Frame del personaje fuente: **64 × 72 px**.
- Personaje en Unity: **1 × 1.125 unidades**.
- Relaciones diferentes conservan 16:9 mediante letterbox o pillarbox.

---

## 5. Fase 1 — Cimentación: COMPLETADA E INTEGRADA

- Proyecto Unity 6.3 LTS / URP creado.
- Arquitectura por assemblies.
- Escenas `Bootstrap` y `Lab`.
- Transición automática `Bootstrap → Lab`.
- Identidad definitiva aplicada y protegida.
- Validación: EditMode **2/2**, PlayMode **1/1**, consola limpia.
- PR #1 fusionado mediante `ece618c`.

---

## 6. Fase 2 — Calibración visual, espacial y de pantalla: COMPLETADA E INTEGRADA

- Vista cenital XY y Physics 2D.
- Resolución lógica 960 × 540, 16:9 y 64 PPU.
- Cámara ortográfica frontal con letterbox/pillarbox.
- Laboratorio con referencias de terreno, sprites, profundidad y efectos 3D.
- `BUG-0001` verificado.
- Validación final: EditMode **4/4**, PlayMode **2/2**, consola limpia.
- PR #2 fusionado mediante `7ac3f91`.

---

## 7. Fase 3 — Base de movimiento del jugador: COMPLETADA E INTEGRADA

Incluye movimiento Physics 2D, WASD, flechas, stick izquierdo, D-pad, zona muerta radial, magnitud analógica, diagonales normalizadas, cuatro orientaciones, collider de pies y límites compatibles con el pivote en los pies.

Validación local final de Arturo:

- EditMode: **11/11 aprobadas**.
- PlayMode: **4/4 aprobadas**.
- Consola: **0 errores**.
- Teclado y DualSense: aprobados.
- Cuerpo y marcador: visibles en los cuatro extremos.
- `BUG-0002` y `BUG-0003`: **VERIFICADOS**.

Integración:

- PR #3: `Add keyboard and DualSense player movement foundation`.
- Merge commit: `32186f1d6a315abd5a3e642cbf54e7cd6b997f4d`.
- Registro posterior en `main`: `3df4d04e24a88c327317f6fb0c1f39eb1c953c4b`.

---

## 8. Fase 4 — Sprite y animación del jugador: IMPLEMENTACIÓN REMOTA PENDIENTE DE VALIDACIÓN

### Fuente congelada

- Archivo original: `public/assets/farmer/farmer-spritesheet.png`.
- Repositorio: `xArtXtrAx/farming-game-A`.
- Commit: `dd32056c9f8142a2322bc2c1d41f0b05b002598f`.
- Dimensiones: **192 × 288 px**.
- Distribución: **3 columnas × 4 filas**.
- Frame: **64 × 72 px**.
- Filas superiores hacia inferiores: abajo, izquierda, derecha, arriba.
- Frame central de cada fila: idle.
- Caminata a **8 FPS**:
  - abajo: `[0, 1, 2, 1]`;
  - izquierda: `[3, 4, 5, 4]`;
  - derecha: `[6, 7, 8, 7]`;
  - arriba: `[9, 10, 11, 10]`.
- Pivote equivalente al prototipo: **X 0.5 / Y 0.12**, cerca de los pies.

### Implementación publicada

Commit funcional: `d68b9e8ed04dc0a61c7a353bfcb88f703fb43c9d` — `feat: add animated farmer sprite pipeline`.

Añadido:

- [x] `PlayerAnimationModel` puro en Application con layout, estados, nombres y secuencias.
- [x] Estados idle y walk para abajo, izquierda, derecha y arriba.
- [x] `PlayerSpriteAnimator` que traduce orientación y movimiento del motor a estados del Animator.
- [x] `PlayerSpriteRuntimeInstaller` que instala `SpriteRenderer + Animator` en `Lab`.
- [x] El rectángulo rojo se desactiva visualmente, pero se conserva como referencia técnica para no alterar el motor, collider ni pruebas existentes.
- [x] El marcador amarillo continúa disponible como depuración.
- [x] `FarmerSpriteAssetPipeline` de Editor, idempotente y ejecutado después de compilar.
- [x] Descarga el PNG desde la URL congelada solamente cuando no existe localmente.
- [x] Importa como Multiple Sprite, 64 PPU, filtro Point, sin mipmaps y sin compresión.
- [x] Corta los 12 frames con nombres estables y pivote personalizado.
- [x] Genera ocho clips `.anim` y `FarmerAnimator.controller` dentro de Resources.
- [x] Menú manual: `Tools → Farm Simulator → Rebuild Farmer Sprite Assets`.
- [x] Nuevas pruebas puras del mapeo original.
- [x] Nuevas pruebas EditMode de importación y Animator.
- [x] Nueva prueba PlayMode de instalación y cambio idle/caminar.

### Recursos que Unity debe generar localmente

Después de sincronizar y abrir Unity deben aparecer bajo:

`Assets/_Project/Resources/Characters/Farmer/`

- `farmer-spritesheet.png` y su `.meta`;
- carpeta `Animations`;
- ocho clips `.anim` y sus `.meta`;
- `FarmerAnimator.controller` y su `.meta`.

Estos archivos generados deben revisarse en GitHub Desktop y versionarse después de validar. No hacer commit local antes de inspeccionarlos.

### Estado de validación

La implementación fue validada estructuralmente en remoto, pero todavía **no** se ha compilado ni ejecutado en Unity local. No afirmar que está aprobada hasta la validación de Arturo.

Resultado esperado cuando termine la descarga, importación y generación:

- EditMode: **25/25**.
- PlayMode: **5/5**.
- Consola: **0 errores y 0 advertencias**.
- El personaje real sustituye visualmente al rectángulo rojo.
- Quieto utiliza el frame central de la última dirección.
- Caminando reproduce cuatro frames a 8 FPS.
- Teclado y DualSense siguen funcionando sin cambios.
- Collider y límites siguen usando la base ya validada.

No existe un bug nuevo confirmado en este bloque. Cualquier fallo observado debe registrarse en `BUGS.MD` antes de corregirse.

---

## 9. Línea base funcional por migrar

El proyecto anterior contiene granja, casa, mercado, personaje, teclado/mouse/gamepad, hotbar, inventario, almacenamiento, doce parcelas, herramientas, tres cultivos de primavera, tienda, economía, envíos, reloj, estaciones, sueño, fin de día y guardado JSON versionado. El detalle está en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 10. Próximo paso exacto

1. Cerrar Unity.
2. Permanecer en `feature/player-sprite-animation`.
3. Ejecutar **Fetch origin** y **Pull origin** en GitHub Desktop.
4. Abrir Unity `6000.3.21f1` y esperar sin intervenir mientras compila, descarga el PNG, reimporta y genera las animaciones.
5. Esperar a que desaparezcan los indicadores de compilación/importación.
6. Revisar la consola; son esperables mensajes informativos de descarga y generación, pero no errores ni advertencias.
7. Ejecutar EditMode: esperado **25/25**.
8. Ejecutar PlayMode: esperado **5/5**.
9. Abrir `Bootstrap.unity`, pulsar Play y probar idle/caminar en las cuatro direcciones con teclado y DualSense.
10. Confirmar que el personaje real aparece con píxeles nítidos, pivote estable en los pies y sin modificar colisiones ni límites.
11. Enviar capturas de Test Runner, consola, Game y de los cambios locales de GitHub Desktop.
12. No hacer commit de los recursos generados hasta revisarlos.

---

## 11. Protocolo para un chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Antes de modificar archivos, lee `BITÁCORA_GPT.MD`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, revisa ramas y commits recientes, resume el estado y continúa desde el próximo paso documentado.
