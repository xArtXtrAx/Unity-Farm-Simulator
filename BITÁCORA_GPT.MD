# BITÁCORA GPT — Unity Farm Simulator

Documento maestro de continuidad. Debe permitir retomar el proyecto en un chat nuevo sin depender de la memoria de conversaciones anteriores.

> **Regla de continuidad:** antes de modificar el repositorio, GPT debe leer este archivo, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, el estado de ramas y los commits recientes.

> **Regla transaccional obligatoria:** ninguna implementación o corrección realizada por GPT se considera terminada hasta actualizar esta bitácora con lo realizado, lo comprobado, lo pendiente y el siguiente paso exacto.

---

## 1. Identidad y entorno

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Propietario:** Arturo García (`xArtXtrAx`)
- **Tipo:** simulador de granja 2.5D con arte principal basado en sprites 2D
- **Motor fijado:** Unity 6.3 LTS `6000.3.21f1`
- **Plantilla:** Universal 3D / URP
- **Plataforma inicial:** Windows PC
- **Control prioritario:** DualSense, además de teclado y mouse
- **Ruta local de Arturo:** `D:\Git\Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Rama activa:** `feature/lab-spatial-calibration`
- **Proyecto fuente de migración:** `xArtXtrAx/farming-game-A`
- **Commit fuente congelado:** `dd32056c9f8142a2322bc2c1d41f0b05b002598f`
- **Identidad de Unity:**
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

### Modelo espacial 2.5D decidido

- El mundo lógico utiliza el plano **XZ** para desplazamiento sobre el suelo.
- El eje **Y** representa altura real.
- La física principal será **3D**.
- La cámara base será **ortográfica e inclinada**.
- El arte principal seguirá siendo 2D mediante sprites o tarjetas planas dentro del espacio 3D.
- El espacio 3D permitirá partículas, iluminación, sombras, trayectorias en arco, saltos y objetos tridimensionales ocasionales.
- Una celda de calibración equivale provisionalmente a **1 unidad de Unity**.
- La referencia humana provisional mide **1.8 unidades**.
- Resolución, relación de aspecto, Pixels Per Unit y escala artística definitiva continúan pendientes de evaluación visual.

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

### Fase 1 — Cimentación: COMPLETADA, VALIDADA E INTEGRADA

- [x] Proyecto Unity 6.3 LTS / URP creado y protegido para Git.
- [x] Arquitectura por assemblies creada.
- [x] `Bootstrap` y `Lab` creadas y conectadas.
- [x] Paquetes de plantilla innecesarios retirados.
- [x] Identidad definitiva aplicada y protegida.
- [x] EditMode: **2/2 aprobadas**.
- [x] PlayMode: **1/1 aprobada**.
- [x] Consola: **0 errores y 0 advertencias**.
- [x] PR #1 fusionado en `main` mediante `ece618c`.

### Fase 2 — Calibración espacial: IMPLEMENTACIÓN INICIAL EN RAMA

Commit funcional: `8ce5c5e` — `feat: add 2.5D spatial calibration lab`.

Implementado:

- [x] `SpatialModel.cs` fija XZ, altura Y, física 3D, cámara ortográfica y medidas provisionales.
- [x] `LabSpatialCalibrationInstaller.cs` instala automáticamente la calibración cuando se carga `Lab`.
- [x] `LabSpatialCalibration.cs` genera durante la ejecución:
  - suelo de 16 × 12 unidades con `BoxCollider`;
  - cuadrícula de celdas de 1 unidad;
  - ejes X y Z diferenciados;
  - tarjeta plana de personaje de 1.8 unidades;
  - tarjetas a distintas profundidades;
  - regla vertical sobre Y;
  - trayectoria parabólica para representar movimientos y partículas 3D;
  - cámara ortográfica inclinada y luz direccional con sombras.
- [x] La calibración se genera por código al entrar en Play Mode; no ensucia la escena con decenas de objetos serializados.
- [x] La prueba EditMode ahora valida también el contrato espacial.
- [x] La prueba PlayMode ahora valida cámara, suelo, collider, dimensiones y referencia de sprite.

Pendiente de validación local:

- [ ] Unity debe compilar el commit `8ce5c5e` sin errores.
- [ ] Resultado esperado de EditMode: **3/3**.
- [ ] Resultado esperado de PlayMode: **2/2**.
- [ ] Consola esperada: **0 errores y 0 advertencias**.
- [ ] Ejecutar el juego desde `Bootstrap` y observar la escena `Lab` en la pestaña Game.
- [ ] Evaluar si encuadre, inclinación, tamaño de celda y referencia humana resultan adecuados.
- [ ] No se añadió bug a `BUGS.MD`; todavía no existe evidencia local de un fallo.

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

### 2026-08-05 — Fase 1 integrada

- PR #1 fusionado en `main` mediante `ece618c`.
- Commit `141c51e`: bitácora actualizada después del merge.
- Se creó `feature/lab-spatial-calibration` desde la base integrada.

### 2026-08-05 — Modelo espacial definido

- Arturo aclaró que el juego utilizará principalmente sprites 2D, complementados con partículas, iluminación y movimientos tridimensionales.
- Se decidió usar mundo XZ, altura Y, física 3D y cámara ortográfica inclinada.
- La estética seguirá leyendo como 2D; la tercera dimensión funcionará como infraestructura para profundidad y efectos.

### 2026-08-05 — Laboratorio de calibración implementado

- Commit `8ce5c5e`: modelo espacial, generador de laboratorio y nuevas pruebas.
- La escena no fue reescrita manualmente en YAML; la calibración se instala al cargar `Lab`, reduciendo riesgo de corrupción y manteniendo la escena limpia.
- La validación remota se limita a creación correcta del árbol, commit y actualización de la rama.
- La compilación, las pruebas y el resultado visual permanecen pendientes de Arturo.

---

## 9. Próximo paso exacto

1. Cerrar Unity si está abierto.
2. En GitHub Desktop, sobre `feature/lab-spatial-calibration`, ejecutar **Fetch origin** y **Pull origin**.
3. Abrir el proyecto con Unity `6000.3.21f1` y esperar la compilación.
4. Confirmar consola en `0` errores y `0` advertencias.
5. Ejecutar Test Runner:
   - EditMode esperado: **3/3**.
   - PlayMode esperado: **2/2**.
6. Abrir `Bootstrap.unity` o ejecutar desde la configuración actual y pulsar Play.
7. Esperar la transición a `Lab` y tomar una captura de la pestaña **Game**.
8. Reportar si la cuadrícula, el personaje de referencia, la trayectoria y la profundidad se ven completos y legibles.
9. Con esa evidencia, ajustar cámara y escala o pasar al controlador de movimiento con teclado y DualSense.

---

## 10. Protocolo para un chat nuevo

Usar una instrucción equivalente a:

> Continúa el desarrollo de `xArtXtrAx/Unity-Farm-Simulator`. Antes de modificar archivos, lee `BITÁCORA_GPT.MD`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, revisa ramas y commits recientes, resume el estado y continúa desde el próximo paso documentado.

---

## 11. Decisiones pendientes

- Resolución base y relación de aspecto objetivo.
- Tamaño definitivo de celda y escala artística.
- Pixels Per Unit para los sprites.
- Inclinación y tamaño ortográfico definitivos de la cámara.
- Dirección artística y primer pack de sprites.
- Estrategia de CI para pruebas y build de Windows.
