# Bitácora de migración — Farming Game A → Unity Farm Simulator

Documento operativo para migrar el prototipo funcional `xArtXtrAx/farming-game-A` al proyecto `xArtXtrAx/Unity-Farm-Simulator` sin perder reglas, decisiones, pruebas ni contexto histórico.

> **Principio de migración:** portar comportamientos, datos y criterios de aceptación; reconstruir la presentación y la integración con el motor usando herramientas nativas de Unity. No traducir TypeScript a C# línea por línea.

---

## 1. Línea base estudiada

- **Repositorio fuente:** `xArtXtrAx/farming-game-A`
- **Rama fuente:** `main`
- **Commit fuente de referencia:** `dd32056c9f8142a2322bc2c1d41f0b05b002598f`
- **Fecha de auditoría:** 5 de agosto de 2026
- **Tecnología fuente:** Phaser 4.2.1, TypeScript, Vite, Vitest y Playwright
- **Estado del PR #43:** fusionado en `main`; añade zanahoria y col junto al nabo.

### Inconsistencia documental detectada

`docs/AI_HANDOFF.md` y partes de `HISTORIAL_GPT.MD` todavía describen el PR #43 como pendiente de validación e integración. GitHub y el HEAD real confirman que el PR fue fusionado en el commit `dd32056`. Para la migración, la fuente de verdad es el contenido actual de `main`, no esos párrafos obsoletos.

### Estado funcional de la línea base

El proyecto fuente ya contiene una vertical funcional con:

- tres escenas: granja, interior de la casa y zona norte del pueblo/mercado;
- personaje animado y movimiento en cuatro direcciones;
- teclado, mouse y gamepad, con atención específica al DualSense;
- hotbar de ocho espacios;
- inventario, cofre/almacenamiento y transferencias;
- doce parcelas agrícolas;
- azada, regadera y semillas;
- nabo, zanahoria y col;
- crecimiento por días regados, cosecha y restricciones estacionales;
- tienda de semillas;
- caja de envíos y economía;
- reloj, días, estaciones y años;
- sueño, cierre del día y resumen de ventas;
- guardado versionado y restauración defensiva;
- colisiones, interacciones y transiciones entre escenas;
- herramientas de desarrollo para geometría, cuadrícula, densidad y diagnóstico;
- pruebas unitarias, de integración y E2E, con 806 pruebas documentadas al integrar los cultivos de primavera.

---

## 2. Qué se conserva, qué se reconstruye y qué se descarta

### 2.1 Conservar como especificación

- Identificadores estables de objetos, cultivos, parcelas e interactuables.
- Reglas de inventario, stacks, compras, ventas y ausencia de mutaciones parciales.
- Reglas de labranza, riego, siembra, crecimiento y cosecha.
- Calendario de 28 días por estación.
- Flujo de cierre del día.
- Contrato de guardado versionado, validación y rollback.
- Diseño de controles y prioridades de interacción.
- Estructura de las tres escenas y sus conexiones.
- Casos de prueba y regresiones importantes.
- Sprites originales permitidos y su configuración conocida.
- Política de no integrar cambios funcionales sin pruebas y validación apropiadas.

### 2.2 Reconstruir con Unity

- Escenas y composición visual.
- Colisiones y límites mediante Colliders y física nativa.
- Selección, jerarquía, inspector, cuadrícula y snapping mediante el editor de Unity.
- Animaciones mediante Animation Clips y Animator.
- Input mediante el paquete Input System.
- Transiciones mediante `SceneManager` y un coordinador de fundido.
- UI del juego mediante uGUI inicialmente; UI Toolkit se reservará principalmente para herramientas de editor.
- Efectos del estanque mediante URP, Shader Graph, partículas o animación, según la prueba visual.
- Persistencia en disco mediante JSON en `Application.persistentDataPath`.
- Pruebas mediante Unity Test Framework en Edit Mode y Play Mode.

### 2.3 No portar como sistema propio

- Editor genérico de geometría.
- Jerarquía propia de objetos de escena.
- Overlay general de colisiones.
- Cuadrícula visual genérica del runtime.
- Snapping de objetos de desarrollo.
- Selector universal de objetos y panel de propiedades.
- Lógica web de densidad física frente a resolución lógica.
- Integración específica con DOM, Canvas, `localStorage`, Vite o Playwright.

Estas funciones fueron valiosas para el prototipo web, pero Unity ya proporciona sus equivalentes. Solo se crearán extensiones especializadas para contenido del simulador.

