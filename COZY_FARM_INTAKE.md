# Cozy Farm — recepción piloto

## Estado actual

- Rama: `chore/cozy-farm-art-intake`.
- El héroe actual se conserva sin cambios.
- El paquete completo `full version.zip` permanece fuera del repositorio.
- A1, recepción de cinco hojas fuente: **VALIDADA LOCALMENTE**.
- A2, slicing curado y pruebas automáticas: **IMPLEMENTADO REMOTAMENTE; VALIDACIÓN LOCAL PENDIENTE**.

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

Arturo confirmó el 5 de agosto de 2026 que la importación y todas las pruebas solicitadas quedaron OK. Como A1 no añadió pruebas, la referencia continúa siendo EditMode **124/124** y PlayMode **6/6**.

## A2 — Slicing curado

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

### Alias visuales provisionales

El paquete denomina **radish** al recurso usado provisionalmente para el ID de dominio `turnip`, y **lettuce** al usado provisionalmente para `cabbage`. Los IDs de dominio no cambiaron. Estos alias son reversibles y deben revisarse antes de una decisión artística definitiva.

### Herramientas

`tools.png` contiene máquinas de procesamiento y mobiliario, no iconos adecuados de azada y regadera. Permanece en modo Single y sin slicing. No se asignaron sustitutos falsos a `hoe` o `watering-can`.

## Pruebas añadidas

`CozyFarmPilotArtTests.cs` añade seis casos EditMode que comprueban:

- dimensiones de las cinco hojas;
- configuración pixel-art;
- permanencia de `tools.png` sin slicing;
- nombres de objetos y semillas;
- 18 etapas de cultivo;
- cuatro tiles de terreno.

Resultado esperado pendiente de confirmación local:

- EditMode: **130/130**;
- PlayMode: **6/6**.

## Exclusiones vigentes

No incluir ZIP, GIF, `global.png`, `item_carry.png`, personajes, animales, edificios, enemigos, máquinas adicionales ni variantes estacionales completas. No crear todavía Tilemaps, paletas, prefabs, escenas, UI o conexión con el inventario.

## Próximo paso local

1. Hacer Fetch/Pull de `chore/cozy-farm-art-intake`.
2. Abrir Unity y esperar la reimportación de las cuatro hojas modificadas.
3. Confirmar que aparecen 3 sprites de objetos, 3 de semillas, 18 de cultivos y 4 de terreno.
4. Ejecutar EditMode completo; esperado **130/130**.
5. Ejecutar PlayMode completo; esperado **6/6**.
6. Revisar visualmente que los cultivos tengan el pivote en la base y que los alias nabo/radish y col/lettuce sean aceptables como provisionales.
7. Reportar resultados antes de crear cualquier Tilemap, hotbar o conexión funcional.
