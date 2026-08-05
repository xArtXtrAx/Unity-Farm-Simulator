# BITÁCORA GPT — Unity Farm Simulator

Documento maestro de continuidad del proyecto. Su objetivo es conservar el contexto técnico, creativo y operativo necesario para retomar el desarrollo en un chat nuevo sin depender de la memoria de conversaciones anteriores.

> **Regla de continuidad:** antes de iniciar trabajo en una conversación nueva, GPT debe leer este archivo, `BUGS.MD`, el estado actual del repositorio y los commits recientes.

---

## 1. Identidad del proyecto

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Propietario:** Arturo García (`xArtXtrAx`)
- **Tipo de proyecto:** simulador de granja 2.5D
- **Motor:** Unity
- **Plataforma inicial:** Windows PC
- **Control prioritario:** DualSense, sin abandonar teclado y mouse
- **Estado del repositorio:** público temporalmente durante el desarrollo colaborativo; posteriormente será privado
- **Inicio formal de la bitácora:** 5 de agosto de 2026

---

## 2. Visión general

Desarrollar un simulador de granja 2.5D con una arquitectura limpia, escalable y orientada a datos. Unity funcionará como el intermediario visual y técnico común entre Arturo y GPT:

- Arturo dirige la visión, diseño, estética, sensación de juego, composición de escenas y validación dentro del editor.
- GPT diseña e implementa arquitectura, scripts C#, automatizaciones, herramientas especializadas, pruebas, documentación y mantenimiento del repositorio.
- Unity aporta las herramientas generales de escenas, sprites, prefabs, colisiones, cuadrículas, animación, física e inspección.
- GitHub conserva el historial, permite revisar cambios y sirve como punto de continuidad entre sesiones.

### Principio rector

> No construir un editor de videojuegos desde cero. Construir el juego usando el editor de Unity y crear únicamente herramientas específicas que aporten valor al simulador.

---

## 3. Decisiones tomadas

### 3.1 Motor y enfoque

- Se eligió Unity para evitar recrear herramientas generales ya disponibles en el motor.
- El proyecto anterior/prototipo sirve como referencia de mecánicas, necesidades y diseño, pero no se continuará ampliando como editor genérico.
- La primera meta será una **prueba vertical pequeña y completamente jugable**, no una migración masiva inmediata.

### 3.2 Versión y plantilla previstas

- **Versión objetivo:** Unity 6.3 LTS, usando el parche estable más reciente disponible dentro de la rama seleccionada al crear el proyecto.
- **Plantilla prevista:** Universal 3D / URP.
- La elección está orientada a combinar sprites 2D con profundidad, iluminación, cámara y elementos tridimensionales propios de un juego 2.5D.

> Registrar aquí la versión exacta del editor una vez creado el proyecto, por ejemplo: `6000.3.xf1`.

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
- Actualización continua de esta bitácora y de `BUGS.MD`.

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
- Colliders y visualización física.
- Animaciones y Animator.
- Undo/Redo.

### Crear herramientas propias únicamente para necesidades específicas

Ejemplos potenciales:

- Catálogo de cultivos, vegetación, muebles, construcciones, NPC y objetos interactivos.
- Validación automática de prefabs y escenas.
- Importación estandarizada de packs de sprites.
- Generación de datos y ScriptableObjects.
- Herramientas de balance y depuración de sistemas del simulador.

---

## 6. Estrategia para sprites y contenido

El contenido gráfico se tratará en dos niveles:

1. **Recursos originales**, conservados sin modificación destructiva.
2. **Contenido preparado para el juego**, organizado, importado, cortado y conectado a prefabs o datos.

Estructura conceptual prevista:

```text
Assets/
├── Art/
│   ├── Raw/
│   ├── Sprites/
│   │   ├── Characters/
│   │   ├── Crops/
│   │   ├── Vegetation/
│   │   ├── Objects/
│   │   └── UI/
│   └── Tilesets/
├── Animations/
├── Prefabs/
├── Scenes/
├── ScriptableObjects/
├── Scripts/
└── Tests/
```

