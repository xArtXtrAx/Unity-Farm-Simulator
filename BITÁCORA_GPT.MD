# BITÁCORA GPT — Unity Farm Simulator

Documento maestro de continuidad. Debe permitir retomar el proyecto en un chat nuevo sin depender de la memoria de conversaciones anteriores.

> **Regla de continuidad:** antes de modificar el repositorio, GPT debe leer este archivo, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, el estado de las ramas y los commits recientes.

> **Regla transaccional obligatoria:** ninguna implementación o corrección realizada por GPT se considera terminada hasta actualizar esta bitácora con lo realizado, lo comprobado, lo pendiente y el siguiente paso exacto.

---

## 1. Identidad y entorno

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Propietario:** Arturo García (`xArtXtrAx`)
- **Tipo:** simulador de granja 2.5D
- **Motor fijado:** Unity 6.3 LTS `6000.3.21f1`
- **Plantilla:** Universal 3D / URP
- **Plataforma inicial:** Windows PC
- **Control prioritario:** DualSense, además de teclado y mouse
- **Ruta local:** `D:\Git\Unity-Farm-Simulator`
- **Rama activa:** `chore/unity-project-foundation`
- **Proyecto fuente:** `xArtXtrAx/farming-game-A`
- **Commit fuente congelado:** `dd32056c9f8142a2322bc2c1d41f0b05b002598f`

---

## 2. Principios rectores

### Unity como intermediario

No construir un editor de videojuegos desde cero. Unity debe resolver jerarquía, inspector, escenas, prefabs, sprites, cuadrículas, snapping, colisiones, animación, física y depuración. Solo se crearán herramientas internas específicas del simulador cuando aporten valor real.

### Migración

Migrar comportamientos, reglas, datos, identificadores y criterios de aceptación. No traducir TypeScript/Phaser a C# línea por línea ni recrear en Unity las herramientas genéricas del editor web.

### Git y continuidad

- Arturo usa principalmente GitHub Desktop.
- `main` debe permanecer estable y jugable.
- Las mejoras relevantes se desarrollan en ramas descriptivas.
- Evitar ZIP y comprobaciones Git repetitivas salvo necesidad técnica.
- Toda implementación de GPT debe actualizar esta bitácora antes de cerrar el bloque.

---

## 3. Arquitectura acordada

El código propio vive bajo `Assets/_Project`; no colocar código nuevo en `Assembly-CSharp`.

### Assemblies

- `FarmSimulator.Domain`: reglas puras, sin `UnityEngine`.
- `FarmSimulator.Application`: casos de uso; depende de Domain y tampoco usa `UnityEngine`.
- `FarmSimulator.Infrastructure`: persistencia y adaptadores.
- `FarmSimulator.Presentation`: escenas, `MonoBehaviour`, UI, input y representación visual.
- `FarmSimulator.Editor`: herramientas exclusivas del Editor.
- `FarmSimulator.Tests.EditMode`: reglas y configuración.
- `FarmSimulator.Tests.PlayMode`: integración con escenas y ciclo de vida.

### Datos y guardado

- `ScriptableObject` para definiciones estáticas.
- DTO e IDs de texto estables para progreso mutable.
- Preservar el guardado versionado, defensivo y transaccional del proyecto fuente.

### Control de versiones de Unity

Confirmado:

- `Visible Meta Files`.
- `Force Text`.
- `.meta` incluidos siempre.
- `.gitignore` excluye cachés, builds y archivos regenerables de IDE.
- `.gitattributes` normaliza texto/YAML a LF y protege binarios.

---

## 4. Línea base funcional anterior

`farming-game-A` contiene granja, casa, mercado, personaje, teclado/mouse/gamepad, hotbar, inventario, almacenamiento, doce parcelas, herramientas, tres cultivos, tienda, economía, envíos, calendario, sueño, guardado versionado, colisiones, transiciones y 806 pruebas documentadas.

El plan detallado está en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 5. Primera prueba vertical prevista

- `Bootstrap` y `Lab`.
- Personaje con DualSense y teclado.
- Cámara, suelo y colisiones.
- Una parcela.
- Azada, regadera y semillas de nabo.
- Cultivo con varias etapas.
- Hotbar e interacción básica.
- Avance de día de prueba.
- Guardado mínimo.

---

## 6. Estado actual

### Completado

