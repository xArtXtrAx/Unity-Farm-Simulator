# Bitácora — Pipeline de Tilemaps y cultivos como entidades

Fecha: 2026-08-06  
Rama: `agent/cozy-art-pipeline`  
PR: #14 — `Automate Cozy Tile Palette authoring`

Este documento es un anexo transaccional de `BITÁCORA_GPT.MD`.

## Estado validado antes de este cambio

- Unity 6.3 LTS `6000.3.21f1`.
- `com.unity.2d.tilemap` instalado.
- Tile Manager y paletas automáticas operativas.
- EditMode completo aprobado.
- PlayMode aprobado **10/10**.
- Transparencia y centrado manual de cultivos seguían mostrando defectos al pintarlos como tiles.

## Decisión de arquitectura

Los cultivos dejan de ser tiles de autoría.

La estructura final es:

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

Responsabilidades:

- `Ground`, `Paths`, `Soil` y `Decoration` se pintan con Tile Palette.
- semillas, brotes, plantas maduras y cosechas son sprites runtime propiedad de `FarmPlotBehaviour`.
- el estado de crecimiento sigue viviendo en `FarmPlotState`.
- `FarmPlotBehaviour.Render()` cambia el sprite de la entidad visual al avanzar el día.
- una planta nunca sustituye el tile de suelo porque ya no ocupa una celda del Tilemap.

Esta separación sigue el patrón habitual de los simuladores agrícolas 2D: el mapa estático se autoriza con Tilemaps y los elementos con estado, crecimiento e interacción se representan como entidades runtime. Esto permite cultivos de distintos tamaños, animaciones, sombras, partículas y estados especiales sin quedar limitados a una celda de Tilemap.

## Implementación

- `FarmTilemapLayers` elimina la referencia `Crops`.
- `CozyPaletteCategory` queda en `Ground`, `Paths`, `Soil` y `Decoration`.
- se elimina la paleta agrícola pintable `Cozy Farm - Crops`.
- `LegacyCropPaletteCleanup` borra la paleta antigua al recompilar.
- `CozyFarmTileCatalog` continúa generando 18 sprites transparentes, pero exclusivamente para runtime.
- `FarmSceneFarmingUpgrader` genera `Farming Core Loop v4`.
- cada parcela contiene un `Crop Entity Visual` con `SpriteRenderer`.
- el runtime carga las seis etapas transparentes de nabo, zanahoria y col.
- la posición, escala y etapa se actualizan sin modificar ningún Tilemap.

Commits principales:

- `d731f7d3c1fe9ee1630e95f1e958939a60a3d3ce`
- `0d4796dfeb93bc2dbf7fb35a437ef2682ffd0a30`
- `0132fea7b46c184b19785a630ad7151cfad1b8e2`
- `25027bdcb1f8da0f3e0901e4f42e4eaf97ef1025`
- `552c15054b73507c1b8399940f4a6c656b8234a8`
- `dfefec9eca7a20048af791d1309bca7d9c0adb8f`
- `7232212635f187e489696eb499879e3920c4e186`

## Regresión de compilación detectada

Durante el refactor de `FarmPlotBehaviour`, `ValidateStages()` utilizó `Sprite[].Any(...)` sin importar `System.Linq`. Unity detuvo la compilación con `CS1061`.

Se sustituyó la consulta LINQ por un ciclo explícito que valida cada entrada del arreglo. La solución evita una dependencia innecesaria y no genera enumeradores ni asignaciones adicionales.

- Bug: `BUG-0011`.
- Corrección: `02d1d696137a2692983e02d342ded5a7f41020df`.
- Estado: **CORREGIDO**, pendiente de confirmar compilación y suites completas en Unity.

## Pruebas actualizadas

`FarmingScenePipelineTests` comprueba:

- nueve parcelas configuradas;
- cuatro Tilemaps de autoría;
- ausencia de un Tilemap `Crops`;
- un `SpriteRenderer` de cultivo por parcela;
- cuatro paletas de mundo;
- 18 sprites runtime importados con transparencia, 16 PPU y pivote central.

## Reconstrucción de la casa con el atlas completo

La fachada modular basada en paneles del paquete piloto fue retirada. La casa exterior ahora se obtiene directamente de:

```text
Assets/_Project/Art/ThirdParty/CozyFarm/Full/Buildings/buildings.png
```

`CozyFarmBuildingCatalog` centraliza los rectángulos de origen del atlas y genera sprites transparentes reutilizables en:

```text
Assets/_Project/Art/Generated/CozyFarm/Buildings
```

La primera variante es `starter-green-gable-house.png`. El generador:

- lee el PNG original sin alterar su importador;
- extrae una casa completa mediante un `RectInt` documentado;
- detecta los límites alfa visibles;
- recorta el espacio vacío;
- conserva transparencia real;
- importa a 16 PPU, filtro Point, sin mipmaps ni compresión;
- utiliza pivote inferior centrado para apoyar la fachada sobre el suelo.

`CozyFarmHouseExteriorUpgrader` usa un único `SpriteRenderer` para la casa completa y conserva el root funcional, portal, collider y puntos de aparición. La estructura generada es:

```text
Hero House Exterior
└── Cozy Full-Pack House v4
    ├── Starter Green Gable House
    └── Entrance Grounding Shadow
```

Commits:

- `f4ddd073822f93493ca4f16eb717ddd3499d7d9b`
- `3782565ee49a30235ba17b23308c7a38a894c525`
- `e0d8c96a471ff191abb040351c1f1b00013d210b`
- `854fd76cc125e004db1a093a037c356db42e1670`

La selección visual exacta del rectángulo del atlas queda pendiente de validación local en Unity. Si requiere ajuste, solo se modifica `StarterHouseSource`; la escena y la lógica no cambian.

## Próximo paso exacto

1. Hacer Pull de `agent/cozy-art-pipeline`.
2. Esperar compilación e importación.
3. Ejecutar `Generate Cozy Full-Pack Building Sprites`.
4. Ejecutar `Apply Cozy House Exterior To Farm Scene`.
5. Abrir `Farm` y validar la casa completa, transparencia, escala y alineación de la puerta.
6. Entrar en Play Mode y comprobar portal, collider y regreso desde el interior.
7. Ejecutar EditMode y PlayMode completos.
8. Si el recorte incluye piezas vecinas o corta parte del edificio, ajustar exclusivamente `CozyFarmBuildingCatalog.StarterHouseSource`.

## Exclusiones

No se modificaron reglas de crecimiento, consumo de semillas, inventario persistente, sueño, portales, interiores ni collider del héroe.

---

## Traspaso para la próxima ventana

### Estado al cerrar esta ventana

- Rama activa: `agent/cozy-art-pipeline`.
- PR activo: #14, todavía en borrador.
- Último commit funcional y documental: `854fd76cc125e004db1a093a037c356db42e1670`.
- La casa modular `Cozy House Facade v3` fue sustituida en el reconstruidor por una casa completa procedente del atlas Full-Pack.
- El sprite generado esperado es:

```text
Assets/_Project/Art/Generated/CozyFarm/Buildings/starter-green-gable-house.png
```

- El root funcional `Hero House Exterior` no se reemplaza. Se conservan portal, collider, punto de aparición y transición al interior.
- El rectángulo del atlas y los datos de la variante inicial están centralizados en `CozyFarmBuildingCatalog`; cualquier corrección visual debe hacerse allí, no reconstruyendo la escena a mano.
- La integración todavía no se considera validada hasta verla en la instalación local de Unity del usuario.

### Prueba que debe realizar el usuario

Después de hacer **Fetch origin → Pull origin** y esperar a que Unity termine de importar y compilar:

1. Ejecutar:

```text
Tools → Farm Simulator → Generate Cozy Full-Pack Building Sprites
```

