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
- **Rama activa:** `feature/player-movement-foundation`
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
- `BUG-0001` verificado y documentado en `BUGS.MD`.

Validación local final de Arturo:

- EditMode: **4/4 aprobadas**.
- PlayMode: **2/2 aprobadas**.
- Consola: **0 errores y 0 advertencias**.
- Vista y escala: aprobadas.

Integración:

- PR #2: `Calibrate top-down pixel-art presentation`.
- Merge commit: `7ac3f91ddba1f1e6bd8136e22d6ed0a70d19fce8`.
- Registro posterior en `main`: `660c7f0`.

---

## 7. Fase 3 — Movimiento del jugador

### Implementación inicial

Commit funcional: `e0be2afa9681e56a987725385affb8c5e96efdf7` — `feat: add keyboard and gamepad player movement foundation`.

Implementado:

- [x] `PlayerMovementModel.cs` en Application, sin dependencia de UnityEngine.
- [x] Zona muerta radial inicial `0.18`.
- [x] Magnitud analógica remapeada después de la zona muerta.
- [x] Normalización de entradas digitales diagonales para evitar mayor velocidad.
- [x] Resolución de cuatro direcciones visuales: arriba, abajo, izquierda y derecha.
- [x] `TopDownPlayerMotor` separado de la lectura de input.
- [x] Movimiento mediante `Rigidbody2D.MovePosition` en `FixedUpdate`.
- [x] `CapsuleCollider2D` de pies, gravedad cero, rotación bloqueada e interpolación.
- [x] `UnifiedMovementInput` admite WASD, flechas, stick izquierdo y D-pad.
- [x] Se elige la fuente activa con mayor magnitud entre teclado y gamepad.
- [x] El primer gamepad conectado funciona aunque todavía no sea `Gamepad.current`.
- [x] `PlayerProxyFacingView` muestra la orientación mediante un marcador provisional.
- [x] `Lab` genera un personaje rojo controlable con marcador amarillo.
- [x] El suelo es trigger y el perímetro utiliza `EdgeCollider2D` para contener al jugador.
- [x] Presentation referencia explícitamente `Unity.InputSystem`.
- [x] Pruebas puras de zona muerta, fuerza analógica, diagonal y orientación.
- [x] Pruebas PlayMode para composición Physics 2D y desplazamiento normalizado.

### Validación local confirmada por Arturo

Estado `724152e`:

- [x] EditMode: **11/11 aprobadas**.
- [x] PlayMode: **4/4 aprobadas**.
- [x] Consola: **0 errores y 0 advertencias**.
- [x] WASD y flechas mueven correctamente al jugador.
- [x] Stick izquierdo y D-pad del DualSense funcionan correctamente.
- [x] El marcador cambia de acuerdo con la orientación.
- [x] Las pruebas automatizadas protegen zona muerta, diagonales, orientación, Physics 2D y movimiento.

### BUG-0002 — Marcador de orientación descentrado

Arturo observó que el marcador lateral amarillo cambiaba correctamente de dirección, pero estaba alineado con el pivote de los pies en vez de con el centro del cuerpo rojo.

Corrección implementada:

- Commit `dd7357c` — `fix: center player facing marker`.
- Reconciliación de código y documentación: `40ba7e7`.
- Las posiciones ahora se calculan desde el ancho y alto reales del personaje.
- Izquierda y derecha se centran verticalmente sobre el cuerpo.
- Arriba queda sobre la cabeza y abajo bajo los pies.
- La prueba PlayMode existente incorpora comprobaciones de alineación lateral.
- Registro completo en `BUGS.MD` como `BUG-0002`.

Estado: **CORREGIDO**, pendiente de validación local para pasar a **VERIFICADO**.

El sprite real y Animator permanecen pendientes; primero debe verificarse el marcador corregido sobre la referencia provisional.

---

## 8. Línea base funcional por migrar

El proyecto anterior contiene granja, casa, mercado, personaje, teclado/mouse/gamepad, hotbar, inventario, almacenamiento, doce parcelas, herramientas, tres cultivos de primavera, tienda, economía, envíos, reloj, estaciones, sueño, fin de día y guardado JSON versionado. El detalle está en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

Datos relevantes:

- mundo lógico: 960 × 540;
- personaje: 64 × 72 px;
- colisión de referencia: radio de 12 px;
- movimiento analógico y DualSense ya existían en el prototipo.

---

## 9. Próximo paso exacto

1. Detener Play Mode y cerrar Unity.
2. En GitHub Desktop, permanecer en `feature/player-movement-foundation`.
3. Ejecutar **Fetch origin** y **Pull origin**.
4. Abrir Unity `6000.3.21f1` y esperar la compilación.
5. Ejecutar EditMode: esperado **11/11**.
6. Ejecutar PlayMode: esperado **4/4**.
7. Confirmar consola limpia.
8. Ejecutar `Bootstrap → Lab` y orientar al jugador en las cuatro direcciones.
9. Confirmar que los marcadores laterales están centrados verticalmente respecto al cuerpo rojo, el superior está sobre la cabeza y el inferior bajo los pies.
10. Tras validar, marcar `BUG-0002` como **VERIFICADO** y comenzar la importación del sprite real y su Animator.

---

## 10. Protocolo para un chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Antes de modificar archivos, lee `BITÁCORA_GPT.MD`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, revisa ramas y commits recientes, resume el estado y continúa desde el próximo paso documentado.