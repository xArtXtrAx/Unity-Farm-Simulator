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

### Implementación remota inicial

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
- [x] El suelo de calibración es trigger y el perímetro utiliza `EdgeCollider2D` para contener al jugador.
- [x] Presentation referencia explícitamente `Unity.InputSystem`.
- [x] Nuevas pruebas puras de zona muerta, fuerza analógica, diagonal y orientación.
- [x] Nuevas pruebas PlayMode para composición Physics 2D y desplazamiento normalizado.

### Estado de validación

La implementación solo está validada estructuralmente en remoto. No afirmar compilación ni pruebas aprobadas hasta ejecutar Unity localmente.

Resultado esperado después de sincronizar:

- EditMode: **8/8**.
- PlayMode: **4/4**.
- Consola: **0 errores y 0 advertencias**.
- En `Bootstrap → Lab`, el rectángulo rojo debe moverse con WASD y flechas.
- El DualSense debe moverlo con stick izquierdo y D-pad.
- El marcador amarillo debe cambiar de lado según la dirección dominante.
- Las diagonales no deben sentirse más rápidas que los ejes rectos.
- El borde del laboratorio debe impedir salir del área visible.

El sprite real y Animator permanecen pendientes; primero se valida el motor y los dispositivos con la referencia provisional.

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

1. En GitHub Desktop, sincronizar `main` para recibir el merge y el registro `660c7f0`.
2. Cambiar a `feature/player-movement-foundation` y hacer Pull.
3. Abrir Unity `6000.3.21f1` y esperar la compilación.
4. Limpiar la consola.
5. Ejecutar EditMode: esperado **8/8**.
6. Ejecutar PlayMode: esperado **4/4**.
7. Abrir `Bootstrap.unity`, pulsar Play y probar WASD y flechas.
8. Conectar el DualSense y probar stick izquierdo y D-pad.
9. Comprobar zona muerta, velocidad diagonal, marcador de orientación y límites del mapa.
10. Enviar capturas de Test Runner, consola y Game, además de indicar si teclado y DualSense respondieron correctamente.

---

## 10. Protocolo para un chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Antes de modificar archivos, lee `BITÁCORA_GPT.MD`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, revisa ramas y commits recientes, resume el estado y continúa desde el próximo paso documentado.
