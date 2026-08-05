# Cozy Farm — recepción piloto

## Estado actual

- Rama: `chore/cozy-farm-art-intake`.
- El héroe actual se conserva sin cambios.
- El paquete completo `full version.zip` permanece fuera del repositorio.
- A1, recepción de cinco hojas fuente: **VALIDADA LOCALMENTE**.
- A2, slicing curado y pruebas automáticas: **VALIDADO LOCALMENTE**.
- A3, escena artística de exhibición: **GENERADOR IMPLEMENTADO; GENERACIÓN LOCAL Y VALIDACIÓN PENDIENTES**.

## A1 — Fuente piloto validada

Commit local publicado por Arturo:

```text
e4540b42d275b650f726bad41d4546787ae544e9
```

Archivos fuente:

```text
Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/
├── items.png
├── seeds.png
├── tools.png
├── crops.png
└── tiles.png
```

Arturo confirmó el 5 de agosto de 2026 que la importación inicial quedó correcta. La configuración permanece en 16 PPU, Point, sin mipmaps, Clamp y sin compresión.

## A2 — Slicing curado validado

No se duplicaron ni recortaron los PNG. Se conservaron las hojas originales y se añadieron únicamente rectángulos nombrados en sus `.meta`.

### Objetos y semillas

`items.png` expone:

- `cozy_turnip`;
- `cozy_carrot`;
- `cozy_cabbage`.

`seeds.png` expone:

- `cozy_turnip_seeds`;
- `cozy_carrot_seeds`;
- `cozy_cabbage_seeds`.

### Cultivos

`crops.png` expone seis etapas para cada cultivo:

- `cozy_turnip_stage_0` a `cozy_turnip_stage_5`;
- `cozy_carrot_stage_0` a `cozy_carrot_stage_5`;
- `cozy_cabbage_stage_0` a `cozy_cabbage_stage_5`.

Los cultivos usan pivote inferior central para futura colocación sobre la parcela.

### Terreno

`tiles.png` expone solo cuatro tiles piloto:

- `cozy_grass`;
- `cozy_dirt`;
- `cozy_water`;
- `cozy_tilled_soil`.

### Validación local A2 — 2026-08-05

Arturo comprobó dentro de Unity exactamente:

- `items.png`: **3** sprites;
- `seeds.png`: **3** sprites;
- `crops.png`: **18** sprites;
- `tiles.png`: **4** sprites;
- `tools.png`: una sola planilla sin cortes.

Después ejecutó todas las pruebas:

- EditMode: **130/130**;
- PlayMode: **6/6**;
- errores: **0**.

A2 queda aprobado completamente.

### Alias visuales provisionales

El paquete denomina **radish** al recurso usado provisionalmente para el ID de dominio `turnip`, y **lettuce** al usado provisionalmente para `cabbage`. Los IDs de dominio no cambiaron. Estos alias son reversibles.

### Herramientas

`tools.png` contiene máquinas de procesamiento y mobiliario, no iconos adecuados de azada y regadera. Permanece en modo Single y sin slicing. No se asignaron sustitutos falsos a `hoe` o `watering-can`.

## A3 — Escena artística de exhibición

Se añadió un generador Editor reproducible:

```text
Assets/_Project/Scripts/Editor/CozyFarmShowcaseScenePipeline.cs
```

Al terminar de compilar/importar, genera automáticamente:

```text
Assets/_Project/Scenes/CozyFarmShowcase.unity
```

También puede regenerarse desde:

```text
Tools > Farm Simulator > Rebuild Cozy Farm Showcase
```

La escena es independiente de `Lab` y contiene:

- cámara ortográfica con viewport lógico 16:9;
- fondo de césped;
- cuatro muestras de terreno;
- tres objetos cosechados;
- tres bolsas de semillas;
- dieciocho etapas de cultivo distribuidas en tres filas;
- una instancia conectada al prefab actual del héroe, colocada sobre una referencia de tierra para comparar escala.

No se añadieron Tilemaps, paletas, UI, hotbar, integración de inventario ni lógica de juego.

### Pruebas A3

`CozyFarmShowcaseSceneTests.cs` añade cuatro casos EditMode para comprobar:

- generación y firma de la escena;
- presencia de grupos y sprites curados;
- uso del prefab vigente del héroe sin reemplazarlo;
- cámara ortográfica y `ReferenceAspectCamera`.

Resultado esperado, todavía no confirmado localmente:

- EditMode: **134/134**;
- PlayMode: **6/6**.

## Exclusiones vigentes

No incluir ZIP, GIF, `global.png`, `item_carry.png`, personajes adicionales, animales, edificios, enemigos, máquinas adicionales ni variantes estacionales completas. No crear todavía Tilemaps, paletas funcionales, UI o conexión con el inventario.

## Próximo paso local

1. Hacer Fetch/Pull de `chore/cozy-farm-art-intake`.
2. Abrir Unity y esperar compilación e importación completas.
3. Confirmar que se genere `Assets/_Project/Scenes/CozyFarmShowcase.unity`.
4. Abrir esa escena y pulsar Play.
5. Comprobar que `Lab` permanece intacta y que la escena muestra héroe, terreno, objetos, semillas y las 18 etapas.
6. Ejecutar EditMode completo; esperado **134/134**.
7. Ejecutar PlayMode completo; esperado **6/6**.
8. Reportar apariencia, conteos y cualquier error o advertencia.
9. No hacer commit todavía de la escena generada ni avanzar a Tilemaps/hotbar hasta revisar el resultado visual.
