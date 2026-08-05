# BITÁCORA GPT — Unity Farm Simulator

Documento maestro de continuidad. Debe permitir retomar el proyecto en un chat nuevo sin depender de la memoria de conversaciones anteriores.

> **Regla de continuidad:** antes de modificar el repositorio, GPT debe leer este archivo, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, el estado de ramas y los commits recientes.

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
- **Ruta local de Arturo:** `D:\Git\Unity-Farm-Simulator`
- **Rama de cimentación concluida:** `chore/unity-project-foundation`
- **Proyecto fuente de migración:** `xArtXtrAx/farming-game-A`
- **Commit fuente congelado:** `dd32056c9f8142a2322bc2c1d41f0b05b002598f`
- **Identidad definitiva de Unity:**
  - Company Name: `jarturoga`
  - Product Name: `Unity Farm Simulator`
  - Standalone Identifier: `com.jarturoga.unityfarmsimulator`

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
- Evitar ZIP, manifiestos, hashes y comprobaciones Git repetitivas salvo necesidad técnica.
- Toda implementación de GPT debe actualizar esta bitácora antes de cerrar el bloque.

---

## 3. Arquitectura acordada

El código propio vive bajo `Assets/_Project` y no debe colocarse en la assembly global `Assembly-CSharp`.

### Assemblies

- `FarmSimulator.Domain`: reglas puras, sin `UnityEngine`.
- `FarmSimulator.Application`: casos de uso y coordinación; depende de Domain y tampoco usa `UnityEngine`.
- `FarmSimulator.Infrastructure`: persistencia y adaptadores; depende de Domain y Application.
- `FarmSimulator.Presentation`: escenas, `MonoBehaviour`, UI, input y representación visual.
- `FarmSimulator.Editor`: herramientas exclusivas del Editor.
- `FarmSimulator.Tests.EditMode`: reglas y configuración que no requieren Play Mode.
- `FarmSimulator.Tests.PlayMode`: integración con escenas y ciclo de vida de Unity.

### Datos y guardado

- Los `ScriptableObject` contendrán definiciones estáticas de objetos y cultivos.
- El progreso mutable de la partida se representará con DTO e identificadores de texto estables.
- Se preservará el enfoque versionado, defensivo y transaccional del guardado del proyecto fuente.

### Control de versiones de Unity

Confirmado:

- `Visible Meta Files`.
- `Force Text`.
- `.meta` incluidos siempre.
- `.gitignore` excluye cachés, builds y archivos regenerables del IDE.
- `.gitattributes` normaliza texto/YAML a LF y protege binarios.

---

## 4. Línea base funcional del proyecto anterior

La auditoría de `farming-game-A` confirmó:

- granja, casa y mercado norte;
- personaje, teclado, mouse y gamepad;
- hotbar de ocho espacios;
- inventario y almacenamiento;
- doce parcelas;
- azada, regadera y semillas;
- nabo, zanahoria y col de primavera;
- tienda, economía y caja de envíos;
- reloj, días, estaciones y años;
- sueño y procesamiento de fin de día;
- guardado JSON versionado con validación y rollback;
- colisiones, interacciones, transiciones y spawns;
- 806 pruebas documentadas al integrar los cultivos de primavera.

El plan detallado y los criterios de paridad están en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 5. Primera prueba vertical prevista

La primera versión jugable será pequeña y deberá incluir:

- escenas `Bootstrap` y `Lab`;
- personaje con DualSense y teclado;
- cámara, suelo y colisiones;
- una parcela;
- azada, regadera y semillas de nabo;
- cultivo con varias etapas;
- hotbar e interacción básica;
- avance de día de prueba;
- guardado mínimo.

---

## 6. Estado actual

### Fase 1 — Cimentación del proyecto: COMPLETADA Y VALIDADA

