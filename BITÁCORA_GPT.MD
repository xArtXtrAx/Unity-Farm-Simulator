# BITÁCORA GPT — Unity Farm Simulator

Documento maestro de continuidad. Debe permitir reanudar el proyecto sin depender de la memoria del chat.

> **Regla de continuidad:** antes de modificar el repositorio, leer `CONTINUIDAD_GPT.md` desde `main`; después leer este archivo, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, ramas, PR y commits recientes.

> **Regla transaccional:** ninguna implementación, corrección o integración realizada por GPT se considera terminada hasta registrar aquí lo realizado, lo comprobado, lo pendiente y el siguiente paso exacto.

> **Regla de validación:** no marcar una corrección como `VERIFICADA` únicamente porque el código fue publicado. La verificación requiere confirmación local de Arturo sobre compilación, comportamiento visual o funcional y pruebas relevantes.

---

## 1. Identidad y estado operativo

- **Repositorio:** `xArtXtrAx/Unity-Farm-Simulator`
- **Propietario:** Arturo García (`xArtXtrAx`)
- **Motor:** Unity 6.3 LTS `6000.3.21f1`
- **Plantilla:** Universal 3D / URP
- **Plataforma inicial:** Windows PC
- **Control prioritario:** DualSense, además de teclado y mouse
- **Ruta local habitual:** `D:\Git\Unity-Farm-Simulator`
- **Rama estable:** `main`
- **Head integrado más reciente:** `6b38b2b9c71d5c1ac5d7db12ff4caf82811d449d`
- **Integración más reciente:** PR #16 — `Synchronize project continuity documentation`
- **Rama activa:** `dev/farm-scene-authoring`
- **Base histórica de la rama activa:** `main` después del merge del PR #15
- **Proyecto fuente de reglas:** `xArtXtrAx/farming-game-A`
- **Commit fuente congelado:** `dd32056c9f8142a2322bc2c1d41f0b05b002598f`
- **Identidad Unity:** `jarturoga` / `Unity Farm Simulator` / `com.jarturoga.unityfarmsimulator`

`CONTINUIDAD_GPT.md` quedó sincronizado con el estado posterior al PR #15 mediante el PR #16. Desde ese merge vuelve a ser el punto de entrada operativo válido leído desde `main`, mientras esta bitácora conserva el detalle transaccional.

---

## 2. Contrato técnico vigente

- Vista cenital y plano lógico XY.
- Physics 2D.
- Cámara ortográfica frontal.
- Resolución lógica 960 × 540, 16:9.
- Frame del jugador: 64 × 72 px.
- Pivote cercano a los pies: X 0.5 / Y 0.12.
- Teclado y DualSense.
- Profundidad visual basada en la coordenada Y de los pies.
- `FarmSimulator.Domain` no depende de `UnityEngine`.
- Los Tilemaps representan contenido estático del mundo.
- Los cultivos con estado y crecimiento son entidades runtime con `SpriteRenderer`.
- Los edificios separan tres conceptos independientes: visual, collider físico y footprint lógico.

Arquitectura:

- `FarmSimulator.Domain`
- `FarmSimulator.Application`
- `FarmSimulator.Infrastructure`
- `FarmSimulator.Presentation`
- `FarmSimulator.Editor`
- `FarmSimulator.Tests.EditMode`
- `FarmSimulator.Tests.PlayMode`

Regla de dependencias:

```text
Domain
  ↑
Application
  ↑
Infrastructure / Presentation
```

---

## 3. Línea histórica integrada

Las fases iniciales permanecen integradas:

1. Cimentación del proyecto Unity/URP y assemblies.
2. Calibración cenital XY, cámara ortográfica y resolución lógica.
3. Movimiento por teclado y DualSense.
4. Héroe animado en cuatro direcciones.
5. Prefab reutilizable y profundidad visual por Y.
6. Catálogo de objetos e inventario puro en Domain.
7. Piloto artístico Cozy Farm, slicing curado y calibración visual.
8. Hotbar uGUI de ocho slots conectada a `InventoryState`.
9. Casa del héroe, interior, cama, sueño y avance de día.
10. Núcleo agrícola con arado, riego, siembra, crecimiento y cosecha.
11. Tilemaps separados para `Ground`, `Paths`, `Soil` y `Decoration`.
12. Farm Development Kit para catálogo, generación y colocación de edificios Cozy Farm.

