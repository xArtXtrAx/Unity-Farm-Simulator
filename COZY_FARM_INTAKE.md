# Cozy Farm — recepción piloto

## Estado actual

- Rama: `chore/cozy-farm-art-intake`.
- Head funcional A3.2: `d4d3757640aa5b4f232bbd35f28d9e924bb328b7`.
- El héroe actual se conserva; no se sustituyó su spritesheet ni su prefab.
- El paquete completo `full version.zip` permanece fuera del repositorio.
- A1, recepción de cinco hojas fuente: **VALIDADA LOCALMENTE**.
- A2, slicing curado: **VALIDADO LOCALMENTE**.
- A3, primera exhibición: **VALIDADA TÉCNICAMENTE; RECHAZADA VISUALMENTE**.
- A3.1, composición compacta: **VALIDADA TÉCNICAMENTE; LA PROPORCIÓN DEL HÉROE SIGUE RECHAZADA**.
- A3.2, calibración visual del héroe a 1.5×: **IMPLEMENTADA REMOTAMENTE; VALIDACIÓN LOCAL PENDIENTE**.

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

## A3.1 — Composición compacta

Commits funcionales:

```text
4e6f8e756a38c6dd1ba9a74032f943105e73a9e7
39abe438bb6068b21438fb836b5eea01295f0db3
```

Cambios:

- firma `cozy-farm-showcase-scene-v2`;
- objetos y semillas a escala **0.55**;
- cultivos y héroe a escala **1.0**;
- cama compartida 6×3;
- eliminación de `soil_for_*`;
- cuatro muestras de un solo tile;
- héroe sobre referencia 2×2;
- dos pruebas EditMode nuevas.

Validación local A3.1 — 2026-08-05:

- EditMode: **136/136**;
- PlayMode: **6/6**;
- errores: **0**.

La distribución mejoró, pero las capturas con el héroe colocado junto a cada región demostraron que la conclusión visual anterior era incorrecta: el héroe sigue viéndose demasiado pequeño frente a un tile y frente a los cultivos maduros.

### Diagnóstico geométrico corregido

- Frame del héroe: 64 × 72 px a 64 PPU → **1 × 1.125 unidades**.
- Tile Cozy Farm: 16 × 16 px a 16 PPU → **1 × 1 unidad**.
- El héroe queda prácticamente de un solo tile de alto; para un personaje humano cenital resulta demasiado bajo en comparación con cultivos que llenan una celda.
- No conviene reducir los tiles: deben conservar continuidad y tamaño de grid.
- No conviene escalar la raíz del jugador: escalaría collider y referencias técnicas.

A3.1 queda **VALIDADA TÉCNICAMENTE**, pero la proporción héroe/mundo permanece **RECHAZADA VISUALMENTE**.

## A3.2 — Calibración visual del héroe

Commits funcionales:

```text
8c48c55ed5bcc9d6d836f3e83bcc79cc5d7e0200
d4d3757640aa5b4f232bbd35f28d9e924bb328b7
```

Cambios:

- firma elevada a `cozy-farm-showcase-scene-v3`;
- nueva constante `HeroVisualScale = 1.5f`;
- la raíz `Current Hero` permanece en escala **1.0**;
- únicamente `Playable Player Sprite` se escala a **1.5×**;
- collider, Rigidbody2D, motor, pivote lógico, feet sorting y prefab fuente permanecen sin cambios;
- tiles y cultivos permanecen en escala **1.0**;
- el tamaño visual esperado del héroe pasa a aproximadamente **1.5 × 1.6875 unidades**.

La escala 1.5 sigue siendo compatible con la cuadrícula visual: el arte del héroe usa bloques lógicos 4×4, que a 1.5× se muestran como bloques 6×6 en la resolución de referencia, evitando escalado fraccional de esos píxeles lógicos.

### Pruebas A3.2

`CozyFarmShowcaseSceneTests.cs` pasa de seis a **siete** casos.

El caso nuevo verifica:

- raíz del héroe en escala 1.0;
- hijo visual en escala 1.5;
- collider con exactamente el ancho, alto y offset calibrados previamente.

A3.2 es solo una calibración dentro de la exhibición. El prefab real no cambiará hasta que Arturo apruebe la nueva proporción.

Resultado esperado:

- EditMode: **137/137**;
- PlayMode: **6/6**.

## Exclusiones vigentes

No incluir el ZIP, GIF, personajes adicionales, animales, edificios, enemigos, máquinas adicionales ni variantes estacionales completas. No modificar todavía el prefab real, agricultura funcional, Tilemap final, hotbar conectada o integración con inventario.

## Próximo paso exacto

1. Cerrar `CozyFarmShowcase` en Unity.
2. Hacer Fetch/Pull de `chore/cozy-farm-art-intake`.
3. Abrir Unity y esperar compilación/importación.
4. La firma `v3` debe regenerar la escena automáticamente.
5. Si aparece el aviso de escena abierta, cerrarla y ejecutar `Tools > Farm Simulator > Rebuild Cozy Farm Showcase`.
6. Abrir la escena y colocar al héroe junto a:
   - un tile individual;
   - un cultivo joven;
   - un cultivo maduro;
   - la cama 6×3.
7. Compartir una captura para decidir si 1.5× es la escala visual definitiva o si conviene probar 1.25×.
8. Ejecutar EditMode; esperado **137/137**.
9. Ejecutar PlayMode; esperado **6/6**.
10. No hacer commit de `CozyFarmShowcase.unity` ni modificar el prefab real antes de aprobar la comparación.