GPT podrá preparar scripts de importación, slicing, convenciones de nombres, animaciones, prefabs y ScriptableObjects. Arturo realizará la validación visual final dentro de Unity.

Todo recurso externo debe registrar su licencia y confirmar que permite el uso previsto, especialmente si el juego se comercializa.

---

## 7. Primera prueba vertical prevista

La primera versión jugable debe ser deliberadamente pequeña e incluir:

- Una escena de laboratorio o pequeña granja.
- Personaje controlable con DualSense y teclado.
- Cámara funcional.
- Suelo o Tilemap básico.
- Cuadrícula y colisiones mediante herramientas nativas de Unity.
- Una casa u objeto grande.
- Un árbol.
- Una roca.
- Una parcela cultivable.
- Un cultivo con varias etapas de crecimiento.
- Interacción básica.
- Guardado mínimo cuando sea pertinente.

El objetivo no es producir contenido abundante, sino validar el flujo completo Unity ↔ GitHub ↔ Arturo ↔ GPT.

---

## 8. Estado actual

### Completado

- [x] Decisión de migrar el desarrollo principal a Unity.
- [x] Definición de Unity como intermediario visual y GitHub como memoria compartida.
- [x] Creación del repositorio `Unity-Farm-Simulator`.
- [x] Creación de `BITÁCORA_GPT.MD`.
- [x] Creación planificada de `BUGS.MD`.
- [x] Definición inicial del flujo de trabajo y responsabilidades.

### En curso

- [ ] Instalación de Unity Hub y Unity en la computadora de Arturo.
- [ ] Confirmación de la versión exacta instalada.

### Próximo paso inmediato

1. Arturo instala Unity.
2. Se confirma la versión exacta y los módulos instalados.
3. Se crea o vincula el proyecto Unity dentro de este repositorio.
4. Se configuran `.gitignore`, serialización, archivos `.meta` y estructura inicial.
5. Se abre la primera escena de laboratorio.

---

## 9. Registro cronológico

### 2026-08-05 — Inicio formal del repositorio

- Se verificó acceso de lectura y escritura al repositorio.
- Se estableció este documento como memoria maestra de continuidad.
- Se acordó conservar el historial de errores en `BUGS.MD`.
- El repositorio se encuentra vacío de implementación Unity al momento de este registro.
- Se definió que Unity resolverá las herramientas genéricas y que solo se construirán extensiones específicas del simulador.

---

## 10. Protocolo de actualización

Al terminar cada bloque importante de trabajo, actualizar este archivo con:

- Fecha.
- Objetivo realizado.
- Archivos o sistemas modificados.
- Decisiones tomadas y motivo.
- Pruebas ejecutadas y resultado.
- Bugs abiertos o corregidos.
- Estado actual del juego.
- Próximo paso concreto.

No convertir la bitácora en un volcado de cada comando. Registrar decisiones, resultados y contexto que permitan continuar el proyecto.

---

## 11. Protocolo para comenzar un chat nuevo

En una conversación nueva, utilizar una instrucción equivalente a:

> Continúa el desarrollo del repositorio `xArtXtrAx/Unity-Farm-Simulator`. Antes de modificar archivos, lee `BITÁCORA_GPT.MD`, `BUGS.MD`, el estado actual del repositorio y los commits recientes. Resume brevemente el estado del proyecto y continúa desde el próximo paso documentado.

---

## 12. Datos pendientes por definir

- Versión exacta de Unity instalada.
- Ruta local definitiva del repositorio.
- Resolución base y relación de aspecto objetivo.
- Cámara ortográfica o perspectiva controlada.
- Escala visual, tamaño de tiles y Pixels Per Unit.
- Dirección artística definitiva.
- Primer pack de sprites o recursos de prueba.
- Convenciones definitivas de namespaces y assemblies.
- Estrategia de pruebas y automatización CI.