Última línea base de pruebas confirmada antes del bloque de tiles estacionales:

- EditMode completo: **211/211**.
- PlayMode previamente confirmado: **10/10**.

Estos conteos no deben extrapolarse automáticamente a los cambios posteriores. Después de los últimos ajustes de editor, cámara, límites y colliders sigue pendiente una ejecución completa nueva.

---

## 4. Arquitectura agrícola y Tilemaps vigente

Jerarquía conceptual:

```text
Farm Authoring Grid
├── Ground
├── Paths
├── Soil
└── Decoration

Farm Plot Field
└── Plot x-y
    ├── Soil Visual
    └── Crop Entity Visual (SpriteRenderer)
```

Decisiones:

- `Ground`, `Paths`, `Soil` y `Decoration` se pintan con tiles.
- Las semillas, plantas y cosechas no son tiles pintables.
- `FarmPlotBehaviour` controla la etapa visual del cultivo.
- Un cultivo nunca reemplaza el tile del suelo.
- La escena no debe reconstruirse para reparar una capa de terreno.
- Cualquier herramienta de recuperación debe operar sobre la escena existente, usar Undo y preservar objetos y tiles manuales.

El detalle histórico de esta migración se conserva también en `BITÁCORA_TILE_PIPELINE_2026-08-06.md`.

---

## 5. Farm Development Kit — edificios

Los edificios reutilizables usan esta estructura:

```text
Building Root
├── Building Composition
│   └── Building Visual
├── Footprint Anchor
├── Door Anchor
├── Portal Anchor
└── Spawn Anchor
```

Responsabilidades:

- `Building Visual`: representación artística.
- `BoxCollider2D`: colisión física del edificio.
- `GridBuildingFootprint`: máscara lógica de celdas ocupadas.
- `Footprint Anchor`: origen local de la huella lógica.
- `Door/Portal/Spawn Anchor`: puntos de transición y aparición.

### Footprint Editor

El editor permite:

- escoger una definición de edificio;
- visualizar bounds del sprite, collider y footprint;
- editar el lienzo lógico y celdas ocupadas;
- editar `Footprint Anchor`;
- rellenar o limpiar la máscara;
- restaurar el footprint predeterminado de categoría;
- guardar y regenerar el prefab.

La corrección del lienzo par centralizó el origen horizontal en:

- `GridBuildingFootprint.GetCanvasMinimumX()`;
- `GridBuildingFootprint.CreateRectangleOffsets()`.

`BUG-0014` continúa registrado como `CORREGIDO`, pendiente de una nueva suite EditMode explícita antes de marcarlo `VERIFICADO`.

### Autoría del collider desde Footprint Editor

Implementación publicada:

- `19bb965b15541819062340eb5a93e8f3e0427b72` — persistencia de geometría de collider autorizada.
- `8d33e728bd8e12f89c0fb03413b3effcf3c90cb7` — controles de collider en Footprint Editor.
- `170c341727e3272df67753c07e31921ea83b4fc9` — documentación del incremento.

Controles disponibles:

- `Collider Center`;
- `Collider Size`;
- paso de ajuste fino;
- movimiento izquierda, derecha, arriba y abajo;
- ensanchar, estrechar, aumentar o reducir altura;
- alinear el borde inferior con la base visual;
- restaurar el collider del catálogo;
- previsualización naranja inmediata con centro marcado;
- `Save + regenerate prefab` para aplicar al `BoxCollider2D` generado.

Espacio de autoría:

```text
X = 0 → centro horizontal del edificio
Y = 0 → base visual inferior del edificio
```

