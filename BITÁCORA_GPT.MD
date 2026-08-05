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
- **Rama activa:** `feature/lab-spatial-calibration`
- **Proyecto fuente:** `xArtXtrAx/farming-game-A`
- **Commit fuente congelado:** `dd32056c9f8142a2322bc2c1d41f0b05b002598f`
- **Identidad Unity:**
  - Company Name: `jarturoga`
  - Product Name: `Unity Farm Simulator`
  - Standalone Identifier: `com.jarturoga.unityfarmsimulator`

---

## 2. Principios rectores

- Unity resuelve jerarquía, inspector, escenas, prefabs, sprites, Tilemaps, snapping, colisiones, animación, física y depuración.
- Migrar comportamientos, reglas, datos e identificadores; no traducir Phaser/TypeScript línea por línea.
- `main` debe permanecer estable.
- Arturo usa principalmente GitHub Desktop.
- Todo código propio vive bajo `Assets/_Project` y fuera de `Assembly-CSharp`.
- Los `ScriptableObject` contendrán definiciones estáticas; el progreso mutable usará DTO e identificadores de texto estables.
- Cada implementación o corrección de GPT debe actualizar esta bitácora en el mismo bloque.

---

## 3. Arquitectura

Assemblies vigentes:

- `FarmSimulator.Domain`: reglas puras, sin `UnityEngine`.
- `FarmSimulator.Application`: casos de uso y contratos; depende de Domain, sin `UnityEngine`.
- `FarmSimulator.Infrastructure`: persistencia y adaptadores.
- `FarmSimulator.Presentation`: escenas, UI, input y representación visual.
- `FarmSimulator.Editor`: herramientas exclusivas del Editor.
- `FarmSimulator.Tests.EditMode`.
- `FarmSimulator.Tests.PlayMode`.

Control de versiones confirmado:

- `Visible Meta Files`.
- `Force Text`.
- `.meta` incluidos.
- `.gitignore` y `.gitattributes` adecuados para Unity.

---

## 4. Dirección visual y modelo espacial definitivo

La dirección artística objetivo es una vista cenital de sprites y Tilemaps 2D, inspirada en la legibilidad de juegos como Harvest Moon de SNES y Stardew Valley, sin copiar recursos ni mapas.

### Contrato espacial

- Plano lógico principal: **XY**.
- Profundidad y orden visual: **Z** y orden de renderizado.
- Cámara: **ortográfica frontal**, sin inclinación ni rotación isométrica.
- Física principal: **Physics 2D**.
- Arte principal: sprites y Tilemaps 2D.
- Se permiten partículas, luces, objetos, trayectorias y efectos 3D cuando aporten valor.
- Los saltos y arcos pueden representarse mediante desplazamientos visuales, animación o efectos 3D sin convertir el mapa base en XZ.

### Contrato de pantalla y escala de pixel art

Se conserva la escala lógica demostrada por el prototipo anterior:

- Resolución lógica de referencia: **960 × 540**.
- Relación de aspecto objetivo: **16:9**.
- Pixels Per Unit inicial: **64**.
- Una celda de 64 × 64 px equivale a **1 × 1 unidad** de Unity.
- Campo visible lógico: **15 × 8.4375 unidades**.
- Tamaño ortográfico de cámara: **4.21875**.
- Frame de personaje fuente: **64 × 72 px**.
- Referencia de personaje en Unity: **1 × 1.125 unidades**.
- Profundidad de cámara: Z `-10`.
- En salidas 4:3 o ultrawide, la cámara preservará 16:9 mediante letterbox o pillarbox.

Esta decisión evita reescalar destructivamente `hero_hd.png` y mantiene compatibilidad conceptual con el mundo lógico de 960 × 540 del prototipo.

### Decisión descartada

La primera calibración usó mundo XZ, física 3D y cámara ortográfica inclinada. Produjo una vista isométrica en rombo que no coincide con la dirección artística. Se conserva solo como aprendizaje histórico.

---

## 5. Fase 1 — Cimentación: COMPLETADA E INTEGRADA

- [x] Proyecto Unity 6.3 LTS / URP creado.
- [x] Arquitectura por assemblies.
- [x] Escenas `Bootstrap` y `Lab` registradas.
- [x] Transición automática `Bootstrap → Lab`.
- [x] Paquetes de plantilla innecesarios retirados.
- [x] Identidad definitiva aplicada y protegida.
- [x] EditMode: 2/2 aprobadas.
- [x] PlayMode: 1/1 aprobada.
- [x] Consola: 0 errores y 0 advertencias.
- [x] PR #1 fusionado en `main` mediante `ece618c`.