2. Confirmar que se genera el archivo:

```text
Assets/_Project/Art/Generated/CozyFarm/Buildings/starter-green-gable-house.png
```

3. Ejecutar:

```text
Tools → Farm Simulator → Apply Cozy House Exterior To Farm Scene
```

4. Abrir `Assets/_Project/Scenes/Farm.unity` y comprobar en modo 2D:

- aparece una casa completa real del atlas `buildings.png`;
- el fondo alrededor de la casa es transparente;
- no se incluyen fragmentos de casas o accesorios vecinos;
- el techo, paredes y base no están recortados;
- la escala es coherente con el héroe y el resto del escenario;
- la puerta queda razonablemente alineada con el acceso actual;
- la jerarquía contiene `Cozy Full-Pack House v4`;
- la Console no muestra errores rojos.

5. Entrar en Play Mode y validar:

- el héroe no atraviesa las paredes de la casa;
- puede acercarse a la puerta y entrar;
- la escena interior carga correctamente;
- puede salir y reaparece frente a la casa;
- el hotbar, las parcelas y el ciclo agrícola continúan funcionando;
- no reaparece `Inventory hotbar has not been initialized`.

6. Ejecutar las suites completas:

```text
EditMode → Run All
PlayMode → Run All
```

### Evidencia solicitada

Para continuar en la próxima ventana, registrar uno de estos resultados:

- **Aprobado:** captura de la casa en Scene o Game, confirmación del portal y total de pruebas aprobadas.
- **Recorte incorrecto:** captura donde se vea qué borde está cortado o qué elemento vecino aparece.
- **Escala o puerta incorrectas:** captura frontal con el héroe junto a la fachada.
- **Error técnico:** texto completo del primer error rojo de Console y archivo/línea indicados por Unity.

### Plan posterior a la validación

1. **Corrección visual mínima, si es necesaria**
   - ajustar `StarterHouseSource`, pivote, escala o desplazamiento local;
   - no modificar el portal ni la lógica de escenas salvo que la puerta real requiera mover el punto de interacción;
   - añadir o actualizar una prueba de regresión para el dato corregido.

2. **Catálogo de variantes de edificios**
   - registrar varias casas del atlas con identificadores estables;
   - incluir nombre, región de origen, pivote, escala recomendada y ancla de puerta;
   - generar todos los sprites mediante una sola orden reproducible.

3. **Selector de casa en Unity**
   - ampliar la herramienta de editor para escoger una variante desde un menú o desplegable;
   - reconstruir la fachada sin tocar portal, collider ni escenas manualmente;
   - permitir futuras mejoras o estaciones visuales.

4. **Metadatos funcionales por edificio**
   - definir ancla de entrada, límites de colisión y punto de sombra para cada variante;
   - alinear automáticamente portal y collider con el estilo seleccionado;
   - preparar la misma infraestructura para granero, molino, mercado y otros edificios del atlas.

5. **Continuación del pipeline de arte**
   - ampliar el slicer/catálogo del Full-Pack para terreno, caminos, agua, vegetación, cercas y decoración;
   - crear paletas más completas y categorías legibles;
   - evaluar Rule Tiles para conexiones automáticas de caminos, agua y cercas.

6. **Cierre de la iteración**
   - ejecutar EditMode y PlayMode completos;
   - actualizar `BUGS.MD` con cualquier regresión encontrada;
   - actualizar esta bitácora con la variante aprobada y los resultados;
   - sacar el PR #14 de borrador cuando el pipeline de casa, Tilemaps y cultivos quede validado localmente.

### Decisión que debe conservarse

Los Tilemaps se usan para autoría del mundo estático. Los cultivos siguen siendo entidades runtime con `SpriteRenderer`. Los edificios completos se extraen del atlas Full-Pack mediante un catálogo reproducible; no se deben volver a aproximar con paneles del paquete piloto cuando exista una variante completa disponible.
