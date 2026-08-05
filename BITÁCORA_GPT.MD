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
- **Rama activa siguiente:** `feature/player-movement-foundation`
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
- Cámara: **ortográfica frontal**, sin inclinación ni rotación isométrica.
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
- Cámara: Z `-10`.
- Relaciones diferentes conservan 16:9 mediante letterbox o pillarbox.

La primera calibración XZ/isométrica fue descartada porque no coincide con la estética deseada.

---

## 5. Fase 1 — Cimentación: COMPLETADA E INTEGRADA

- Proyecto Unity 6.3 LTS / URP creado.
- Arquitectura por assemblies.
- Escenas `Bootstrap` y `Lab`.
- Transición automática `Bootstrap → Lab`.
- Paquetes innecesarios retirados.
- Identidad definitiva aplicada y protegida.
- Validación: EditMode **2/2**, PlayMode **1/1**, consola limpia.
- PR #1 fusionado mediante `ece618c`.

---

## 6. Fase 2 — Calibración visual, espacial y de pantalla: COMPLETADA E INTEGRADA

### Implementación

- Laboratorio cenital XY con cámara frontal y Physics 2D.
- Cuadrícula rectangular, caminos, parcelas, estanque, árboles, referencias de sprites, profundidad y arco visual.
- Resolución lógica 960 × 540, 16:9 y 64 PPU.
- `ReferenceAspectCamera` para letterbox/pillarbox.
- Pruebas EditMode y PlayMode para contrato espacial, escala, cámara y colliders.

### Incidente resuelto

`BUG-0001` — conflicto entre `BoxCollider` 3D y `BoxCollider2D`:

- causa: `GameObject.CreatePrimitive` añadía un collider 3D que `Destroy` retiraba al final del frame;
- corrección: separar el suelo lógico Physics 2D de su representación visual;
- estado: **VERIFICADO**;
- registro completo: `BUGS.MD`.

### Validación local final de Arturo

- EditMode: **4/4 aprobadas**.
- PlayMode: **2/2 aprobadas**.
- Consola: **0 errores y 0 advertencias**.
- `Bootstrap → Lab`: aprobado.
- Vista cenital: aprobada.
- Escala 960 × 540 / 64 PPU: aprobada visualmente.
- La cámara preserva el marco 16:9 en la pestaña Game.

### Integración

- PR #2: `Calibrate top-down pixel-art presentation`.
- Merge commit: `7ac3f91ddba1f1e6bd8136e22d6ed0a70d19fce8`.
- Fecha: 2026-08-05.

---

## 7. Fase 3 — Movimiento del jugador: SIGUIENTE

Objetivo del primer bloque:

- personaje provisional controlable dentro de `Lab`;
- movimiento analógico en ocho direcciones;
- cuatro direcciones visuales;
- teclado: WASD y flechas;
- DualSense/gamepad: stick izquierdo y D-pad;
- zona muerta inicial `0.18`;
- velocidad uniforme, sin ventaja diagonal;
- `Rigidbody2D` y collider 2D;
- separación entre lectura de input, procesamiento y motor físico;
- pruebas puras y PlayMode.

El sprite real y Animator se incorporarán después de validar el motor y los controles con la referencia provisional.

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

1. Crear `feature/player-movement-foundation` desde `main` después de `7ac3f91`.
2. Implementar procesamiento analógico puro con zona muerta `0.18` y normalización diagonal.
3. Implementar motor `Rigidbody2D` separado de la lectura de input.
4. Admitir WASD, flechas, stick izquierdo y D-pad.
5. Generar un personaje provisional controlable en `Lab`.
6. Añadir pruebas EditMode y PlayMode.
7. Validar en Unity con teclado y DualSense antes de importar el sprite definitivo.

---

## 10. Protocolo para un chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Antes de modificar archivos, lee `BITÁCORA_GPT.MD`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, revisa ramas y commits recientes, resume el estado y continúa desde el próximo paso documentado.
