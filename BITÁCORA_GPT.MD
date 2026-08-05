# BITÁCORA GPT — Unity Farm Simulator

Documento maestro de continuidad del proyecto. Su objetivo es conservar el contexto técnico, creativo y operativo necesario para retomar el desarrollo en un chat nuevo sin depender de la memoria de conversaciones anteriores.

> **Regla de continuidad:** antes de iniciar trabajo en una conversación nueva, GPT debe leer este archivo, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, el estado actual del repositorio y los commits recientes.

> **Regla crucial de sincronización:** toda nueva implementación, corrección o cambio técnico realizado por GPT debe ir seguido, dentro del mismo bloque de trabajo, por una actualización de esta bitácora. Ninguna implementación se considera terminada hasta registrar qué cambió, qué se comprobó, cuál es el estado resultante y cuál es el siguiente paso exacto. Esta regla existe para que, si el contexto del chat se interrumpe súbitamente, el proyecto pueda retomarse de inmediato usando únicamente el repositorio.

---

## 1. Identidad del proyecto

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Propietario:** Arturo García (`xArtXtrAx`)
- **Tipo de proyecto:** simulador de granja 2.5D
- **Motor:** Unity
- **Versión instalada:** Unity 6.3 LTS `6000.3.21f1`
- **Plataforma inicial:** Windows PC
- **Control prioritario:** DualSense, sin abandonar teclado y mouse
- **Ruta local del repositorio de Arturo:** `D:\Git\Unity-Farm-Simulator`
- **Estado del repositorio:** público temporalmente durante el desarrollo colaborativo; posteriormente será privado
- **Inicio formal de la bitácora:** 5 de agosto de 2026
- **Proyecto fuente:** `xArtXtrAx/farming-game-A`
- **Línea base de migración:** `main` en `dd32056c9f8142a2322bc2c1d41f0b05b002598f`

---

## 2. Visión general

Desarrollar un simulador de granja 2.5D con una arquitectura limpia, escalable y orientada a datos. Unity funcionará como el intermediario visual y técnico común entre Arturo y GPT:

- Arturo dirige la visión, diseño, estética, sensación de juego, composición de escenas y validación dentro del editor.
- GPT diseña e implementa arquitectura, scripts C#, automatizaciones, herramientas especializadas, pruebas, documentación y mantenimiento del repositorio.
- Unity aporta las herramientas generales de escenas, sprites, prefabs, colisiones, cuadrículas, animación, física e inspección.
- GitHub conserva el historial, permite revisar cambios y sirve como punto de continuidad entre sesiones.

### Principio rector

> No construir un editor de videojuegos desde cero. Construir el juego usando el editor de Unity y crear únicamente herramientas específicas que aporten valor al simulador.

### Principio de migración

> Portar comportamientos, datos, identificadores y criterios de aceptación; reconstruir la presentación y la integración con el motor usando herramientas nativas de Unity. No traducir TypeScript a C# línea por línea.

---

## 3. Decisiones tomadas

### 3.1 Motor y enfoque

- Se eligió Unity para evitar recrear herramientas generales ya disponibles en el motor.
- `farming-game-A` queda como referencia ejecutable de mecánicas, reglas, escenas, balance y pruebas.
- No se portarán como sistemas propios el editor de geometría, la jerarquía, el inspector, el overlay general de colisiones ni el snapping del prototipo web, porque Unity ya cubre esas necesidades.
- La primera meta será una **prueba vertical pequeña y completamente jugable**, no una migración masiva inmediata.
- Después de validar el flujo Unity–GitHub, se buscará paridad funcional con las tres escenas y sistemas del commit fuente.

### 3.2 Versión y plantilla

- **Versión instalada y fijada para el proyecto:** Unity 6.3 LTS `6000.3.21f1`.
- **Plantilla prevista:** Universal 3D / URP.
- La elección está orientada a combinar sprites 2D con profundidad, iluminación, cámara y elementos tridimensionales propios de un juego 2.5D.
- No abrir ni actualizar el proyecto con otra versión de Unity sin documentar y aprobar previamente la migración del editor.

### 3.3 Flujo Git

- Arturo utilizará principalmente **GitHub Desktop**.
- Se evitarán comprobaciones Git redundantes, ZIP, manifiestos y hashes salvo necesidad técnica clara.
- `main` debe mantenerse estable y jugable.
- Las mejoras relevantes se desarrollarán en ramas con nombres descriptivos, por ejemplo:
  - `feature/player-controller`
  - `feature/crop-system`
  - `feature/content-catalog`
  - `fix/camera-jitter`