---

## 3. Inventario de sistemas y destino en Unity

| Sistema fuente | Archivos o módulos principales | Destino previsto en Unity | Tratamiento |
|---|---|---|---|
| Estado global | `core/GameState.ts` | `GameSession`, estados de dominio y DTO de snapshot | Reimplementar conservando transacciones |
| Catálogo de objetos | `data/items.ts` | `ItemDefinition : ScriptableObject` + registro por ID | Migrar datos |
| Catálogo de cultivos | `data/crops.ts` | `CropDefinition : ScriptableObject` + registro por ID | Migrar datos |
| Parcelas y cultivos | `farming/*` | Dominio C# puro + componentes de presentación | Reimplementar reglas |
| Herramientas | `tools/*` | Estrategias/acciones de herramienta desacopladas de la vista | Reimplementar reglas |
| Inventario | `inventory/*` | Dominio C# puro + hotbar uGUI | Reimplementar reglas y UI |
| Almacenamiento | `storage/*` | Estado de contenedor + menú uGUI | Reimplementar |
| Caja de envíos | `shipping/*` | Estado, procesador de cierre diario y menú uGUI | Reimplementar |
| Economía y tienda | `economy/*`, `shop/*` | Estado monetario, servicios de compra y menú uGUI | Reimplementar |
| Tiempo | `time/*` | `GameTimeState`, reloj y coordinador de fin de día | Reimplementar |
| Input | `input/*` | Asset `.inputactions`, `PlayerInput` y adaptadores | Sustituir por Input System |
| Jugador | `entities/player/*` | Prefab, controlador, Animator y colisión nativa | Reconstruir |
| Interacciones | `interaction/*` | `IInteractable`, sensor frontal y prioridades | Reimplementar contrato |
| Escenas | `scenes/*` | Escenas `.unity`, prefabs y spawns identificados | Reconstruir visualmente |
| Colisiones | `world/CollisionSystem.ts` | Colliders nativos, capas y Physics Debugger | No portar solver manual |
| Profundidad | `world/DepthSystem.ts` | Eje de profundidad, Sorting Groups o mundo XZ | Rediseñar para 2.5D |
| Guardado | `save/*` | JSON versionado en disco, DTO y migradores | Reimplementar contrato |
| Render y estanque | `render/*` | URP, materiales, Shader Graph y VFX | Reconstruir después de la paridad mecánica |
| Herramientas web | `debug/*`, parte de `grid/*` | Editor nativo de Unity | No portar como runtime |
| Grid de dominio | `grid/*` | Solo conservar si la agricultura o NPC requieren una cuadrícula lógica propia | Revaluar por consumidor |
| Pruebas | `tests/*` | Edit Mode, Play Mode y build automatizado | Migrar casos de comportamiento |

---

## 4. Arquitectura objetivo

La arquitectura deberá mantener las reglas importantes fuera de `MonoBehaviour` para que puedan probarse sin cargar escenas.

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
│   ├── Items/
│   ├── Crops/
│   └── Catalogs/
├── Prefabs/
│   ├── Characters/
│   ├── Interactables/
│   ├── Farming/
│   ├── Environment/
│   └── UI/
├── Scenes/
│   ├── Bootstrap.unity
│   ├── Lab.unity
│   ├── Farm.unity
│   ├── HouseInterior.unity
│   └── VillageNorth.unity
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

### Ensamblados previstos

```text
FarmSim.Domain
FarmSim.Application
FarmSim.Infrastructure
FarmSim.Presentation
FarmSim.Editor
FarmSim.Tests.EditMode
FarmSim.Tests.PlayMode
```

### Regla de dependencias

```text
Domain
  ↑
Application
  ↑
Infrastructure / Presentation
```

- `Domain` no debe depender de `UnityEngine`.
- `Application` coordina casos de uso.
- `Infrastructure` implementa guardado, carga y adaptadores externos.
- `Presentation` contiene `MonoBehaviour`, escenas, UI, animación y efectos.
- `Editor` contiene importadores, validadores y ventanas específicas del simulador.

---

## 5. Contratos de paridad funcional

La migración inicial se considerará funcionalmente equivalente cuando cumpla estos comportamientos:

### Jugador y controles

- Movimiento en ocho direcciones analógicas, con cuatro direcciones visuales.
- Teclado y DualSense desde el mismo mapa de acciones.
- Zona muerta ajustable, tomando `0.18` como punto inicial de referencia.
- Acción, cancelar, selección directa de ocho slots y ciclo de hotbar.
- Animación de caminar a cuatro direcciones y estado idle por dirección.

