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
- **Ruta local de Arturo:** `D:\Git\Unity-Farm-Simulator`
- **Rama de cimentación:** `chore/unity-project-foundation`
- **Proyecto fuente de migración:** `xArtXtrAx/farming-game-A`
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
- Evitar ZIP, manifiestos, hashes y comprobaciones Git repetitivas salvo necesidad técnica.
- Toda implementación de GPT debe actualizar esta bitácora antes de cerrar el bloque.

---

## 3. Arquitectura acordada

El código propio vive bajo `Assets/_Project` y no debe colocarse en la assembly global `Assembly-CSharp`.

### Assemblies

- `FarmSimulator.Domain`: reglas puras, sin `UnityEngine`.
- `FarmSimulator.Application`: casos de uso y coordinación; depende de Domain y tampoco usa `UnityEngine`.
- `FarmSimulator.Infrastructure`: persistencia y adaptadores; depende de Domain y Application.
- `FarmSimulator.Presentation`: escenas, `MonoBehaviour`, UI, input y representación visual; depende de Domain y Application.
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
- `.gitignore` excluye `Library`, `Temp`, `Obj`, `Logs`, `UserSettings`, builds, `.vscode`, `.sln`, `.slnx`, `.csproj` y archivos regenerables.
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

El objetivo es validar el flujo Unity ↔ GitHub ↔ Arturo ↔ GPT antes de migrar las tres escenas completas.

---

## 6. Estado actual

### Completado

- [x] Decisión de migrar el desarrollo principal a Unity.
- [x] Creación de `BITÁCORA_GPT.MD`, `BUGS.MD` y `MIGRACION_DESDE_FARMING_GAME_A.MD`.
- [x] Auditoría del repositorio fuente y fijación de `dd32056` como línea base.
- [x] Instalación de Unity 6.3 LTS `6000.3.21f1`.
- [x] Clonación en `D:\Git\Unity-Farm-Simulator`.
- [x] Creación del proyecto Universal 3D / URP dentro del repositorio.
- [x] Primera importación con consola en `0` errores, `0` advertencias y `0` mensajes.
- [x] Configuración `Visible Meta Files` y `Force Text`.
- [x] Eliminación de recursos tutoriales de la plantilla.
- [x] Publicación de `chore/unity-project-foundation`.
- [x] Commit `ecf6eb3`: proyecto Unity 6.3 URP inicial.
- [x] Commit `3aa30f0`: assemblies, estructura `_Project`, `.gitattributes` e higiene del repositorio.
- [x] Validación local de las assemblies con consola completamente limpia.
- [x] Auditoría inicial de paquetes de plantilla.
- [x] Creación remota de `Bootstrap` y `Lab`.
- [x] Registro de ambas escenas en Build Settings, con `Bootstrap` primero.
- [x] Implementación del salto automático `Bootstrap` → `Lab`.
- [x] Añadidas pruebas Edit Mode y Play Mode para la cimentación.

### Pendiente de validación local

- [ ] Unity debe resolver el nuevo `Packages/manifest.json`.
- [ ] `Packages/packages-lock.json` debe regenerarse localmente y después versionarse.
- [ ] Confirmar compilación sin errores tras retirar paquetes.
- [ ] Ejecutar pruebas Edit Mode.
- [ ] Ejecutar pruebas Play Mode y confirmar el salto `Bootstrap` → `Lab`.

### En curso

- [ ] Cierre formal de la Fase 1.
- [ ] Confirmación de módulos instalados junto al editor.
- [ ] Preparación de la calibración visual, cámara, movimiento y DualSense.

---

## 7. Paquetes

### Conservados deliberadamente

- `com.unity.render-pipelines.universal`: render 2.5D con URP.
- `com.unity.inputsystem`: teclado, mouse y DualSense.
- `com.unity.test-framework`: Edit Mode y Play Mode.
- `com.unity.ai.navigation`: futura navegación de NPC.
- `com.unity.timeline`: secuencias y eventos futuros.
- `com.unity.ide.visualstudio`: integración con el IDE instalado.
- `com.unity.ugui`: UI de runtime.

