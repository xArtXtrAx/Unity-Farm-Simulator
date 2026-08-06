# Bitácora — Pipeline de Tilemaps y Tile Palettes

Fecha: 2026-08-06  
Rama: `agent/cozy-art-pipeline`  
PR: #14 — `Automate Cozy Tile Palette authoring`

Este documento es un anexo transaccional de `BITÁCORA_GPT.MD` para la iteración de autoría de mapas. Debe incorporarse al documento maestro durante el cierre del PR.

## Estado validado antes de este incremento

- Unity 6.3 LTS `6000.3.21f1`.
- Paquete `com.unity.2d.tilemap` instalado y ventana Tile Palette operativa.
- Paletas categorizadas generadas automáticamente.
- Botones del Tile Manager cargan paleta y Tilemap objetivo.
- Todas las pruebas EditMode aprobadas.
- PlayMode aprobado **10/10** después de eliminar la dependencia de orden del hotbar.

## Incidencias encontradas durante la validación manual

1. `Farming` contenía tierra y cultivos en el mismo Tilemap.
2. Un cultivo pintado en la misma coordenada reemplazaba la tierra arada.
3. Semillas y algunas etapas conservaban un cuadro de fondo opaco.
4. Las etapas no compartían una base visual consistente.
5. La vista Scene estaba en perspectiva 3D; para autoría debe usarse 2D frontal ortográfica.

Los bugs correspondientes quedaron registrados en `BUGS.MD` como `BUG-0009` y `BUG-0010`.

## Implementación realizada

### Capas de autoría

La jerarquía objetivo pasa de:

```text
Ground
Paths
Farming
Decoration
```

a:

```text
Ground
Paths
Soil
Crops
Decoration
```

Cambios:

- `FarmTilemapLayers` expone `Soil` y `Crops` por separado.
- `FarmSceneFarmingUpgrader` genera `Farming Core Loop v3`.
- `Soil` usa sorting de suelo.
- `Crops` usa un orden superior y puede ocupar la misma coordenada sin sustituir a `Soil`.
- El Tile Manager presenta cinco botones y cinco paletas.

Commits:

- `94bb3fba22b8e609cf568a4abe9fe880dda3dee5`
- `d1a7654d3b6c574eddd252c63067b9bdeb0727ef`
- `5f30046020c8e6181c2f596b0b9f0812436e65cb`

### Normalización de cultivos

El catálogo ya no usa directamente los recortes opacos de `crops.png` para los tiles de autoría.

Durante `Rebuild`:

1. habilita temporalmente lectura de la textura fuente;
2. extrae cada uno de los 18 recortes curados;
3. usa el color de la esquina inferior izquierda como máscara cuando es opaco;
4. genera PNG RGBA independientes;
5. los importa con 16 PPU, Point, sin mipmaps, sin compresión y Clamp;
6. fija pivote inferior centrado `(0.5, 0.0)`;
7. crea los tiles a partir de esos sprites normalizados.

Salida generada:

```text
Assets/_Project/Tiles/Generated/Crops
```

Commit:

- `3fbd03dafb59aeb53f8e313e9d9e13eb531adeeb`

### Pruebas

`FarmingScenePipelineTests` ahora comprueba:

- existencia de cinco Tilemaps;
- `Soil` y `Crops` como objetos distintos;
- orden de dibujo de cultivos por encima del suelo;
- cinco Palette Assets;
- 18 sprites agrícolas generados;
- transparencia habilitada;
- 16 PPU;
- pivote inferior centrado.

Commit:

- `067aa4f43416c6ff836eb68c44e12e125f481513`

## Próximo paso exacto

1. Hacer Pull de `agent/cozy-art-pipeline`.
2. Esperar compilación e importación.
3. Abrir `Farm`.
4. Ejecutar `Tools > Farm Simulator > Tile Manager`.
5. Pulsar `Rebuild Cozy Tile Catalog + Palettes`.
6. Ejecutar `Tools > Farm Simulator > Apply Farming Field To Farm Scene`.
7. Confirmar jerarquía `Ground / Paths / Soil / Crops / Decoration`.
8. En vista Scene, activar **2D** y usar proyección ortográfica frontal.
9. Pintar tierra en `Soil` y un cultivo en la misma celda desde `Crops`.
10. Confirmar que ambos se ven simultáneamente, sin cuadro opaco y con la planta apoyada en la base.
11. Guardar, cerrar y reabrir `Farm` para verificar persistencia.
12. Ejecutar EditMode y PlayMode completos.
13. Si la validación es satisfactoria, cambiar `BUG-0009` y `BUG-0010` de `CORREGIDO` a `VERIFICADO`.

## Exclusiones

Este incremento no modifica:

- reglas de crecimiento del dominio;
- consumo de semillas;
- sueño y avance de día;
- inventario persistente;
- portales;
- casa e interiores;
- prefab o collider del héroe.