### Sprite inicial del jugador

- Recurso de referencia: `src/assets/hero_hd.png`.
- Frame de referencia: `64 × 72`.
- Doce frames organizados en cuatro direcciones.
- Caminata de referencia: secuencias `0,1,2,1`, `3,4,5,4`, `6,7,8,7`, `9,10,11,10`.
- Velocidad visual de referencia: 8 FPS.
- Pivot visual aproximado de referencia: `(0.5, 0.88)`.
- El recurso original debe preservarse sin edición destructiva.

### Estado inicial

- Ocho slots de hotbar.
- Azada en el primer slot.
- Regadera en el segundo slot.
- Veinte semillas de nabo en el tercero.
- Doce parcelas identificadas de `farm-plot-01` a `farm-plot-12`.

### Cultivos

| Cultivo | Temporada | Días regados | Precio semilla | Precio venta | Cosecha |
|---|---|---:|---:|---:|---:|
| Nabo | Primavera | 3 | 30 | 60 | 1 |
| Zanahoria | Primavera | 4 | 50 | 100 | 1 |
| Col | Primavera | 5 | 75 | 150 | 1 |

Reglas obligatorias:

- no se puede labrar dos veces;
- no se puede regar terreno sin labrar;
- no se puede regar dos veces el mismo día;
- no se puede sembrar en terreno sin labrar ni ocupado;
- comprar o sembrar fuera de temporada no modifica dinero, inventario ni parcela;
- un cultivo avanza solo si la parcela terminó el día regada;
- el riego se limpia al finalizar el día;
- una cosecha madura solo tras el número exacto de días regados;
- la cosecha requiere capacidad suficiente en inventario;
- cosechar libera el cultivo, conserva la tierra labrada y limpia el riego.

### Calendario y día

- Inicio: Año 1, Primavera 1, 08:00.
- 28 días por estación.
- Orden: primavera, verano, otoño, invierno.
- Al pasar invierno 28, comienza Año 2, Primavera 1.
- Límite de reloj de referencia: 23:50.
- Dormir procesa crecimiento, caja de envíos, dinero, avance de día y guardado.

### Escenas e interacciones

- `Farm`: casa, puerta, caja de envíos, estanque, árboles, doce parcelas y salida norte.
- `HouseInterior`: cama, cofre y salida hacia la granja.
- `VillageNorth`: tienda de semillas y salida sur hacia la granja.
- Cada transición debe usar un spawn identificado y orientación coherente.
- Las interacciones deben seleccionar el objetivo por dirección, alcance, tolerancia lateral y prioridad.
- Las interacciones automáticas de salida no deben dispararse repetidamente mientras el jugador permanece dentro del mismo volumen.

### Economía, almacenamiento y envíos

- Compra atómica: si faltan fondos o espacio, no cambia ningún estado.
- Transferencias entre hotbar y cofre sin pérdida ni duplicación.
- Caja de envíos solo acepta objetos vendibles.
- Los objetos enviados se liquidan al dormir, no inmediatamente.
- El resumen del nuevo día muestra líneas vendidas y total ganado.

### Guardado

- Documento JSON con número de versión y fecha de guardado.
- Validación antes de restaurar.
- Restauración completa o rollback al estado anterior.
- Rechazo claro de documentos corruptos o versiones no soportadas.
- Separar datos de partida de preferencias técnicas y herramientas de desarrollo.

---

## 6. Plan por fases

## Fase 0 — Congelar y documentar la fuente

**Objetivo:** asegurar una referencia estable antes de comenzar la reimplementación.

- [x] Identificar el commit fuente `dd32056`.
- [x] Confirmar que PR #43 está fusionado.
- [x] Inventariar sistemas, escenas, assets y pruebas.
- [ ] Etiquetar o registrar una release de referencia en el repositorio fuente si Arturo lo considera útil.
- [ ] Conservar capturas o video de los tres escenarios y de los flujos principales.
- [ ] Exportar una partida de ejemplo v2 para usarla como fixture de compatibilidad futura.
- [ ] Registrar procedencia y licencia de cada asset reutilizable.

**Puerta de salida:** existe una referencia reproducible del juego web y no necesitamos seguir modificándolo para desarrollar Unity.

---

## Fase 1 — Crear correctamente el proyecto Unity

**Objetivo:** establecer una base limpia y versionable.