- Los cambios deben agruparse de forma lógica y documentarse con commits claros.

### 3.4 Control de versiones en Unity

Al crear el proyecto se configurará:

- `Version Control Mode`: **Visible Meta Files**
- `Asset Serialization`: **Force Text**
- Incluir en Git:
  - `Assets/`
  - `Packages/`
  - `ProjectSettings/`
- Excluir archivos regenerables como:
  - `Library/`
  - `Temp/`
  - `Obj/`
  - `Logs/`
  - `Build/` y `Builds/`
  - `UserSettings/`
- Evaluar Git LFS únicamente para archivos binarios pesados que realmente lo necesiten.

### 3.5 Arquitectura prevista

- Las reglas críticas vivirán en C# puro, preferentemente sin dependencia de `UnityEngine`.
- Los `ScriptableObject` contendrán definiciones de objetos, cultivos y contenido, no el progreso mutable de la partida.
- El estado de partida se guardará mediante DTO e identificadores de texto estables.
- `MonoBehaviour`, escenas, prefabs, animación y UI serán la capa de presentación.
- Las pruebas Edit Mode protegerán reglas; las Play Mode protegerán integración con Unity.

### 3.6 Bitácora transaccional obligatoria

- Cada implementación o corrección realizada por GPT debe actualizar `BITÁCORA_GPT.MD` antes de dar el bloque de trabajo por concluido.
- La actualización debe formar parte del mismo conjunto lógico de cambios y, cuando sea posible, del mismo commit o PR documentalmente coherente.
- Debe registrarse como mínimo:
  - objetivo y fase afectada;
  - archivos y sistemas modificados;
  - decisiones o supuestos relevantes;
  - pruebas realmente ejecutadas y sus resultados;
  - bugs abiertos, corregidos o pendientes de validación;
  - estado exacto de la rama o implementación;
  - próximo paso concreto y cualquier acción requerida de Arturo.
- Si la implementación queda parcial, bloqueada o pendiente de prueba manual, debe decirse expresamente; nunca registrar como terminado lo que no se comprobó.
- Si el chat se interrumpe antes de una respuesta final, la prioridad al recuperar acceso será actualizar la bitácora con el último estado verificable antes de continuar desarrollando.
- Los detalles específicos de errores permanecerán en `BUGS.MD`; esta bitácora debe enlazar o resumir su efecto sobre el estado general.

---

## 4. División de responsabilidades

### Arturo

- Dirección creativa y prioridades.
- Instalación y operación local de Unity.
- Diseño visual de escenas y colocación de elementos.
- Selección y validación de sprites, modelos, audio y otros recursos.
- Pruebas de jugabilidad, sensación de movimiento y control.
- Capturas, videos y reporte de errores observados en Unity.

### GPT

- Arquitectura del proyecto.
- Scripts C# y sistemas del juego.
- Herramientas personalizadas del editor cuando aporten valor específico.
- Importadores, validadores y automatización de contenido.
- Revisión del repositorio, commits, ramas, pull requests y CI cuando corresponda.
- Pruebas automatizadas y documentación.
- Actualización obligatoria de esta bitácora después de cada implementación o corrección, además del mantenimiento de la bitácora de migración y de `BUGS.MD`.

---

## 5. Política para herramientas internas

Antes de crear una herramienta personalizada, comprobar si Unity ya resuelve el problema adecuadamente.

### Usar herramientas nativas de Unity para

- Jerarquía y selección de objetos.
- Inspector y edición de componentes.
- Cuadrículas y snapping.
- Tilemaps y Tile Palette.
- Sprite Editor y slicing.
- Prefabs y Prefab Variants.
- Colliders, Gizmos y visualización física.
- Animaciones y Animator.
- Undo/Redo.

### Crear herramientas propias únicamente para necesidades específicas

Ejemplos potenciales:

- Catálogo de cultivos, vegetación, muebles, construcciones, NPC y objetos interactivos.
- Validación automática de prefabs y escenas.
- Importación estandarizada de packs de sprites.
- Generación de datos y ScriptableObjects.
- Herramientas de balance y depuración de sistemas del simulador.
- Validación de IDs, spawns, licencias y referencias obligatorias.

---

## 6. Estrategia para sprites y contenido

El contenido gráfico se tratará en dos niveles:

1. **Recursos originales**, conservados sin modificación destructiva.
2. **Contenido preparado para el juego**, organizado, importado, cortado y conectado a prefabs o datos.

Estructura conceptual prevista:

```text
Assets/_Project/
├── Art/
│   ├── Raw/
│   ├── Characters/
│   ├── Crops/
│   ├── Environment/
│   └── UI/
├── Audio/
├── Data/
├── Prefabs/
├── Scenes/
├── Scripts/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   ├── Presentation/
│   └── Editor/
└── Tests/
    ├── EditMode/
    └── PlayMode/
```

GPT podrá preparar scripts de importación, slicing, convenciones de nombres, animaciones, prefabs y ScriptableObjects. Arturo realizará la validación visual final dentro de Unity.

Todo recurso externo debe registrar su licencia y confirmar que permite el uso previsto, especialmente si el juego se comercializa.

---

## 7. Línea base funcional del proyecto anterior

La auditoría de `farming-game-A` confirmó que el prototipo ya contiene:

- tres escenas: granja, interior de la casa y mercado norte;
- personaje animado y controles por teclado, mouse y gamepad;
- hotbar de ocho espacios;
- inventario, almacenamiento y transferencias;
- doce parcelas agrícolas;
- azada, regadera y semillas;
- nabo, zanahoria y col de primavera;
- tienda, economía y caja de envíos;
- reloj, días, estaciones y años;
- sueño y procesamiento transaccional de fin de día;
- guardado JSON versionado con validación y rollback;
- colisiones, interacciones, transiciones y spawns;
- una suite documentada de 806 pruebas al integrar los cultivos de primavera.

El PR #43 está realmente fusionado en `main`, aunque documentos antiguos del repositorio fuente todavía lo describen como pendiente. El HEAD y GitHub prevalecen sobre esa documentación obsoleta.

Los detalles, equivalencias, fases y puertas de salida están en `MIGRACION_DESDE_FARMING_GAME_A.MD`.

---

## 8. Primera prueba vertical prevista

La primera versión jugable debe ser deliberadamente pequeña e incluir:

- escena `Lab` o pequeña granja;
- personaje controlable con DualSense y teclado;
- cámara funcional;
- suelo básico;
- cuadrícula y colisiones mediante herramientas nativas de Unity;
- una parcela cultivable;
- azada, regadera y semillas de nabo;
- un cultivo con varias etapas;
- hotbar e interacción básica;
- avance de día de prueba;
- guardado mínimo.

El objetivo no es producir contenido abundante, sino validar el flujo completo Unity ↔ GitHub ↔ Arturo ↔ GPT antes de recrear las tres escenas.

---

## 9. Estado actual

### Completado

- [x] Decisión de migrar el desarrollo principal a Unity.
- [x] Definición de Unity como intermediario visual y GitHub como memoria compartida.
- [x] Creación del repositorio `Unity-Farm-Simulator`.
- [x] Creación de `BITÁCORA_GPT.MD`.
- [x] Creación de `BUGS.MD`.
- [x] Definición inicial del flujo de trabajo y responsabilidades.
- [x] Auditoría del repositorio `farming-game-A`.
- [x] Confirmación de la línea base `dd32056` y del PR #43 fusionado.
- [x] Creación de `MIGRACION_DESDE_FARMING_GAME_A.MD` con fases, contratos y criterios de cierre.
- [x] Establecimiento de la actualización obligatoria de la bitácora después de cada implementación o corrección.
- [x] Instalación de Unity 6.3 LTS `6000.3.21f1`.
- [x] Clonación del repositorio en `D:\Git\Unity-Farm-Simulator`.

### En curso

- [ ] Creación segura del proyecto Unity dentro del repositorio ya clonado.
- [ ] Confirmación de los módulos instalados junto al editor.

### Próximo paso inmediato

1. Crear un proyecto temporal con plantilla **Universal 3D** usando Unity 6.3 LTS.
2. Cerrar el editor cuando termine la importación inicial.
3. Copiar únicamente `Assets`, `Packages` y `ProjectSettings` desde el proyecto temporal a `D:\Git\Unity-Farm-Simulator`.
4. Añadir la raíz clonada a Unity Hub mediante **Add/Open project from disk**.
5. Abrirla exclusivamente con `6000.3.21f1` y comprobar que no existan errores de consola.
6. Configurar `.gitignore`, serialización, archivos `.meta`, paquetes y assemblies para completar la **Fase 1**.