### Retirados del manifest

- `com.unity.collab-proxy`: usamos GitHub, no Unity Version Control.
- `com.unity.ide.rider`: el entorno elegido es Visual Studio.
- `com.unity.multiplayer.center`: el proyecto no es multijugador.
- `com.unity.visualscripting`: la lógica se desarrollará en C# versionable y comprobable.

`packages-lock.json` no se editó manualmente en GitHub para evitar fabricar un lock no resuelto por Unity. El Editor debe regenerarlo en la próxima apertura.

---

## 8. Registro cronológico

### 2026-08-05 — Inicio y planificación

- Se verificó acceso de lectura y escritura al repositorio.
- Se establecieron GitHub como memoria compartida y Unity como intermediario visual.
- Se creó la documentación de continuidad y bugs.
- Se auditó `farming-game-A` y se creó el plan de migración.
- Se hizo obligatoria la actualización de esta bitácora después de cada implementación.

### 2026-08-05 — Proyecto Unity inicial

- Arturo instaló Unity `6000.3.21f1` y clonó el repositorio.
- Se creó un proyecto Universal 3D / URP sin AI Assistant ni proveedor adicional de control de código fuente.
- Se configuraron `Visible Meta Files`, `Force Text` y `.gitignore`.
- La importación inicial finalizó con consola limpia.
- Se publicó `chore/unity-project-foundation` con `ecf6eb3`.

### 2026-08-05 — Cimentación arquitectónica

- GPT creó `3aa30f0` con las assemblies Domain, Application, Infrastructure, Presentation, Editor, EditMode y PlayMode.
- Se añadieron `.gitattributes`, reglas de ubicación del código y exclusión de archivos del IDE.
- Arturo sincronizó y confirmó que Unity compiló todo con `0` errores, `0` advertencias y `0` mensajes.

### 2026-08-05 — Escenas, arranque y auditoría de paquetes

- GPT creó el commit `d46b08cb9a9c15bb078ba2450218078d8e9d80c5` — `feat: add Bootstrap and Lab project foundation`.
- `SampleScene` fue reemplazada por:
  - `Assets/_Project/Scenes/Bootstrap.unity`;
  - `Assets/_Project/Scenes/Lab.unity`.
- `EditorBuildSettings.asset` registra `Bootstrap` como escena 0 y `Lab` como escena 1.
- Se añadió `ProjectSceneNames.cs` para centralizar nombres y rutas.
- Se añadió `BootstrapStartup.cs`, que instala un listener de escenas y carga `Lab` cuando se abre `Bootstrap`.
- La inicialización restablece correctamente el estado estático incluso con las opciones rápidas de Play Mode.
- Se añadió `ProjectFoundationTests.cs` para comprobar el orden de Build Settings.
- Se añadió `BootstrapStartupTests.cs` para comprobar la transición en Play Mode.
- Se retiraron del manifest Unity Version Control, Rider, Multiplayer Center y Visual Scripting.
- Validación remota realizada: commit creado y rama actualizada correctamente.
- Validación local pendiente: no afirmar que compila o que las pruebas pasan hasta que Arturo lo confirme desde Unity.
- No se abrió entrada en `BUGS.MD` porque aún no existe un fallo observado; cualquier error de compilación, paquete o prueba se registrará tras la validación.

---

## 9. Próximo paso exacto

1. Cerrar Unity antes de sincronizar.
2. En GitHub Desktop, sobre `chore/unity-project-foundation`, ejecutar **Fetch origin** y **Pull origin**.
3. Abrir el proyecto con Unity `6000.3.21f1`.
4. Esperar a que Package Manager termine de resolver y retirar paquetes.
5. Abrir `Console` y confirmar los contadores.
6. Abrir `Window → General → Test Runner`.
7. Ejecutar primero **EditMode** y después **PlayMode**.
8. Enviar captura de la consola y resultados de pruebas.
9. Revisar GitHub Desktop: debe aparecer modificado `Packages/packages-lock.json`; no mezclar otros archivos inesperados.
10. Tras esa validación, versionar el lock regenerado y cerrar formalmente la Fase 1.

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