- [ ] Crear el proyecto con la versión exacta instalada de Unity 6.3 LTS y plantilla Universal 3D/URP.
- [ ] Registrar la versión exacta en `BITÁCORA_GPT.MD`.
- [ ] Configurar `Visible Meta Files` y `Force Text`.
- [ ] Añadir `.gitignore` oficial de Unity.
- [ ] Confirmar que `Assets`, `Packages` y `ProjectSettings` están versionados.
- [ ] Instalar y fijar paquetes necesarios: Input System, Test Framework, herramientas 2D y Cinemachine solo si aporta valor a la cámara.
- [ ] Crear estructura `_Project` y archivos `.asmdef`.
- [ ] Crear escenas `Bootstrap` y `Lab`.
- [ ] Establecer `main` como base compilable sin errores.

**Puerta de salida:** clonar el repo en otra carpeta permite abrir Unity y ejecutar `Lab` sin errores.

---

## Fase 2 — Calibración visual y jugador en laboratorio

**Objetivo:** validar el pipeline de sprites, movimiento, cámara y DualSense antes de migrar sistemas.

- [ ] Copiar el sprite del jugador a `Art/Raw/Characters` sin modificar el original.
- [ ] Configurar Sprite Mode Multiple, slicing `64 × 72`, filtro y compresión adecuados.
- [ ] Probar alternativas de Pixels Per Unit y documentar la seleccionada.
- [ ] Crear clips idle/walk y Animator.
- [ ] Crear asset de Input Actions con `Move`, `Interact`, `Cancel`, `HotbarSelect`, `HotbarPrevious` y `HotbarNext`.
- [ ] Crear prefab del jugador con colisión nativa.
- [ ] Implementar movimiento sobre el plano elegido para el 2.5D.
- [ ] Configurar cámara y seguimiento.
- [ ] Validar teclado y DualSense real en Windows 10.
- [ ] Añadir pruebas Play Mode básicas de movimiento y cambio de orientación.

**Decisión pendiente:** escoger tras la prueba si el mundo utilizará principalmente plano XZ con física 3D o XY con física 2D y profundidad simulada. No mezclar ambos modelos sin una razón documentada.

**Puerta de salida:** el personaje se mueve y anima correctamente en `Lab` con teclado y DualSense, sin jitter ni diferencias de escala.

---

## Fase 3 — Núcleo de dominio y pruebas Edit Mode

**Objetivo:** reconstruir las reglas sin depender de escenas.

Orden recomendado:

1. IDs y registros de objetos/cultivos.
2. `InventoryState` y stacks.
3. `StorageState` y transferencias.
4. `EconomyState`.
5. `GameTimeState`.
6. `FarmState` y parcelas.
7. herramientas y cosecha.
8. caja de envíos y procesador.
9. compras y restricciones estacionales.
10. `GameSession` y snapshots transaccionales.

- [ ] Crear ScriptableObjects para definiciones editables.
- [ ] Mantener estados de partida como C# serializable separado de los ScriptableObjects.
- [ ] Portar primero los casos unitarios de mayor valor del proyecto fuente.
- [ ] Probar límites, operaciones repetidas y operaciones que no deben mutar nada.
- [ ] Mantener IDs estables en lugar de referencias directas dentro de los archivos de guardado.

**Puerta de salida:** todas las reglas principales pasan en Edit Mode sin abrir una escena.

---

## Fase 4 — Primera vertical mínima en la granja

**Objetivo:** demostrar el ciclo completo con el menor contenido posible.

Contenido mínimo:

- una sección de la granja;
- jugador;
- una parcela;
- azada;
- regadera;
- semillas de nabo;
- un nabo con etapas visuales;
- hotbar;
- interacción;
- avance de día de prueba;
- guardado mínimo.

- [ ] Crear `GameRoot` persistente desde `Bootstrap`.
- [ ] Vincular dominio y presentación mediante adaptadores.
- [ ] Crear prefab de parcela y presentación por estado.
- [ ] Implementar sensor de interacción frontal.
- [ ] Implementar uso de herramienta seleccionada.
- [ ] Mostrar prompt y marcador de interacción.
- [ ] Añadir pruebas Play Mode del ciclo labrar → regar → sembrar → crecer → cosechar.

**Puerta de salida:** la prueba vertical completa funciona y puede guardarse/cargarse sin pérdida de estado.

---

## Fase 5 — Recrear la escena completa de la granja

**Objetivo:** trasladar la composición funcional de `FarmScene` usando Unity.

