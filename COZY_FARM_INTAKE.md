# Cozy Farm — recepción piloto

## Estado actual

- Rama: `chore/cozy-farm-art-intake`.
- Head funcional A3.1: `39abe438bb6068b21438fb836b5eea01295f0db3`.
- El héroe actual se conserva sin cambios.
- El paquete completo `full version.zip` permanece fuera del repositorio.
- A1, recepción de cinco hojas fuente: **VALIDADA LOCALMENTE**.
- A2, slicing curado: **VALIDADO LOCALMENTE**.
- A3, primera exhibición: **VALIDADA TÉCNICAMENTE; RECHAZADA VISUALMENTE**.
- A3.1, exhibición reequilibrada: **VALIDADA TÉCNICAMENTE; DIAGNÓSTICO VISUAL COMPLETADO**.
- Próximo bloque propuesto: **A3.2 — separar mundo real, referencia técnica e interfaz**.

## A1 — Fuente piloto

Commit de assets publicado por Arturo:

```text
e4540b42d275b650f726bad41d4546787ae544e9
```

Hojas versionadas:

```text
Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/
├── items.png       160 × 192
├── seeds.png       112 × 96
├── tools.png       592 × 64
├── crops.png        96 × 592
└── tiles.png       864 × 800
```

Configuración común:

- Sprite 2D/UI;
- 16 PPU;
- Point;
- sin mipmaps;
- Clamp;
- sin compresión.

No entraron ZIP, GIF, `global.png`, personajes, animales, edificios, enemigos ni variantes estacionales completas.

Validación A1:

- EditMode: **124/124**;
- PlayMode: **6/6**;
- errores: **0**.

## A2 — Slicing curado

Slices aprobados:

- objetos: `cozy_turnip`, `cozy_carrot`, `cozy_cabbage`;
- semillas: `cozy_turnip_seeds`, `cozy_carrot_seeds`, `cozy_cabbage_seeds`;
- cultivos: 6 etapas para nabo, zanahoria y col, **18 sprites** en total;
- muestras de terreno: `cozy_grass`, `cozy_dirt`, `cozy_water`, `cozy_tilled_soil`.

`tools.png` permanece Single: contiene máquinas y mobiliario, no iconos apropiados de azada o regadera.

Alias provisionales y reversibles:

- arte `radish` → ID de dominio `turnip`;
- arte `lettuce` → ID de dominio `cabbage`.

Validación A2 — 2026-08-05:

- `items.png`: **3**;
- `seeds.png`: **3**;
- `crops.png`: **18**;
- `tiles.png`: **4**;
- `tools.png`: sin cortes;
- EditMode: **130/130**;
- PlayMode: **6/6**;
- errores: **0**.

## A3 — Primera exhibición

El pipeline Editor genera localmente:

```text
Assets/_Project/Scenes/CozyFarmShowcase.unity
```

Comando manual:

```text
Tools > Farm Simulator > Rebuild Cozy Farm Showcase
```

La escena es independiente de `Lab`, utiliza `SpatialModel.CameraOrthographicSize` (**4.21875**) y conserva el prefab actual del héroe.

Validación técnica A3:

- EditMode: **134/134**;
- PlayMode: **6/6**;
- errores: **0**.

Problemas visuales observados:

- iconos presentados como objetos físicos de mundo;
- 18 círculos de suelo repetidos;
- muestras 2×2 sobredimensionadas;
- distribución dispersa.

## A3.1 — Exhibición reequilibrada

Commits funcionales:

```text
4e6f8e756a38c6dd1ba9a74032f943105e73a9e7
39abe438bb6068b21438fb836b5eea01295f0db3
```

Cambios:

- firma `cozy-farm-showcase-scene-v2`;
- objetos y semillas a escala **0.55** sobre referencia 3×2;
- cultivos y héroe a escala de mundo **1.0**;
- cama compartida de tierra 6×3;
- eliminación de `soil_for_*`;
- cuatro muestras de un solo tile;
- héroe sobre referencia 2×2;
- dos pruebas EditMode nuevas;
- protección frente a reconstrucción con la escena abierta.

No se modificaron PNG, slices A2, `Lab`, prefab/spritesheet del héroe, Domain, inventario, Input System, Tilemaps, paletas o hotbar.

### Validación local A3.1 — 2026-08-05

Arturo regeneró la escena y confirmó:

- EditMode: **136/136**;
- PlayMode: **6/6**;
- errores: **0**.

La segunda captura confirma que la relación geométrica básica ya es coherente:

- el héroe ocupa aproximadamente un tile de ancho y algo más de uno de alto;
- los cultivos maduros caben dentro de una celda;
- las 18 etapas respetan una cuadrícula de 6×3;
- los iconos son menores que el héroe.

Por ello **no se recomienda reducir indiscriminadamente héroe, cultivos o tiles**. La sensación restante de desproporción procede principalmente de mezclar tres roles visuales en la misma composición:

1. objetos de mundo;
2. iconos de interfaz;
3. lámina técnica con todas las etapas simultáneas.

También se confirma:

- `cozy_grass` desaparece visualmente sobre el fondo del mismo material;
- `cozy_tilled_soil` es una silueta circular similar a un hoyo o montículo de plantación, no una parcela cuadrada completa;
- el héroe de 64×72 tiene más detalle interno que el arte base de 16×16, por lo que existe una diferencia estilística que no se resolverá únicamente cambiando escalas de Transform.

A3.1 queda **VALIDADA TÉCNICAMENTE** y cumple su objetivo diagnóstico. La composición todavía no se considera una escena final del juego.

## A3.2 — Próximo experimento propuesto

Separar la comparación en contextos reales:

### Mundo

Crear una viñeta pequeña de granja con:

- césped;
- una parcela compacta;
- héroe a escala 1.0;
- un cultivo joven, uno intermedio y uno maduro;
- borde de agua;
- sin mostrar simultáneamente las 18 etapas.

Esto permitirá juzgar la proporción que realmente verá el jugador.

### Interfaz

Importar únicamente un panel/slot desde una hoja UI de Cozy Farm, preferentemente `ui/inventory_chopped.png` o un fragmento equivalente de `UI_all.png`, y mostrar los seis iconos en **Canvas Screen Space**. No volver a utilizar tierra de mundo como fondo de inventario.

### Referencia técnica

Conservar las 18 etapas en una vista secundaria o grupo técnico, no como composición principal.

### Semántica de terreno

- dejar de llamar parcela completa a `cozy_tilled_soil`;
- tratarla provisionalmente como `planting_hole`/montículo;
- usar `cozy_dirt` para la parcela hasta localizar y validar un recurso rectangular más adecuado.

## Exclusiones vigentes

No incluir el ZIP, GIF, personajes adicionales, animales, edificios, enemigos, máquinas adicionales ni variantes estacionales completas. No crear todavía agricultura funcional, Tilemap final, hotbar conectada o integración con inventario.

## Próximo paso exacto

1. Registrar A3.1 como validada: **136/136 EditMode**, **6/6 PlayMode**, cero errores.
2. No cambiar todavía la escala del héroe ni de los cultivos.
3. Preparar A3.2 como viñeta de mundo + panel UI separado.
4. Incorporar solo el mínimo recurso UI necesario mediante un bundle controlado.
5. Mantener `CozyFarmShowcase.unity` sin commit hasta cerrar la decisión visual.