- [x] Proyecto Unity 6.3 LTS Universal 3D / URP creado en el repositorio.
- [x] Importación inicial con consola limpia.
- [x] `Visible Meta Files`, `Force Text`, `.gitignore` y `.gitattributes` configurados.
- [x] Recursos tutoriales eliminados.
- [x] Assemblies Domain, Application, Infrastructure, Presentation, Editor, EditMode y PlayMode creadas y validadas.
- [x] `Bootstrap` y `Lab` creadas.
- [x] `Bootstrap` registrada como escena 0 y `Lab` como escena 1.
- [x] Salto automático `Bootstrap → Lab` implementado.
- [x] Auditoría de paquetes de plantilla realizada.
- [x] Retirados Unity Version Control, Rider, Multiplayer Center y Visual Scripting.
- [x] Unity resolvió el nuevo manifest sin errores ni advertencias.
- [x] Prueba EditMode ejecutada: **1/1 aprobada**.
- [x] Prueba PlayMode ejecutada: **1/1 aprobada**.
- [x] Consola posterior a pruebas: **0 errores y 0 advertencias**.

### Pendiente para cerrar formalmente la Fase 1

- [ ] Revisar en GitHub Desktop el `Packages/packages-lock.json` regenerado por Unity.
- [ ] Versionar el lock resuelto sin mezclar archivos inesperados.
- [ ] Sustituir los nombres heredados `DefaultCompany` y `UnityFarmSimulatorTemp` por valores definitivos.
- [ ] Volver a abrir Unity y confirmar consola limpia tras esos cambios.
- [ ] Fusionar la rama de cimentación en `main` una vez validada.

### Después de la Fase 1

- [ ] Definir modelo espacial 2.5D, cámara y escala.
- [ ] Crear calibración visual de `Lab`.
- [ ] Implementar movimiento con teclado y DualSense.

---

## 7. Paquetes

### Conservados

- `com.unity.render-pipelines.universal`
- `com.unity.inputsystem`
- `com.unity.test-framework`
- `com.unity.ai.navigation`
- `com.unity.timeline`
- `com.unity.ide.visualstudio`
- `com.unity.ugui`

### Retirados

- `com.unity.collab-proxy`
- `com.unity.ide.rider`
- `com.unity.multiplayer.center`
- `com.unity.visualscripting`

`packages-lock.json` debe provenir de la resolución local de Unity, no fabricarse manualmente.

---

## 8. Registro cronológico

### 2026-08-05 — Inicio y planificación

- Se creó la documentación de continuidad y bugs.
- Se auditó `farming-game-A` y se fijó `dd32056` como base.
- Se acordó usar Unity como intermediario y actualizar esta bitácora tras cada implementación.

### 2026-08-05 — Proyecto y arquitectura

- Arturo instaló Unity `6000.3.21f1`, clonó el repo y creó la base URP.
- Commit `ecf6eb3`: proyecto Unity inicial.
- Commit `3aa30f0`: arquitectura, assemblies e higiene del repositorio.
- Unity compiló las assemblies con consola limpia.

### 2026-08-05 — Escenas, arranque y paquetes

- Commit `d46b08c`: `Bootstrap`, `Lab`, arranque automático y pruebas.
- Se retiraron cuatro paquetes innecesarios del manifest.
- Commit `8dc4676`: bitácora del bloque y validación local pendiente.

### 2026-08-05 — Pruebas de cimentación aprobadas

- Arturo ejecutó el Test Runner tras resolver los paquetes.
- EditMode descubrió y aprobó `FarmSimulator` — **1 prueba aprobada, 0 fallidas**.
- PlayMode descubrió y aprobó `FarmSimulator` — **1 prueba aprobada, 0 fallidas**.
- La transición `Bootstrap → Lab` quedó comprobada por la prueba PlayMode.
- La consola mostró `0` errores y `0` advertencias; los mensajes blancos eran informativos del Test Runner.
- No se detectó un bug funcional y no fue necesario modificar `BUGS.MD`.
- La ruta `DefaultCompany/UnityFarmSimulatorTemp` observada en los resultados proviene de Player Settings heredados de la plantilla, no de haber abierto el proyecto temporal.

---

## 9. Próximo paso exacto

1. Mantener Unity abierto o cerrarlo sin guardar cambios manuales adicionales.
2. Abrir GitHub Desktop en `chore/unity-project-foundation`.
3. Mostrar la lista completa de cambios locales, especialmente `Packages/packages-lock.json`.
4. No hacer commit todavía.
5. GPT revisará esos cambios, corregirá los nombres definitivos del proyecto y preparará el cierre de la Fase 1.

---

## 10. Protocolo para un chat nuevo

> Continúa el desarrollo de `xArtXtrAx/Unity-Farm-Simulator`. Lee `BITÁCORA_GPT.MD`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, revisa ramas y commits recientes, resume el estado y continúa desde el próximo paso documentado.

---

## 11. Decisiones pendientes

- Nombre definitivo de estudio/empresa mostrado por Unity.
- Resolución base y relación de aspecto.
- Cámara ortográfica o perspectiva controlada.
- Mundo XZ con física 3D o XY con física 2D.
- Tamaño de tiles, escala y Pixels Per Unit.
- Dirección artística y primer pack de sprites.
- Estrategia de CI para pruebas y build de Windows.
