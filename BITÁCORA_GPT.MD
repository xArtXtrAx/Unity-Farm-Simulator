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
- **Rama de cimentación:** `chore/unity-project-foundation`
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

### Completado y validado localmente

- [x] Proyecto Unity 6.3 LTS Universal 3D / URP creado en el repositorio.
- [x] Importación inicial con consola limpia.
- [x] `Visible Meta Files`, `Force Text`, `.gitignore` y `.gitattributes` configurados.
- [x] Recursos tutoriales eliminados.
- [x] Assemblies Domain, Application, Infrastructure, Presentation, Editor, EditMode y PlayMode creadas.
- [x] Assemblies validadas con `0` errores, `0` advertencias y `0` mensajes.
- [x] `Bootstrap` y `Lab` creadas.
- [x] `Bootstrap` registrada como escena 0 y `Lab` como escena 1.
- [x] Salto automático `Bootstrap → Lab` implementado.
- [x] Retirados Unity Version Control, Rider, Multiplayer Center y Visual Scripting.
- [x] Manifest resuelto localmente por Unity.
- [x] `Packages/packages-lock.json` regenerado y versionado en `214e0fc`.
- [x] `ProjectSettings/SceneTemplateSettings.json` generado y versionado.
- [x] Prueba EditMode: **1/1 aprobada**.
- [x] Prueba PlayMode: **1/1 aprobada**.
- [x] Consola posterior a pruebas: **0 errores y 0 advertencias**.
- [x] Commit local y documentación remota reconciliados mediante merge `fad8692`.

### Implementado remotamente; pendiente de validación local

- [x] `ProjectIdentity.cs` centraliza Company Name, Product Name e identificador Standalone.
- [x] `ProjectIdentityConfigurator.cs` aplica automáticamente esa identidad al abrir/importar el proyecto.
- [x] Menú manual añadido: `Tools → Farm Simulator → Validate Project Identity`.
- [x] Segunda prueba EditMode añadida para verificar los tres valores.
- [ ] Unity debe compilar el commit `ba6b62f` localmente.
- [ ] Al abrir el proyecto, `ProjectSettings/ProjectSettings.asset` debe actualizarse automáticamente.
- [ ] EditMode debe mostrar **2/2 pruebas aprobadas**.
- [ ] PlayMode debe conservar **1/1 prueba aprobada**.
- [ ] La consola debe permanecer sin errores ni advertencias.
- [ ] Los cambios locales de `ProjectSettings.asset` deben versionarse antes de fusionar.

### Después de cerrar la Fase 1

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

---

## 8. Registro cronológico reciente

### 2026-08-05 — Proyecto y arquitectura

- Commit `ecf6eb3`: proyecto Unity inicial.
- Commit `3aa30f0`: arquitectura, assemblies e higiene del repositorio.
- Unity compiló las assemblies con consola limpia.

### 2026-08-05 — Escenas, arranque y paquetes

- Commit `d46b08c`: `Bootstrap`, `Lab`, arranque automático y pruebas.
- Commit `8dc4676`: registro documental del bloque.
- EditMode y PlayMode fueron ejecutados y aprobados.
- Commit `214e0fc`: lock de paquetes y configuración de plantillas resueltos localmente.
- Merge `fad8692`: reconciliación del commit local con la documentación remota.

### 2026-08-05 — Identidad definitiva automatizada

- Commit `ba6b62f`: identidad centralizada y configurador automático del Editor.
- Se evitó editar manualmente el YAML completo de `ProjectSettings.asset` para reducir riesgo de corrupción o pérdida de campos.
- La identidad se aplicará mediante API nativa de Unity en la próxima apertura.
- La prueba EditMode ahora debe descubrir dos pruebas: Build Settings e identidad.
- Validación remota realizada: árbol, commit y referencia de rama actualizados correctamente.
- Validación local pendiente; no afirmar que compila ni que la identidad quedó persistida hasta recibir la comprobación de Arturo.
- No se añadió entrada en `BUGS.MD`, porque no se observó un fallo: los nombres temporales eran configuración heredada de la plantilla.

---

## 9. Próximo paso exacto

1. Cerrar Unity antes de sincronizar.
2. En GitHub Desktop, sobre `chore/unity-project-foundation`, ejecutar **Fetch origin** y **Pull origin**.
3. Abrir el proyecto con Unity `6000.3.21f1` y esperar compilación/importación.
4. Revisar consola: debe mostrar `0` errores y `0` advertencias.
5. Abrir `Window → General → Test Runner`.
6. Ejecutar EditMode: resultado esperado **2/2**.
7. Ejecutar PlayMode: resultado esperado **1/1**.
8. Revisar GitHub Desktop. Se espera una modificación en `ProjectSettings/ProjectSettings.asset` con los nombres definitivos.
9. No hacer commit todavía; enviar capturas de pruebas y cambios para revisión.
10. Tras versionar `ProjectSettings.asset`, cerrar formalmente la Fase 1 y fusionar `chore/unity-project-foundation` en `main`.

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