Las definiciones guardan si el collider fue autorizado manualmente. `Rebuild definitions` actualiza el valor original del catálogo, pero no sobrescribe un collider autorizado. Solo `Reset collider to catalog default` elimina la personalización.

**Validación confirmada por Arturo:** la herramienta visual le agradó y funciona como editor de collider.

**Pendiente:** ejecutar EditMode completo y validar en Play la colisión física de un prefab regenerado antes de considerar el incremento totalmente verificado.

---

## 6. Selector de tiles estacionales

Rama de desarrollo histórica: `dev/seasonal-tile-browser`.

Commits principales:

- `34cd57780d008bca65201a1e071b0e69ce80ce0e` — selector estacional inicial.
- `bbdb64b6ecfd9fa5bfffc40343c844fd6843be62` — corrección de compilación.
- `186f6e66cab7f2079e7c918c9ae2d089160e431a` — metadata.
- `cc42802702def1cccda4ef9579bde9ffcdf34c84` — thumbnails reales.
- `3818ecf344c0e6cdca518d4073f193a96216ce16` — arrastre directo a la Scene.

La ventana:

- analiza las texturas del paquete Cozy Farm Full/Tiles;
- corta sprites de 16 × 16;
- organiza por estación;
- muestra thumbnails reales;
- genera assets `Tile` únicamente cuando se necesitan;
- permite arrastrar un tile directamente sobre el Tilemap de la Scene;
- respeta la capa seleccionada y registra Undo.

Ruta de tiles generados:

```text
Assets/_Project/Art/Generated/CozyFarm/SeasonalTiles/{Season}/Tiles
```

### Tinte rosa al entrar en Play

El terreno completo aparecía rosa cuando la Tile Palette conservaba un objetivo de pintura activo. La causa observada fue el estado de foco/tinte del editor, no un shader roto ni una textura perdida.

Corrección:

- `6295b506eaff833f88a986447a30c83318d11c3a` — limpiar el objetivo de pintura de Tile Palette antes de entrar en Play.

La corrección debe mantenerse como protección de editor. No se ha registrado una ejecución completa de regresión posterior específica para este cambio.

---

## 7. Reset antiguo de la granja — desactivado

Se identificó que `FarmSceneGridLayoutResetter` era una herramienta heredada destructiva. Su inicialización automática podía ejecutar una reconstrucción antigua de `Farm`, reemplazando composición, fachada y decoración actuales.

El comando antiguo llegó a ejecutarse manualmente durante el diagnóstico y reconstruyó una versión obsoleta de la escena. Esto confirmó que no era una herramienta válida para reparar césped o límites.

Corrección:

- `7711e95ac1994c526133d6074683c70ee63a1a26` — desactivar la ejecución automática y mover el comando a `Legacy` con confirmación explícita.

Estado actual:

- `EnsureApplied()` no reconstruye nada.
- No existe reset automático al abrir el proyecto.
- El comando está bajo:

```text
Tools/Farm Simulator/Legacy/Reset Farm To Old Starter Layout
```

Regla vigente: no recomendar ni ejecutar este comando para reparar terreno. Las reparaciones de Tilemap deben modificar solo la capa afectada de la escena existente.

---

## 8. Tamaño de escenas, cámara y área jugable

### Scene Size

Commits iniciales:

- `b8cef00de5850eeb4fbde862061ecb119685107c` — selector de tamaño para Farm e interior.
- `c4cc4c367df3c62cdf249919de275b0daed35e57` — metadata.

Ruta:

```text
Tools
→ Farm Simulator
→ Farm Development Kit
→ Scene Size
```

La ventana permite seleccionar:

- `Farm`;
- `HouseInterior`.

Y configurar:

- ancho en celdas;
- alto en celdas;
- celda central;
- para Farm, relleno opcional de celdas vacías de `Ground` con césped;
- limpieza opcional y explícitamente destructiva de tiles fuera del límite.

La herramienta no debe reconstruir casas, decoración o sistemas de escena.

### Cámara de seguimiento

Commits:

- `b80a74ad4a5911e09301f58f518e92e04b1a9219` — `PlayerFollowCamera2D`.
- `e571301ea5a8bc8f49af637129ee97c8fc51a079` — metadata.
- `e739e780f1356e37e7fd1479ce67ddc2025b55bc` — configuración desde Scene Size.
- `255c64b56574e9d05b005b7f162269665e19369f` — limpieza de placeholder.
- `68ee45992fa28769a6868be5941776ee889fa96f` — documentación.

Configuración de Farm:

- modo `Follow Player`;
- zoom fijo expresado como altura visible en celdas;
- seguimiento suavizado;
- clamp opcional a `Scene Authoring Bounds`.

El tamaño del mapa y el zoom son conceptos separados:

```text
Tamaño del mapa = cuántas celdas existen
Vista de cámara  = cuántas celdas se ven simultáneamente
```

### Sincronización del área física recorrible

El primer selector modificaba el área visual/lógica, pero el jugador seguía bloqueado por los colliders antiguos de `Movement Boundary`.

Corrección:

- `7f580db74e2e447642f83389f970f2e33125b556` — sincronizar los límites físicos con el tamaño autorizado.
- `37e74aad1236d167ad9cb631689fec19034c1277` — metadata.

Jerarquía resultante:

```text
Movement Boundary
├── Boundary Left
├── Boundary Right
├── Boundary Bottom
└── Boundary Top
```

Los cuatro colliders quedan fuera del rectángulo jugable para no perder media celda en los bordes.

**Validación confirmada por Arturo:** después de reaplicar el tamaño, el personaje pudo recorrer el área ampliada correctamente. Este comportamiento funcional queda aprobado.

**Pendiente:** ejecutar suites completas después de la integración para registrar una línea base nueva.

---

## 9. Integración del PR #15

PR:

```text
#15 — Consolidate seasonal tiles, scene sizing and collider authoring
```

Rama fuente:

```text
dev/seasonal-tile-browser
```

Base:

```text
main
```

Contenido integrado:

- selector estacional de tiles;
- thumbnails reales y arrastre a Scene;
- protección contra tinte de Tile Palette al entrar en Play;
- desactivación del reset automático antiguo;
- selector de tamaño para Farm e interior;
- cámara de seguimiento acotada;
- sincronización de límites físicos con el tamaño de escena;
- edición persistente del collider desde Footprint Editor;
- documentación del pipeline de tiles y edificios.

Merge commit:

```text
794aab44916f974bc885aec0a203823041c72f8f
```

Estado de validación al integrar:

- expansión del área jugable: **validada por Arturo**;
- editor visual del collider: **validado por Arturo**;
- suite EditMode completa posterior: **pendiente**;
- suite PlayMode completa posterior: **pendiente**;
- validación física exhaustiva de colliders autorizados: **pendiente**.

No afirmar que todos los cambios del PR #15 están verificados hasta cerrar esas pruebas.

---

## 10. Sincronización documental — PR #16

PR:

```text
#16 — Synchronize project continuity documentation
```

Merge commit:

```text
6b38b2b9c71d5c1ac5d7db12ff4caf82811d449d
```

Resultado:

- `CONTINUIDAD_GPT.md` en `main` refleja el estado posterior al PR #15;
- `BITÁCORA_GPT.MD` conserva el estado maestro y los pendientes reales;
- se eliminó la instrucción operativa de tratar el punto de entrada como obsoleto;
- no hubo cambios de código, escenas, assets ni configuración de Unity.

---

## 11. Bugs y documentación relacionada

`BUGS.MD` contiene actualmente:

- `BUG-0001` a `BUG-0008` verificados;
- `BUG-0009`, `BUG-0010`, `BUG-0011` y `BUG-0014` corregidos, con validaciones pendientes según su registro.

Los problemas recientes que deben quedar reflejados o conciliados en el reporte de bugs son:

- reset automático heredado capaz de sobrescribir `Farm`;
- límites físicos que no acompañaban el tamaño autorizado;
- tinte rosa de Tile Palette en Play.