---

## 10. Registro cronológico

### 2026-08-05 — Inicio formal del repositorio

- Se verificó acceso de lectura y escritura al repositorio.
- Se estableció este documento como memoria maestra de continuidad.
- Se acordó conservar el historial de errores en `BUGS.MD`.
- Se definió que Unity resolverá las herramientas genéricas y que solo se construirán extensiones específicas del simulador.

### 2026-08-05 — Auditoría y planificación de la migración

- Se estudió la estructura completa de `xArtXtrAx/farming-game-A`.
- Se inventariaron escenas, sistemas de dominio, input, guardado, herramientas y pruebas.
- Se confirmó que la línea base correcta es `main` en `dd32056` y que el PR #43 ya fue fusionado.
- Se determinó que deben migrarse reglas, datos, IDs y casos de prueba, no el código Phaser de forma literal.
- Se decidió sustituir las herramientas genéricas web por las herramientas nativas de Unity.
- Se creó `MIGRACION_DESDE_FARMING_GAME_A.MD` con once fases posteriores a la congelación de la fuente, criterios de paridad, riesgos y puertas de salida.

### 2026-08-05 — Regla obligatoria de continuidad inmediata

- Arturo estableció como requisito crucial que toda implementación de GPT vaya seguida de una actualización de esta bitácora.
- Se convirtió el requisito en una puerta de cierre: una implementación no se considera terminada hasta que su estado quede documentado.
- La actualización debe permitir reconstruir el punto exacto de trabajo incluso si la memoria del chat se interrumpe sin aviso.
- Se definieron los datos mínimos que debe contener cada registro y la obligación de distinguir entre completado, parcial, bloqueado y pendiente de validación manual.

### 2026-08-05 — Entorno local preparado

- Arturo instaló Unity 6.3 LTS `6000.3.21f1` mediante Unity Hub.
- El repositorio se clonó correctamente en `D:\Git\Unity-Farm-Simulator`.
- La raíz del repositorio todavía contiene únicamente documentación y debe convertirse en proyecto Unity sin eliminar esos archivos.
- El siguiente paso es generar un proyecto temporal Universal 3D y trasladar a la raíz solo las carpetas nativas necesarias.

---

## 11. Protocolo de actualización

### Regla obligatoria

Después de **cada implementación, corrección o cambio técnico realizado por GPT**, actualizar esta bitácora antes de declarar terminado el bloque de trabajo. No esperar al final de una fase grande ni acumular varias implementaciones sin documentarlas.

Cada actualización debe registrar:

- Fecha.
- Objetivo realizado.
- Fase de migración afectada.
- Archivos o sistemas modificados.
- Decisiones tomadas y motivo.
- Pruebas realmente ejecutadas y resultado.
- Bugs abiertos, corregidos o pendientes.
- Estado exacto del juego, rama o PR.
- Próximo paso concreto.
- Cualquier acción manual requerida de Arturo.

Cuando un cambio sea únicamente documental o administrativo, bastará un registro breve. Cuando sea funcional, el registro debe contener suficiente información para que otro chat pueda continuar sin reconstruir el contexto desde cero.

No convertir la bitácora en un volcado de cada comando. Registrar decisiones, resultados, límites y contexto operativo que permitan continuar el proyecto.

---

## 12. Protocolo para comenzar un chat nuevo

En una conversación nueva, utilizar una instrucción equivalente a:

> Continúa el desarrollo del repositorio `xArtXtrAx/Unity-Farm-Simulator`. Antes de modificar archivos, lee `BITÁCORA_GPT.MD`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, el estado actual del repositorio y los commits recientes. Resume brevemente el estado del proyecto y continúa desde el próximo paso documentado.

La bitácora debe poder indicar por sí misma cuál fue la última implementación, qué quedó comprobado, qué está pendiente y qué debe hacerse inmediatamente después.

---

## 13. Datos pendientes por definir

- Módulos exactos instalados junto a Unity 6.3 LTS.
- Resolución base y relación de aspecto objetivo.
- Cámara ortográfica o perspectiva controlada.
- Modelo espacial definitivo: mundo XZ con física 3D o XY con física 2D.
- Escala visual, tamaño de tiles y Pixels Per Unit.
- Dirección artística definitiva.
- Primer pack de sprites o recursos de prueba.
- Convenciones definitivas de namespaces y assemblies.
- Estrategia final de CI para Edit Mode, Play Mode y build de Windows.