---

## 6. Fase 2 — Calibración visual y espacial

### Calibración cenital validada

- Commit inicial isométrico: `8ce5c5e`.
- Corrección a vista cenital XY: `edd6846`.
- Estado remoto reconciliado: `48adb0f`.
- Corrección de colliders: `cdb6cdd`.
- Prueba de regresión de colliders: `c8342a2`.
- Validación visual documentada: `f0e5df6`.

Validación local confirmada por Arturo:

- [x] EditMode previo: **3/3 aprobadas**.
- [x] PlayMode final de la corrección: **2/2 aprobadas**.
- [x] Consola sin errores.
- [x] `Bootstrap → Lab` funciona.
- [x] Cuadrícula rectangular alineada a pantalla.
- [x] Caminos, parcelas, estanque, árboles, sprites, profundidad y arco visual se representan sin perspectiva isométrica.
- [x] La dirección visual cenital queda aceptada.

### BUG-0001 — VERIFICADO

El conflicto entre `BoxCollider` 3D y `BoxCollider2D` quedó resuelto y verificado localmente. El registro completo permanece en `BUGS.MD`.

### Calibración de resolución y PPU implementada remotamente

Commit funcional: `0b26b2e` — `feat: calibrate pixel-art display scale`.

Implementado:

- [x] `PixelArtDisplayModel.cs` fija resolución 960 × 540, 16:9, 64 PPU, campo visible, cámara y tamaño del personaje.
- [x] Política pura de viewport para 16:9, 4:3 y ultrawide.
- [x] `ReferenceAspectCamera.cs` aplica letterbox/pillarbox y responde a cambios de tamaño de ventana.
- [x] `SpatialModel` adopta 15 columnas × 8 filas de referencia y escala 64 px = 1 unidad.
- [x] `LabSpatialCalibration` se reencuadra para el campo visible de 960 × 540.
- [x] Personajes de referencia pasan a 1 × 1.125 unidades, equivalentes a 64 × 72 px.
- [x] EditMode añade validación de resolución, PPU, cámara y viewports.
- [x] PlayMode valida componente de aspecto, `Camera.rect`, dimensiones y tamaño de personaje.
- [x] `BUGS.MD` marca `BUG-0001` como verificado en `9592532`.

Pendiente de validación local del nuevo commit:

- [ ] Unity debe compilar `0b26b2e` sin errores.
- [ ] EditMode esperado: **4/4**.
- [ ] PlayMode esperado: **2/2**.
- [ ] Consola esperada: **0 errores y 0 advertencias**.
- [ ] `Lab` debe verse más acercado, mostrando una referencia de 15 × 8 celdas con pequeño margen vertical.
- [ ] La ventana Game debe conservar 16:9 y mostrar barras en relaciones distintas.
- [ ] No fusionar todavía esta rama.

---

## 7. Línea base funcional por migrar

El proyecto anterior contiene granja, casa, mercado, personaje, teclado/mouse/gamepad, hotbar, inventario, almacenamiento, doce parcelas, azada, regadera, semillas, nabo, zanahoria, col, tienda, economía, envíos, reloj, estaciones, sueño, fin de día y guardado JSON versionado. El detalle está en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

Datos visuales relevantes del prototipo:

- mundo lógico de escena: 960 × 540;
- personaje: frames de 64 × 72;
- parcelas de referencia: 48 px con separación de 58 px;
- colisión de jugador: radio de 12 px;
- cámara lógica centrada en 480, 270.

---

## 8. Próximo paso exacto

1. Detener Play Mode y cerrar Unity.
2. En GitHub Desktop, permanecer en `feature/lab-spatial-calibration`.
3. Ejecutar **Fetch origin** y **Pull origin**.
4. Abrir Unity `6000.3.21f1` y esperar la compilación.
5. Limpiar la consola.
6. Ejecutar EditMode: resultado esperado **4/4**.
7. Ejecutar PlayMode: resultado esperado **2/2**.
8. Abrir `Assets/_Project/Scenes/Bootstrap.unity` y pulsar Play.
9. Revisar la pestaña Game en `Free Aspect` y, de ser posible, en una resolución 16:9.
10. Enviar capturas del Test Runner, consola y nueva vista Game.
11. Tras validar, cerrar la calibración espacial y comenzar movimiento con teclado y DualSense.

---

## 9. Protocolo para un chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Antes de modificar archivos, lee `BITÁCORA_GPT.MD`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, revisa ramas y commits recientes, resume el estado y continúa desde el próximo paso documentado.