- [x] Proyecto Unity 6.3 LTS Universal 3D / URP creado en el repositorio.
- [x] `Visible Meta Files`, `Force Text`, `.gitignore` y `.gitattributes` configurados.
- [x] Recursos tutoriales eliminados.
- [x] Assemblies Domain, Application, Infrastructure, Presentation, Editor, EditMode y PlayMode creadas.
- [x] `Bootstrap` y `Lab` creadas y registradas en Build Settings.
- [x] Salto automático `Bootstrap → Lab` implementado.
- [x] Retirados Unity Version Control, Rider, Multiplayer Center y Visual Scripting.
- [x] `Packages/packages-lock.json` regenerado por Unity y versionado.
- [x] `ProjectSettings/SceneTemplateSettings.json` versionado.
- [x] Identidad definitiva aplicada y protegida mediante configurador y prueba automática.
- [x] EditMode: **2/2 pruebas aprobadas**.
- [x] PlayMode: **1/1 prueba aprobada**.
- [x] Consola final: **0 errores y 0 advertencias**.
- [x] Ruta de resultados confirmada bajo `jarturoga/Unity Farm Simulator`.
- [x] `ProjectSettings/ProjectSettings.asset` versionado en `bd17f89`.
- [x] No se detectaron bugs que requirieran entrada en `BUGS.MD`.

### Estado de integración

- [x] Rama `chore/unity-project-foundation` lista para integrarse en `main`.
- [ ] Abrir y fusionar el pull request de cimentación.
- [ ] Sincronizar `main` localmente después de la fusión.

### Próxima fase

**Fase 2 — Calibración visual, modelo espacial, cámara y movimiento base.**

Objetivos inmediatos:

- decidir mundo XZ con física 3D o XY con física 2D;
- decidir cámara ortográfica o perspectiva controlada;
- definir escala, resolución base y relación de aspecto;
- preparar `Lab` con suelo y referencias visuales;
- implementar movimiento con teclado y DualSense;
- validar zona muerta, velocidad, cámara y sensación de control.

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

---

## 8. Registro cronológico reciente

### 2026-08-05 — Proyecto y arquitectura

- Commit `ecf6eb3`: proyecto Unity inicial.
- Commit `3aa30f0`: arquitectura, assemblies e higiene del repositorio.
- Unity compiló las assemblies con consola limpia.

### 2026-08-05 — Escenas, arranque y paquetes

- Commit `d46b08c`: `Bootstrap`, `Lab`, arranque automático y pruebas.
- Commit `214e0fc`: lock de paquetes y configuración de plantillas resueltos localmente.
- Merge `fad8692`: reconciliación del commit local con la documentación remota.

### 2026-08-05 — Identidad definitiva

- Commit `ba6b62f`: identidad centralizada y configurador automático del Editor.
- Commit `bd17f89`: persistencia final de `ProjectSettings.asset` con la identidad correcta.
- EditMode terminó en **2/2**, PlayMode en **1/1** y la consola en **0 errores / 0 advertencias**.
- La Fase 1 se considera cerrada con evidencia visual y archivos versionados.

---

## 9. Próximo paso exacto

1. Abrir un pull request de `chore/unity-project-foundation` hacia `main`.
2. Revisar el diff y fusionar sin borrar todavía la rama remota hasta confirmar sincronización local.
3. En GitHub Desktop, cambiar a `main` y ejecutar **Fetch origin / Pull origin**.
4. Abrir Unity desde `main` y confirmar una última vez que la consola permanezca limpia.
5. Crear la siguiente rama: `feature/lab-spatial-calibration`.
6. Documentar y decidir el modelo espacial 2.5D, cámara, escala y resolución antes de implementar movimiento.

---

## 10. Protocolo para un chat nuevo

Usar una instrucción equivalente a:

> Continúa el desarrollo de `xArtXtrAx/Unity-Farm-Simulator`. Antes de modificar archivos, lee `BITÁCORA_GPT.MD`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, revisa ramas y commits recientes, resume el estado y continúa desde el próximo paso documentado.

---

## 11. Decisiones pendientes

- Resolución base y relación de aspecto.
- Cámara ortográfica o perspectiva controlada.
- Mundo XZ con física 3D o XY con física 2D.
- Tamaño de tiles, escala y Pixels Per Unit.
- Dirección artística y primer pack de sprites.
- Estrategia de CI para pruebas y build de Windows.
