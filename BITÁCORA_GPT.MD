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
- **Proyecto fuente de migración:** `xArtXtrAx/farming-game-A`
- **Commit fuente congelado:** `dd32056c9f8142a2322bc2c1d41f0b05b002598f`
- **Identidad Unity:**
  - Company Name: `jarturoga`
  - Product Name: `Unity Farm Simulator`
  - Standalone Identifier: `com.jarturoga.unityfarmsimulator`

---

## 2. Principios rectores

- Unity debe resolver jerarquía, inspector, escenas, prefabs, sprites, Tilemaps, snapping, colisiones, animación, física y depuración.
- Migrar comportamientos, reglas, datos e identificadores; no traducir Phaser/TypeScript línea por línea.
- `main` debe permanecer estable.
- Arturo usa principalmente GitHub Desktop.
- Todo código propio vive bajo `Assets/_Project` y fuera de `Assembly-CSharp`.
- Los `ScriptableObject` contendrán definiciones estáticas; el progreso mutable usará DTO e identificadores de texto estables.

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

### Contrato espacial vigente

- Plano lógico principal: **XY**.
- Profundidad y orden visual: **Z** y orden de renderizado.
- Cámara: **ortográfica frontal**, sin inclinación ni rotación isométrica.
- Física principal del mapa, jugador e interacciones: **Physics 2D**.
- Arte principal: sprites y Tilemaps 2D.
- Se permiten partículas, luces, objetos, trayectorias y efectos 3D cuando aporten valor.
- Los saltos y arcos pueden representarse mediante desplazamientos visuales, animación o efectos 3D sin convertir el mapa base en XZ.
- Celda provisional: `1` unidad de Unity.
- Referencia provisional de personaje: `1.8` unidades de alto y `0.8` de ancho.
- Cámara provisional: tamaño ortográfico `6.75`, posición Z `-10`.
- Pixels Per Unit, resolución base y relación de aspecto siguen pendientes de calibración artística.

### Decisión descartada

La primera calibración usó mundo XZ, física 3D y cámara ortográfica inclinada. Produjo una vista isométrica en rombo que no coincide con la dirección artística. Se conserva solo como aprendizaje histórico; ya no es el contrato del proyecto.

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

### Primer intento isométrico

- Commit `8ce5c5e`: laboratorio XZ con cámara inclinada, cuadrícula, referencias y pruebas.
- Commit `6078d67`: documentación del intento.
- Validación visual local: funcionó técnicamente, pero la vista resultó isométrica y no adecuada para el objetivo artístico.

### Corrección cenital 2D

- Commit funcional inicial: `edd6846` — `fix: switch calibration to top-down 2D`.
- Estado remoto reconciliado: `48adb0f`.
- Documentación alineada: `5b3dbda`.

Cambios implementados:

- [x] `SpatialModel` fija plano `XY`, profundidad `Z`, Physics 2D y cámara ortográfica frontal.
- [x] Se mantiene explícitamente la libertad de integrar efectos tridimensionales.
- [x] Cámara en `(0, 0, -10)` con rotación identidad.
- [x] Cuadrícula rectangular paralela a la pantalla.
- [x] Referencias cenitales simples: caminos, estanque, parcelas, árboles, sprites, pila de profundidad y trayectoria en arco.
- [x] Materiales planos y sin sombras.
- [x] EditMode protege el contrato espacial XY/Physics 2D.
- [x] PlayMode protege la cámara frontal, dimensiones, profundidad visual y collider del suelo.

### Validación local del estado `5b3dbda`

- [x] EditMode: **3/3 aprobadas**.
- [ ] PlayMode: **1/2 aprobada**.
- [x] Se identificó la prueba fallida: `LabBuildsFrontFacingOrthographicXYCalibration`.
- [x] Consola mostró conflicto entre `BoxCollider` 3D y `BoxCollider2D`.
- [x] Incidente registrado como `BUG-0001` en `BUGS.MD`.

### BUG-0001 — Corrección remota pendiente de verificación

Causa raíz:

- `GameObject.CreatePrimitive(PrimitiveType.Cube)` agrega un `BoxCollider` 3D.
- `Destroy(collider)` no lo elimina hasta el final del frame.
- El código intentaba agregar `BoxCollider2D` al mismo objeto antes de que el collider 3D desapareciera.

Corrección aplicada:

- Commit `cdb6cdd`: separa el objeto lógico `Calibration Ground` de su representación `Calibration Ground Visual`.
- `Calibration Ground` se crea como `GameObject` limpio y recibe directamente `BoxCollider2D`.
- La primitiva verde queda en un objeto visual separado y sus colliders 3D se eliminan normalmente.
- Commit `c8342a2`: refuerza PlayMode para verificar que no queden colliders 3D en los objetos visuales generados.
- Commit `a791641`: registra `BUG-0001` con causa, evidencia y solución.
- Estado del bug: **CORREGIDO**, no **VERIFICADO** hasta repetir pruebas localmente.

Pendiente:

- [ ] Sincronizar los commits posteriores a `5b3dbda`.
- [ ] Unity debe compilar sin errores.
- [ ] EditMode esperado: **3/3**.
- [ ] PlayMode esperado: **2/2**.
- [ ] La consola no debe volver a mostrar el conflicto de colliders.
- [ ] Abrir `Bootstrap`, entrar en Play y evaluar la vista cenital corregida.
- [ ] No fusionar todavía esta rama.

---

## 7. Línea base funcional por migrar

El proyecto anterior contiene granja, casa, mercado, personaje, teclado/mouse/gamepad, hotbar, inventario, almacenamiento, doce parcelas, azada, regadera, semillas, nabo, zanahoria, col, tienda, economía, envíos, reloj, estaciones, sueño, fin de día y guardado JSON versionado. El detalle está en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Próximo paso exacto

1. Detener Play Mode y cerrar Unity.
2. En GitHub Desktop, permanecer en `feature/lab-spatial-calibration`.
3. Ejecutar **Fetch origin** y **Pull origin**.
4. Abrir Unity `6000.3.21f1` y esperar compilación.
5. Limpiar la consola antes de volver a probar.
6. Ejecutar EditMode: resultado esperado **3/3**.
7. Ejecutar PlayMode: resultado esperado **2/2**.
8. Confirmar que ya no aparece el mensaje `Can't add component 'BoxCollider2D'...`.
9. Abrir `Assets/_Project/Scenes/Bootstrap.unity`, pulsar Play y enviar captura de la pestaña Game.
10. Tras validar la corrección, cambiar `BUG-0001` a **VERIFICADO** y continuar con resolución base, relación de aspecto y Pixels Per Unit.

---

## 9. Protocolo para un chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Antes de modificar archivos, lee `BITÁCORA_GPT.MD`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, revisa ramas y commits recientes, resume el estado y continúa desde el próximo paso documentado.
