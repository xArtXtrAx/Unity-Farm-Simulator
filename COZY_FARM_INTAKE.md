# Cozy Farm — recepción piloto

## Estado actual

- Rama: `chore/cozy-farm-art-intake`.
- El héroe actual se conserva; su prefab real todavía no se modifica.
- El paquete completo `full version.zip` permanece fuera del repositorio.
- A1, recepción de fuentes: **VALIDADA**.
- A2, slicing curado: **VALIDADO**.
- A3 y A3.1, exhibiciones iniciales: **VALIDADAS TÉCNICAMENTE; DESCARTADAS COMO PROPORCIÓN FINAL**.
- A3.2, visual del héroe a 1.5×: **APROBADA VISUALMENTE**.
- A3.3, iconos a 0.75× y semillas plantadas centradas: **IMPLEMENTADA REMOTAMENTE; VALIDACIÓN LOCAL PENDIENTE**.

## A1 — Fuentes piloto

Commit de assets publicado por Arturo:

```text
e4540b42d275b650f726bad41d4546787ae544e9
```

Hojas versionadas:

```text
Assets/_Project/Art/ThirdParty/CozyFarm/Pilot/Source/
├── items.png
├── seeds.png
├── tools.png
├── crops.png
└── tiles.png
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
- cultivos: seis etapas para nabo, zanahoria y col, **18 sprites**;
- terreno: `cozy_grass`, `cozy_dirt`, `cozy_water`, `cozy_tilled_soil`.

`tools.png` permanece Single porque contiene máquinas y mobiliario, no iconos apropiados de azada o regadera.

Alias provisionales:

- arte `radish` → ID `turnip`;
- arte `lettuce` → ID `cabbage`.

Validación A2:

- `items.png`: **3** sprites;
- `seeds.png`: **3** sprites;
- `crops.png`: **18** sprites;
- `tiles.png`: **4** sprites;
- EditMode: **130/130**;
- PlayMode: **6/6**;
- errores: **0**.

## A3 — Pipeline de exhibición

El pipeline Editor genera localmente:

```text
Assets/_Project/Scenes/CozyFarmShowcase.unity
```

Comando manual:

```text
Tools > Farm Simulator > Rebuild Cozy Farm Showcase
```

La escena es independiente de `Lab`, utiliza `SpatialModel.CameraOrthographicSize` y conserva una instancia conectada al prefab actual del héroe.

### A3 original

Validación:

- EditMode: **134/134**;
- PlayMode: **6/6**;
- errores: **0**.

Problemas: iconos tratados como objetos físicos, 18 bases circulares repetidas, muestras 2×2 sobredimensionadas y distribución dispersa.

### A3.1 — Composición compacta

Cambios:

- iconos a 0.55;
- cama compartida 6×3;
- eliminación de `soil_for_*`;
- muestras individuales;
- héroe inicialmente a escala visual 1.0.

Validación:

- EditMode: **136/136**;
- PlayMode: **6/6**;
- errores: **0**.

Cinco comparaciones directas demostraron que el héroe seguía demasiado pequeño frente a tiles y cultivos maduros.

## A3.2 — Calibración visual del héroe

Implementación:

- firma `cozy-farm-showcase-scene-v3`;
- raíz `Current Hero` en **1.0**;
- hijo `Playable Player Sprite` en **1.5**;
- collider, Rigidbody2D, motor, pivote lógico y sorting intactos;
- tiles y cultivos en **1.0**;
- tamaño visual aproximado del héroe: **1.5 × 1.6875 unidades**.

Arturo confirmó el 5 de agosto de 2026 que:

- todas las pruebas solicitadas pasaron sin errores;
- el héroe ya se percibe proporcional frente a las hortalizas plantadas y a los tiles;
- la escala 1.5 es claramente mejor que 1.0.

El prefab real aún no se modifica. La escala continúa aplicada únicamente a la instancia generada de exhibición hasta cerrar todo el piloto visual.

## A3.3 — Iconos y semillas plantadas

Estado: **IMPLEMENTADA REMOTAMENTE; VALIDACIÓN LOCAL PENDIENTE**.

Commits funcionales:

```text
0666f807d2a3f99ad42e648bef7f904c0f0753a1
879a05894f7989c51df650810a9c1b6c199838af
```

### Iconos del catálogo

Los seis iconos —tres hortalizas cosechadas y tres bolsas de semillas— cambian de:

```text
0.55 → 0.75
```

Motivo:

- con el héroe a 1.5, 0.55 resultaba demasiado pequeño;
- 0.75 conserva una relación visual más clara;
- a la resolución lógica del proyecto, cada píxel fuente Cozy Farm se representa mediante **3 píxeles de pantalla**, evitando escalado fraccional borroso.

### Semillas plantadas

Las etapas `stage_0` contienen el dibujo de las semillas en el centro de su rectángulo 16×16, pero comparten pivote inferior con brotes y plantas. Eso las elevaba visualmente dentro del tile.

Corrección:

```text
PlantedSeedStageYOffset = -0.3
```

Solo las tres etapas `stage_0` reciben el desplazamiento. Los brotes y las demás etapas conservan sus posiciones originales.

La corrección centra aproximadamente la masa visual de las semillas dentro de cada tile sin cambiar pivotes, PNG ni `.meta` artísticos.

### Firma y pruebas

- firma elevada a `cozy-farm-showcase-scene-v4`;
- `CozyFarmShowcaseSceneTests.cs` pasa de **7 a 8 casos**;
- la prueba existente fija los iconos en 0.75;
- la prueba nueva verifica el desplazamiento −0.3 de las tres etapas sembradas y confirma que `stage_1` no se mueve.

Resultado esperado:

- EditMode: **138/138**;
- PlayMode: **6/6**.

## Alcance protegido

A3.3 no modifica:

- PNG o slices;
- `Lab`;
- prefab, spritesheet o animaciones reales del héroe;
- collider o movimiento;
- Domain, inventario o Input System;
- Tilemaps, agricultura funcional, hotbar o UI conectada.

## Próximo paso exacto

1. Cerrar `CozyFarmShowcase` en Unity.
2. Hacer Fetch/Pull de `chore/cozy-farm-art-intake`.
3. Abrir Unity y esperar compilación/importación.
4. La firma `v4` debe regenerar la escena.
5. Si la escena anterior estaba abierta, cerrarla y ejecutar `Tools > Farm Simulator > Rebuild Cozy Farm Showcase`.
6. Revisar que los seis iconos sean mayores pero todavía menores que el héroe.
7. Revisar que las tres semillas `stage_0` queden centradas dentro de sus tiles y que los brotes no hayan cambiado.
8. Ejecutar EditMode; esperado **138/138**.
9. Ejecutar PlayMode; esperado **6/6**.
10. Compartir captura y resultados antes de modificar el prefab real o avanzar a Tilemaps/hotbar.
11. No hacer commit todavía de `CozyFarmShowcase.unity` ni de su `.meta`.