- [ ] Crear doce parcelas en una matriz 3 × 4.
- [ ] Crear casa, puerta y spawn exterior.
- [ ] Crear caja de envíos.
- [ ] Crear estanque con collider nativo.
- [ ] Crear árboles como prefabs con collider y orden visual.
- [ ] Crear límites y salida norte.
- [ ] Usar Gizmos, Physics Debugger y herramientas nativas para validar geometría.
- [ ] Conservar `960 × 540` solo como referencia visual; no convertirlo en una dependencia rígida del gameplay.
- [ ] Documentar la conversión utilizada entre coordenadas del prototipo y unidades Unity.

**Puerta de salida:** la escena permite recorrer, cultivar e interactuar sin atravesar objetos ni requerir un editor de colisiones propio.

---

## Fase 6 — Casa, almacenamiento y cierre de día

**Objetivo:** completar el segundo escenario y el ciclo diario.

- [ ] Crear `HouseInterior`.
- [ ] Implementar puerta y spawns de entrada/salida.
- [ ] Implementar cama y confirmación de sueño si se decide conservarla.
- [ ] Implementar cofre y transferencias.
- [ ] Procesar crecimiento y riego al dormir.
- [ ] Procesar envíos y economía.
- [ ] Avanzar fecha y volver al spawn de cama.
- [ ] Mostrar resumen del nuevo día.
- [ ] Guardar después de una transición diaria exitosa.

**Puerta de salida:** dormir completa una transacción diaria coherente, incluso con cultivos y envíos simultáneos.

---

## Fase 7 — Pueblo, tienda y tres cultivos

**Objetivo:** alcanzar la paridad de contenido del commit fuente.

- [ ] Crear `VillageNorth`.
- [ ] Implementar tienda de semillas.
- [ ] Añadir selección de cultivo y cantidades 1, 5 y 10.
- [ ] Añadir nabo, zanahoria y col como ScriptableObjects.
- [ ] Aplicar restricciones de primavera en compra y siembra.
- [ ] Mostrar precios, días y temporada.
- [ ] Añadir controles completos de teclado, mouse y gamepad en menús.
- [ ] Probar compra con fondos exactos, fondos insuficientes e inventario lleno.

**Puerta de salida:** el flujo granja → mercado → compra → granja → cultivo → envío → sueño funciona con los tres cultivos.

---

## Fase 8 — Guardado robusto y compatibilidad

**Objetivo:** superar el guardado mínimo y establecer el contrato permanente.

- [ ] Crear DTO explícito de guardado, sin serializar `MonoBehaviour`, GameObjects ni ScriptableObjects completos.
- [ ] Implementar versión de formato.
- [ ] Validar IDs, cantidades, índices, fechas y etapas antes de restaurar.
- [ ] Implementar escritura atómica con archivo temporal y reemplazo seguro.
- [ ] Implementar rollback de memoria si la restauración falla.
- [ ] Probar archivo ausente, corrupto, incompleto, futuro y legado.
- [ ] Evaluar un importador opcional del save web v2; no bloquear la migración principal por esta compatibilidad.

**Puerta de salida:** ninguna carga inválida puede destruir la sesión actual y existe una ruta documentada para migraciones futuras.

---

## Fase 9 — Transformación visual 2.5D

**Objetivo:** pasar de paridad mecánica a identidad visual de Unity.

- [ ] Definir cámara final: ortográfica o perspectiva controlada.
- [ ] Definir eje de profundidad y reglas de sorting.
- [ ] Sustituir formas provisionales por sprites, tiles, prefabs o modelos autorizados.
- [ ] Configurar iluminación URP, sombras y materiales.
- [ ] Reconstruir el estanque con herramientas nativas.
- [ ] Añadir vegetación y ambiente sin acoplarlos a las reglas de dominio.
- [ ] Medir rendimiento en la PC objetivo.
- [ ] Validar legibilidad y sensación con Arturo.

**Puerta de salida:** el juego conserva sus reglas, pero ya se percibe como un proyecto 2.5D propio y no como una reproducción literal del canvas web.

---

## Fase 10 — Herramientas específicas de contenido

**Objetivo:** acelerar la expansión sin recrear el editor de Unity.

Herramientas candidatas, solo después de necesitarse realmente:

- navegador de cultivos y objetos;
- creación asistida de `ItemDefinition` y `CropDefinition`;
- validador de IDs duplicados;
- validador de prefabs sin collider, sprite o referencia obligatoria;
- importador estandarizado de sprite sheets;
- generador de Animation Clips;
- reporte de licencias y procedencia de assets;
- validador de escenas y spawns.

