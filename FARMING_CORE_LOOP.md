# Primera vertical agrícola

## Recorrido

```text
seleccionar azada → arar parcela
seleccionar semillas → sembrar
seleccionar regadera → regar
entrar en casa → dormir
volver a la parcela → observar crecimiento
repetir riego y sueño → cosechar
```

## Controles

- `1–8`: seleccionar espacio de la hotbar.
- Rueda del ratón o `L1/R1`: cambiar selección.
- `E` o botón sur del gamepad: usar el objeto seleccionado sobre una parcela.

## Reglas

- Una parcela debe ararse antes de regarse o sembrarse.
- Cada bolsa sembrada consume una semilla del inventario.
- Un cultivo solo crece al avanzar el día si la parcela estaba regada.
- La humedad se consume al comenzar el nuevo día.
- La cosecha se añade al inventario y la parcela permanece arada.
- La granja y el inventario viven en `GameSessionRuntime`, por lo que sobreviven al cambio entre `Farm` y `HouseInterior`.

## Presentación visual

`Farm` utiliza un `Grid` con cuatro capas de Tilemap:

```text
Farm Authoring Grid
├── Ground
├── Paths
├── Farming
└── Decoration
```

El césped y el camino se pintan como tiles reales. Las parcelas sin arar no muestran una pieza adicional: se funden visualmente con `Ground`. Al arar aparece el sprite de tierra trabajada.

Cada etapa de cultivo se ajusta automáticamente a un máximo de 0.62 × 0.72 unidades para impedir que una planta invada las celdas vecinas.

## Tile Manager

Abrir:

```text
Tools > Farm Simulator > Tile Manager
```

La ventana permite:

- reconstruir el catálogo de tiles;
- abrir la ventana oficial `Window > 2D > Tile Palette`;
- seleccionar rápidamente las capas `Ground`, `Paths`, `Farming` o `Decoration`;
- localizar el prefab `Cozy Farm Starter Palette`.

El catálogo inicial contiene:

- césped;
- tierra de camino;
- agua;
- tierra arada;
- etapas 0–5 de nabo, zanahoria y col.

Para registrar la paleta por primera vez:

1. Abrir `Window > 2D > Tile Palette`.
2. Localizar `Assets/_Project/Tiles/Palettes/Cozy Farm Starter Palette.prefab`.
3. Arrastrar el prefab a la barra de la ventana Tile Palette.
4. Seleccionar una capa del `Farm Tile Manager` como objetivo activo.
5. Pintar sobre la retícula de la vista Scene.

La arquitectura admite añadir nuevas categorías de los paquetes completos sin cambiar las escenas ni el runtime agrícola.

## Generación

El campo se aplica automáticamente a `Farm.unity`. También puede forzarse desde:

```text
Tools > Farm Simulator > Apply Farming Field To Farm Scene
```

El generador añade:

```text
Farming Core Loop v2
├── Farm Authoring Grid
└── Farm Plot Field
```

La cantidad inicial continúa siendo 3 × 3 parcelas, pero `Columns`, `Rows` y la celda inicial están centralizados en `FarmSceneFarmingUpgrader` para permitir futuros tamaños configurables.

## Validación

1. Actualizar la rama `agent/farming-core-loop`.
2. Esperar la compilación y la actualización de `Farm`.
3. Ejecutar todas las pruebas EditMode.
4. Abrir `Farm` directamente y validar el recorrido completo.
5. Ejecutar las pruebas PlayMode para confirmar que la casa y el sueño no sufrieron regresiones.