La integración del PR #15 no actualizó completamente `BUGS.MD` ni `BUGS_TILE_PIPELINE_2026-08-06.md`. Esto sigue siendo deuda documental y no debe ocultarse.

---

## 12. Rama activa actual

Después del merge del PR #15 se creó:

```text
dev/farm-scene-authoring
```

Esta rama fue creada desde el `main` integrado en `794aab44916f974bc885aec0a203823041c72f8f` y se mantiene como rama activa para el siguiente incremento.

Propósito general:

- continuar herramientas y autoría segura de escenas de granja e interiores;
- evitar regeneradores destructivos;
- conservar las escenas existentes y aplicar cambios específicos con Undo;
- registrar cada incremento en esta bitácora antes de considerarlo cerrado.

---

## 13. Estado comprobado, pendiente y siguiente paso exacto

### Comprobado

- El área jugable de Farm se expande al cambiar ancho y alto.
- La cámara puede seguir al jugador manteniendo zoom fijo.
- `Movement Boundary` se sincroniza con los límites autorizados.
- Footprint Editor permite modificar visualmente centro y tamaño del collider.
- El reset antiguo dejó de ejecutarse automáticamente.
- El PR #15 está fusionado en `main`.
- `CONTINUIDAD_GPT.md` y esta bitácora quedaron sincronizados mediante el PR #16.
- La rama `dev/farm-scene-authoring` permanece como rama de trabajo.

### Pendiente

- Ejecutar EditMode completo sobre la línea posterior al PR #15.
- Ejecutar PlayMode completo sobre la misma línea.
- Validar un prefab regenerado con collider personalizado dentro de Farm.
- Confirmar que `Rebuild definitions` conserva el collider autorizado.
- Confirmar que `Reset collider to catalog default` restaura correctamente el valor original.
- Conciliar `BUGS.MD` y `BUGS_TILE_PIPELINE_2026-08-06.md` con los incidentes recientes.

### Siguiente paso exacto

1. Permanecer en `dev/farm-scene-authoring`.
2. Sincronizar la rama local con el `main` posterior al PR #16 antes de implementar.
3. Abrir Unity y confirmar compilación sin errores.
4. Ejecutar EditMode completo y registrar el conteo real.
5. Ejecutar PlayMode completo y registrar el conteo real.
6. Regenerar una casa con collider personalizado y probar la colisión en Play.
7. Ejecutar `Rebuild definitions`, reabrir la definición y confirmar que el collider personalizado permanece.
8. Probar `Reset collider to catalog default` en una definición de prueba.
9. Actualizar `BUGS.MD` y `BUGS_TILE_PIPELINE_2026-08-06.md` con resultados reales.
10. Solo después escoger el siguiente incremento funcional de autoría de escenas.

---

## 14. Prompt vigente para un chat nuevo

> Continúa `xArtXtrAx/Unity-Farm-Simulator`. Lee primero `CONTINUIDAD_GPT.md` desde `main`; está sincronizado mediante el PR #16. Después lee `BITÁCORA_GPT.MD` desde `dev/farm-scene-authoring`, `MIGRACION_DESDE_FARMING_GAME_A.MD`, `BUGS.MD`, `BITÁCORA_TILE_PIPELINE_2026-08-06.md` y los commits recientes. El PR #15 fue fusionado mediante `794aab44916f974bc885aec0a203823041c72f8f` e integra el selector estacional de tiles, protección de Tile Palette, Scene Size, cámara de seguimiento, límites físicos sincronizados y edición persistente del collider. El PR #16 sincronizó la documentación maestra mediante `6b38b2b9c71d5c1ac5d7db12ff4caf82811d449d`. Arturo validó la expansión del área jugable y el editor visual del collider. Las suites completas posteriores al PR #15 y la validación física exhaustiva del collider siguen pendientes. La rama activa es `dev/farm-scene-authoring`. No uses regeneradores antiguos para reparar escenas y no marques bugs como verificados sin confirmación local.
