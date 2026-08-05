# Cozy Farm — recepción piloto

## Estado integrado

El piloto artístico Cozy Farm está **validado e integrado en `main`**.

- Rama de trabajo: `chore/cozy-farm-art-intake`.
- PR: **#7 — Add Cozy Farm pilot art and visual calibration**.
- Head validado: `0f711adcf7dd0ed0c268b57d053ab36f044ea7cc`.
- Método de integración: **Squash and merge**.
- Squash commit: `7860095d0d165c83585f21579e9794ea57ec0a35`.
- Validación final: **138/138 EditMode**, **6/6 PlayMode**, **0 errores**.
- El héroe actual se conserva; su prefab real no fue modificado por el piloto.
- El ZIP completo, los GIF y el resto del paquete permanecen fuera del repositorio.

## Fuentes incorporadas

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
- filtro Point;
- sin mipmaps;
- Clamp;
- sin compresión.

## Slicing aprobado

- 3 objetos cosechados: `cozy_turnip`, `cozy_carrot`, `cozy_cabbage`;
- 3 bolsas de semillas;
- 18 etapas de cultivo: seis para nabo, zanahoria y col;
- 4 muestras de terreno;
- `tools.png` permanece Single y sin cortes.

Alias provisionales y reversibles:

- arte `radish` → ID `turnip`;
- arte `lettuce` → ID `cabbage`.

`cozy_tilled_soil` se conserva como hoyo o montículo provisional, no como parcela rectangular completa.

## Pipeline de exhibición

El pipeline Editor genera localmente:

```text
Assets/_Project/Scenes/CozyFarmShowcase.unity
```

Comando manual:

```text
Tools > Farm Simulator > Rebuild Cozy Farm Showcase
```

La escena es una salida reproducible y no una fuente canónica. Está excluida mediante `.gitignore`, junto con su `.meta`.

## Decisiones visuales aprobadas

- mantener el héroe actual;
- raíz y collider del jugador en escala **1.0**;
- visual del héroe en **1.5** frente al mundo Cozy Farm;
- tiles y cultivos en escala **1.0**;
- iconos del catálogo en **0.75** dentro de la referencia actual;
- aplicar offset vertical **−0.3** a `stage_0` de los tres cultivos;
- no escalar la raíz, collider ni referencias técnicas para resolver diferencias visuales.

La escala 1.5 solo existe por ahora dentro de la exhibición generada. Su traslado al prefab real deberá hacerse en una rama funcional separada.

## Evolución de la validación

- A1: fuentes piloto — 124/124 EditMode, 6/6 PlayMode.
- A2: slicing curado — 130/130 EditMode, 6/6 PlayMode.
- A3: primera exhibición — 134/134 EditMode, 6/6 PlayMode; composición rechazada.
- A3.1: composición compacta — 136/136 EditMode, 6/6 PlayMode; héroe a 1.0 rechazado.
- A3.2: visual del héroe a 1.5 — 137/137 EditMode, 6/6 PlayMode; proporción aprobada.
- A3.3: iconos a 0.75 y semillas centradas — 138/138 EditMode, 6/6 PlayMode; resultado final aprobado.

## Alcance protegido

El piloto no modificó:

- `Lab`;
- prefab, spritesheet o animaciones reales del héroe;
- collider, movimiento o Input System;
- Domain, catálogo o inventario;
- Tilemaps, agricultura funcional, hotbar o UI conectada.

## Próximo paso

1. Actualizar la copia local de `main` mediante Fetch/Pull.
2. Confirmar que la escena Showcase generada no aparezca como cambio pendiente.
3. Mantener cerrada la rama artística para funcionalidad nueva.
4. Abrir una rama funcional separada para el siguiente bloque de migración.
5. Trasladar decisiones visuales al prefab, hotbar, mundo o agricultura únicamente cuando exista un consumidor funcional y pruebas específicas.