No crear una herramienta si Hierarchy, Inspector, Prefabs, Sprite Editor, Tile Palette, Grid, Physics Debugger o Animation ya resuelven adecuadamente la necesidad.

**Puerta de salida:** cada herramienta creada elimina trabajo repetitivo concreto y tiene pruebas o validación reproducible.

---

## Fase 11 — CI, regresiones y cierre de paridad

**Objetivo:** dejar el proyecto listo para crecer con seguridad.

- [ ] Ejecutar pruebas Edit Mode y Play Mode automáticamente.
- [ ] Crear build de Windows automatizado cuando el proyecto sea estable.
- [ ] Registrar cada bug confirmado en `BUGS.MD`.
- [ ] Añadir una prueba de regresión por bug automatizable.
- [ ] Crear una matriz de paridad contra el commit fuente.
- [ ] Validar manualmente DualSense, escenas, menús, arte y sensación.
- [ ] Mantener `main` compilable y jugable.
- [ ] Archivar el prototipo web como referencia, no eliminarlo.

**Puerta de salida:** todos los contratos de la sección 5 están verificados y el desarrollo futuro puede continuar exclusivamente en Unity.

---

## 7. Estrategia de pruebas migrada

### Edit Mode

Prioridad alta:

- inventario y stacks;
- cultivos y parcelas;
- calendario y estaciones;
- economía;
- tienda y restricciones;
- caja de envíos;
- almacenamiento;
- snapshots, validación, rollback y migraciones.

### Play Mode

Prioridad alta:

- input y movimiento;
- selección de interactuable;
- transiciones y spawns;
- hotbar y uso de herramientas;
- navegación de menús con gamepad;
- ciclo completo de cultivo;
- cierre del día;
- guardado y recarga dentro del juego.

### Validación humana

Arturo validará especialmente:

- sensación del movimiento;
- DualSense físico;
- cámara;
- pivots y animaciones;
- colisiones percibidas;
- legibilidad de UI;
- arte, iluminación y atmósfera;
- rendimiento y sesiones prolongadas.

---

## 8. Riesgos y protecciones

### Riesgo: traducir demasiado código web

**Protección:** usar el código fuente como especificación y sus pruebas como catálogo de comportamientos.

### Riesgo: acoplar la lógica a MonoBehaviour

**Protección:** mantener dominio y snapshots en ensamblados sin `UnityEngine`.

### Riesgo: reconstruir herramientas que Unity ya ofrece

**Protección:** justificar cada herramienta personalizada con una necesidad específica del simulador.

### Riesgo: perder IDs y romper guardados futuros

**Protección:** usar IDs de texto estables y registros validados.

### Riesgo: mezclar datos de definición con estado de partida

**Protección:** ScriptableObjects para definiciones; DTO serializable para progreso.

### Riesgo: migrar todo antes de probar el flujo Unity–GitHub

**Protección:** completar primero `Lab` y una vertical de una parcela/un cultivo.

### Riesgo: imponer la geometría 960 × 540 al 2.5D

**Protección:** usarla solo como referencia y documentar una conversión a unidades Unity.

### Riesgo: olvidar la procedencia de assets

**Protección:** conservar originales y registrar licencia, autor, fuente y modificaciones.

---

## 9. Estado de esta migración

### Completado

- [x] Acceso y estudio del repositorio fuente.
- [x] Auditoría de arquitectura, módulos, escenas, datos, input, guardado y pruebas.
- [x] Confirmación del HEAD fuente y del PR #43 fusionado.
- [x] Clasificación de sistemas: conservar, reconstruir o descartar.
- [x] Definición del plan por fases y criterios de paridad.

### Próximo paso inmediato

1. Arturo termina la instalación de Unity.
2. Se registra la versión exacta instalada.
3. Se crea el proyecto Unity dentro del repositorio actual.
4. Se completa la Fase 1.
5. Se inicia la escena `Lab` y la calibración del jugador.

---

## 10. Regla de actualización de esta bitácora

Después de cada fase o cambio material, registrar:

- fecha;
- fase y objetivo;
- decisiones tomadas;
- archivos/sistemas creados;
- equivalencia alcanzada con el proyecto fuente;
- pruebas realmente ejecutadas;
- validación manual pendiente o completada;
- bugs relacionados;
- siguiente acción concreta.

No marcar una fase como completada únicamente porque exista código. Debe cumplir su puerta de salida y quedar verificable en el repositorio.
